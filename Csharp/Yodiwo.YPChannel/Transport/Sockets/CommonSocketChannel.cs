using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
#if NETFX
using System.Net.Sockets;
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
#if NETFX
        public bool IsConnected => _sock?.Connected ?? false;
#else
        public bool IsConnected => _sock != null;
#endif
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
        public bool NoDelay { get { return _sock?.NoDelay ?? true; } set { try { if (_sock != null) _sock.NoDelay = value; } catch { } } }
#elif UNIVERSAL
        public bool NoDelay { get { return _sock?.Control.NoDelay ?? true; } set { try { if (_sock != null) _sock.Control.NoDelay = value; } catch { } } }
#endif
        //------------------------------------------------------------------------------------------------------------------------
        public bool IsDisposed { get; protected set; }
        //------------------------------------------------------------------------------------------------------------------------
        public const int MaxPacketSize = (1024 * 1024) * 10; // 10 mb
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public CommonSocketChannel(Protocol[] Protocols, ChannelRole ChannelRole, ChannelSerializationMode SupportedChannelSerializationModes, ChannelSerializationMode PreferredChannelSerializationModes)
            : base(Protocols, ChannelRole, SupportedChannelSerializationModes, PreferredChannelSerializationModes, true)
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
#if NETFX
        protected void SetupStream(Stream baseStream)
#elif UNIVERSAL
        protected void SetupStream()
