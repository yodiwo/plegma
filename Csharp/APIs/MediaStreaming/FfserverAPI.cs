using System;

namespace Yodiwo.API.MediaStreaming
{

    public class FFServer2Core
    {
        public string ffserverip;
        public int rtspPort;
        public int httpPort;
        public Feed feed;
        public string streamPath;
        public bool Success = true;
        public int vlcPort;
        public int vlcprocessId;
        public string getRtspStream() { return "rtsp://" + ffserverip + ":" + rtspPort.ToString() + "/" + streamPath; }
        public string getHttpStream() { return "http://" + ffserverip + ":" + httpPort.ToString() + "/" + streamPath; }
        public string getTranscodedStream() { return "http://" + ffserverip + ":" + vlcPort.ToString() + "/?action=stream"; }
    }

    public class Core2FFServer
    {
        public FFServerMessages Type;
        public string Json;
    }

    public class TransCodeStreamFFServer
    {
        public TranscodeAction action;
        public string streamName;
        public string feedName;
        public VideoCodec codec;
        public int vlcPort;
        public int vlcprocessId;
        public string ffserverip;
        public string getHttpStream() { return "http://" + ffserverip + vlcPort.ToString() + "/?action=stream"; }
    }

    public class StreamFFserver
    {
        public FFServerAction action;
        public StreamContainer container;
        public string feedname;
        public string filemaxsize;
        public string requestId;
    }

    public enum FFServerMessages
    {
        Stream,
        Trancode
    }

    public enum TranscodeAction
    {
        Start,
        Stop
    }

    public enum FFServerAction
    {
        ASSIGN,
        FREE
    }

    public enum StreamContainer
    {
        h264,
        flv,
        webm,
        mpjpeg,
        ogg,
        rtp
    }

    public enum VideoCodec
    {
        none,
        libx264,
        libtheora,
        libvpx,
        flv,
        mpeg4,
        msmpeg4v2,
        mjpeg
    }

    public class AVOptionVideo
    {
        public string genericflags;
        public int qmin;
        public int qmax;
        public string quality;
    }

    public class Feed
    {
        public string FeedName;
        public string File;
        public string FileMaxSize;
        public string ACL;

        public string GetConfig()
        {
            string s = "";
            s += "<Feed " + FeedName + ">" + System.Environment.NewLine;
            s += "File " + File + System.Environment.NewLine;
            s += "FileMaxSize " + FileMaxSize + System.Environment.NewLine;
            s += "ACL allow " + ACL + System.Environment.NewLine;
            s += "</Feed>" + System.Environment.NewLine;
            s += System.Environment.NewLine;
            return s;
        }
    }

}

