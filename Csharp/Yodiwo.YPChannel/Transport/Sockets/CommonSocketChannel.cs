using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if NETFX
using System.Net.Sockets;
#else
using Windows.Networking.Sockets;
#endif
using System.Text;
using System.Threading.Tasks;

#if UNIVERSAL
using Socket = Windows.Networking.Sockets.StreamSocket;
#endif

namespace Yodiwo.YPChannel.Transport.Sockets
{
    public abstract class CommonSocketChannel : Channel, IDisposable
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        protected internal Socket _sock;
#if NETFX
        protected internal Stream netstream;
#endif
        protected internal StatisticsStream streamIn;
        protected internal StatisticsStream streamOut;
        //------------------------------------------------------------------------------------------------------------------------
        public string LocalHost = string.Empty;
        public string LocalPort = string.Empty;
        public override string LocalIdentifier { get { return LocalHost + ":" + LocalPort; } }
        //------------------------------------------------------------------------------------------------------------------------
        public string RemoteHost = string.Empty;
        public string RemotePort = string.Empty;
        public override string RemoteIdentifier { get { return RemoteHost + ":" + RemotePort; } }
        //------------------------------------------------------------------------------------------------------------------------
        public override Int64 ReceivedBytesTotal { get { return streamIn == null ? -1 : streamIn.TotalBytesRead; } set { if (streamIn != null) streamIn.TotalBytesRead = value; } }
        public override Int64 TransmittedBytesTotal { get { return streamIn == null ? -1 : streamIn.TotalBytesWritten; } set { if (streamIn != null) streamIn.TotalBytesWritten = value; } }
        public override Int64 ReceivedBytesPerSecond { get { return streamIn == null ? -1 : streamIn.ReadBytesPerSecond; } }
        public override Int64 TransmittedBytesPerSecond { get { return streamIn == null ? -1 : streamIn.WriteBytesPerSecond; } }
        //------------------------------------------------------------------------------------------------------------------------
        object readerLock = new object();
        object writerLock = new object();
        //------------------------------------------------------------------------------------------------------------------------
#if NETFX
        public bool NoDelay { get { return _sock.NoDelay; } set { _sock.NoDelay = value; } }
#else
        public bool NoDelay { get { return _sock.Control.NoDelay; } set { _sock.Control.NoDelay = value; } }
#endif
        //------------------------------------------------------------------------------------------------------------------------
        public bool IsDisposed { get; protected set; }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public CommonSocketChannel(Protocol[] Protocols, ChannelRole ChannelRole, ChannelSerializationMode ChannelSerializationMode)
            : base(Protocols, ChannelRole, ChannelSerializationMode, true)
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
#if NETFX
        protected void SetupStream(Stream baseStream)
#else
        protected void SetupStream()
#endif
        {
#if NETFX
            this.netstream = baseStream;
            this.streamIn = new StatisticsStream(netstream, false);
            this.streamOut = new StatisticsStream(netstream, false);
#else
            this.streamIn = new StatisticsStream(_sock.InputStream.AsStreamForRead(), false);
            this.streamOut = new StatisticsStream(_sock.OutputStream.AsStreamForWrite(), false);
#endif
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override void _flush()
        {
            try
            {
                if (streamIn != null)
                    streamIn.Flush();
                if (streamOut != null)
                    streamOut.Flush();
            }
            catch { }
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override YPMessagePacked _readPackedMessage()
        {
            //check connection
            if (streamIn == null || (!IsNegotiating && !IsOpen))
                return null;

            lock (readerLock)
            {
                try
                {
                    //read size
                    var rSize = new byte[4];
                    int r = 0;
                    while (r < 4 && r >= 0)
                        r += streamIn.Read(rSize, r, rSize.Length - r);
                    if (r < 0)
                        return null;
                    var packet_size = BitConverter.ToUInt32(rSize, 0);
                    //read packet
                    var packet = new byte[packet_size];
                    r = 0;
                    while (r < packet_size)
                    {
                        var _r = streamIn.Read(packet, r, (int)(packet_size - r));
                        if (_r < 0)
                            return null;
                        r += _r;
                    }
                    //read packet
                    using (var memStream = new MemoryStream(packet))
                    {
                        //unpack
                        if (ChannelSerializationMode == YPChannel.ChannelSerializationMode.MessagePack)
                            return MessagePackEncapsulationSerializer.Unpack(memStream);
                        else if (ChannelSerializationMode == YPChannel.ChannelSerializationMode.Json)
                        {
                            using (var streamJsonReader = new StreamReader(this.streamIn, Encoding.UTF8, false, 1024, true))
                            using (var jsonTextReader = new Newtonsoft.Json.JsonTextReader(streamJsonReader))
                                return JSONEncapsulationReadSerializer.Deserialize<YPMessagePacked>(jsonTextReader);
                        }
                        else
                        {
                            DebugEx.Assert("Unkown serialization method");
                            throw new Exception("Unkown serialization method");
                        }
                    }
                }
                catch { return null; }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override void _sendPackedMessage(YPMessagePacked msg)
        {
            //check connection
            if (streamOut == null || (!IsNegotiating && !IsOpen))
                return;

            lock (writerLock)
            {
                try
                {
                    //pack
                    byte[] enc_msg;
                    if (ChannelSerializationMode == YPChannel.ChannelSerializationMode.MessagePack)
                        enc_msg = MessagePackEncapsulationSerializer.PackSingleObject(msg);
                    else if (ChannelSerializationMode == YPChannel.ChannelSerializationMode.Json)
                    {
                        var str = msg.ToJSON(HtmlEncode: false);
                        enc_msg = System.Text.Encoding.UTF8.GetBytes(str);
                    }
                    else
                    {
                        DebugEx.Assert("Unkown serialization method");
                        throw new Exception("Unkown serialization method");
                    }
                    //send packet size
                    var tSize = BitConverter.GetBytes((UInt32)enc_msg.Length);
                    streamOut.Write(tSize, 0, tSize.Length);
                    //send packet
                    streamOut.Write(enc_msg, 0, enc_msg.Length);
                    streamOut.Flush();
                }
                catch { Close(); }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override void onClose()
        {
            try
            {
                //close all
                try
                {
                    if (streamIn != null)
                    {
#if NETFX
                        streamIn.Close();
#endif
                        streamIn.Dispose();
                    }
                }
                catch { }

                try
                {
                    if (streamOut != null)
                    {
#if NETFX
                        streamOut.Close();
#endif
                        streamOut.Dispose();
                    }
                }
                catch { }

                try
                {
#if NETFX
                    if (_sock.Connected)
                        _sock.Disconnect(true);
#endif
                }
                catch { }
                //null them
                streamIn = null;
                streamOut = null;
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Exception while disconnecting"); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Dispose()
        {
            if (IsDisposed)
                return;
            else
                IsDisposed = true;

            //Close channel
            if (State != ChannelStates.Closed)
                try { Close(); } catch { }

            //disconnect/close/dispose sockets
            if (_sock != null)
            {
#if NETFX
                try { if (_sock.Connected) _sock.Disconnect(false); } catch { }
                try { _sock.Close(3); } catch { }
#endif
                try { _sock.Dispose(); } catch { }
            }

            //release reference
            _sock = null;
        }
        //------------------------------------------------------------------------------------------------------------------------
        ~CommonSocketChannel()
        {
            try
            {
                Dispose();
            }
            catch (Exception ex)
            {
                DebugEx.TraceError(ex, "YPChannel Socket CommonSocketChannel() destructor caught unhandled exception");
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

    }
}
