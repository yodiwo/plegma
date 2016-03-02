using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
#if NETFX
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
#else
using Windows.Networking.Sockets;
#endif
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#if UNIVERSAL
using Socket = Windows.Networking.Sockets.StreamSocket;
#endif

namespace Yodiwo.YPChannel.Transport.Sockets
{
    public class Client : CommonSocketChannel
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public Client(Protocol Protocol, ChannelSerializationMode ChannelSerializationMode = ChannelSerializationMode.MessagePack)
            : this(new[] { Protocol }, ChannelSerializationMode: ChannelSerializationMode)
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        public Client(Protocol[] Protocols, ChannelSerializationMode ChannelSerializationMode = ChannelSerializationMode.MessagePack)
            : base(Protocols, ChannelRole.Client, ChannelSerializationMode)
        {
            //create socket
#if NETFX
            _sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
#else
            _sock = new StreamSocket();
#endif
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public Task<SimpleActionResult> ConnectAsync(string RemoteHost, int RemotePort, bool Secure, string CertificateServerName = null)
        {
            return Task.Run(() => ConnectAsync(RemoteHost, RemotePort, Secure, CertificateServerName: CertificateServerName));
        }
        //------------------------------------------------------------------------------------------------------------------------
#if NETFX
        public SimpleActionResult Connect(string RemoteHost, int RemotePort, bool Secure, string CertificateServerName = null, IEnumerable<X509Certificate2> CustomCertificates = null)
#else
        public SimpleActionResult Connect(string RemoteHost, int RemotePort, bool Secure, string CertificateServerName = null)
#endif
        {
            //init results
            bool isSecured = false;
            string sslProtocol = "";
            var result = new SimpleActionResult()
            {
                IsSuccessful = false,
                Message = "",
            };

            lock (this)
            {
                //Try to connect
                try
                {
                    //attemp connection
#if NETFX
                    _sock.Connect(RemoteHost, RemotePort);
#else
                    _sock.ConnectAsync(new Windows.Networking.HostName(RemoteHost), RemotePort.ToStringInvariant(), !Secure ? SocketProtectionLevel.PlainSocket : (SocketProtectionLevel.Tls10 | SocketProtectionLevel.Tls11 | SocketProtectionLevel.Tls12)).AsTask().Wait();
#endif
                }
                catch (Exception ex)
                {
                    DebugEx.Assert(ex, "Connection Error");
                    result.Message = ex.Message;
                    return result;
                }

                //create network stream
#if NETFX
                //Stream _netstream = new BufferedStream(new NetworkStream(_sock, true));
                Stream _netstream = new NetworkStream(_sock, true);
#endif

                //write packers
                var smode = new byte[] { (byte)this.ChannelSerializationMode };
#if NETFX
                var _nodelay = _sock.NoDelay;
                _sock.NoDelay = true; //Disable the Nagle Algorithm
                _sock.Send(smode);
                _sock.NoDelay = _nodelay; //Restore (default:enable) the Nagle Algorithm
#else                
                {
                    var wStream = _sock.OutputStream.AsStreamForWrite();
                    wStream.WriteByte(smode[0]);
                    wStream.Flush();
                }
#endif

                //read final packer
#if NETFX
                while (_sock.Receive(smode, 1, SocketFlags.None) != 1);
#else
                smode[0] = (byte)_sock.InputStream.AsStreamForRead().ReadByte();
#endif
                var packerType = (ChannelSerializationMode)smode[0];
                if (packerType != this.ChannelSerializationMode)
                {
                    DebugEx.Assert("Invalid ChannelSerializationMode. Server uses  " + packerType);
                    result.Message = "Invalid ChannelSerializationMode. Server uses  " + packerType;
#if NETFX
                    try { _netstream.Close(); _netstream.Dispose(); } catch { }
                    try { _sock.Close(); } catch { }
#endif
                    try { _sock.Dispose(); } catch { }
                    try { Close(); } catch { }
                    return result;
                }


                //Wrap with a secure stream?
                if (Secure)
                {
#if NETFX
                    //collect certificates
                    var certs = Yodiwo.Tools.Certificates.CollectCertificates();
                    if (CustomCertificates != null)
                        foreach (var c in CustomCertificates)
                            certs.Add(c);

                    //create ssl stream
                    var sslstream = new SslStream(_netstream, false);
#endif
                    try
                    {
#if NETFX
                        //try authenticate
                        sslstream.AuthenticateAsClient(!string.IsNullOrWhiteSpace(CertificateServerName) ? CertificateServerName : RemoteHost,
                                                        certs,
                                                        System.Security.Authentication.SslProtocols.Tls | System.Security.Authentication.SslProtocols.Tls11 | System.Security.Authentication.SslProtocols.Tls12,
                                                        true
                                                        );


                        //checks
                        if (!sslstream.IsAuthenticated)
                        {
                            DebugEx.Assert("Not authenticated");
                            throw new Exception("Not authenticated");
                        }
                        if (!sslstream.IsEncrypted)
                        {
                            DebugEx.Assert("No encryption");
                            throw new Exception("Not encryption");
                        }

                        //get info
                        isSecured = true;
                        sslProtocol = sslstream.SslProtocol.ToStringInvariant();

                        //use this stream from now on
                        _netstream = sslstream;
#else

                        _sock.UpgradeToSslAsync(SocketProtectionLevel.Tls10 | SocketProtectionLevel.Tls11 | SocketProtectionLevel.Tls12, new Windows.Networking.HostName(CertificateServerName)).GetResults();
#endif
                    }
                    catch (Exception ex)
                    {
                        DebugEx.TraceError(ex, "Certificate not accepted, " + ex.Message);
                        result.Message = "Certificate not accepted, " + ex.Message;
                        if (ex.InnerException != null)
                            result.Message += "  (inner msg=" + ex.InnerException.Message + ")";
#if NETFX
                        try { _netstream.Close(); _netstream.Dispose(); } catch { }
                        try { sslstream.Close(); sslstream.Dispose(); } catch { }
                        try { _sock.Close(); } catch { }
#endif
                        try { _sock.Dispose(); } catch { }
                        try { Close(); } catch { }
                        return result;
                    }
                }

                //setup info
                try
                {
#if NETFX
                    this.LocalHost = _sock.LocalEndPoint.GetIPAddress().ToString();
                    this.RemotePort = _sock.LocalEndPoint.GetPort().ToStringInvariant();
#else
                    this.LocalHost = _sock.Information.LocalAddress.ToString();
                    this.RemotePort = _sock.Information.LocalPort;
#endif
                }
                catch { }

                //setup info
                this.RemoteHost = RemoteHost;
                this.RemotePort = RemotePort.ToStringInvariant();

                //log
                DebugEx.TraceLog("YPClient (socks) new connection to " + RemoteHost + ":" + RemotePort + " (Secure=" + isSecured + ",SSL=" + sslProtocol + ")");

                //create stream
#if NETFX
                SetupStream(_netstream);
#else
                SetupStream();
#endif

                //start heartbeat
                Start();

                //hook for system close
                Channel.OnSystemShutDownRequest.Add(Yodiwo.WeakAction<object>.Create(ShutdownRedirect));
            }

            //wait for negotiation finish
            Task.Delay(1).Wait();
            while (State == ChannelStates.Initializing || State == ChannelStates.Negotiating)
                Task.Delay(100).Wait();

            //set message
            if (State != ChannelStates.Open)
                result.Message = "Could not open channel (Negotiation Failed)";

            //return state
            result.IsSuccessful = State == ChannelStates.Open;
            if (result.IsSuccessful && result.Message == "")
                result.Message = "Connection Established (Channel Openned)";
            return result;
        }
        //------------------------------------------------------------------------------------------------------------------------
        void ShutdownRedirect(object Sender) { Close(); }
        //------------------------------------------------------------------------------------------------------------------------
#if DEBUG
        ~Client()
        {
            DebugEx.TraceLog("YPC Client destructed (name=" + Name + ")");
        }
#endif
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
