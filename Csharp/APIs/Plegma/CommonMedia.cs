using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;

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

    public class ErrorMessage
    {
        public int value;
        public string Message;
    }

    public class OngoingMediaStreamDescriptor
    {
        public string vlcvideoserver;
        public int vlcprocessId;
        public string videoFeedName;
        public string videoStreamName;
        public string audioFeedName;
        public string audioStreamName;
        public string transcodedStreamName;
        public NodeKey nodekey;
        public int AssignedAVbot;
        public string intermediateCallee;
        public string devvideouri;
    }
}
