using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using Yodiwo;


namespace Yodiwo.Media.Video.Sink.MjpegServer
{
    /// <summary>
    /// Provides a streaming server that can be used to stream any images source
    /// to any client.
    /// </summary>
    public class MjpegServer : IDisposable
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        private Thread _ListenerThread = null;
        private Dictionary<string, VideoFeed> videofeeds = new Dictionary<string, VideoFeed>();
        public int Port;
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the interval in milliseconds (or the delay time) between 
        /// the each image and the other of the stream (the default is . 
        /// </summary>
        public int Interval { get; set; }
        //------------------------------------------------------------------------------------------------------------------------
        public bool IsRunning = false;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        public MjpegServer(int Port = 5859)
        {
            this.Interval = 0;
            this.Port = Port;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Starts the server to accepts any new connections on the specified port.
        /// </summary>
        /// <param name="port"></param>
        public void Start()
        {
            lock (this)
            {
                //set flag
                IsRunning = true;

                //start listener
                _ListenerThread = new Thread(new ParameterizedThreadStart(ServerThread));
                _ListenerThread.IsBackground = true;
                _ListenerThread.Start(this.Port);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void Stop()
        {
            if (IsRunning)
            {
                lock (this)
                {
                    //close server
                    IsRunning = false;
                    try
                    {
                        _ListenerThread.Join();
                        _ListenerThread.Abort();
                    }
                    finally { _ListenerThread = null; }

                    //cleanup feeds
                    lock (videofeeds)
                    {
                        //stop and dispose feeds
                        foreach (var feed in videofeeds.Values)
                        {
                            feed._InternalStop();
                            feed.Dispose();
                        }

                        //clear all feeds
                        videofeeds.Clear();
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// This the main thread of the server that serves all the new 
        /// connections from clients.
        /// </summary>
        /// <param name="port"></param>
        private void ServerThread(object port)
        {
            try
            {
                //start server
                Socket Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Server.Bind(new IPEndPoint(IPAddress.Any, (int)port));
                Server.Listen(10);

                //wait for new clients
                while (IsRunning)
                {
                    var client = Server.Accept();
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ClientThread), client);
                }
            }
            catch (Exception ex)
            {
                DebugEx.TraceErrorException(ex);
            }

            this.Stop();
        }

        //------------------------------------------------------------------------------------------------------------------------

        public VideoFeed CreateVideoFeed(string videoFeedName)
        {
            VideoFeed vf;
            lock (videofeeds)
                if (videofeeds.TryGetValue(videoFeedName, out vf) == false)
                {
                    vf = new VideoFeed(this, videoFeedName);
                    videofeeds.Add(videoFeedName, vf);
                }
            return vf;
        }
        //------------------------------------------------------------------------------------------------------------------------

        public void DestroyVideoFeed(VideoFeed videofeed)
        {
            DestroyVideoFeed(videofeed.Name);
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void DestroyVideoFeed(string videoFeedName)
        {
            //find feed
            VideoFeed vf = null;
            lock (videofeeds)
                if (videofeeds.TryGetValue(videoFeedName, out vf))
                    videofeeds.Remove(videoFeedName); //remove
            //stop
            if (vf != null)
            {
                vf._InternalStop();
                vf.Dispose();
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        private void ClientThread(object client)
        {
            //get client socket
            var socket = (Socket)client;
            DebugEx.TraceLog(string.Format("New client from " + socket.RemoteEndPoint.ToString()));

            //get feed name request
            string feedName = string.Empty;
            using (var netStream = new NetworkStream(socket, false))
            using (var reader = new StreamReader(netStream))
            {
                var req = reader.ReadLine();
                try
                {
                    if (req != null)
                    {
                        feedName = req.Split(new[] { ' ' })[1];
                        if (feedName[0] == '/')
                            feedName = feedName.Substring(1);
                    }
                }
                catch (Exception ex)
                {
                    DebugEx.TraceError(ex.Message);
                }
            }

            //find feed
            VideoFeed feed = null;
            lock (videofeeds)
                if (!string.IsNullOrEmpty(feedName))
                    feed = videofeeds.TryGetOrDefault(feedName);

            //give client to feed
            if (feed != null)
                feed.AddClient(socket);
            else
                DebugEx.TraceLog("Client requested feed not found");
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void Dispose()
        {
            try
            {
                if (IsRunning)
                    Stop();
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Dispose failed");
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
