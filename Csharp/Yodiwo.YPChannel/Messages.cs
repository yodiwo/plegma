using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.YPChannel
{
    /// <summary>
    /// Message used by server to inform client about the negotiation start
    /// </summary>
    public class HELLORequest
    {
        public int[] SuportedProtocols;
        public String Message;
        public UInt32 ChannelFlags;
    }
    /// <summary>
    /// Message used by client to inform server about the negotiation start
    /// </summary>
    public class HELLOResponse
    {
        public int Version;
        public String Message;
        public string ChannelKey;
        public UInt32 ChannelFlags;

        public class MessageTypeGroupPacked
        {
            public string GroupName;
            public string[] MessageTypes;
        }
        public MessageTypeGroupPacked[] ProtocolDefinitions;
    }

    /// <summary>
    /// Message used by server to inform client about the negotiation results
    /// </summary>
    public class NegotationFinishMessage
    {
        public bool Success;
        public String Message;
        public string ChannelKey;
        public string RedirectionTarget;
    }

    /// <summary>
    /// Message used by channel to inform client about channel closing
    /// </summary>
    public class ChannelCloseMessage
    {
        public String Message;
    }

    /// <summary>
    /// Message used by channel to monitor/measure connection
    /// </summary>
    public class PingRequest
    {
        public String Message;
    }

    /// <summary>
    /// Message used by channel to monitor/measure connection
    /// </summary>
    public class PongResponse
    {
        public String Message;
    }

    /// <summary>
    /// Message used by channel to inform about stream open
    /// </summary>
    public class StreamOpenRequest
    {
        public UInt32 StreamID;
        public UInt32 Flags;
    }

    /// <summary>
    /// Message used by channel to inform about stream open status
    /// </summary>
    public class StreamOpenResponse
    {
        public bool Result;
        public string Message;
        public UInt32 Flags;
    }

    /// <summary>
    /// Message used by channel to inform about remote stream close
    /// </summary>
    public class StreamClosed
    {
        public UInt32 StreamID;
        public string Message;
        public UInt32 Flags;
    }

    /// <summary>
    /// Message used by channel to transfer stream data chunks
    /// </summary>
    public class StreamFragment
    {
        public UInt32 StreamID;
        public byte[] Data;
    }

    /// <summary>
    /// Message used by channel to inform about bandwidth management
    /// </summary>
    public class BandwidthManagementMessage
    {
        public bool Enable;
        public uint BandwidthCap;    // max bytes per sec (uint.MaxValue equals no limit)
        public uint MessageCap;      // max messages per sec (uint.MaxValue equals no limit)
    }

}
