using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Sockets;
using Yodiwo;
using System.Drawing;
using Yodiwo.Media.Video.Sink;

namespace Yodiwo.Media.Video.Sink.MjpegServer
{
    public class VideoFeed : IDisposable, IVideoSink
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public readonly MjpegServer Server;
        //------------------------------------------------------------------------------------------------------------------------
        class ClientInfo
        {
            public Socket Socket;
            public MjpegWriter Writer;
        }
        private Dictionary<Socket, ClientInfo> _Clients = new Dictionary<Socket, ClientInfo>();
        //------------------------------------------------------------------------------------------------------------------------
        private RequestQueueConsumer<Image> requestQueueConsumer;
        //------------------------------------------------------------------------------------------------------------------------
        public readonly string Name;
        //------------------------------------------------------------------------------------------------------------------------
        MemoryStream ms;
        //------------------------------------------------------------------------------------------------------------------------
        public bool IsDisposed = false;
        //------------------------------------------------------------------------------------------------------------------------
        public bool IsActive { get; private set; }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        internal VideoFeed(MjpegServer Server, string Name)
        {
            this.Server = Server;
            this.Name = Name;
            requestQueueConsumer = new RequestQueueConsumer<Image>(servicequeuecb);
            Start();
        }

        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions

        public void Start()
        {
            IsActive = true;
            requestQueueConsumer.Start();
            requestQueueConsumer.IsPaused = false;
            requestQueueConsumer.MaxQueuedRequests = 10;
        }

        //------------------------------------------------------------------------------------------------------------------------
        public void AddClient(Socket ClientSocket)
        {
            //prepare MjpegWriter and write header for client
            MjpegWriter wr;
            try
            {
                //create writer
                wr = new MjpegWriter(new NetworkStream(ClientSocket, true));
                // Writes the response header to the client.
                wr.WriteHeader();

                //client is now ready for streaming
                var clientInfo = new ClientInfo()
                {
                    Socket = ClientSocket,
                    Writer = wr,
                };
                lock (_Clients)
                    if (!IsDisposed)
                        _Clients.Add(ClientSocket, clientInfo);
                    else
                    {
                        //we are disposed, so close client socket
                        try
                        {
                            ClientSocket.Close();
                            ClientSocket.Dispose();
                        }
                        catch { }
                    }
            }
            catch (Exception ex)
            {
                DebugEx.TraceLog("Streaming Error: " + ex.Message);
                //close socket on error
                try
                {
                    ClientSocket.Close();
                    ClientSocket.Dispose();
                }
                catch { }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void RemoveClient(Socket ClientSocket)
        {
            lock (_Clients)
            {
                //find client to be removed
                ClientInfo clientInfo;
                if (_Clients.TryGetValue(ClientSocket, out clientInfo))
                {
                    try
                    {
                        //dispose writer
                        clientInfo.Writer.Dispose();
                        //close socket
                        ClientSocket.Close();
                        ClientSocket.Dispose();
                    }
                    catch { }
                    //remove from dictionary
                    _Clients.Remove(ClientSocket);
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void AddFrame(Image Frame)
        {
            if (!IsDisposed)
                requestQueueConsumer.Enqueue(Frame);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void AddFrame(Byte[] bytes)
        {
            var ms = new MemoryStream();
            ms.Write(bytes, 0, bytes.Length);
            ms.Position = 0;
            Bitmap bitmap = new Bitmap(ms);
            if (!IsDisposed)
                requestQueueConsumer.Enqueue(bitmap);
        }
        //------------------------------------------------------------------------------------------------------------------------

        private void servicequeuecb(Image newFrame)
        {
            ms = new MemoryStream();
            if (newFrame != null && !IsDisposed)
            {
                //get stream
                ms.SetLength(0);
                newFrame.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                //broadcast frame
                lock (_Clients)
                    foreach (var entry in _Clients)
                    {
                        ms.Position = 0;
                        try
                        {
                            entry.Value.Writer.Write(ms);
                        }
                        catch
                        {
                            //remove client that caused the exception
                            RemoveClient(entry.Key);
                            break; //because we changed the collection
                        }
                    }

                //dispose frame
                newFrame.Dispose();
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        internal void _InternalStop()
        {
            //clear queue
            lock (this)
            {
                requestQueueConsumer.Clear();
                //..
            }

            //clear clients
            lock (_Clients)
                if (_Clients.Count > 0)
                    foreach (var client in _Clients.Keys.ToArray())
                        RemoveClient(client);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Stop()
        {
            IsActive = false;
            Server.DestroyVideoFeed(this);
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

                //stop feed
                _InternalStop();
            }
            catch { }
        }
        //------------------------------------------------------------------------------------------------------------------------

        public void Flush()
        { }
        //------------------------------------------------------------------------------------------------------------------------

        public void Clear()
        { }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
