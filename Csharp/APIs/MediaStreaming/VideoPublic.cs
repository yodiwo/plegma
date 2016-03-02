using System;
using System.Collections.Generic;
using Yodiwo.API.Plegma;
using Yodiwo.API.BotAPI;

namespace Yodiwo.API.MediaStreaming
{
    public enum StatusCode
    {
        None,
        Success,
        AlreadyInProgress,
        Error, //TODO: please elaborate?
    }

    public class InfoMessage
    {
        public string subThingId;
        public int value;
        public string Message;
    }

    public class StreamOpenRequest : ApiMsg
    {
        public string ffserverIP;
        public int ffserverPort;
        public string subThingId;
        public string videoType;
        public string url;
        public string vcodec;
        public string frameRate;
        public int width;
        public int height;
        public string acodec;
        public string feedName;
    }

    public class StreamOpenResponse : ApiMsg
    {
        public StatusCode responseCode;
        public int ffmpegprocessId;
    }

    public class StreamCloseRequest : ApiMsg
    {
        public string feedName;
        public int value;
        public int ffmpegprocessId;
        public StatusCode responseCode;
    }

    public class StreamCloseResponse : ApiMsg
    {
        public StatusCode responseCode;
        public string feedName;
    }

    public class MjpegServerStartRequest : ApiMsg
    {
        public int fps;
    }

    public class MjpegServerStartResponse : ApiMsg
    {
        public string streamName;
        public StatusCode responseCode;
    }

    public class MjpegServerStopRequest : ApiMsg
    {
        public StatusCode responseCode;
    }

    public class MjpegServerStopResponse : ApiMsg
    {
        public StatusCode responseCode;
    }

    public class ErrorMessage
    {
        public int value;
        public string Message;
    }

    public class VideoParameters
    {
        public string videoType;
        public string videoFormat;
        public string vcodec;
        public string frameRate;
        public Tuple<int> resolution;
        public string acodec;
    }

    public static class YodiwoVideoAPI
    {

        public static Type[] ApiMessages =
        {
            typeof(InfoMessage),
            typeof(StreamOpenRequest),
            typeof(StreamOpenResponse),
            typeof(StreamCloseRequest),
            typeof(StreamCloseResponse),
            typeof(ErrorMessage),
        };

        public static Dictionary<Type, int> lookup = new Dictionary<Type, int>();
        public static Dictionary<int, Type> revLookup = new Dictionary<int, Type>();

        static YodiwoVideoAPI()
        {
            for (int i = 0; i < ApiMessages.Length; i++)
            {
                lookup.Add(ApiMessages[i], i);
                revLookup.Add(i, ApiMessages[i]);
            }
        }
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

    public enum AudioIn
    {
        None,
        Node,
        Mp3Url
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

    public class AudioMediaDescriptor
    {
        public string uri;
        public string uname;
        public string pwd;
        public string protocol;
        public AudioIn audioDevice;
        public AudioAction action;
    }

    public class OngoingMediaStreamDescriptor
    {
        public int ffmpegprocessId;
        public int vlcprocessId;
        public string feedName;
        public string videostreamName;
        public string ffserverIP;
        public string transcodeduri;
        public NodeKey nodekey;
        public int AssignedAVbot;
        public bool desktopshare;
        public string audiofeedname;
        public string intermediatecallee;
        public string mp3streamName;
    }



}