using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
#if NETFX
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
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
    public class ServerChannel : CommonSocketChannel
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public readonly Server Server;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public ServerChannel(Server Server, Protocol[] Protocols, ChannelSerializationMode ChannelSerializationMode, Socket _sock)
            : base(Protocols, ChannelRole.Server, ChannelSerializationMode)
        {
            this.Server = Server;
            this._sock = _sock;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
#if NETFX
        public virtual bool SetupServerSocket()
#else
        public virtual bool SetupServerSocket()
#endif
        {
            try
            {
                //keep
                bool isSecured = false;
                string sslProtocol = "";

                //create network stream
#if NETFX
                //Stream _netstream = new BufferedStream(new NetworkStream(base._sock, true));
                Stream _netstream = new NetworkStream(base._sock, true);
#endif

                //read clients packers
                var smode = new byte[1];
#if NETFX
                while (_sock.Receive(smode, 1, SocketFlags.None) != 1) ;
#else
                smode[0] = (byte)_sock.InputStream.AsStreamForRead().ReadByte();
#endif
                var clientPackers = (ChannelSerializationMode)smode[0];

                //write packer
                smode[0] = (byte)this.ChannelSerializationMode;
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
                //Wrap with a secure stream?
#if NETFX
                if (Server.Certificate != null)
                {
                    var sslstream = new SslStream(_netstream, false);
                    try
                    {
                        //try authenticate
                        sslstream.AuthenticateAsServer(Server.Certificate, false, SslProtocols.Tls | SslProtocols.Tls12 | SslProtocols.Tls11, true);

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
                    }
                    catch (Exception ex)
                    {
                        var msg = ex.Message;
                        if (ex.InnerException != null && ex.InnerException.Message != ex.Message)
                            msg += "  (inner msg=" + ex.InnerException.Message + ")";
                        DebugEx.TraceError("Certificate not accepted, " + msg);
                        try { sslstream.Close(); base._sock.Dispose(); } catch { }
                        try { _netstream.Close(); _netstream.Dispose(); } catch { }
                        try { _sock.Close(); _sock.Dispose(); } catch { }
                        try { Close(); } catch { }
                        return false; //failed
                    }
                }
#endif

                //setup info
                try
                {
#if NETFX
                    this.LocalHost = base._sock.LocalEndPoint.GetIPAddress().ToString();
                    this.RemotePort = _sock.LocalEndPoint.GetPort().ToStringInvariant();
#else
                    this.LocalHost = _sock.Information.LocalAddress.ToString();
                    this.RemotePort = _sock.Information.LocalPort;
#endif
                }
                catch { }

                //setup info
#if NETFX
                this.RemoteHost = base._sock.RemoteEndPoint.GetIPAddress().ToString();
                this.RemotePort = _sock.RemoteEndPoint.GetPort().ToStringInvariant();
#else
                this.LocalHost = _sock.Information.RemoteAddress.ToString();
                this.RemotePort = _sock.Information.RemotePort;
#endif

                //log
                DebugEx.TraceLog("YPServer (socks) new connection from " + RemoteHost + ":" + RemotePort + " (Secure=" + isSecured + ",SSL=" + sslProtocol + ")");

                //setup stream
#if NETFX
                SetupStream(_netstream);
#else
                SetupStream();
#endif

                //all ok
                return true;
            }
            catch (Exception ex)
            {
                DebugEx.TraceError(ex, "Unhandled exception caught");
                return false;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override void onClose()
        {
            try
            {
                if (Server != null)
                    Server.onChannelClose(this);
            }
            catch (Exception ex) { DebugEx.TraceError(ex, "ServerChannel onClose() unhandled exception"); }
        }
        //------------------------------------------------------------------------------------------------------------------------
#if DEBUG
        ~ServerChannel()
        {
            DebugEx.TraceLog("YPC ServerChannel destructed (name=" + Name + ")");
        }
#endif
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}