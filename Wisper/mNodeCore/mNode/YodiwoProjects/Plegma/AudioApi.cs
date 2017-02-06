using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;

namespace Yodiwo.API.MediaStreaming
{
    /// <summary>
    /// type of audio actions that each audio source can handle
    /// </summary>
    public enum AudioFlow
    {
        /// <summary>None, should not be used!</summary>
        None,
        /// <summary>AudioSource should start streaming</summary>
        Start,
        /// <summary>AudioSource should stop streaming</summary>
        Stop
    }
    /// <summary>
    /// type of audiosources
    /// </summary>
    public enum AudioIn
    {
        /// <summary>None, should not be used!</summary>
        None,
        /// <summary>Node, the audio source is coming from a local device i.e. microphone</summary>
        Node,
        /// <summary>Mp3Url, the audio source is coming from a publicly accessible mp3 url</summary>
        Mp3Url
    }

    /// <summary>
    ///AudioServerAction: specifies the operations that a YAudioServer should perform
    /// </summary>
    public enum AudioServerAction
    {
        /// <summary>None, should not be used!</summary>
        None,
        /// <summary>GetMP3Feed, get a MP3 Audio Feed</summary>
        GetMP3Feed,
        /// <summary>GetAudioPipe, get an Audio Pipe</summary>
        GetAudioPipe,
        /// <summary>Release Feed, release a binded Audio Feed</summary>
        ReleaseAudioFeed,
    }

    /// <summary>
    /// Basic Output Type of AudioSources
    /// </summary>
    [Serializable]
    public class AudioMediaDescriptor
    {
        /// <summary>Uri is configured from the Nodekey and the name of the AudioSource </summary>
        public string Uri;
        /// <summary>AudioDevice specify the type of the AudioSource </summary>
        public AudioIn AudioDevice;
        /// <summary>Action specify the action that the AudioSource should perform </summary>
        public AudioFlow Action;
    }

    /// <summary>
    ///Audio: Static class of the Audio Api Messages, that are used towards audio streaming
    /// </summary>
    public static class Audio
    {
        /// <summary>ApiGroupName: specify the name of the Audio Api protocol</summary>
        public const string ApiGroupName = "MediaStreaming.Audio";
        /// <summary>ApiMessages: specify the types of the Audio Messages</summary>
        public static Type[] ApiMessages = new Type[]
        {
            typeof(AudioDataReq),
            typeof(AudioDataResp),
            typeof(AudioData),
            typeof(AudioAuthenticationRequest),
            typeof(AudioAuthenticationResponse),
            typeof(AudioServerFeedRequest),
            typeof(AudioServerFeedResponse),
            typeof(AudioServerConnectRequest),
            typeof(AudioServerDisconnectRequest),
            typeof(AudioServerConnectResponse),
            typeof(AudioServerDisconnectResponse)
        };
    }

    /// <summary>
    /// AudioDataReq: used to request from a YAudioClient to start/stop send its audio data to the YAudioServer
    /// </summary>
    [Serializable]
    public class AudioDataReq : PlegmaApiMsg
    {
        /// <summary>specifies if the Yaudioclient should start/stop streaming </summary>
        public AudioFlow Aflow;
    }

    /// <summary>
    /// AudioDataResp: used as response from a YAudioClient to the YaudioServer
    /// </summary>
    [Serializable]
    public class AudioDataResp : PlegmaApiMsg
    {
        /// <summary>Status specify the status code of the audio streaming action</summary>
        public StatusCode Status;
        /// <summary>Audiotoken specify the unique identifier that a YAudioClient has used so as to connect to the YAudioServer</summary>
        public string Audiotoken;
    }

    /// <summary>
    ///AudioAuthenticationRequest: used as request from the YAudioServer for the negotiation with a YAudioClient
    /// </summary>
    [Serializable]
    public class AudioAuthenticationRequest : PlegmaApiMsg
    {
    }

