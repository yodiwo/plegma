using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using MsgPack.Serialization;
using System.Reflection;
using System.Collections;
using System.Threading.Tasks;
using System.Threading;

namespace Yodiwo.YPChannel
{
    public abstract class Channel
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public string Name = string.Empty;
        //------------------------------------------------------------------------------------------------------------------------
        internal string _ChannelKey = string.Empty;
        public string ChannelKey { get { return _ChannelKey; } }
        //------------------------------------------------------------------------------------------------------------------------
        ChannelFlags _LocalChannelFlags = ChannelFlags.None;
        public ChannelFlags LocalChannelFlags { get { return _LocalChannelFlags; } }
        //------------------------------------------------------------------------------------------------------------------------
        ChannelFlags _RemoteChannelFlags = ChannelFlags.None;
        public ChannelFlags RemoteChannelFlags { get { return _RemoteChannelFlags; } }
        //------------------------------------------------------------------------------------------------------------------------
        public readonly ChannelSerializationMode ChannelSerializationMode;
        //------------------------------------------------------------------------------------------------------------------------
        DateTime _channelCreatedTimestamp = DateTime.Now;
        public DateTime ChannelCreatedTimestamp => _channelCreatedTimestamp;
        //------------------------------------------------------------------------------------------------------------------------
        internal protected Task task;
        object syncRoot = new object();
        object readLock = new object();
        object writeLock = new object();
        byte[] sizeBuf = new byte[2];
        Int32 messageIDCnt = 0;
        int heartBeatThreadID = -1;
        //------------------------------------------------------------------------------------------------------------------------
        public delegate bool OnNegiationEventHandler(Channel Channel);
        public OnNegiationEventHandler NegotiationHandler = null;
        //------------------------------------------------------------------------------------------------------------------------
        public delegate void OnOpenEventHandler(Channel Channel);
        public event OnOpenEventHandler OnOpenEvent = null;
        //------------------------------------------------------------------------------------------------------------------------
        public delegate void OnClosedEventHandler(Channel Channel);
        public event OnClosedEventHandler OnClosedEvent = null;
        //------------------------------------------------------------------------------------------------------------------------
        public delegate void OnMessageReceivedEventHandler(Channel Channel, YPMessage Message);
        public event OnMessageReceivedEventHandler OnMessageReceived = null;
        //------------------------------------------------------------------------------------------------------------------------
        public class YPMessagePacked
        {
            public UInt32 Type;
            public byte Flags;
            public UInt32 MessageID;
            public UInt32 RequestID;
            public MessageIE[] MessageIE;
            public byte[] Payload;
        }
        //------------------------------------------------------------------------------------------------------------------------
        internal protected MessagePackSerializer<YPMessagePacked> MessagePackEncapsulationSerializer = MessagePackSerializer.Get<YPMessagePacked>();
        internal protected Newtonsoft.Json.JsonSerializer JSONEncapsulationReadSerializer = new Newtonsoft.Json.JsonSerializer();
        internal protected Newtonsoft.Json.JsonSerializer JSONEncapsulationWriteSerializer = new Newtonsoft.Json.JsonSerializer();
        //------------------------------------------------------------------------------------------------------------------------
        protected class ProtocolHelper
        {
            public int Version;
            public Dictionary<Type, UInt32> ProtocolLookup = new Dictionary<Type, UInt32>();
            public Dictionary<UInt32, Type> ProtocolReverseLookup = new Dictionary<UInt32, Type>();
            public Dictionary<string, Type[]> ProtocolDefinitions = new Dictionary<string, Type[]>();
        }
        protected Dictionary<int, ProtocolHelper> ProtocolHelpers = new Dictionary<int, ProtocolHelper>();
        protected ProtocolHelper ActiveProtocol = null;
        //------------------------------------------------------------------------------------------------------------------------
        class RequestInfo
        {
            public UInt32 RequestID;
            public YPMessage resp = null;
        }
        Dictionary<UInt32, RequestInfo> PendingRequests = new Dictionary<UInt32, RequestInfo>();
        //------------------------------------------------------------------------------------------------------------------------
        protected ChannelStates _State = ChannelStates.Initializing;
        public ChannelStates State { get { return _State; } }
        //------------------------------------------------------------------------------------------------------------------------
        public bool IsNegotiating { get { return _State == ChannelStates.Negotiating; } }
        public bool IsOpen { get { return _State == ChannelStates.Open; } }
        //------------------------------------------------------------------------------------------------------------------------
        public abstract string LocalIdentifier { get; }
        public abstract string RemoteIdentifier { get; }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual Int64 ReceivedBytesTotal { get { return 0; } set { } }
        public virtual Int64 TransmittedBytesTotal { get { return 0; } set { } }
        public virtual Int64 ReceivedBytesPerSecond { get { return 0; } set { } }
        public virtual Int64 TransmittedBytesPerSecond { get { return 0; } set { } }
        //------------------------------------------------------------------------------------------------------------------------
        public BandwidthManagementMessage RequestedBandwidthManagement = null;
        BandwidthManagementMessage LocalTxBandwidthManagement = null;
        //------------------------------------------------------------------------------------------------------------------------
#if DEBUG
        static HashSet<Type> CheckedTypes = new HashSet<Type>();
#endif
        //------------------------------------------------------------------------------------------------------------------------
        public readonly ChannelRole ChannelRole;
        public readonly bool IsPoint2Point;
        //------------------------------------------------------------------------------------------------------------------------
        static Type[] IntrinsicMessages = new Type[]
        {
            typeof(HELLORequest),
            typeof(HELLOResponse),
            typeof(NegotationFinishMessage),
            typeof(ChannelCloseMessage),
            typeof(PingRequest),
            typeof(PongResponse),
            typeof(StreamOpenRequest),
            typeof(StreamOpenResponse),
            typeof(StreamClosed),
            typeof(StreamFragment),
            typeof(BandwidthManagementMessage),
            typeof(RedirectionMessage),
        };
        //------------------------------------------------------------------------------------------------------------------------
        public readonly DictionaryTS<Type, object> Tags = new DictionaryTS<Type, object>();
        //------------------------------------------------------------------------------------------------------------------------
        bool _closing = false;
        //------------------------------------------------------------------------------------------------------------------------
        public int DefaultRequestTimeoutms = 15 * 1000;
        //------------------------------------------------------------------------------------------------------------------------
        ///<summary>This is a static global event that will shutdown all server instances</summary>
        public static HashSetTS<WeakAction<object>> OnSystemShutDownRequest = new HashSetTS<WeakAction<object>>();
        #region Raise OnSystemShutDownRequest helper
        public static void RaiseOnSystemShutDownRequest(object sender)
        {
            try
            {
                foreach (var entry in OnSystemShutDownRequest)
                    try { entry.Invoke(sender); } catch { }
                OnSystemShutDownRequest.Clear();
            }
            catch { }
        }
        #endregion 
        //------------------------------------------------------------------------------------------------------------------------
        public bool EnableExtendedTypes = false;
        //------------------------------------------------------------------------------------------------------------------------
        public int PongReplySizeLimit = 1024;
        //------------------------------------------------------------------------------------------------------------------------
        public TimeSpan NegotiationTimeout = TimeSpan.FromSeconds(60);
        //------------------------------------------------------------------------------------------------------------------------
        TaskCompletionSource<bool> pauseCompletion = null;
        public bool IsPaused
        {
            get { return State == ChannelStates.Paused; }
            set
            {
                lock (syncRoot)
                {
                    if (State == ChannelStates.Open || State == ChannelStates.Paused)
                    {
                        //change state to paused
                        _State = value ? ChannelStates.Paused : ChannelStates.Open;
                        //setup task completion
                        if ((pauseCompletion == null && value == false) ||
                            (pauseCompletion != null && value == true))
                            return; //nothing to do
                        else if (pauseCompletion != null && value == true)
                            pauseCompletion = new TaskCompletionSource<bool>(false); //create a completion task
                        else
                            pauseCompletion.SetResult(true); //finish completion task
                    }
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public TimeSpan HeartBeatSpinDelay = TimeSpan.Zero;
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary> Custom to code to run at the start of each heartbeat</summary>
        public Action<Channel> HeartBeatPrepare = null;
        //------------------------------------------------------------------------------------------------------------------------
        public bool AutoRedirect = true;
        public int MaximunRedirections = 10;
        int _redirections = 0;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public Channel(Protocol[] Protocols, ChannelRole ChannelRole, ChannelSerializationMode ChannelSerializationMode, bool IsPoint2Point)
        {
            //set
            this.ChannelRole = ChannelRole;
            this.ChannelSerializationMode = ChannelSerializationMode;
            this.IsPoint2Point = IsPoint2Point;

            //build lookups    
            DebugEx.Assert(Protocols != null && Protocols.Length > 0, "Null protocol");
            if (Protocols != null && Protocols.Length > 0)
            {
                if (ChannelRole == ChannelRole.Client)
                    DebugEx.Assert(Protocols.Length == 1, "Only one concurrent protocol is supported for Clients");

                //process protocols
                foreach (var proto in Protocols)
                {
                    var protoHelper = new ProtocolHelper()
                    {
                        Version = proto.Version,
                    };

                    //Zero id is reserved for NoType
                    UInt32 id = 0;

                    //build channel intrinsic protocol messages lookups
                    for (UInt32 n = 0; n < IntrinsicMessages.Length; n++)
                    {
                        id++;
                        protoHelper.ProtocolLookup.Add(IntrinsicMessages[n], id);
                        protoHelper.ProtocolReverseLookup.Add(id, IntrinsicMessages[n]);
                    }
                    DebugEx.Assert(id < 100, "Protocol Group Overflow");
                    id = 100;

                    //setup definitions
                    var gnIND = 0;
                    foreach (var entry in proto.ProtocolDefinitions)
                    {
                        if (!string.IsNullOrEmpty(entry.GroupName))
                            protoHelper.ProtocolDefinitions.ForceAdd(entry.GroupName, entry.MessageTypes);
                        else
                        {
                            DebugEx.Assert("Null Protocol GroupName detected");
                            //release mode failure handling (guestimating)
                            protoHelper.ProtocolDefinitions.TryAdd("UnnamedGroup-" + gnIND, entry.MessageTypes);
                            gnIND++;
                        }
                    }

                    //create lookup helpers
                    for (int pGroup = 0; pGroup < proto.ProtocolDefinitions.Count; pGroup++)
                    {
                        //get group
                        var group = proto.ProtocolDefinitions[pGroup];
                        DebugEx.Assert(group.MessageTypes != null, "Null Protocol Group MessageTypes detected");
                        if (group.MessageTypes == null)
                            continue;
                        DebugEx.Assert(group.MessageTypes.Length < 100, "Protocol Group Overflow");

                        //compute id start for group
                        id = (UInt32)((pGroup + 1) * 100);

                        //add group items
                        for (int n = 0; n < group.MessageTypes.Length; n++)
                        {
                            CheckType(group.MessageTypes[n], group.MessageTypes[n].Name);
                            id++;
                            protoHelper.ProtocolLookup.Add(group.MessageTypes[n], id);
                            protoHelper.ProtocolReverseLookup.Add(id, group.MessageTypes[n]);
                        }
                    }
                    //add protocol helper
                    ProtocolHelpers.Add(proto.Version, protoHelper);
                }

                //select default active protocol
                ActiveProtocol = ProtocolHelpers.First().Value;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public void Start()
        {
            //check if already started
            if (_State != ChannelStates.Initializing)
                return;

            //set state
            _State = ChannelStates.Negotiating;

            //start negotiation monitor
            Task.Run(() =>
            {
                Task.Delay(NegotiationTimeout).Wait();
                if (State == ChannelStates.Negotiating)
                {
                    DebugEx.TraceError("Channel Negotation Timeout (name=" + Name + ")");
                    Close();
                }
            });

            //start negotiation if needed, else open it
            if (!IsPoint2Point)
            {
                //change state
                _State = ChannelStates.Open;
                DebugEx.TraceLog("Opened channel (name=" + Name + ")");

                //open channel
                onOpen();
            }
            else if (ChannelRole == YPChannel.ChannelRole.Server)
            {
                //start negotiation
                if (!doNegotiation())
                {
                    //not connected
                    if (State == ChannelStates.Closed)
                        return;
                    //send open channel message to client
                    var negResultMsg = new NegotationFinishMessage()
                    {
                        StatusCode = YPCStatusCodes.NegotiationFailed,
                    };
                    _sendMessage(negResultMsg, null, null, false);
                    //give some time for the message to leave before force-closing the channel
                    _flush();
                    Task.Delay(200).Wait();
                    //negotiation failed.. close channel
                    Close();
                    return;
                }

                //change state
                _State = ChannelStates.Open;
                DebugEx.TraceLog("Opened channel (channel name=" + Name + ")");

                //send negotiation results (will open channel at client)
                {
                    var negoResultMsg = new NegotationFinishMessage()
                    {
                        StatusCode = YPCStatusCodes.OK,
                        ChannelKey = ChannelKey,
                    };
                    SendMessage(negoResultMsg);
                }

                //open channel
                onOpen();
            }

            //start heartbeat thread
            task = Task.Run((Action)HeartBeat);
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected virtual bool doNegotiation()
        {
            DebugEx.Assert(ChannelRole == YPChannel.ChannelRole.Server, "Only server can do negotiation");

            //do any negotiation stuff
            try
            {
                //do intrinsic negotiation
                var heloMsg = new HELLORequest()
                {
                    SuportedProtocols = ProtocolHelpers.Values.Select(h => h.Version).ToArray(),
                    Message = "Hello from " + LocalIdentifier,
                    ChannelFlags = (UInt32)LocalChannelFlags,
                    ChannelKey = ChannelKey,
                };
                var heloRsp = SendRequest<HELLOResponse>(heloMsg, Timeout: TimeSpan.FromSeconds(30));
                if (heloRsp == null)
                {
                    DebugEx.TraceError("YPChannel HELLO request failed (remote=" + RemoteIdentifier + ")");
                    return false;
                }

                //activate protocol
                if (ProtocolHelpers.ContainsKey(heloRsp.Version) == false)
                {
                    DebugEx.TraceError("YPChannel HELLO response provided invalid protocol version (remote=" + RemoteIdentifier + ")");
                    return false;
                }
                else
                    ActiveProtocol = ProtocolHelpers[heloRsp.Version];

                //get flags
                _RemoteChannelFlags = (ChannelFlags)heloRsp.ChannelFlags;

                //user negotiation
                if (NegotiationHandler != null)
                {
                    if (!NegotiationHandler(this))
                        return false;
                }
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Unhandle exception from derived class or event while negotiating channel open"); Close(); return false; }

            //clear all requests
            lock (PendingRequests)
                PendingRequests.Clear();

            //negotiation completed
            return true;
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected virtual void onOpen()
        {
            //change state
            _State = ChannelStates.Open;
            DebugEx.TraceLog("Opened channel (channel name=" + Name + ")");

            //raise event
            try
            {
                if (OnOpenEvent != null)
                    OnOpenEvent(this);
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Unhandle exception from derived class or event while opening channel"); Close(); return; }
        }

        //------------------------------------------------------------------------------------------------------------------------
        protected virtual void onRedirect(string Target)
        {
            //override withOUT calling base in order to redirect
            Close();
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Close()
        {
            lock (syncRoot)
            {
                try
                {
                    //not connected
                    if (_State == ChannelStates.Closed || _closing)
                        return;

                    //mark closing
                    _closing = true;

                    //unpause channel
                    IsPaused = false;

                    //send open channel message to client (only if we are 1-1 channel and had negotiation)
                    if (IsPoint2Point)
                    {
                        var negResultMsg = new ChannelCloseMessage()
                        {
                        };
                        _sendMessage(negResultMsg, null, null, false);
                    }

                    //set flag to closed (after sending message)
                    _State = ChannelStates.Closed;

                    //give some time for the message to leave before force-closing the channel
                    _flush();
                    Task.Delay(100).Wait();

                    //inform
                    DebugEx.TraceLog("Closed channel (channel name=" + Name + ")");

                    //examine pending requests
                    lock (PendingRequests)
                    {
                        //unlock pending request threads
                        foreach (var req in PendingRequests.Values)
                            lock (req)
                                Monitor.Pulse(req);

                        //clear pending requests
                        PendingRequests.Clear();
                    }

                    //catch derived class or event exceptions
                    try
                    {
                        //inform derived classes
                        onClose();
                        //raise event
                        //TODO: raise event
                        var cbClosed = OnClosedEvent;
                        if (cbClosed != null)
                            cbClosed(this);
                    }
                    catch (Exception ex) { DebugEx.Assert(ex, "Unhandle exception from derived class or event  while closing channel"); }
                }
                catch (Exception ex) { DebugEx.Assert(ex, "Exception while closing channel"); }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected virtual void onClose()
        {
            //nothing.. override to add stuff
        }
        //------------------------------------------------------------------------------------------------------------------------
        internal void HeartBeat()
        {
            try
            {
                //heartbeat
                while (IsOpen || IsNegotiating)
                {
                    //check if paused
                    if (!IsNegotiating && IsPaused)
                    {
                        var pc = pauseCompletion;
                        if (pc != null)
                            pc.Task.GetResults(); //wait for unpause
                    }

                    //heartheat delay
                    if (HeartBeatSpinDelay != TimeSpan.Zero)
                        Task.Delay(HeartBeatSpinDelay).Wait();

                    //custom prepare
                    var _hbp = HeartBeatPrepare;
                    if (_hbp != null)
                        _hbp(this);

                    //keep heartbeat thread id
#if NETFX
                    heartBeatThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#else
                    heartBeatThreadID = task.Id;
#endif

                    //receive message object
                    bool ChannelError;
                    var msg = messageReceiver(out ChannelError);
                    if (ChannelError)
                        break;
                    if (msg == null)
                        continue;
                    if (msg.Payload == null)
                    {
                        DebugEx.Assert("Null payload detected");
                        continue;
                    }
                    //is response?
                    if (msg.Flags.HasFlag(MessageFlags.Response))
                    {
                        //get request id
                        var reqID = (UInt32)msg.RespondToRequestID;
                        //find request (and consume)
                        RequestInfo req;
                        lock (PendingRequests)
                        {
                            req = PendingRequests.TryGetOrDefault(reqID);
                            PendingRequests.Remove(reqID);
                        }
                        //check for valid object
                        if (req == null)
                        {
                            DebugEx.TraceWarning("Could not find request object from msg id (timed out?)");
                            continue;
                        }
                        //give response to request object and pulse-wake it
                        lock (req)
                        {
                            req.resp = msg;
                            Monitor.Pulse(req);
                        }
                    }
                    else
                    {
                        //handle intrinsic messages                        
                        if (msg.Payload is HELLORequest)
                        {
                            if (ChannelRole == YPChannel.ChannelRole.Client)
                            {
                                var typed = msg.Payload as HELLORequest;
                                //check for supported versions
                                if (typed.SuportedProtocols.Contains(ActiveProtocol.Version) == false)
                                {
                                    DebugEx.Assert("No supported protocol version detected");
                                    Close();
                                }
                                else
                                {
                                    //send response
                                    var helloResponse = new HELLOResponse()
                                    {
                                        Version = ActiveProtocol.Version,
                                        Message = "Hello from " + LocalIdentifier,
                                        ChannelKey = ChannelKey,
                                        ChannelFlags = (UInt32)LocalChannelFlags,
                                        ProtocolDefinitions = ActiveProtocol.ProtocolDefinitions
                                                                            .Select(kv =>
                                                                            new HELLOResponse.MessageTypeGroupPacked()
                                                                            {
                                                                                GroupName = kv.Key,
                                                                                MessageTypes = kv.Value.Select(t => t.AssemblyQualifiedName_Portable()).ToArray()
                                                                            }).ToArray(),
                                    };
                                    SendResponse(helloResponse, msg.MessageID);
                                }
                            }
                            else
                            {
                                DebugEx.Assert("Server received a HELLORequest from client.. Server doesn't like this");
                                Close();
                            }
                        }
                        else if (msg.Payload is RedirectionMessage)
                        {
                            if (ChannelRole == YPChannel.ChannelRole.Client)
                            {
                                var typed = msg.Payload as RedirectionMessage;
                                if (!string.IsNullOrWhiteSpace(typed.RedirectionTarget) && AutoRedirect)
                                {
                                    _redirections++;
                                    if (_redirections >= MaximunRedirections)
                                        Close();
                                    else
                                    {
                                        DebugEx.TraceLog("YPChannel : redirecting client to " + typed.RedirectionTarget);
                                        onRedirect(typed.RedirectionTarget);
                                    }
                                }
                                else
                                    Close();
                            }
                            else
                            {
                                DebugEx.Assert("Server received a RedirectionMessage from client.. Server doesn't like this");
                                Close();
                            }
                        }
                        else if (msg.Payload is NegotationFinishMessage)
                        {
                            if (ChannelRole == YPChannel.ChannelRole.Client)
                            {
                                var typed = msg.Payload as NegotationFinishMessage;
                                //open client-side channel
                                if (typed.IsSuccessCode())
                                {
                                    //keep channel key
                                    _ChannelKey = typed.ChannelKey;
                                    //open channel
                                    onOpen();
                                }
                                else
                                {
                                    //close channel
                                    Close();
                                }
                            }
                            else
                            {
                                DebugEx.Assert("Server received a negotiation finish from client.. Server doesn't like this");
                                Close();
                            }
                        }
                        else if (msg.Payload is BandwidthManagementMessage)
                        {
                            RequestedBandwidthManagement = msg.Payload as BandwidthManagementMessage;
                        }
                        else if (msg.Payload is ChannelCloseMessage)
                        {
                            //closing channel
                            if (State != ChannelStates.Closed)
                                Close();
                        }
                        else if (msg.Payload is PingRequest)
                        {
                            try
                            {
                                var pingMsg = (msg.Payload as PingRequest).Message;
                                if (pingMsg == null || (pingMsg != null && pingMsg.Length < PongReplySizeLimit))
                                {
                                    var pongMsg = new PongResponse() { Message = pingMsg };
                                    SendResponse(pongMsg, msg.MessageID);
                                }
                            }
                            catch (Exception exx) { DebugEx.TraceError(exx, "Error while replying to ping"); }
                        }
                        else
                        {
                            //generic handler
                            var OnMessageReceivedCB = OnMessageReceived;
                            try
                            {
                                if (OnMessageReceivedCB != null)
                                    OnMessageReceivedCB(this, msg);
                            }
                            catch (Exception ex) { DebugEx.Assert(ex, "YPChannel message handler caused an exception"); }
                        }
                    }
                }
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Channel Heartbeat had exception"); }

            //close channel
            Close();
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected virtual void _flush()
        { }
        //------------------------------------------------------------------------------------------------------------------------
        protected abstract YPMessagePacked _readPackedMessage();
        protected abstract void _sendPackedMessage(YPMessagePacked msg);
        //------------------------------------------------------------------------------------------------------------------------
        protected virtual YPMessage messageReceiver(out bool ChannelError)
        {
            try
            {
                //unpack encapsulation
                YPMessagePacked packedMsg;
                lock (readLock)
                    packedMsg = _readPackedMessage();
                if (packedMsg == null)
                {
                    ChannelError = true;
                    return null;
                }

                //unpack message
                var msg = new YPMessage();
                msg.Flags = (MessageFlags)packedMsg.Flags;
                msg.MessageID = packedMsg.MessageID;
                msg.RespondToRequestID = packedMsg.RequestID;
                if (packedMsg.MessageIE != null)
                {
                    msg.MessageIE = new Dictionary<string, object>();
                    foreach (var ie in packedMsg.MessageIE)
                        msg.MessageIE.Add(ie.Key, ie.Value.Object);
                }

                //unpack payload
                {
                    Type objType = null;
                    //find type by protocol msg id
                    var typeID = packedMsg.Type;
                    ActiveProtocol.ProtocolReverseLookup.TryGetValue(typeID, out objType);
                    DebugEx.Assert(objType != null, "YPChannel : Message received is not part of protocol");
                    //try to find type by extended type
                    if (EnableExtendedTypes && objType == null && msg.MessageIE != null && msg.MessageIE.ContainsKey(MessageIEKeys.ExtendedType))
                        objType = Yodiwo.TypeCache.GetType(msg.MessageIE[MessageIEKeys.ExtendedType] as string);
                    //check
                    if (objType == null)
                    {
                        DebugEx.Assert("Could not resolve type");
                        ChannelError = false;
                        return null;
                    }
                    ///unpack
                    if (ChannelSerializationMode == YPChannel.ChannelSerializationMode.MessagePack)
                    {
                        var serializer2 = MessagePackSerializer.Get(objType);
                        msg.Payload = serializer2.UnpackSingleObject(packedMsg.Payload);
                    }
                    else if (ChannelSerializationMode == YPChannel.ChannelSerializationMode.Json)
                    {
                        var serialiser = new Newtonsoft.Json.JsonSerializer();
                        using (var memStream = new MemoryStream(packedMsg.Payload))
                        using (StreamReader reader = new StreamReader(memStream, Encoding.UTF8))
                        using (var jsonTextReader = new Newtonsoft.Json.JsonTextReader(reader))
                            msg.Payload = JSONEncapsulationReadSerializer.Deserialize(jsonTextReader, objType);
                    }
                    else
                    {
                        DebugEx.Assert("Unkown serialization method");
                        ChannelError = true;
                        return null;
                    }
                }

                //return final objext
                ChannelError = false;
                return msg;
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "MessageReceiver catched unhandled exception");
                //Something went wrong here..
                Task.Delay(50).Wait();
                ChannelError = true;
                return null;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected UInt32 GenerateMessageID()
        {
            //generate id
            UInt32 id;
            var id_int = Interlocked.Increment(ref messageIDCnt);
            unchecked { id = (UInt32)id_int; }
            return id;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public TResponse SendRequest<TResponse>(object Payload, TimeSpan? Timeout = null, TResponse Defaut = default(TResponse))
        {
            var resp = SendRequest(Payload, Timeout: Timeout);
            if (resp == null)
                return Defaut;
            else
                return (TResponse)resp.Payload;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual YPMessage SendRequest(object Payload, TimeSpan? Timeout = null)
        {
            if (IsNegotiating && ChannelRole != YPChannel.ChannelRole.Server)
            {
                DebugEx.Assert("Only server can send responses while negotiating.");
                Task.Delay(50).Wait(); //penalty
                return null;
            }

            //check for closed channel
            if (_State == ChannelStates.Closed)
            {
                Task.Delay(50).Wait(); //penalty
                return null;
            }

            //check for open channel
            if (!IsNegotiating)
            {
                //check if channel is open
                if (!IsOpen)
                {
                    DebugEx.TraceWarning("Trying to send message on a channel that is not open (channel name=" + Name + " , msg=" + Payload.GetType() + ")");
                    Task.Delay(50).Wait(); //failsafe penalty
                    return null;
                }

                //get threadID and check it is heartbeat.. this will protect against deadlock
#if NETFX
                var tid = Thread.CurrentThread.ManagedThreadId;
#else
                var tid = Task.CurrentId == null ? 0 : Task.CurrentId.Value;
#endif
                if (heartBeatThreadID == tid)
                {
                    DebugEx.Assert("Cannot send a request from the same thread that you are handling incoming messages, as this will result in a deadlock." + Environment.NewLine + "Use a Task.Run() to send from a different thread");
                    Task.Delay(50).Wait(); //failsafe penalty
                    return null;
                }
            }

            //generate an id for new message
            var id = GenerateMessageID();

            //create a request 'token'
            var req = new RequestInfo()
            {
                RequestID = id,
            };
            //add to dictionary
            if (!IsNegotiating)
                lock (PendingRequests)
                    PendingRequests.Add(id, req);

            //lock on request (before sending message!)
            lock (req)
            {
                //send message
                _sendMessage(Payload, id, null, true);
                if (IsNegotiating)
                {
                    //warmup channel (flush)
                    Task.Delay(50).Wait();
                    //wait for response in the negotiating thread (heartbeat)
                    bool ChannelError;
                    YPMessage respMsg = null;
                    while (respMsg == null || (respMsg != null && respMsg.RespondToRequestID != id))
                    {
                        respMsg = messageReceiver(out ChannelError);
                        if (ChannelError)
                        {
                            Close();
                            return null;
                        }
                    }
                    return respMsg;
                }
                else
                {
                    //wait for response
                    if (!Timeout.HasValue)
                        Monitor.Wait(req, DefaultRequestTimeoutms);
                    else if (Timeout.HasValue && Timeout.Value != TimeSpan.Zero)
                        Monitor.Wait(req, (int)Timeout.Value.TotalMilliseconds);
                    else
                        Monitor.Wait(req);
                    //get response
                    var resp = req.resp;
                    //if null(timeout) then remove id from dictionary
                    if (resp == null)
                        lock (PendingRequests)
                            PendingRequests.Remove(id);
                    //return new response (null if timeout)
                    return resp;
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public Task SendResponseAsync(object Payload, UInt32 ResponseToMsg)
        {
            return Task.Run(() => SendResponse(Payload, ResponseToMsg));
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual void SendResponse(object Payload, UInt32 ResponseToMsg)
        {
            if (IsNegotiating && ChannelRole != YPChannel.ChannelRole.Client)
            {
                DebugEx.Assert("Only client can send responses while negotiating.");
                Task.Delay(50).Wait();  //penalty
                return;
            }
            _sendMessage(Payload, null, ResponseToMsg, false);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual void SendMessage(object Payload)
        {
            if (IsNegotiating)
            {
                DebugEx.Assert("Cannot send message while negotiating. Only Request/Respose");
                Task.Delay(50).Wait();  //penalty
                return;
            }
            _sendMessage(Payload, null, null, false);
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected virtual void _sendMessage(object Payload, UInt32? MessageID, UInt32? ResponseToMsg, bool IsRequest)
        {
            //declares
            var flags = MessageFlags.None;
            var MessageIE = new List<MessageIE>();

            //snaity check
            DebugEx.Assert(Payload != null, "Cannot send Null");
            if (Payload == null || ActiveProtocol == null)
            {
                Task.Delay(50).Wait();  //failsafe penalty
                return;
            }

            //check if channel is open
            if (State == ChannelStates.Closed)
            {
                DebugEx.TraceWarning("Trying to send message on a channel that is not open (channel name=" + Name + ")");
                Task.Delay(50).Wait();  //failsafe penalty
                return;
            }

            //get type
            var objType = Payload.GetType();

            //serialize object(payload)
            byte[] payloadBuffer;
            ///unpack
            if (ChannelSerializationMode == YPChannel.ChannelSerializationMode.MessagePack)
            {
                var serializer2 = MessagePackSerializer.Get(Payload.GetType());
                payloadBuffer = serializer2.PackSingleObject(Payload);
            }
            else if (ChannelSerializationMode == YPChannel.ChannelSerializationMode.Json)
            {
                payloadBuffer = Payload.ToJSON2(HtmlEncode: false);
            }
            else
            {
                DebugEx.Assert("Unkown serialization method");
                return;
            }

            //get or generate id
            UInt32 id;
            if (MessageID.HasValue)
                id = MessageID.Value;
            else
                id = GenerateMessageID();

            //find msgtype id (or add as extended type in IE)
            UInt32 TypeID = 0;
            if (ActiveProtocol.ProtocolLookup.TryGetValue(objType, out TypeID) == false)
            {
                DebugEx.Assert("YPChannel : Message send is not part of protocol");
                if (EnableExtendedTypes)
                {
                    CheckType(Payload.GetType(), Payload.GetType().Name);
                    MessageIE.Add(new MessageIE(MessageIEKeys.ExtendedType, Payload.GetType().AssemblyQualifiedName_Portable()));
                }
                else
                {
                    DebugEx.Assert("Message is not part of protocol (" + objType.ToString() + ")");
                    return;
                }
            }

            //is response?
            if (ResponseToMsg.HasValue)
                flags |= MessageFlags.Response;

            //is request?
            if (IsRequest)
                flags |= MessageFlags.Request;

            //encapsulate
            var msg = new YPMessagePacked()
            {
                Type = TypeID,
                Flags = (byte)flags,
                MessageID = id,
                RequestID = ResponseToMsg.HasValue ? ResponseToMsg.Value : 0,
                Payload = payloadBuffer,
                MessageIE = MessageIE.Count > 0 ? MessageIE.ToArray() : null,
            };

            //get writelock
            lock (writeLock)
            {
                //check throttling
                if ((RequestedBandwidthManagement != null && RequestedBandwidthManagement.Enable) ||
                    (LocalTxBandwidthManagement != null && LocalTxBandwidthManagement.Enable))
                {
                    while (State != ChannelStates.Closed)
                    {
                        //get caps
                        var reqBand = RequestedBandwidthManagement;
                        var reqBandCap = reqBand == null || reqBand.Enable == false ? uint.MaxValue : reqBand.BandwidthCap;
                        var reqMsgCap = reqBand == null || reqBand.Enable == false ? uint.MaxValue : reqBand.MessageCap;
                        var localBand = LocalTxBandwidthManagement;
                        var localBandCap = localBand == null || localBand.Enable == false ? uint.MaxValue : localBand.BandwidthCap;
                        var localMsgCap = localBand == null || localBand.Enable == false ? uint.MaxValue : localBand.MessageCap;

                        //choose caps
                        var bandCap = Math.Min(reqBandCap, localBandCap);
                        var msgCap = Math.Min(reqMsgCap, localMsgCap);

                        //check metrics
                        if (TransmittedBytesPerSecond > bandCap)
                            Task.Delay(200).Wait();
                    }

                    //check for closed channel
                    if (State == ChannelStates.Closed)
                        return;
                }

                //serialize and send message
                _sendPackedMessage(msg);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void SetRemoteBandwidthManagment(bool Enable, uint BandwidthCap, uint MessageCap)
        {
            var msg = new BandwidthManagementMessage()
            {
                Enable = Enable,
                BandwidthCap = BandwidthCap,
                MessageCap = MessageCap
            };
            SendMessage(msg);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void SetLocalTxBandwidthManagment(bool Enable, uint BandwidthCap, uint MessageCap)
        {
            var msg = new BandwidthManagementMessage()
            {
                Enable = Enable,
                BandwidthCap = BandwidthCap,
                MessageCap = MessageCap
            };
            LocalTxBandwidthManagement = msg;
        }
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary> Check that is MsgPack-friendly type </summary>
        [System.Diagnostics.Conditional("DEBUG")]
        protected virtual void CheckType(Type type, string errorPath)
        {
#if DEBUG
            //ignore if checked already
            lock (CheckedTypes)
                if (CheckedTypes.Contains(type))
                    return;
                else
                    CheckedTypes.Add(type);
#endif
            //basic type check
#if NETFX
            if (type.IsAbstract || type.IsInterface || type.GetType() == typeof(object) || type.IsNotPublic)
#else
            if (type.GetTypeInfo().IsAbstract || type.GetTypeInfo().IsInterface || type.GetType() == typeof(object) || type.GetTypeInfo().IsNotPublic)
#endif
            {
                DebugEx.Assert("Cannot use Abstract/Interface/Object/NonPublic types for YPChannel message." + Environment.NewLine + "Type failed : " + type.GetFriendlyName() + Environment.NewLine + "Path: " + errorPath);
                return;
            }

            //check for StructLayout
#if NETFX
            if (!type.IsPrimitive && type != typeof(string) && !typeof(IList).IsAssignableFrom(type) && !type.IsArray && !type.IsEnum)
#else
            if (!type.GetTypeInfo().IsPrimitive && type != typeof(string) && !typeof(IList).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()) && !type.IsArray && !type.GetTypeInfo().IsEnum)
#endif
            {
                //DebugEx.Assert(type.IsLayoutSequential, "Messages should define the StructLayout Attribute with Sequential layout" + Environment.NewLine + "Hint: add [StructLayout(LayoutKind.Sequential)] on class" + Environment.NewLine + "Type failed : " + type.GetFriendlyName() + Environment.NewLine + "Path: " + errorPath);

                //check members
#if NETFX
                var members = type.GetMembers(BindingFlags.Public | BindingFlags.Instance);
#else
                var members = type.GetTypeInfo().DeclaredMembers;
#endif
                foreach (var member in members)
                    if (member is FieldInfo || member is PropertyInfo)
                    {
                        //skip non-serializables
                        if (member.IsDefined(typeof(NonSerializedAttribute)))
                            continue;

                        var memberType = member.GetMemberType();
                        if (memberType != null)
                            CheckType(memberType, errorPath + " -> " + member.Name);
                    }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public TimeSpan Ping(string Message, TimeSpan? Timeout = null)
        {
            var timestamp = DateTime.Now;
            var pingMsg = new PingRequest() { Message = Message };
            var pong = SendRequest<PongResponse>(pingMsg, Timeout: Timeout);
            if (pingMsg.Message == pong.Message)
                return DateTime.Now - timestamp;
            else
                return TimeSpan.Zero;
        }
        //------------------------------------------------------------------------------------------------------------------------
        ~Channel()
        {
            try
            {
                if (State != ChannelStates.Closed)
                    Close();
            }
            catch (Exception ex)
            {
                DebugEx.TraceError(ex, "Channel destructor caught unhandled exception");
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
