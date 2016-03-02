using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.YPChannel;
using Yodiwo.Media.Video.Sink.MjpegServer;
using Yodiwo.API.MediaStreaming;
using Yodiwo.Media.Video.Sink;
using System.Security.Cryptography.X509Certificates;

namespace Yodiwo.Services.Media.Video
{

    public class YVideoServer
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        //ypserver
        Yodiwo.YPChannel.Transport.Sockets.Server server;
        //ypchannel port
        public int YPort;
        //map videotokens with video feeds
        Dictionary<string, IVideoSink> videofeeds = new Dictionary<string, IVideoSink>();
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        public YVideoServer(int YPort = 5969)
        {
            this.YPort = YPort;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public void Start(X509Certificate2 certificate)
        {
            //start sip server
            DebugEx.TraceLog("start yp server for mjpeg streaming ....");
            //create protocol
            var proto = new YPChannel.Protocol()
            {
                Version = 1,
                ProtocolDefinitions = new List<YPChannel.Protocol.MessageTypeGroup>()
                    {
                        new YPChannel.Protocol.MessageTypeGroup() {GroupName=Yodiwo.API.MediaStreaming.Video.ApiGroupName, MessageTypes=Yodiwo.API.MediaStreaming.Video.ApiMessages }
                    },
            };
            //create server
            server = new Yodiwo.YPChannel.Transport.Sockets.Server(proto);
            server.OnNewChannel += (_server, channel) =>
            {
                channel.Name = "YPMjegServer";
                channel.NegotiationHandler = serverNegotiation;
                channel.OnOpenEvent += this.serverChannel_OnOpenEvent;
                channel.OnMessageReceived += this.serverOnMessageReceived;
            };
            //start ypserver
            server.Start(this.YPort, certificate: certificate);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void AddNewFeed(string mjpegtoken, IVideoSink videofeed)
        {

            lock (this)
            {
                //add to dictionary
                videofeeds.Add(mjpegtoken, videofeed);
            }

        }
        //------------------------------------------------------------------------------------------------------------------------
        public bool serverNegotiation(Yodiwo.YPChannel.Channel channel)
        {
            //requires authentication
            var resp = channel.SendRequest<VideoAuthenticationResponse>(new VideoAuthenticationRequest());
            if (resp == null)
                return false;
            else
            {
                //handle response
                if (!videofeeds.ContainsKey(resp.videotoken))
                    return false;
                else
                {
                    //create context
                    var ctx = new ChannelContext()
                    {
                        videoInfo = videofeeds[resp.videotoken],
                    };
                    channel.Tags.Add(typeof(ChannelContext), ctx);
                    ctx.Receiving = true;
                    return true;
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void serverOnMessageReceived(Yodiwo.YPChannel.Channel channel, Yodiwo.YPChannel.YPMessage msg)
        {
            //get context
            var ctx = channel.Tags.TryGetOrDefault(typeof(ChannelContext)) as ChannelContext;
            //handle data
            if (msg.Payload is VideoDataResp)
            {
                var resp = (msg.Payload as VideoDataResp);
            }
            else if (msg.Payload is VideoData)
            {
                var resp = (msg.Payload as VideoData);
                //add bitmap to videofeed
                if (ctx.Receiving && ctx.videoInfo != null && ctx.videoInfo.IsActive)
                    ctx.videoInfo.AddFrame(resp.linearBytes);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void serverChannel_OnOpenEvent(Yodiwo.YPChannel.Channel channel)
        {
            DebugEx.TraceLog("A mjpeg recorder has been connected to me");
            Task.Run(() =>
            {
                var req = new VideoDataReq()
                {
                    vflow = VideoFlow.Start
                };
                channel.SendRequest(req);
            });
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }

    #region HelperClass
    class ChannelContext
    {
        public IVideoSink videoInfo;
        public bool Receiving = false;
    }
    #endregion
}

