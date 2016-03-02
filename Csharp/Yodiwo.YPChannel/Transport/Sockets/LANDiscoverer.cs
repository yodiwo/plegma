using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
#if NETFX
using System.Net.Sockets;
#else
using Windows.Networking.Sockets;
#endif
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

#if UNIVERSAL
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Networking.Connectivity;
#endif


namespace Yodiwo.YPChannel.Transport.Sockets
{
    public class LANDiscoverer
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public const int DefaultBroadcastPort = 23630;
        public int BroadcastPort;
        //------------------------------------------------------------------------------------------------------------------------
#if NETFX
        public Socket _sock;
        public Socket Sock => _sock;
#else
        public DatagramSocket _sock;
        public DatagramSocket Sock => _sock;
#endif
        //------------------------------------------------------------------------------------------------------------------------
        public Task BroadcastTask;
        public Task DiscoveryTask;
        //------------------------------------------------------------------------------------------------------------------------
        public bool IsRunning;
        //------------------------------------------------------------------------------------------------------------------------
        Type DiscoveryMessageType;
        DiscoveryMessageBase DiscoveryMessage;
        int DiscoveryMessageSize;
        byte[] DiscoveryMessageData;
        //------------------------------------------------------------------------------------------------------------------------
        public int myID = 0;
        //------------------------------------------------------------------------------------------------------------------------
        public struct RemoteEndpointID
        {
            public string IPAddress;
            public int ID;
        }
        public class RemoteEndpointInfo
        {
            public RemoteEndpointID ID;
            public string IPAddress => ID.IPAddress;
            public DateTime LastMessageTimestamp;
            public DiscoveryMessageBase LastDiscoveryMessage;
        }
        public readonly DictionaryTS<RemoteEndpointID, RemoteEndpointInfo> DiscoveredEndpoints = new DictionaryTS<RemoteEndpointID, RemoteEndpointInfo>();
        //------------------------------------------------------------------------------------------------------------------------
        public delegate void OnNewEndpointDiscoveredDelegate(RemoteEndpointInfo newEndpoint);
        public event OnNewEndpointDiscoveredDelegate OnNewEndpointDiscovered;
        //------------------------------------------------------------------------------------------------------------------------
        public delegate void OnEndpointMsgRxDelegate(RemoteEndpointInfo endpoint, DiscoveryMessageBase msg);
        public event OnEndpointMsgRxDelegate OnEndpointMsgRx;
        //------------------------------------------------------------------------------------------------------------------------
        public delegate void OnEndpointTimeoutDelegate(RemoteEndpointInfo endpoint);
        public event OnEndpointTimeoutDelegate OnEndpointTimeout;
        //------------------------------------------------------------------------------------------------------------------------
        public TimeSpan EndpointTimeout = TimeSpan.FromSeconds(30);
        //------------------------------------------------------------------------------------------------------------------------
        public TimeSpan BroadcastDelay = TimeSpan.FromSeconds(5);
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        public LANDiscoverer(DiscoveryMessageBase DiscoveryMessage, int BroadcastPort = DefaultBroadcastPort)
        {
            DebugEx.Assert(DiscoveryMessage != null, "DiscoveryMessage cannot be null");
            if (DiscoveryMessage == null)
                throw new NullReferenceException("DiscoveryMessage cannot be null");

            //keep info
            this.BroadcastPort = BroadcastPort;

            //generate id
            while (myID == 0)
                myID = MathTools.GetRandomNumber();

            //update discovery message
            UpdateDiscoveryMessage(DiscoveryMessage);

            //hook event for shutdown
            Channel.OnSystemShutDownRequest.Add(Yodiwo.WeakAction<object>.Create(Deinitialize));
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public void Initialize(bool EnableDiscovery, bool EnableBroadcasting)
        {
            lock (this)
            {
                try
                {
                    //setup socket
                    if (EnableDiscovery || EnableBroadcasting)
                    {
#if NETFX
                        _sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        _sock.EnableBroadcast = true;
                        _sock.ExclusiveAddressUse = false;
                        _sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                        if (EnableDiscovery)
                            try { _sock.Bind(new IPEndPoint(IPAddress.Any, BroadcastPort)); } catch (Exception ex2) { DebugEx.Assert(ex2, "Discovery Bind failed"); }
#else
                        _sock = new DatagramSocket();
                        _sock.Control.DontFragment = true;
                        _sock.MessageReceived += _sock_MessageReceived;
                        if (EnableDiscovery)
                            try { _sock.BindServiceNameAsync(BroadcastPort.ToStringInvariant()).AsTask().Wait(); } catch (Exception ex2) { DebugEx.Assert(ex2, "Discovery Bind failed"); }
#endif
                    }

                    //start tasks
                    IsRunning = true;
#if NETFX
                    if (EnableDiscovery)
                        DiscoveryTask = Task.Run((Action)DiscoveryTaskEntryPoint);
#endif
                    if (EnableBroadcasting)
                        BroadcastTask = Task.Run((Action)BroadcastTaskEntryPoint);
                }
                catch (Exception ex)
                {
                    Deinitialize();
                    DebugEx.Assert(ex);
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Deinitialize()
        {
            lock (this)
            {
                try
                {
                    //close socket
#if NETFX
                    try { _sock?.Disconnect(false); } catch { }
                    try { _sock?.Close(); } catch { }
#endif
                    try { _sock?.Dispose(); } catch { }
                    _sock = null;

                    //stop tasks
                    IsRunning = false;
                    DiscoveryTask?.Wait(200);
                    BroadcastTask?.Wait(200);
                    DiscoveryTask = null;
                    BroadcastTask = null;
                }
                catch (Exception ex) { DebugEx.Assert(ex); }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void UpdateDiscoveryMessage(DiscoveryMessageBase _discoveryMessage)
        {
            DebugEx.Assert(_discoveryMessage != null, "DiscoveryMessage cannot be null");
            if (_discoveryMessage == null)
                throw new NullReferenceException("DiscoveryMessage cannot be null");

            //setup id on message
            _discoveryMessage.Id = myID;

            //get size and type
            var _discoveryMessageSize = Marshal.SizeOf(_discoveryMessage);
            var _discoveryMessageType = _discoveryMessage.GetType();

            //create payload
            var data = Tools.Mashalling.ToBytes(_discoveryMessage);

            //update
            this.DiscoveryMessage = _discoveryMessage;
            this.DiscoveryMessageSize = _discoveryMessageSize;
            this.DiscoveryMessageType = _discoveryMessageType;
            this.DiscoveryMessageData = data;

        }
        //------------------------------------------------------------------------------------------------------------------------
#if NETFX
        void DiscoveryTaskEntryPoint()
        {
            //heartbeat
            while (IsRunning && _sock?.IsBound == true)
            {
                try
                {
                    //receive datagram
                    var iep = new IPEndPoint(IPAddress.Any, BroadcastPort);
                    var ep = (EndPoint)iep;

                    //receive packet
                    byte[] buffer = new byte[DiscoveryMessageSize];
                    var offset = 0;
                    while (offset < DiscoveryMessageSize)
                        offset += _sock.ReceiveFrom(buffer, offset, DiscoveryMessageSize - offset, SocketFlags.None, ref ep);

                    //get address
                    var addr = ep.GetIPAddress();

                    //handle new packet
                    HandleNewPacket(buffer, addr.ToStringInvariant());
                }
                catch (Exception ex) { DebugEx.TraceError(ex, "DiscoveryTaskEntryPoint error"); }
            }
        }
#endif
        //------------------------------------------------------------------------------------------------------------------------
#if UNIVERSAL
        private void _sock_MessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs e)
        {
            lock (this)
            {
                var address = e.RemoteAddress.ToString();
                var port = int.Parse(e.RemotePort);

                using (var reader = e.GetDataReader())
                {
                    var data = reader.DetachBuffer().ToArray();
                    HandleNewPacket(data, address);
                }
            }
        }
#endif
        //------------------------------------------------------------------------------------------------------------------------
        void HandleNewPacket(byte[] data, string addr)
        {
            try
            {
                //deserialize packet
                DiscoveryMessageBase msg;
                try
                {
                    msg = Tools.Mashalling.ToObject(DiscoveryMessageType, data) as DiscoveryMessageBase;
                }
                catch (Exception ex1) { DebugEx.TraceError(ex1, "DiscoveryTaskEntryPoint could not unpack msg"); return; }

                //is it me? (then discard msg)
                if (msg.Id == 0 || msg.Id == myID)
                    return;

                //create remote id
                var remID = new RemoteEndpointID() { IPAddress = addr, ID = msg.Id };

                //examine existing association
                var info = DiscoveredEndpoints.TryGetOrDefault(remID);
                if (info == null)
                {
                    DebugEx.TraceLog($"LANDiscoverer : Discovered new endpoint. (ip:{addr} id:{msg.Id} flags:{msg.Flags})");
                    //create info
                    info = new RemoteEndpointInfo
                    {
                        ID = remID,
                        LastMessageTimestamp = DateTime.Now,
                        LastDiscoveryMessage = msg,
                    };
                    //add to discovered nodes
                    DiscoveredEndpoints.Add(remID, info);
                    //raise event
                    OnNewEndpointDiscovered?.Invoke(info);
                }

                //custom handle?
                OnEndpointMsgRx?.Invoke(info, msg);

                //update discovery message
                Interlocked.Exchange(ref info.LastDiscoveryMessage, msg);
            }
            catch (Exception ex) { DebugEx.TraceError(ex, "DiscoveryTaskEntryPoint error"); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        void BroadcastTaskEntryPoint()
        {
            try
            {
                //create broadcast endpoint
#if NETFX
                var iep = new IPEndPoint(IPAddress.Broadcast, BroadcastPort);
#endif
                //heartbeat
                while (IsRunning && _sock != null)
                {
                    try
                    {
                        var data = DiscoveryMessageData;
                        if (data != null)
                        {
                            //send discovery request
#if NETFX
                            _sock?.SendTo(data, iep);
#else
                            using (var stream = _sock.GetOutputStreamAsync(new Windows.Networking.HostName("255.255.255.255"), BroadcastPort.ToStringInvariant()).GetResults())
                                stream.WriteAsync(data.AsBuffer(0, data.Length)).AsTask().Wait();
#endif
                        }
                        //wait
                        Task.Delay(BroadcastDelay).Wait();
                    }
                    catch (Exception ex) { DebugEx.TraceError(ex, "BroadcastTaskEntryPoint error"); }
                }
            }
            catch (Exception ex) { DebugEx.TraceError(ex, "BroadcastTaskEntryPoint error"); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        void CleanUp()
        {
            foreach (var entry in DiscoveredEndpoints)
                if ((DateTime.Now - entry.Value.LastMessageTimestamp) > EndpointTimeout)
                {
                    DiscoveredEndpoints.Remove(entry.Key);
                    OnEndpointTimeout?.Invoke(entry.Value);
                }
        }
        //------------------------------------------------------------------------------------------------------------------------
        ~LANDiscoverer()
        {
            try
            {
                if (IsRunning)
                    Deinitialize();
            }
            catch (Exception ex) { DebugEx.TraceError(ex, "~LANDiscoverer error"); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public class DiscoveryMessageBase
    {
        //Info
        public int Id;
        public int MajorVersion;
        public int MinorVersion;
        public int BuildVersion;
        public int Flags;
    }

}
