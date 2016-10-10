using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
#if NETFX
using System.Net.Sockets;
#elif UNIVERSAL
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
#elif UNIVERSAL
        public DatagramSocket _sock;
        public DatagramSocket Sock => _sock;
#endif
        //------------------------------------------------------------------------------------------------------------------------
#if NETFX
        public Thread BroadcastTask;
        public Thread DiscoveryTask;
#elif UNIVERSAL
        public Task BroadcastTask;
        public Task DiscoveryTask;
#endif
        //------------------------------------------------------------------------------------------------------------------------
        public bool IsRunning;
        //------------------------------------------------------------------------------------------------------------------------
        Type DiscoveryMessageType;
        IDiscoveryMessageBase DiscoveryMessage;
        int DiscoveryMessageMaxSize;
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
            public IDiscoveryMessageBase LastDiscoveryMessage;
        }
        public readonly DictionaryTS<RemoteEndpointID, RemoteEndpointInfo> DiscoveredEndpoints = new DictionaryTS<RemoteEndpointID, RemoteEndpointInfo>();
        //------------------------------------------------------------------------------------------------------------------------
        public delegate void OnNewEndpointDiscoveredDelegate(RemoteEndpointInfo newEndpoint);
        public event OnNewEndpointDiscoveredDelegate OnNewEndpointDiscovered;
        //------------------------------------------------------------------------------------------------------------------------
        public delegate void OnEndpointMsgRxDelegate(RemoteEndpointInfo endpoint, IDiscoveryMessageBase msg);
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
        public LANDiscoverer(IDiscoveryMessageBase DiscoveryMessage, int DiscoveryMessageMaxSize = 1024, int BroadcastPort = DefaultBroadcastPort, int? ID = null)
        {
            DebugEx.Assert(DiscoveryMessage != null, "DiscoveryMessage cannot be null");
            if (DiscoveryMessage == null)
                throw new NullReferenceException("DiscoveryMessage cannot be null");

            //keep info
            this.BroadcastPort = BroadcastPort;
            this.DiscoveryMessageMaxSize = DiscoveryMessageMaxSize;

            //set id
            if (ID.HasValue)
                myID = ID.Value;

            //generate id
            while (myID == 0)
#if NETFX
                myID = MathTools.GetRandomNumber(0, 10000);
#elif UNIVERSAL
                myID = MathTools.GetRandomNumber(20000, 100000);
#endif

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
                        _sock.DontFragment = true;
                        _sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                        if (EnableDiscovery)
                            try { _sock.Bind(new IPEndPoint(IPAddress.Any, BroadcastPort)); } catch (Exception ex2) { DebugEx.Assert(ex2, "Discovery Bind failed"); }
#elif UNIVERSAL
                        _sock = new DatagramSocket();
                        _sock.Control.DontFragment = true;
                        if (EnableDiscovery)
                        {
                            _sock.MessageReceived += _sock_MessageReceived;
                            try { _sock.BindServiceNameAsync(BroadcastPort.ToStringInvariant()).AsTask().Wait(); } catch (Exception ex2) { DebugEx.Assert(ex2, "Discovery Bind failed"); }
                        }
#endif
                    }

                    //start tasks
                    IsRunning = true;
#if NETFX
                    if (EnableDiscovery)
                    {
                        DiscoveryTask = new Thread(DiscoveryTaskEntryPoint);
                        DiscoveryTask.Name = "YPC LANDiscoverer discovery heartbeat";
                        DiscoveryTask.IsBackground = true;
                        DiscoveryTask.Start();
                    }
#endif

                    if (EnableBroadcasting)
                    {
#if NETFX
                        BroadcastTask = new Thread(BroadcastTaskEntryPoint);
                        BroadcastTask.Name = "YPC LANDiscoverer broadcast heartbeat";
                        BroadcastTask.IsBackground = true;
                        BroadcastTask.Start();
#elif UNIVERSAL
                        BroadcastTask = BroadcastTaskEntryPoint();
#endif
                    }
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
#if NETFX
                    DiscoveryTask?.Join(200);
                    BroadcastTask?.Join(200);