    /// <summary>
    ///AudioAuthenticationResponse: used as response from the YAudioClient so as to authenticate itself to the YAudioServer
    /// </summary>
    [Serializable]
    public class AudioAuthenticationResponse : PlegmaApiMsg
    {
        /// <summary>Audiotoken specify the unique identifier that a YAudioClient has used so as to connect to the YAudioServer</summary>
        public string Audiotoken;
    }

    /// <summary>
    ///AudioData: used for streaming the audio data
    /// </summary>
    [Serializable]
    public class AudioData : PlegmaApiMsg
    {
        /// <summary>LinearBytes include the audio data</summary>
        public Byte[] LinearBytes;
    }

    /// <summary>
    ///AudioServerFeedRequest: used for sending requests to the AudioManager
    /// </summary>
    [Serializable]
    public class AudioServerFeedRequest : PlegmaApiMsg
    {
        /// <summary>Action: specify the action that the audiomanager should handle</summary>
        public AudioServerAction Action;
        /// <summary>Identification: specify the unique identifier of an audio feed </summary>
        public string Identification;
        /// <summary>PreferredAudioToken: specify an audiotoken, that the audio feed will use as unique identifier </summary>
        public string PreferredAudioToken;
    }

    /// <summary>
    ///AudioServerFeedResponse: used for sending Audio Feed Responses from the AudioManager
    /// </summary>
    [Serializable]
    public class AudioServerFeedResponse : PlegmaApiMsg
    {
        /// <summary>ServerIp: specify the IP address of the YAudioServer that a YAudioClient has to connect with</summary>
        public string ServerIp;
        /// <summary>Port: specify the Port of the YAudioServer that a YAudioClient has to connect with</summary>
        public int Port;
        /// <summary>AudioToken: Specify the unique identifier that a YAudioClient has to use in the negotiation phase so as to conenct to the YAudioServer</summary>
        public string AudioToken;
        /// <summary>Status: Specify the status code of the requested action</summary>
        public StatusCode Status;
        /// <summary>MP3Steam: Specify the MP3 uri, that the MP3 audio stream is public accesible</summary>
        public string Mp3Stream;
    }

    /// <summary>
    ///AudioServerConnectRequest: used for sending requests to a Yodiwo Node, which supports audio streaming, so as to connect to the Audio Server
    /// </summary>
    [Serializable]
    public class AudioServerConnectRequest : PlegmaApiMsg
    {
        /// <summary>ServerIp: specify the IP address of the YAudioServer that a YAudioClient has to connect with</summary>
        public string ServerIp;
        /// <summary>Port: specify the Port of the YAudioServer that a YAudioClient has to connect with</summary>
        public int Port;
        /// <summary>AudioToken: Specify the unique identifier that a YAudioClient has to use in the negotiation phase so as to conenct to the YAudioServer</summary>
        public string AudioToken;
        /// <summary>AudioDevDesc: Specify a string descriptor of the Audio Device</summary>
        public string AudioDevDesc;
    }

    /// <summary>
    ///AudioServerConnectResponse: used for sending connect responses from a Yodiwo Node, which supports audio streaming, to the YAudioServer
    /// </summary>
    [Serializable]
    public class AudioServerConnectResponse : PlegmaApiMsg
    {
        /// <summary>Status: Specify the status code, when setting up the YAudioSource</summary>
        public StatusCode Status;
    }

    /// <summary>
    ///AudioServerDisconnectRequest: used for sending disconnect requests to a Yodiwo Node, which supports audio streaming, so as to disconnect from the Audio Server
    /// </summary>
    [Serializable]
    public class AudioServerDisconnectRequest : PlegmaApiMsg
    {
        /// <summary>AudioToken: Specify the unique identifier that a YAudioClient has to use in the negotiation phase so as to conenct to the YAudioServer</summary>
        public string AudioToken;
    }

    /// <summary>
    ///AudioServerDisconnectResponse: used for sending disconnect responses from a Yodiwo Node, which supports audio streaming, to the YAudioServer
    /// </summary>
    [Serializable]
    public class AudioServerDisconnectResponse : PlegmaApiMsg
    {
        /// <summary>Status: Specify the status code, when tearing down the YAudioSource</summary>
        public StatusCode Status;
    }

}
