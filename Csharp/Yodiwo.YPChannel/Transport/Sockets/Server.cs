using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
#if NETFX
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
#else
using Windows.Networking.Sockets;
#endif
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yodiwo;

#if UNIVERSAL
using Socket = Windows.Networking.Sockets.StreamSocket;
#endif

namespace Yodiwo.YPChannel.Transport.Sockets
{
    public class Server
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
#if NETFX
        Socket sock;
#else
        Windows.Networking.Sockets.StreamSocketListener sock;
#endif
        //------------------------------------------------------------------------------------------------------------------------
        Task PortListener;
        HashSetTS<ServerChannel> Channels = new HashSetTS<ServerChannel>();
        public Func<Protocol[], Socket, ServerChannel> ChannelConstructor = null;
        int Port;
        ChannelSerializationMode ChannelSerializationMode;
        //------------------------------------------------------------------------------------------------------------------------
        DictionaryTS<string, ServerChannel> IssuedKeys = new DictionaryTS<string, ServerChannel>();
        //------------------------------------------------------------------------------------------------------------------------
        public delegate void OnNewChannelHandler(Server Server, Channel Channel);
        public event OnNewChannelHandler OnNewChannel = null;
        //------------------------------------------------------------------------------------------------------------------------
        Protocol[] Protocols;
        //------------------------------------------------------------------------------------------------------------------------
#if NETFX
        X509Certificate2 _certificate = null;
        public X509Certificate2 Certificate { get { return _certificate; } }
#endif
        //------------------------------------------------------------------------------------------------------------------------
        class ReconnectionBookkeepEntry
        {
            public DateTime ConnectionTimestamp;
            public int Connections;
        }
        DictionaryTS<string, ReconnectionBookkeepEntry> reconnectThrottleBookKeeper = new DictionaryTS<string, ReconnectionBookkeepEntry>();
        public TimeSpan ReconnectionThrottleTimeout = TimeSpan.FromSeconds(2);
        public int ReconnectionThrottleAfterConnectionCount = 10; //after xx connections reconnection throttle will kick-in
        public bool IsReconnectionThrottleEnabled = true;
        //------------------------------------------------------------------------------------------------------------------------
        bool _IsRunning = false;
        public bool IsRunning { get { return _IsRunning; } }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public Server(Protocol Protocol, ChannelSerializationMode ChannelSerializationMode = ChannelSerializationMode.MessagePack)
            : this(new[] { Protocol }, ChannelSerializationMode: ChannelSerializationMode)
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        public Server(Protocol[] Protocols, ChannelSerializationMode ChannelSerializationMode = ChannelSerializationMode.MessagePack)
        {
            this.Protocols = Protocols == null ? (Protocol[])null : (Protocol[])Protocols.Clone(); //copy here to avoid user changing stuff later on
            this.ChannelSerializationMode = ChannelSerializationMode;

            //hook event for shutdown
            Channel.OnSystemShutDownRequest.Add(Yodiwo.WeakAction<object>.Create(ShutdownRedirect));
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        void ShutdownRedirect(object Sender) { Stop(CloseAllChannels: true); }
        //------------------------------------------------------------------------------------------------------------------------
#if NETFX
        public bool Start(int Port, X509Certificate2 certificate = null)
#else
        public bool Start(int Port)
#endif
        {
            lock (this)
            {
                //keep
                this.Port = Port;

                //get certificate
#if NETFX
                this._certificate = certificate;
                if (Certificate != null && !Certificate.HasPrivateKey)
                {
                    DebugEx.Assert("Not private key found in servers certificate");
                    return false;
                }
#endif

                //create socket and bind
                try
                {
#if NETFX
                    sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    sock.Bind(new IPEndPoint(IPAddress.Any, Port));
#else
                    sock = new StreamSocketListener();
                    sock.ConnectionReceived += Sock_ConnectionReceived;
                    sock.BindEndpointAsync(new Windows.Networking.HostName("255.255.255.255"), Port.ToStringInvariant()).AsTask().Wait();
#endif
                }
                catch (Exception ex)
                {
                    DebugEx.Assert(ex, "YPServer failed to bind port " + Port);
                    return false;
                }

                //mark running
                _IsRunning = true;

                //log
#if NETFX
                DebugEx.TraceLog("YPServer (socks) started on port " + Port + "  (Secure=" + (certificate != null).ToStringInvariant() + ")");
#else
                DebugEx.TraceLog("YPServer (socks) started on port " + Port + "  (Secure= False)");
#endif
                //start port listener
#if NETFX
                PortListener = new Task(PortListenerEntryPoint);
                PortListener.Start();
#endif
                //done
                return true;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Stop(bool CloseAllChannels = false)
        {
            try
            {
                lock (this)
                {
                    //close all channels
                    if (CloseAllChannels)
                    {
                        Channels.ForEach(c => { try { c.Close(); } catch (Exception ex) { DebugEx.Assert(ex, "Error while closing channel"); } });
                        Channels.Clear();
                    }

                    //update flag
                    if (!_IsRunning)
                        return;
                    else
                        _IsRunning = false;

                    //close my socket
                    try
                    {
                        if (sock != null)
                        {
#if NETFX
                            try { sock.Close(); } catch { }
#endif
                            try { sock.Dispose(); } catch { }
                        }
                    }
                    catch (Exception ex)
                    {
                        DebugEx.TraceErrorException(ex);
                    }
                    sock = null;

                    //wait for task finish
                    PortListener.Wait(1000);
                    PortListener = null;
                }
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "YPChannel Server Stop() failed");
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
#if NETFX
        void PortListenerEntryPoint()
        {
            try
            {
                //heartbeat
                while (_IsRunning)
                {
                    //listen
                    Socket newsocket = null;

                    //we need to stop?
                    var __sock = sock;  //keep a local copy
                    if (__sock == null)
                        break;

                    //start listening
                    __sock.Listen(10);

                    //accept and get new socket
                    try { newsocket = __sock.Accept(); }
                    catch (Exception ex) { DebugEx.TraceWarning(ex, "YPServer sock.Accept() exception"); continue; }

                    //handlew new connection
                    HandleNewConnection(newsocket);
                }
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "YPChannel server heartbeat error");
            }
        }
#else
        private void Sock_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            HandleNewConnection(args.Socket);
        }
#endif
        //------------------------------------------------------------------------------------------------------------------------
        void HandleNewConnection(Socket newsocket)
        {
            try
            {
                //check reconnection throttle
                try
                {
#if NETFX
                    var re = newsocket.RemoteEndPoint.GetIPAddress().ToStringInvariant();
#else
                    var re = newsocket.Information.RemoteAddress.ToStringInvariant();
#endif
                    if (IsReconnectionThrottleEnabled && re != "127.0.0.1") //no reconnection throttle for localhost connections
                    {
                        var rbe = reconnectThrottleBookKeeper.TryGetOrDefault(re);
                        if (rbe == null)
                        {
                            rbe = new ReconnectionBookkeepEntry()
                            {
                                ConnectionTimestamp = DateTime.Now + TimeSpan.FromMilliseconds(100),
                                Connections = 1,
                            };
                            reconnectThrottleBookKeeper.ForceAdd(re, rbe);
                        }
                        else
                        {
                            rbe.Connections++;
                            if (rbe.Connections > ReconnectionThrottleAfterConnectionCount)
                            {
                                var lastTimestamp = rbe.ConnectionTimestamp;
                                var elapsed = DateTime.Now - lastTimestamp;
                                if (elapsed < ReconnectionThrottleTimeout)
                                {
#if NETFX
                                    try { newsocket.Close(); } catch { }
#endif
                                    try { newsocket.Dispose(); } catch { }
                                    DebugEx.TraceWarning("Connection from " + re + " closed due to reconnection throttle (" + elapsed.Seconds + " sec)");
                                    return;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex) { DebugEx.TraceWarning(ex, "YPServer Reconnection throttle exception"); }

                //cleanup old entries
                reconnectThrottleBookKeeper.RemoveWhere(e => DateTime.Now - e.Value.ConnectionTimestamp > ReconnectionThrottleTimeout);

                //start task new connection
                Task.Run(() =>
                    {
                        try
                        {
                            //check
#if NETFX
                            if (!newsocket.Connected)
                            {
                                DebugEx.TraceWarning("YPServer newsocket not connected?");
                                try { newsocket.Close(); } catch { }
                                return;
                            }
#endif
                            //create channel
                            var con = ChannelConstructor;
                            var channel = con == null ? new ServerChannel(this, Protocols, ChannelSerializationMode, newsocket) : con(Protocols, newsocket);
                            if (channel == null)
                            {
                                DebugEx.Assert("Could not create channel");
#if NETFX
                                try { newsocket.Close(); } catch { }
#endif
                                try { newsocket.Dispose(); } catch { }
                                return;
                            }

                            //add to set
                            string channelKey = null;
                            lock (Channels)
                            {
                                //generate unique key
                                while (IssuedKeys.ContainsKey(channelKey = MathTools.GenerateRandomAlphaNumericString(64))) ;
                                //set on channel
                                channel._ChannelKey = channelKey;
                                //add to lookups
                                Channels.Add(channel);
                                IssuedKeys.Add(channelKey, channel);
                            }

                            //start task timeout monitor
                            bool setupFinished = false;
                            Task.Run(() =>
                            {
                                try
                                {
                                    //wait
                                    Task.Delay(30000).Wait();
                                    //check
                                    if (!setupFinished)
                                    {
                                        DebugEx.TraceLog("ServerChannel setup timeout");
                                        try { channel.Close(); } catch { }
#if NETFX
                                        try { newsocket.Close(); } catch { }
#endif
                                        try { newsocket.Dispose(); } catch { }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    DebugEx.Assert(ex, "Unhandled exception");
#if NETFX
                                    try { newsocket.Close(); } catch { }
#endif
                                    try { newsocket.Dispose(); } catch { }
                                }
                            });

                            //setup channel socket
                            if (channel.SetupServerSocket() == false)
                            {
#if NETFX
                                try { newsocket.Close(); } catch { }
#endif
                                try { newsocket.Dispose(); } catch { }
                                //add to lookups
                                Channels.Remove(channel);
                                IssuedKeys.Remove(channelKey);
                                return;
                            }

                            //mark setup finish
                            setupFinished = true;

                            //call event
                            if (OnNewChannel != null)
                                OnNewChannel(this, channel);

                            //start heartbeat
                            channel.Start();
                        }
                        catch (Exception ex)
                        {
                            DebugEx.Assert(ex, "YPServer: Failed setting up new connection");
#if NETFX
                            try { newsocket.Close(); } catch { }
#endif
                            try { newsocket.Dispose(); } catch { }
                            return;
                        }
                    });
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "YPChannel server setup new connection error");
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        ~Server()
        {
            try
            {
                //stop server
                Stop(CloseAllChannels: true);
                //disconnect/close/dispose sockets
                if (sock != null)
                {
#if NETFX
                    try { sock.Close(); } catch { }
#endif
                    try { sock.Dispose(); } catch { }
                }
            }
            catch (Exception ex)
            {
                DebugEx.TraceError(ex, "YPChannel Server destructor caught unhandled exception");
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        internal void onChannelClose(ServerChannel channel)
        {
            try
            {
                //remove from channel set
                Channels.Remove(channel);
                IssuedKeys.Remove(channel.ChannelKey);
            }
            catch (Exception ex) { DebugEx.TraceError(ex, "Server onChannelClose() unhandled exception"); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}