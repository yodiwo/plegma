using System;
using System.Collections.Generic;

namespace Yodiwo.YPChannel
{
    public class Protocol
    {
        public int Version;

        public class MessageTypeGroup
        {
            public string GroupName;
            public Type[] MessageTypes;
        }
        public List<MessageTypeGroup> ProtocolDefinitions = new List<MessageTypeGroup>();
    }

    public class YPMessage
    {
        public MessageFlags Flags;
        public UInt32 MessageID;
        public UInt32 RespondToRequestID;
        public Dictionary<string, object> MessageIE;
        public object Payload;

        public bool IsRequest { get { return Flags.HasFlag(MessageFlags.Request); } set { Flags |= MessageFlags.Request; } }
        public bool IsResponse { get { return Flags.HasFlag(MessageFlags.Response); } set { Flags |= MessageFlags.Response; } }
    }

    [Flags]
    public enum ChannelFlags : uint
    {
        None = 0,
    }

    [Flags]
    public enum MessageFlags : byte
    {
        None = 0,
        Request = 1 << 1,
        Response = 1 << 2,
    }

    public enum MessageIEValueType : byte
    {
        Null = 0,
        Object = 1,
        String = 2,

        Byte = 10,
        Boolean = 11,

        Int16 = 20,
        Int32 = 21,
        Int64 = 22,

        UInt16 = 30,
        UInt32 = 31,
        UInt64 = 32,

        Single = 40, //floating point
        Double = 41,
    }


    public struct MessageIE
    {
        public string Key;
        public YPCObject Value;

        public MessageIE(string Key, object Value) { this.Key = Key; this.Value = new YPCObject(Value); }
    }


    public static class MessageIEKeys
    {
        public static readonly string ExtendedType = "ExtType";
    }

    public enum ChannelStates
    {
        Initializing,
        Negotiating,
        Open,
        Paused,
        Recoverying,
        Closed,
    }

    public enum ChannelRole
    {
        Unkown,
        Client,
        Server
    }

    [Flags]
    public enum ChannelSerializationMode : byte
    {
        Unkown = 0,
        MessagePack = 1 << 0,
        Json = 1 << 1,
        //xxx = 1 << 2,
        //yyy = 1 << 3,
    }

}
