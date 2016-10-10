using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
#if NETFX
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
#elif UNIVERSAL
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
        public ServerChannel(Server Server, Protocol[] Protocols, ChannelSerializationMode SupportedChannelSerializationModes, ChannelSerializationMode PreferredChannelSerializationModes, Socket _sock)
            : base(Protocols, ChannelRole.Server, SupportedChannelSerializationModes, PreferredChannelSerializationModes)
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
#elif UNIVERSAL
        public virtual bool SetupServerSocket()
#endif
        {
            try
            {
                //keep
                bool isSecured = false;
                string sslProtocol = "";

                //check packer
                if (SupportedChannelSerializationModes.HasFlag(ChannelSerializationMode.MessagePack))
                    DebugEx.Assert(MsgPack != null, "MessagePack serializer not provided");

                //create network stream
#if NETFX
                //Stream _netstream = new BufferedStream(new NetworkStream(base._sock, true));
                Stream _netstream = new NetworkStream(base._sock, true);

                //Wrap with a secure stream?
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
                        try { Close("Certificate not accepted, " + msg); } catch { }
                        try { sslstream.Close(); base._sock.Dispose(); } catch { }
                        try { _netstream.Close(); _netstream.Dispose(); } catch { }
                        try { _sock.Close(); _sock.Dispose(); } catch { }
                        return false; //failed
                    }
                }
#endif

                //read clients packers
                var clientPackers = ChannelSerializationMode.Unkown;
                var clientPreferredPackers = ChannelSerializationMode.Unkown;
#if NETFX
                clientPackers = (ChannelSerializationMode)_netstream.ReadByte();
                clientPreferredPackers = (ChannelSerializationMode)_netstream.ReadByte();
#elif UNIVERSAL
                clientPackers = (ChannelSerializationMode)_sock.InputStream.AsStreamForRead().ReadByte();
                clientPreferredPackers = (ChannelSerializationMode)_sock.InputStream.AsStreamForRead().ReadByte();
#endif

                //filter packers
                clientPackers = clientPackers & SupportedChannelSerializationModes;
                clientPreferredPackers = clientPackers & clientPreferredPackers;
                var serverPreferredPackers = clientPackers & PreferredChannelSerializationModes;
                var commonPreferredPackers = clientPreferredPackers & serverPreferredPackers;

                //choose packer
                if ((_ChannelSerializationMode = _choosePacker(commonPreferredPackers)) == ChannelSerializationMode.Unkown &&
                    (_ChannelSerializationMode = _choosePacker(clientPreferredPackers)) == ChannelSerializationMode.Unkown &&
                    (_ChannelSerializationMode = _choosePacker(serverPreferredPackers)) == ChannelSerializationMode.Unkown &&
                    (_ChannelSerializationMode = _choosePacker(clientPackers)) == ChannelSerializationMode.Unkown)
                {
                    DebugEx.TraceError("Could not decide on packer.");
                    try { Close("Could not decide on packer."); } catch { }
#if NETFX
                    try { _netstream?.Close(); _netstream?.Dispose(); } catch { }
                    try { _sock?.Close(); _sock?.Dispose(); } catch { }
#elif UNIVERSAL
                    try { _sock?.Dispose(); } catch { }
#endif
                    return false; //failed
                }

                //write packer
#if NETFX
                var _nodelay = _sock.NoDelay;
                _sock.NoDelay = true; //Disable the Nagle Algorithm
                _netstream.WriteByte((byte)_ChannelSerializationMode);
                _sock.NoDelay = _nodelay; //Restore (default:enable) the Nagle Algorithm
#elif UNIVERSAL
                {
                    var wStream = _sock.OutputStream.AsStreamForWrite();
                    wStream.WriteByte((byte)_ChannelSerializationMode);
                    wStream.Flush();
                }
#endif

                //setup info
                try
                {
#if NETFX
                    this.LocalHost = base._sock.LocalEndPoint.GetIPAddress().ToString();
                    this.RemotePort = _sock.LocalEndPoint.GetPort().ToStringInvariant();
#elif UNIVERSAL
                    this.LocalHost = _sock.Information.LocalAddress.ToString();
                    this.RemotePort = _sock.Information.LocalPort;
#endif
                }
                catch { }

                //setup info
#if NETFX
                this.RemoteHost = base._sock.RemoteEndPoint.GetIPAddress().ToString();
                this.RemotePort = _sock.RemoteEndPoint.GetPort().ToStringInvariant();
#elif UNIVERSAL
                this.LocalHost = _sock.Information.RemoteAddress.ToString();
                this.RemotePort = _sock.Information.RemotePort;
#endif

                //log
                DebugEx.TraceLog("YPServer (socks) new connection from " + RemoteHost + ":" + RemotePort + " (Secure=" + isSecured + ",SSL=" + sslProtocol + ")");

                //setup stream
#if NETFX
                SetupStream(_netstream);
#elif UNIVERSAL
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
        protected override void onClose(string Message)
        {
            try
            {
                base.onClose(Message);

                if (Server != null)
                    Server.onChannelClose(this);
            }
            catch (Exception ex) { DebugEx.TraceError(ex, "ServerChannel onClose() unhandled exception"); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        ChannelSerializationMode _choosePacker(ChannelSerializationMode modes)
        {
            if (modes.HasFlag(ChannelSerializationMode.Json))
                return ChannelSerializationMode.Json;
            else if (modes.HasFlag(ChannelSerializationMode.MessagePack))
                return ChannelSerializationMode.MessagePack;
            else
                return ChannelSerializationMode.Unkown;
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