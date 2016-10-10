using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.API.Plegma
{
    #region Base wrapper class

    /// <summary>
    /// Wrapper class mainly for providing synchronization services to sync-less protocols (mqtt, websockets, etc)
    /// </summary>
    public class WrapperMsg
    {
        /// <summary> message flags (request/response, etc) </summary>
        public eMsgFlags Flags;

        /// <summary>
        /// for RPC blocking calls: synchronization ID
        /// Message ID or Request message, or number of previous message that this message is responding to
        /// </summary>
        public int SyncId;

        /// <summary>
        /// JSON Serialized payload
        /// </summary>
        public string Payload;

        /// <summary>
        /// Size of packed/serialized payload
        /// </summary>
        public int PayloadSize;

        /// <summary>
        /// (un)mark message as request / check if request
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public bool IsRequest
        {
            get { return Flags.HasFlag(eMsgFlags.Request); }
            set { if (value == true) Flags |= eMsgFlags.Request; else Flags &= ~eMsgFlags.Request; }
        }

        /// <summary>
        /// (un)mark message as response / check if response
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public bool IsResponse
        {
            get { return Flags.HasFlag(eMsgFlags.Response); }
            set { if (value == true) Flags |= eMsgFlags.Response; else Flags &= ~eMsgFlags.Response; }
        }

        /// <summary>wrapper message flags</summary>
        [Flags]
        public enum eMsgFlags : byte
        {
            /// <summary>no flags (async message)</summary>
            None = 0,
            /// <summary>message is a request</summary>
            Request = 1 << 0,
            /// <summary>message is a response to a request</summary>
            Response = 1 << 1,
        }
    }

    #endregion


    #region Websockets

    /// <summary>
    /// Websocket protocol wrapper. Inherits from base WrapperMsg, adds Id and Subid
    /// </summary>  
    public class WebSocketMsg : WrapperMsg
    {
        /// <summary>
        /// Id of message (pairing, api, other message)
        /// </summary>
        public eWSMsgId Id;
        /// <summary>
        /// subid of message (type of api or pairing message)
        /// </summary>
        public string SubId;

        /// <summary> enum of message ID </summary>
        public enum eWSMsgId : byte
        {
            /// <summary> no id / invalid </summary>
            None = 0,
            /// <summary> Pairing type message </summary>
            Pairing = 1,
            /// <summary> Plegma API message </summary>
            Api = 2,
            /// <summary> Warlock API message </summary>
            Warlock = 3,
        }
    }

    #endregion


    #region MQTT

    /// <summary>
    /// Mqtt message encapsulation class.
    /// </summary>
    public class MqttMsg : WrapperMsg
    {
    }

    #endregion

    #region GCM

    /// <summary>
    /// Gcm message encapsulation class.
    /// </summary>
    public class GcmMsg : WrapperMsg
    {
        public string PayloadType;
    }

    #endregion
}