#elif UNIVERSAL
                    DiscoveryTask?.Wait(200);
                    BroadcastTask?.Wait(200);
#endif

                    DiscoveryTask = null;
                    BroadcastTask = null;
                }
                catch (Exception ex) { DebugEx.Assert(ex); }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void UpdateDiscoveryMessage(IDiscoveryMessageBase _discoveryMessage)
        {
            try
            {
                DebugEx.Assert(_discoveryMessage != null, "DiscoveryMessage cannot be null");
                if (_discoveryMessage == null)
                    throw new NullReferenceException("DiscoveryMessage cannot be null");

                //setup id on message
                _discoveryMessage.Id = myID;

                //get size and type
                var _discoveryMessageType = _discoveryMessage.GetType();

                //create payload
                var data = Tools.Marshalling.ToBytes(_discoveryMessage);
                if (data == null)
                    DebugEx.Assert("Could not get bytes from DicoveryMessage");
                else
                {
                    var dataLen = data.Length;
                    Array.Resize<byte>(ref data, data.Length + 2);
                    for (int n = data.Length - 2; n >= 2; n--)
                        data[n] = data[n - 2];
                    data[0] = (byte)(dataLen & 0x000000FF);
                    data[1] = (byte)((dataLen & 0x0000FF00) >> 8);
                }

                //update
                this.DiscoveryMessage = _discoveryMessage;
                this.DiscoveryMessageType = _discoveryMessageType;
                this.DiscoveryMessageData = data;
            }
            catch (Exception ex) { DebugEx.Assert(ex); }
        }
        //------------------------------------------------------------------------------------------------------------------------
#if NETFX
        void DiscoveryTaskEntryPoint()
        {
            byte[] buffer = new byte[DiscoveryMessageMaxSize];
            //heartbeat
            while (IsRunning && _sock?.IsBound == true)
            {
                try
                {
                    //receive datagram
                    var iep = new IPEndPoint(IPAddress.Any, BroadcastPort);
                    var ep = (EndPoint)iep;

                    //clear buffer
                    Array.Clear(buffer, 0, buffer.Length);

                    //receive packet
                    var read = _sock.ReceiveFrom(buffer, 0, DiscoveryMessageMaxSize, SocketFlags.None, ref ep);
                    if (read == 0 || read == -1)
                    {
                        Thread.Sleep(200);
                        continue;
                    }

                    //get length
                    var length = buffer[0] | (buffer[1] << 8);

                    //get address
                    var addr = ep.GetIPAddress();

                    try
                    {
                        //fixup buffer
                        var fixed_buffer = buffer.Skip(2).Take(length).ToArray();

                        //handle new packet
                        HandleNewPacket(fixed_buffer, addr.ToStringInvariant());
                    }
                    catch { }

                    //sleep
                    Thread.Sleep(200);
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
                    var data = reader.DetachBuffer().ToArray().Skip(2).ToArray();
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
                IDiscoveryMessageBase msg;
                try
                {
                    msg = Tools.Marshalling.ToObject(DiscoveryMessageType, data) as IDiscoveryMessageBase;
                }
                catch { return; }

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
#if NETFX
        void BroadcastTaskEntryPoint()
#elif UNIVERSAL
        async Task BroadcastTaskEntryPoint()
#endif
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
#elif UNIVERSAL
                            using (var stream = await _sock.GetOutputStreamAsync(new Windows.Networking.HostName("255.255.255.255"), BroadcastPort.ToStringInvariant()))
                                stream.WriteAsync(data.AsBuffer(0, data.Length)).AsTask().Wait();
#endif
                        }
                    }
                    catch (Exception ex) { DebugEx.TraceError(ex, "BroadcastTaskEntryPoint error"); }

                    //wait
#if NETFX
                    Thread.Sleep(BroadcastDelay);
#elif UNIVERSAL
                    await Task.Delay(BroadcastDelay).ConfigureAwait(false);
#endif
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


    public interface IDiscoveryMessageBase
    {
        //Info
        int Id { get; set; }
        int Flags { get; set; }
    }

}