#endif
        {
#if NETFX
            this.netstream = baseStream;
            this.streamIn = new StatisticsStream(netstream, false);
            this.streamOut = new StatisticsStream(netstream, false);
#elif UNIVERSAL
            this.streamIn = new StatisticsStream(_sock.InputStream.AsStreamForRead(), false);
            this.streamOut = new StatisticsStream(_sock.OutputStream.AsStreamForWrite(), false);
#endif
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override void _flush(int Timeout)
        {
            TaskEx.RunSafe(() =>
            {
                try { streamIn?.Flush(); } catch { }
                try { streamOut?.Flush(); } catch { }
            }).Wait(Timeout);
        }
        //------------------------------------------------------------------------------------------------------------------------
        bool _reader(byte[] buffer, int offset, int count)
        {
            int r = 0;
            int penalty = 0;
            while (r < count && r >= 0)
            {
                var _r = streamIn.Read(buffer, offset + r, count - r);
                if (_r == 0)
                {
                    //penalty for zero read
                    penalty++;
                    Thread.Sleep(100);
                    //check that we are still connected
                    if (!IsConnected || penalty > 10)
                        return false;
                }
                else if (_r < 0)
                {
                    DebugEx.Assert("Channel packet read error (negative readcount)");
                    return false;
                }
                r += _r;

            }
            //check readcount
            if (r <= 0)
            {
                DebugEx.Assert("Channel packet read error (invalid readcount)");
                return false;
            }
            //finished ok
            return true;
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override YPMessagePacked _readPackedMessage()
        {
            //check connection
            if (streamOut == null || _State == ChannelStates.Closed)
                return null;

            lock (readerLock)
            {
                byte[] packet;
                PackedMessageFlags flags;
                try
                {
                    //read size
                    var rSize = new byte[4];
                    if (_reader(rSize, 0, rSize.Length) == false)
                    {
                        Close("Could not read packetSize");
                        return null;
                    }
                    //get flags - higher byte of size is flags
                    flags = (PackedMessageFlags)rSize[3];
                    rSize[3] = 0;
                    //get packet size
                    var packet_size = BitConverter.ToInt32(rSize, 0);
                    if (packet_size <= 0 && packet_size > MaxPacketSize)
                    {
                        DebugEx.Assert("Invalid packet size received (" + packet_size + ")");
                        Close("Invalid packet size received (" + packet_size + ")");
                        return null;
                    }
                    //read packet
                    packet = new byte[packet_size];
                    if (_reader(packet, 0, packet_size) == false)
                    {
                        Close("Packet reading failed");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    DebugEx.TraceErrorException(ex, reportIt: false);
                    Close("Packet reading failed (exception)");
                    return null;
                }
                try
                {
                    //decompress from gzip
                    if (flags.HasFlag(PackedMessageFlags.Compressed_GZip) || flags.HasFlag(PackedMessageFlags.Compressed_Deflate))
                    {
                        using (var outStream = new MemoryStream(packet.Length))
                        {
                            using (var memStream = new MemoryStream(packet))
                            {
                                if (flags.HasFlag(PackedMessageFlags.Compressed_Deflate))
                                    using (var deflStream = new DeflateStream(memStream, CompressionMode.Decompress, true))
                                        deflStream.CopyTo(outStream);
                                else if (flags.HasFlag(PackedMessageFlags.Compressed_GZip))
                                    using (var gzipStream = new GZipStream(memStream, CompressionMode.Decompress, true))
                                        gzipStream.CopyTo(outStream);
                                else
                                {
                                    DebugEx.Assert("Unkown compression");
                                    Close("Unkown compression");
                                }
                            }

                            outStream.Position = 0;
                            packet = outStream.ToArray();
                        }
                    }

                    //unpack
                    if (ChannelSerializationMode.HasFlag(YPChannel.ChannelSerializationMode.Json))
                    {
                        var str_msg = System.Text.Encoding.UTF8.GetString(packet);
                        return str_msg.FromJSON<YPMessagePacked>();
                    }
                    else if (ChannelSerializationMode.HasFlag(YPChannel.ChannelSerializationMode.MessagePack))
                        using (var memStream = new MemoryStream(packet))
                            return MsgPack?.Unpack_YPMessagePacked(memStream);
                    else
                    {
                        DebugEx.Assert("Unkown serialization method");
                        throw new Exception("Unkown serialization method");
                    }
                }
                catch { return null; }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override bool _sendPackedMessage(YPMessagePacked msg)
        {
            //check connection
            if (streamOut == null || _State == ChannelStates.Closed)
                return false;

            lock (writerLock)
            {
                //check connection
                if (streamOut == null || _State == ChannelStates.Closed)
                    return false;

                try
                {
                    //pack
                    var flags = PackedMessageFlags.None;
                    byte[] enc_msg;
                    if (ChannelSerializationMode.HasFlag(YPChannel.ChannelSerializationMode.Json))
                    {
                        var str = msg.ToJSON(HtmlEncode: false);
                        enc_msg = System.Text.Encoding.UTF8.GetBytes(str);
                    }
                    else if (ChannelSerializationMode.HasFlag(YPChannel.ChannelSerializationMode.MessagePack))
                        enc_msg = MsgPack?.Pack_YPMessagePacked(msg);
                    else
                    {
                        DebugEx.Assert("Unkown serialization method");
                        throw new Exception("Unkown serialization method");
                    }

                    //compress?
                    if (enc_msg.Length > CompressThreshold && use_Compression)
                    {
                        using (var outStream = new MemoryStream(enc_msg.Length))
                        {
                            using (var memStream = new MemoryStream(enc_msg))
                            {
                                if (use_Deflate)
                                {
                                    using (var deflStream = new DeflateStream(outStream, CompressionMode.Compress, true))
                                        memStream.CopyTo(deflStream);
                                    //enable deflate
                                    flags |= PackedMessageFlags.Compressed_Deflate;
                                }
                                else if (use_GZip)
                                {
                                    using (var gzipStream = new GZipStream(outStream, CompressionMode.Compress, true))
                                        memStream.CopyTo(gzipStream);
                                    //enable gzip
                                    flags |= PackedMessageFlags.Compressed_GZip;
                                }
                                else
                                {
                                    DebugEx.Assert("Unkown compression");
                                    Close("Unkown compression");
                                }
                            }
                            outStream.Position = 0;
                            enc_msg = outStream.ToArray();
                        }
                    }

                    //create buffer
                    var tSize = BitConverter.GetBytes((UInt32)enc_msg.Length);
                    tSize[3] = (byte)flags;  //higher byte of size is flags
                    var enc_msg2 = new byte[tSize.Length + enc_msg.Length];
                    Buffer.BlockCopy(tSize, 0, enc_msg2, 0, tSize.Length);
                    Buffer.BlockCopy(enc_msg, 0, enc_msg2, tSize.Length, enc_msg.Length);

                    //send
#if NETFX
                    var t = Task.Run(() => { try { streamOut?.Write(enc_msg2, 0, enc_msg2.Length); streamOut?.Flush(); return true; } catch { return false; } });
                    if (!t.Wait(15 * 1000) || t.GetResults() == false) //try to send (with a 15 second sock/stream write timeout)
                    {
                        Close("Failed to write to stream (" + (t.IsCompleted ? "error" : "timeout") + ")");
                        return false;
                    }
#elif UNIVERSAL
                    streamOut?.Write(enc_msg2, 0, enc_msg2.Length);
                    streamOut?.Flush();
#endif
                    return true;
                }
                catch (Exception ex)
                {
                    DebugEx.TraceErrorException(ex, reportIt: false);
                    Close("Packet send exception");
                    return false;
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override void onClose(string Message)
        {
            try
            {
                base.onClose(Message);

                //close all
                try
                {
                    if (streamIn != null)
                    {
#if NETFX
                        TaskEx.RunSafe(streamIn.Close).Wait(1000);
#endif
                        TaskEx.RunSafe(streamIn.Dispose).Wait(1000);
                    }
                }
                catch { }

                try
                {
                    if (streamOut != null)
                    {
#if NETFX
                        TaskEx.RunSafe(streamOut.Close).Wait(1000);
#endif
                        TaskEx.RunSafe(streamOut.Dispose).Wait(1000);
                    }
                }
                catch { }

#if NETFX
                try
                {
                    if (netstream != null)
                    {
                        TaskEx.RunSafe(netstream.Close).Wait(1000);
                        TaskEx.RunSafe(netstream.Dispose).Wait(1000);
                    }
                }
                catch { }
#endif
                try
                {
#if NETFX
                    if (_sock != null && _sock.Connected)
                        try { TaskEx.RunSafe(() => _sock.Disconnect(false)).Wait(1000); } catch { }

                    try { TaskEx.RunSafe(() => _sock?.Close(3)).Wait(5000); } catch { }
#endif
                    try { TaskEx.RunSafe(() => _sock?.Dispose()).Wait(1000); } catch { }
                }
                catch { }

                //null them
                streamIn = null;
                streamOut = null;
#if NETFX
                netstream = null;
#endif
                _sock = null;
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Exception while disconnecting"); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Dispose()
        {
            try
            {
                if (IsDisposed)
                    return;
                else
                    IsDisposed = true;

                //Close channel
                if (State != ChannelStates.Closed)
                    try { Close("Channel disposed"); } catch { }

                //suppress
                GC.SuppressFinalize(this);
            }
            catch (Exception ex) { DebugEx.Assert(ex); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        ~CommonSocketChannel()
        {
            try
            {
                DebugEx.TraceLog("YPC Client destructed (name=" + Name + ")");
                if (!IsDisposed)
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
