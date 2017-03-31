using System;
using System.Collections.Generic;
using Yodiwo.API.Plegma;

namespace Yodiwo.API.MediaStreaming
{
    /// <summary>
    /// type of video actions that each video source can handle
    /// </summary>
    public enum VideoFlow
    {
        /// <summary>None, should not be used!</summary>
        None,
        /// <summary>VideoSource should start streaming</summary>
        Start,
        /// <summary>VideoSource should stop streaming</summary>
        Stop
    }
    /// <summary>
    /// type of videosources
    /// </summary>
    public enum VideoIn
    {
        /// <summary>None, should not be used!</summary>
        None,
        /// <summary>Node, the video source is coming from a local device i.e. webcam, ipcamera,screensharing</summary>
        Node,
        /// <summary>WebUrl, the video source is coming from a public accessible video url</summary>
        WebUrl,
        /// <summary>Sip, the video source is coming from an incoming sip video call</summary>
        Sip,
    }

    /// <summary>
    ///VideoServerAction: specifies the operations that a YVideoServer should perform
    /// </summary>
    public enum VideoServerAction
    {
        /// <summary>None, should not be used!</summary>
        None,
        /// <summary>GetVideoFeed, get a Video Feed</summary>
        GetVideoFeed,
        /// <summary>ReleaseVideoFeed, release a binded Video Feed</summary>
        ReleaseVideoFeed,
    }

    /// <summary>
    /// VideoDescriptor: specify an http(s) or rtsp url where a video stream is accessible
    /// </summary>
    [Serializable]
    public class VideoDescriptor
    {
        /// <summary>Http(s) Url </summary>
        public string HttpUrl;
        /// <summary>Rtsp Url </summary>
        public string RtspUrl;
    }

    /// <summary>
    /// Basic Output Type of VideoSources
    /// </summary>
    [Serializable]
    public class VideoMediaDescriptor
    {
        /// <summary>Uri, is configured from the Nodekey and the name of the VideoSource </summary>
        public string Uri;
        /// <summary>Uname, specifies the Username of the IPCamera </summary>
        public string Uname;
        /// <summary>Pwd, specifies the pwd of the IPCamera </summary>
        public string Pwd;
        /// <summary>Pwd, specifies the protocol of the IPCamera </summary>
        public string Protocol;
        /// <summary>VideoDevice specify the type of the VideoSource </summary>
        public VideoIn VideoDevice;
        /// <summary>Action specify the action that the VideoSource should perform </summary>
        public VideoFlow Action = VideoFlow.Start;
    }

    /// <summary>
    ///Video: Static class of the Video Api Messages, that are used towards video streaming
    /// </summary>
    public static class Video
    {
        /// <summary>ApiGroupName: specify the name of the Video Api protocol</summary>
        public const string ApiGroupName = "MediaStreaming.Video";
        /// <summary>ApiMessages: specify the types of the Video Messages</summary>
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

    /// <summary>
    /// VideoDataReq: used to request from a YVideoClient to start/stop send its video data to the YVideoServer
    /// </summary>
    [Serializable]
    public class VideoDataReq : PlegmaApiMsg
    {
        /// <summary>specifies if the YVideoClient should start/stop streaming </summary>
        public VideoFlow Vflow;
    }

    /// <summary>
    /// VideoDataResp: used as response from the YVideoClient to the YVideoServer
    /// </summary>
    [Serializable]
    public class VideoDataResp : PlegmaApiMsg
    {
        /// <summary>Status specify the status code of the video streaming action</summary>
        public StatusCode Status;
        /// <summary>Videotoken specify the unique identifier that a YVideoClient has used so as to connect to the YVideooServer</summary>
        public string VideoToken;
    }

    /// <summary>
    ///VideoAuthenticationRequest: used as request from the YVideoServer for the negotiation with a YVideoClient
    /// </summary>
    [Serializable]
    public class VideoAuthenticationRequest : PlegmaApiMsg
    {
    }

