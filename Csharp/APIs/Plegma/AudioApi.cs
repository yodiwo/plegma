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
    public enum AudioAction
    {
        None,
        Start,
        Stop
    }
    public enum AudioIn
    {
        None,
        Node,
        Mp3Url
    }
    public class AudioMediaDescriptor
    {
        public string uri;
        public string uname;
        public string pwd;
        public string protocol;
        public AudioIn audioDevice;
        public AudioAction action;
    }
    public class AudioDataReq : ApiMsg
    {
        public AudioFlow aflow;
    }
    public class AudioDataResp : ApiMsg
    {
        public StatusCode status;
        public string audiotoken;
    }
    public class AudioAuthenticationRequest : ApiMsg
    {
    }
    public class AudioAuthenticationResponse : ApiMsg
    {
        public string audiotoken;
    }
    public class AudioData : ApiMsg
    {
        public Byte[] linearBytes;
    }
    public static class Audio
    {
        public const string ApiGroupName = "MediaStreaming.Audio";

        public static Type[] ApiMessages = new Type[]
        {
            typeof(AudioDataReq),
            typeof(AudioDataResp),
            typeof(AudioData),
            typeof(AudioAuthenticationRequest),
            typeof(AudioAuthenticationResponse)
        };
    }
    public enum AudioServerAction
    {
        None,
        GetMP3Feed,
        GetPipeSip,
        ReleaseFeed,
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
        public string audiodevdesc;
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
