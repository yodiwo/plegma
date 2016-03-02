using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Yodiwo;
using Yodiwo.YPChannel;
using Yodiwo.API.MediaStreaming;
using Yodiwo.Media.Audio.Source;

namespace Yodiwo.Services.Media.Audio
{
    public class YAudioClient
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public IAudioSource audiosource;
        YPChannel.Transport.Sockets.Client yclient;
        public string audiotoken;
        //------------------------------------------------------------------------------------------------------------------------
        public bool IsTransmitting;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constuctor
        //------------------------------------------------------------------------------------------------------------------------
        public YAudioClient(IAudioSource audiosrc)
        {
            this.audiosource = audiosrc;
            this.audiosource.OnAudioDataCaptured += Audiosource_OnAudioDataCaptured;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------

        public void connect(string server, string audiotoken, int Port = 6517, bool SecureYPC = false)
        {
            //start a YPclient
            DebugEx.TraceLog("Connect mic client to mic server");
            this.audiotoken = audiotoken;
            //create protocol
            var proto = new YPChannel.Protocol()
            {
                Version = 1,
                ProtocolDefinitions = new List<Protocol.MessageTypeGroup>()
                    {
                        new Protocol.MessageTypeGroup() { GroupName=Yodiwo.API.MediaStreaming.Audio.ApiGroupName, MessageTypes= Yodiwo.API.MediaStreaming.Audio.ApiMessages }
                    },
            };
            //create channel
            yclient = new Yodiwo.YPChannel.Transport.Sockets.Client(proto) { Name = "AudioMediaClient" };
            yclient.OnMessageReceived += clientOnMessageReceived;
            yclient.Connect(server, Port, SecureYPC);
        }

        //------------------------------------------------------------------------------------------------------------------------

        void clientOnMessageReceived(Yodiwo.YPChannel.Channel channel, Yodiwo.YPChannel.YPMessage msg)
        {
            if (msg.Payload is AudioDataReq)
            {
                var req = (msg.Payload as AudioDataReq);
                if (req.aflow == AudioFlow.Start)
                {
                    try
                    {
                        audiosource.Start();
                    }
                    catch { }
                    var resp = new AudioDataResp()
                    {
                        status = StatusCode.Success
                    };
                    yclient.SendResponse(resp, msg.MessageID);
                    IsTransmitting = true;
                }
                else if (req.aflow == AudioFlow.Stop)
                {
                    IsTransmitting = false;
                    audiosource.Stop();
                    var resp = new AudioDataResp()
                    {
                        status = StatusCode.Success
                    };
                    channel.SendResponse(resp, msg.MessageID);
                }
            }
            else if (msg.Payload is AudioAuthenticationRequest)
            {
                var resp = new AudioAuthenticationResponse()
                {
                    audiotoken = this.audiotoken
                };
                channel.SendResponse(resp, msg.MessageID);
            }
        }


        //------------------------------------------------------------------------------------------------------------------------

        public StatusCode TearDown()
        {
            try
            {
                audiosource.Stop();
                yclient.Close();
                return StatusCode.Success;
            }
            catch (Exception ex)
            {
                return StatusCode.Error;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        private void Audiosource_OnAudioDataCaptured(object sender, AudioEventArgs e)
        {
            if (yclient.IsOpen && IsTransmitting)
            {
                var resp = new AudioData()
                {
                    linearBytes = e.pcmdata
                };
                yclient.SendMessage(resp);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
