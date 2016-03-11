using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.MediaStreaming;
using System.Security.Cryptography;
using Yodiwo.Media.Audio.Sink.MP3Server;
using Yodiwo.Media.Audio.Sink;
using System.Security.Cryptography.X509Certificates;

namespace Yodiwo.Services.Media.Audio
{
    public class YAudioServer
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        //ypserver
        Yodiwo.YPChannel.Transport.Sockets.Server server;
        //ypport
        public int YPort;
        //------------------------------------------------------------------------------------------------------------------------
        //map audiotokens with audiofeeds
        Dictionary<string, AudioInfo> audiopipes = new Dictionary<string, AudioInfo>();
        //------------------------------------------------------------------------------------------------------------------------

        public class AudioInfo
        {
            public IAudioSink audiosink;

        }
        class ChannelContext
        {
            public AudioInfo audioInfo;
            public bool Receiving = false;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructor
        public YAudioServer(int yport = 6517)
        {
            this.YPort = yport;
        }
        #endregion

        #region Functions

        //------------------------------------------------------------------------------------------------------------------------
        public void startServer(X509Certificate2 certificate = null)
        {
            //start mp3 server
            DebugEx.TraceLog("start YP server for mp3 server ....");
            //create protocol
            var proto = new YPChannel.Protocol()
            {
                Version = 1,
                ProtocolDefinitions = new List<YPChannel.Protocol.MessageTypeGroup>()
                    {
                        new YPChannel.Protocol.MessageTypeGroup() {GroupName=Yodiwo.API.MediaStreaming.Audio.ApiGroupName, MessageTypes= Yodiwo.API.MediaStreaming.Audio.ApiMessages }
                    },
            };
            //create server
            server = new Yodiwo.YPChannel.Transport.Sockets.Server(proto);
            server.OnNewChannel += (_server, channel) =>
            {
                channel.Name = "YMp3Server";
                channel.NegotiationHandler = serverNegotiation;
                channel.OnOpenEvent += this.serverChannel_OnOpenEvent;
                channel.OnMessageReceived += this.serverOnMessageReceived;
            };
            server.Start(this.YPort, certificate: certificate);
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void AddNewFeed(string audiotoken, AudioInfo audioinfo)
        {
            lock (this)
            {
                if (!audiopipes.ContainsKey(audiotoken))
                    audiopipes.Add(audiotoken, audioinfo);
                else
                    audiopipes[audiotoken] = audioinfo;

            }
        }

        public void RemoveFeed(string audiotoken)
        {
            lock (this)
                try
                {
                    audiopipes.Remove(audiotoken);
                }
                catch (Exception ex)
                {
                    DebugEx.TraceErrorException(ex);
                }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public AudioInfo GetAudioPipe(string token)
        {
            if (audiopipes.ContainsKey(token))
                return audiopipes[token];
            else
                return null;
        }
        //------------------------------------------------------------------------------------------------------------------------

        public bool serverNegotiation(Yodiwo.YPChannel.Channel channel)
        {
            var resp = channel.SendRequest<AudioAuthenticationResponse>(new AudioAuthenticationRequest());
            if (resp == null)
                return false;
            else
            {
                //handle response
                if (!audiopipes.ContainsKey(resp.audiotoken))
                    return false;
                else
                {
                    //create context
                    var ctx = new ChannelContext()
                    {
                        audioInfo = audiopipes[resp.audiotoken],
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
            if (msg.Payload is AudioDataResp)
            {
                var resp = (msg.Payload as AudioDataResp);
            }
            else if (msg.Payload is AudioData)
            {
                var resp = (msg.Payload as AudioData);
                if (ctx.Receiving && ctx.audioInfo.audiosink != null && ctx.audioInfo.audiosink.IsActive)
                {
                    ctx.audioInfo.audiosink.AddPCMdata(resp.linearBytes);
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void serverChannel_OnOpenEvent(Yodiwo.YPChannel.Channel channel)
        {
            DebugEx.TraceLog("A mic recorder has been connected to me");
            Task.Run(() =>
            {
                var req = new AudioDataReq()
                {
                    aflow = AudioFlow.Start
                };
                channel.SendRequest(req);
            });
        }

        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
