using System;
using System.Collections.Generic;
using Yodiwo.API.Plegma;

namespace Yodiwo.API.MediaStreaming
{
    public enum VideoAction
    {
        None,
        Start,
        Stop
    }
    public class VideoDescriptor
    {
        public string httpUrl;
        public string rtspUrl;
    }
    public enum VideoIn
    {
        None,
        Node,
        WebUrl,
        Sip,
    }
    public class VideoMediaDescriptor
    {
        public string uri;
        public string uname;
        public string pwd;
        public string protocol;
        public VideoIn videoDevice;
        public VideoAction action = VideoAction.Start;
    }
    public static class Video
    {
        public const string ApiGroupName = "MediaStreaming.Video";

        public static Type[] ApiMessages = new Type[]
        {
            typeof(VideoDataReq),
            typeof(VideoDataResp),
            typeof(VideoData),
            typeof(VideoAuthenticationRequest),
            typeof(VideoAuthenticationResponse),
            typeof(VideoServerFeedResponse),
            typeof(VideoServerFeedRequest),
            typeof(VideoServerConnectRequest),
            typeof(VideoServerConnectResponse),
            typeof(VideoServerDisconnectRequest),
            typeof(VideoServerDisconnectResponse)
        };
    }
    public class VideoDataReq : ApiMsg
    {
        public VideoFlow vflow;
    }
    public class VideoDataResp : ApiMsg
    {
        public StatusCode status;
        public string videotoken;
    }
    public class VideoAuthenticationRequest : ApiMsg
    {
    }
    public class VideoAuthenticationResponse : ApiMsg
    {
        public string videotoken;
    }
    public class VideoData : ApiMsg
    {
        public Byte[] linearBytes;
    }
    public enum VideoFlow
    {
        None,
        Start,
        Stop
    }
    public class VideoServerFeedResponse : ApiMsg
    {
        public string serverip;
        public int port;
        public string mjpegtoken;
        public StatusCode status;
        public string mjpegstream;
    }
    public class VideoServerFeedRequest : ApiMsg
    {
        public VideoServerAction action;
    }
    public enum VideoServerAction
    {
        None,
        GetFeed,
        ReleaseFeed,
    }
    public class VideoServerConnectRequest : ApiMsg
    {
        public string serverip;
        public int port;
        public string videotoken;
        public string videodevurl;
        public string videodevuname;
        public string videodevpwd;
    }
    public class VideoServerDisconnectRequest : ApiMsg
    {
        public string videotoken;
    }
    public class VideoServerDisconnectResponse : ApiMsg
    {
        public StatusCode status;
    }
    public class VideoServerConnectResponse : ApiMsg
    {
        public StatusCode status;
    }






}