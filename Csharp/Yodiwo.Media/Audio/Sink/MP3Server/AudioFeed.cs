using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Yodiwo.Media.Audio.Sink.MP3Server
{
    public class AudioFeed : IDisposable, IAudioSink
    {
        #region Variables        
        //------------------------------------------------------------------------------------------------------------------------

        class ClientInfo
        {
            public Socket Socket;
            public Mp3Writer Writer;
        }

        //------------------------------------------------------------------------------------------------------------------------

        public readonly MP3Server Server;
        public readonly string Name;
        //each audio feed has a set of clients connected to it
        private Dictionary<Socket, ClientInfo> _Clients = new Dictionary<Socket, ClientInfo>();
        public bool IsDisposed;
        private RequestQueueConsumer<byte[]> requestQueueConsumer;

        //------------------------------------------------------------------------------------------------------------------------
        public bool IsActive { get; private set; }
        //------------------------------------------------------------------------------------------------------------------------

        #endregion


        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        public AudioFeed(MP3Server Server, string Name)
        {
            this.Server = Server;
            this.Name = Name;
            requestQueueConsumer = new RequestQueueConsumer<byte[]>(servicequeuecb);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public void Start()
        {
            requestQueueConsumer.Start();
            requestQueueConsumer.IsPaused = false;
            requestQueueConsumer.MaxQueuedRequests = 10;
            IsActive = true;
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void AddClient(Socket ClientSocket)
        {
            Mp3Writer wr;
            try
            {
                wr = new Mp3Writer(new NetworkStream(ClientSocket, true));
                //write the http audio header to the network stream
                wr.WriteHeader();
                //write mp3 header
                wr.WriteMp3Header();
                //client is now ready for streaming
                //each client is chatracterized by its socket and its mp3writer
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
                DebugEx.TraceError(ex.Message);
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

        internal void _InternalStop()
        {

            //clear clients
            lock (_Clients)
                if (_Clients.Count > 0)
                    foreach (var client in _Clients.Keys.ToArray())
                        RemoveClient(client);
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void Stop()
        {
            Server.DestroyAudioFeed(this.Name);
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

        public void AddPCMdata(byte[] data)
        {
            if (!IsDisposed)
                requestQueueConsumer.Enqueue(data);
        }

        //------------------------------------------------------------------------------------------------------------------------

        private void servicequeuecb(byte[] pcmsamples)
        {
            //broadcast pcmdata
            lock (_Clients)
                foreach (var audioclient in _Clients)
                {
                    try
                    {
                        audioclient.Value.Writer.Write(pcmsamples);
                    }
                    catch (Exception ex)
                    {
                        //remove client that caused the exception
                        RemoveClient(audioclient.Key);
                        break; //because we changed the collection
                    }
                }
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
