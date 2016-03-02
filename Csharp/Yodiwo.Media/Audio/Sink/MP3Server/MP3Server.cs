using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace Yodiwo.Media.Audio.Sink.MP3Server
{
    public class MP3Server
    {

        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        private Thread _ListenerThread = null;
        private bool IsRunning = false;
        public string ip;
        public int Port;
        private Dictionary<string, AudioFeed> audiofeeds = new Dictionary<string, AudioFeed>();
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------

        public void Start(int port = 7090)
        {
            lock (this)
            {
                //set flag
                IsRunning = true;
                this.Port = port;

                //start Listener
                _ListenerThread = new Thread(new ParameterizedThreadStart(ServerThread));
                _ListenerThread.IsBackground = true;
                _ListenerThread.Start(port);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

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
                    // blocking until a new connected client
                    var client = Server.Accept();
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ClientThread), client);
                }
            }
            catch { }

            this.Stop();
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
                    lock (audiofeeds)
                    {
                        //stop and dispose feeds
                        foreach (var feed in audiofeeds.Values)
                        {
                            feed._InternalStop();
                            feed.Dispose();
                        }

                        //clear all feeds
                        audiofeeds.Clear();
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        private void ClientThread(object client)
        {
            var socket = (Socket)client;
            Console.WriteLine(string.Format("New client from " + socket.RemoteEndPoint.ToString()));
            //get feed name request
            string feedName = string.Empty;
            using (var netStream = new NetworkStream(socket, false))
            using (var reader = new StreamReader(netStream))
            {
                var req = reader.ReadLine();
                try
                {
                    feedName = req.Split(new[] { ' ' })[1];
                    if (feedName[0] == '/')
                        feedName = feedName.Substring(1);
                }
                catch { }
            }

            //find feed
            AudioFeed feed = null;
            lock (audiofeeds)
                if (!string.IsNullOrEmpty(feedName))
                    feed = audiofeeds.TryGetOrDefault(feedName);

            //give client to feed
            if (feed != null)
            {
                DebugEx.TraceLog("add new client");
                feed.AddClient(socket);
            }
            else
                DebugEx.TraceLog("Client requested feed not found");
        }

        //------------------------------------------------------------------------------------------------------------------------

        public AudioFeed CreateAudioFeed(string audioFeedName)
        {
            // when a new audio device (i.e. mic is able to stream its audio input to the mp3streaming server
            // a new feed is created
            AudioFeed af;
            lock (audiofeeds)
                if (audiofeeds.TryGetValue(audioFeedName, out af) == false)
                {
                    af = new AudioFeed(this, audioFeedName);
                    audiofeeds.Add(audioFeedName, af);
                }
            return af;
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void DestroyAudioFeed(string audioFeedName)
        {
            //find feed
            AudioFeed af = null;
            lock (audiofeeds)
                if (audiofeeds.TryGetValue(audioFeedName, out af))
                    audiofeeds.Remove(audioFeedName); //remove
            //stop
            if (af != null)
            {
                af._InternalStop();
                af.Dispose();
            }
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
