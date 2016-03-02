using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;

namespace Yodiwo.API.MediaStreaming
{
    public enum AudioFlow
    {
        None,
        Start,
        Stop
    }

    public class AudioDataReq
    {
        public AudioFlow aflow;
    }

    public class AudioDataResp
    {
        public StatusCode status;
        public string audiotoken;
    }

    public class AudioAuthenticationRequest
    {
    }

    public class AudioAuthenticationResponse
    {
        public string audiotoken;
    }

    public class AudioData
    {
        public Byte[] linearBytes;
    }


    public static class Audio
    {
        public static Type[] ApiMessages = new Type[]
        {
            typeof(AudioDataReq),
            typeof(AudioDataResp),
            typeof(AudioData),
            typeof(AudioAuthenticationRequest),
            typeof(AudioAuthenticationResponse)
        };
    }


    public class AudioServerFeedRequest : ApiMsg
    {
        public AudioServerAction action;
    }

    public class AudioServerConnectRequest : ApiMsg
    {
        public string serverip;
        public int port;
        public string audiotoken;
    }

    public class AudioServerDisconnectRequest : ApiMsg
    {
        public string audiotoken;
    }

    public class AudioServerDisconnectResponse : ApiMsg
    {
        public StatusCode status;
    }

    public class AudioServerConnectResponse : ApiMsg
    {
        public StatusCode status;
    }

    public class AudioServerFeedResponse : ApiMsg
    {
        public string serverip;
        public int port;
        public string audiotoken;
        public StatusCode status;
        public string mp3stream;
    }

}
