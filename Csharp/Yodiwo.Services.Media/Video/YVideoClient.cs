using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.Media.Video.Source;
using Yodiwo.YPChannel;
using Yodiwo.API.MediaStreaming;
using System.Drawing;
using System.IO;

namespace Yodiwo.Services.Media.Video
{
    public class YVideoClient
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        IVideoSource videosource;
        string videotoken;
        public Yodiwo.YPChannel.Transport.Sockets.Client yclient;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        public YVideoClient(IVideoSource videosource)
        {
            this.videosource = videosource;
            this.videosource.OnFrameCaptured += Videosource_OnFrameCaptured;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------

        private void Videosource_OnFrameCaptured(object sender, EventArgs e)
        {
            VideoEventArgs myargs = e as VideoEventArgs;
            Bitmap bmp = myargs.bitmap;
            MemoryStream ms = new System.IO.MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            var resp = new VideoData()
            {
                linearBytes = ms.ToArray()
            };
            yclient.SendMessage(resp);
        }

        //------------------------------------------------------------------------------------------------------------------------

        public SimpleActionResult Connect(string server, string videotoken, int Port = 5969, bool SecureYPC = false)
        {
            //start a YPclient
            DebugEx.TraceLog("Connect mic client to mic server");
            this.videotoken = videotoken;
            //create protocol
            var proto = new YPChannel.Protocol()
            {
                Version = 1,
                ProtocolDefinitions = new List<Protocol.MessageTypeGroup>()
                    {
                        new Protocol.MessageTypeGroup() {GroupName=Yodiwo.API.MediaStreaming.Video.ApiGroupName, MessageTypes=Yodiwo.API.MediaStreaming.Video.ApiMessages }
                    },
            };
            //create channel
            yclient = new Yodiwo.YPChannel.Transport.Sockets.Client(proto) { Name = "VideoMediaClient" };
            yclient.OnMessageReceived += clientOnMessageReceived;
            return yclient.Connect(server, Port, SecureYPC, "*.yodiwo.com");
        }

        //------------------------------------------------------------------------------------------------------------------------

        void clientOnMessageReceived(Yodiwo.YPChannel.Channel channel, Yodiwo.YPChannel.YPMessage msg)
        {
            if (msg.Payload is VideoDataReq)
            {
                var req = (msg.Payload as VideoDataReq);
                if (req.vflow == VideoFlow.Start)
                {
                    videosource.Start();
                }
                else if (req.vflow == VideoFlow.Stop)
                {
                    videosource.Stop();
                }
            }
            else if (msg.Payload is VideoAuthenticationRequest)
            {
                var resp = new VideoAuthenticationResponse()
                {
                    videotoken = this.videotoken
                };
                channel.SendResponse(resp, msg.MessageID);
                DebugEx.TraceLog("Sening response for VideoAuthenticationResponse , token=" + this.videotoken);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public StatusCode TearDown()
        {
            try
            {
                videosource.Stop();
                yclient.Close();
                return StatusCode.Success;
            }
            catch (Exception ex)
            {
                return StatusCode.Error;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        #endregion

    }
}