    /// <summary>
    ///VideoAuthenticationResponse: used as response from the YVideoClient so as to authenticate itself to the YVideoServer
    /// </summary>
    [Serializable]
    public class VideoAuthenticationResponse : PlegmaApiMsg
    {
        /// <summary>Videotoken specify the unique identifier that a YVideoClient has used so as to connect to the YVideoServer</summary>
        public string VideoToken;
    }

    /// <summary>
    ///VideoData: used for streaming the video data
    /// </summary>
    [Serializable]
    public class VideoData : PlegmaApiMsg
    {
        /// <summary>LinearBytes include the video data</summary>
        public Byte[] LinearBytes;
    }

    /// <summary>
    ///VideoServerFeedRequest: used for sending requests to the VideoManager
    /// </summary>
    [Serializable]
    public class VideoServerFeedRequest : PlegmaApiMsg
    {
        /// <summary>Action: specify the action that the videomanager should handle</summary>
        public VideoServerAction Action;
        public string PrefferedVideoToken;
        /// <summary>Identification: specify the unique identifier of a video feed </summary>
        public string Identification;
    }

    /// <summary>
    ///VideoServerFeedResponse: used for sending Video Feed Responses from the VideoManager
    /// </summary>
    [Serializable]
    public class VideoServerFeedResponse : PlegmaApiMsg
    {
        /// <summary>ServerIp: specify the IP address of the YVideoServer that a YVideoClient has to connect with</summary>
        public string ServerIP;
        /// <summary>Port: specify the Port of the YVideoServer that a YVideoClient has to connect with</summary>
        public int Port;
        /// <summary>VideoToken: specify the unique identifier that a YVideoClient has to use in the negotiation phase so as to conenct to the YVideoServer</summary>
        public string VideoToken;
        /// <summary>Status: specify the status code of the requested action</summary>
        public StatusCode Status;
        /// <summary>MjpegStream: specify the Mjpeg uri, that the MJPEG video stream is public accesible</summary>
        public string MjpegStream;
    }

    /// <summary>
    ///VideoServerConnectRequest: used for sending requests to a Yodiwo Node, which supports video streaming, so as to connect to the Video Server
    /// </summary>
    [Serializable]
    public class VideoServerConnectRequest : PlegmaApiMsg
    {
        /// <summary>ServerIp: specify the IP address of the YVideoServer that a YVideoClient has to connect with</summary>
        public string ServerIP;
        /// <summary>Port: specify the Port of the YVideoServer that a YVideoClient has to connect with</summary>
        public int Port;
        /// <summary>VideoToken: specify the unique identifier that a YVideoClient has to use in the negotiation phase so as to conenct to the YVideoServer</summary>
        public string VideoToken;
        /// <summary>VideoDevUrl: specify a string descriptor of the Video Device</summary>
        public string VideoDevUrl;
        /// <summary>VideoDevUname: specify the username so as to access the Video Device</summary>
        public string VideoDevUname;
        /// <summary>VideoDevPwd: specify the password so as to access the Video Device</summary>
        public string VideoDevPwd;
    }

    /// <summary>
    ///VideoServerConnectResponse: used for sending connect responses from a Yodiwo Node, which supports video streaming, to the YVideoServer
    /// </summary>
    [Serializable]
    public class VideoServerConnectResponse : PlegmaApiMsg
    {
        /// <summary>Status: Specify the status code, when setting up the YVideoSource</summary>
        public StatusCode Status;
    }

    /// <summary>
    ///VideoServerDisconnectRequest: used for sending disconnect requests to a Yodiwo Node, which supports video streaming, so as to disconnect from the Video Server
    /// </summary>
    [Serializable]
    public class VideoServerDisconnectRequest : PlegmaApiMsg
    {
        /// <summary>VideoToken: Specify the unique identifier that a YVideoClient has to use in the negotiation phase so as to conenct to the YVideoServer</summary>
        public string VideoToken;
    }

    /// <summary>
    ///VideoServerDisconnectResponse: used for sending disconnect responses from a Yodiwo Node, which supports video streaming, to the YVideoServer
    /// </summary>
    [Serializable]
    public class VideoServerDisconnectResponse : PlegmaApiMsg
    {
        /// <summary>Status: Specify the status code, when tearing down the YVideoSource</summary>
        public StatusCode Status;
    }







}