//#define CHECK_TYPES

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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
        ChannelFlags _LocalChannelFlags = ChannelFlags.None | ChannelFlags.GZip | ChannelFlags.Deflate | ChannelFlags.UsePayloadStr | ChannelFlags.AdaptiveProtocol;
        public ChannelFlags LocalChannelFlags { get { return _LocalChannelFlags; } }
        //------------------------------------------------------------------------------------------------------------------------
        ChannelFlags _RemoteChannelFlags = ChannelFlags.None;
        public ChannelFlags RemoteChannelFlags { get { return _RemoteChannelFlags; } }
        //------------------------------------------------------------------------------------------------------------------------
        protected bool use_GZip => _LocalChannelFlags.HasFlag(ChannelFlags.GZip) && _RemoteChannelFlags.HasFlag(ChannelFlags.GZip);
        protected bool use_Deflate => _LocalChannelFlags.HasFlag(ChannelFlags.Deflate) && _RemoteChannelFlags.HasFlag(ChannelFlags.Deflate);
        protected bool use_Compression => use_GZip || use_Deflate;
        protected bool use_PayloadStr => _LocalChannelFlags.HasFlag(ChannelFlags.UsePayloadStr) && _RemoteChannelFlags.HasFlag(ChannelFlags.UsePayloadStr);
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary> The threshold (in bytes) after which the gzip enables </summary>
        public int CompressThreshold = 1 * 1024;
        //------------------------------------------------------------------------------------------------------------------------
        const int ChannelProtocolGroupMessageCount = 1000000;
        //------------------------------------------------------------------------------------------------------------------------
        public readonly ChannelSerializationMode SupportedChannelSerializationModes;
        public readonly ChannelSerializationMode PreferredChannelSerializationModes;
        protected ChannelSerializationMode _ChannelSerializationMode = ChannelSerializationMode.Unkown;
        public ChannelSerializationMode ChannelSerializationMode => _ChannelSerializationMode;
        //------------------------------------------------------------------------------------------------------------------------
        DateTime _channelCreatedTimestamp = DateTime.Now;
        public DateTime ChannelCreatedTimestamp => _channelCreatedTimestamp;
        //------------------------------------------------------------------------------------------------------------------------
#if UNIVERSAL
        internal protected Task heartbeat;
#else
        internal protected Thread heartbeat;
#endif
        object syncRoot = new object();
        object readLock = new object();
        object writeLock = new object();
        byte[] sizeBuf = new byte[2];
        Int32 messageIDCnt = 0;
        int heartBeatThreadID = -1;
        public TimeSpan HeartBeatSpinDelay = TimeSpan.Zero;
        //------------------------------------------------------------------------------------------------------------------------
        public delegate bool OnNegiationEventHandler(Channel Channel);
        public OnNegiationEventHandler NegotiationHandler = null;
        //------------------------------------------------------------------------------------------------------------------------
        public delegate void OnOpenEventHandler(Channel Channel);
        public event OnOpenEventHandler OnOpenEvent = null;
        //------------------------------------------------------------------------------------------------------------------------
        public delegate void OnClosedEventHandler(Channel Channel, string Message);
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
            public string PayloadStr;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public ISerializer MsgPack;
        internal protected Newtonsoft.Json.JsonSerializer JSONEncapsulationReadSerializer = new Newtonsoft.Json.JsonSerializer();
        internal protected Newtonsoft.Json.JsonSerializer JSONEncapsulationWriteSerializer = new Newtonsoft.Json.JsonSerializer();
        //------------------------------------------------------------------------------------------------------------------------
        protected class ProtocolHelper
        {
            public int Version;
            public Dictionary<Type, UInt32> ProtocolLookup = new Dictionary<Type, UInt32>();
            public Dictionary<UInt32, Type> ProtocolReverseLookup = new Dictionary<UInt32, Type>();
            public Dictionary<string, int> ProtocolGroupId = new Dictionary<string, int>();
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
        DictionaryTS<UInt32, RequestInfo> PendingRequests = new DictionaryTS<UInt32, RequestInfo>();
        //------------------------------------------------------------------------------------------------------------------------
        protected ChannelStates _State = ChannelStates.Initializing;
        public ChannelStates State { get { return _State; } }
        //------------------------------------------------------------------------------------------------------------------------
        public bool IsNegotiating { get { return _State == ChannelStates.Negotiating; } }
        public bool IsOpen { get { return _State == ChannelStates.Open; } }
        public bool IsClosed { get { return _State == ChannelStates.Closed; } }
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
        const string IntrinsicMessages_GroupName = "Yodiwo.YPChannel.IntrinsicMessages";
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
            typeof(ErrorMessage)
        };
        //------------------------------------------------------------------------------------------------------------------------
        public readonly DictionaryTS<Type, object> Tags = new DictionaryTS<Type, object>();
        //------------------------------------------------------------------------------------------------------------------------
        bool _closing = false;
        //------------------------------------------------------------------------------------------------------------------------
        public int DefaultRequestTimeoutms = 30 * 1000;
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
                if (State == ChannelStates.Open || State == ChannelStates.Paused)
                    lock (syncRoot)
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
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary> Custom to code to run at the start of each heartbeat</summary>
        public Action<Channel> HeartBeatPrepare = null;
        //------------------------------------------------------------------------------------------------------------------------
        //Keepalive mechanism
#if UNIVERSAL
        internal protected Task keepAliveTask;
#else
        internal protected Thread keepAliveTask;
#endif
        private object keepAliveTaskSyncLock = new object();
        public bool IsKeepAliveEnabled = true;
        public TimeSpan KeepAliveSpinDelay;
        public TimeSpan KeepAlivePingTimeout = TimeSpan.FromMinutes(1);
        //------------------------------------------------------------------------------------------------------------------------
        DateTime _LastActivityTimestamp = DateTime.Now;
        DateTime LastActivityTimestamp => _LastActivityTimestamp;
        //------------------------------------------------------------------------------------------------------------------------
        object _InactiveLineWarmupLocker = new object();
        public TimeSpan InactiveLineWarmupTimeout = TimeSpan.FromMinutes(3);
        public TimeSpan LineWarmupPingTimeout = TimeSpan.FromSeconds(5);
        //------------------------------------------------------------------------------------------------------------------------
        public bool AutoRedirect = true;
        public int MaximunRedirections = 10;
        int _redirections = 0;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public Channel(Protocol[] Protocols, ChannelRole ChannelRole, ChannelSerializationMode SupportedChannelSerializationModes, ChannelSerializationMode PreferredChannelSerializationModes, bool IsPoint2Point, TimeSpan? keepAliveSpinDelay)
        {
            //set
            this.ChannelRole = ChannelRole;
            this.SupportedChannelSerializationModes = SupportedChannelSerializationModes;
            this.PreferredChannelSerializationModes = PreferredChannelSerializationModes;
            this.IsPoint2Point = IsPoint2Point;
            this.KeepAliveSpinDelay = (keepAliveSpinDelay == null) ? TimeSpan.FromMinutes(5) : keepAliveSpinDelay.Value;

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
                    protoHelper.ProtocolGroupId.Add(IntrinsicMessages_GroupName, 0);
                    for (UInt32 n = 0; n < IntrinsicMessages.Length; n++)
                    {
                        id++;
                        protoHelper.ProtocolLookup.Add(IntrinsicMessages[n], id);
                        protoHelper.ProtocolReverseLookup.Add(id, IntrinsicMessages[n]);
                    }
                    protoHelper.ProtocolDefinitions.ForceAdd(IntrinsicMessages_GroupName, IntrinsicMessages);
                    DebugEx.Assert(id < ChannelProtocolGroupMessageCount, "Protocol Group Overflow");
                    id = ChannelProtocolGroupMessageCount;

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
                        DebugEx.Assert(group.MessageTypes.Length < ChannelProtocolGroupMessageCount, "Protocol Group Overflow");

                        //compute id start for group
                        id = (UInt32)((pGroup + 1) * ChannelProtocolGroupMessageCount);
                        protoHelper.ProtocolGroupId.Add(group.GroupName, pGroup + 1);

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
        protected void Reset(bool clearPendingRequests = true)
        {
            try
            {
                _State = ChannelStates.Initializing;
                _closing = false;
                ActiveProtocol = ProtocolHelpers.First().Value;
                _redirections = 0;
                messageIDCnt = 0;

                //clear and unlock pending requests
                if (clearPendingRequests)
                {
                    var preqs = PendingRequests.ToArray();
                    PendingRequests.Clear();
                    foreach (var entry in preqs)
                    {
                        entry.Value.resp = null;
                        lock (entry.Value)
                            Monitor.Exit(entry.Value);
                    }
                }
            }
            catch (Exception ex) { DebugEx.Assert(ex, "YPC Reset failed (name=" + Name + ")"); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Start()
        {
            //check if already started
            if (_State != ChannelStates.Initializing)
                return;

            //set state
            _State = ChannelStates.Negotiating;

            //start negotiation monitor
            Task.Delay(NegotiationTimeout).ContinueWith(_ =>
            {
                if (State == ChannelStates.Negotiating)
                {
                    Close($"YPChannel ({this}) Negotiation Timeout");
                    DebugEx.TraceWarning($"YPChannel ({this}) Negotiation Timeout)");
                }
            });

            //start negotiation if needed, else open it
            if (!IsPoint2Point)
            {
                //change state
                _State = ChannelStates.Open;
                DebugEx.TraceWarning($"Opened channel (name={this})");

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
                    _flush(200);
                    Thread.Sleep(200);
                    //negotiation failed.. close channel
                    Close($"Channel {this} Negotation Failed");
                    return;
                }

                //change state
                _State = ChannelStates.Open;
                DebugEx.TraceLog($"Opened channel (name={this})");

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
#if UNIVERSAL
            heartbeat = Task.Factory.StartNew((Action)HeartBeatEntry, TaskCreationOptions.LongRunning);
#else
            heartbeat = new Thread(HeartBeatEntry);
            heartbeat.Name = $"YPChannel {this} heartbeat";
            heartbeat.IsBackground = true;
            heartbeat.Start();
#endif

#if UNIVERSAL
            keepAliveTask = Task.Factory.StartNew((Action)keepAliveEntry, TaskCreationOptions.LongRunning);
#else
            keepAliveTask = new Thread(keepAliveEntry);
            keepAliveTask.Name = $"YPChannel {this} keep-alive";
            keepAliveTask.IsBackground = true;
            keepAliveTask.Start();
#endif
        }
        //------------------------------------------------------------------------------------------------------------------------
        void keepAliveEntry()
        {
            try
            {
                while (State != ChannelStates.Closed)
                {
                    if (IsKeepAliveEnabled && State == ChannelStates.Open)
                    {
                        //ping
                        var pong = Ping("keepalive", Timeout: KeepAlivePingTimeout);
                        //in debug mode keep pinging but do not close channel if a timeout occurs
                        if (pong == null)
                        {
#if !DEBUG
                            Close("KeepAlive failed");  //failed to respond.. close channel
                            return;                     //exit keepalive spin
#endif
                        }
                    }

                    //sleep
                    lock (keepAliveTaskSyncLock)
                        Monitor.Wait(keepAliveTaskSyncLock, (int)KeepAliveSpinDelay.TotalMilliseconds.ClampFloor(1000));
                }
            }
            catch (Exception ex) { DebugEx.Assert(ex); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected virtual bool doNegotiation()
        {
            DebugEx.Assert(ChannelRole == YPChannel.ChannelRole.Server, $"Only server can do negotiation ({this})");

            //do any negotiation stuff
            try
            {
                //do intrinsic negotiation
                var helloReq = new HELLORequest()
                {
                    SuportedProtocols = ProtocolHelpers.Values.Select(h => h.Version).ToArray(),
                    Message = "Hello from " + LocalIdentifier,
                    ChannelFlags = (UInt32)LocalChannelFlags,
                    ChannelKey = ChannelKey,
                };
                var helloRsp = SendRequest<HELLOResponse>(helloReq, Timeout: TimeSpan.FromSeconds(30));
                if (helloRsp == null)
                {
                    DebugEx.TraceError($"YPChannel ({this}) HELLO request failed (remote={RemoteIdentifier})");
                    return false;
                }

                //get flags
                _RemoteChannelFlags = (ChannelFlags)helloRsp.ChannelFlags;

                //activate protocol
                if (ProtocolHelpers.ContainsKey(helloRsp.Version) == false)
                {
                    DebugEx.TraceError($"YPChannel ({this}) HELLO response provided invalid protocol version (remote={RemoteIdentifier})");
                    return false;
                }
                else
                {
                    //clone and setup protocol
                    ActiveProtocol = ProtocolHelpers[helloRsp.Version].ToJSON().FromJSON<ProtocolHelper>();
                }

                //re-adjust protocol ids based on client information
                if (_RemoteChannelFlags.HasFlag(ChannelFlags.AdaptiveProtocol))
                {
                    //adjust protocol ids
                    var prevLookup = ActiveProtocol.ProtocolDefinitions.Select(kv => new KeyValuePair<string, HashSet<string>>(kv.Key, kv.Value.Select(v => v.AssemblyQualifiedName_Portable()).ToHashSet())).ToDictionary();
                    ActiveProtocol.ProtocolGroupId.Clear();
                    ActiveProtocol.ProtocolLookup.Clear();
                    ActiveProtocol.ProtocolReverseLookup.Clear();

                    //setup protocol
                    foreach (var entry in helloRsp.ProtocolDefinitions)
                        if (prevLookup.ContainsKey(entry.GroupName)) //check that we have this group
                        {
                            var groupDef = prevLookup[entry.GroupName];
                            ActiveProtocol.ProtocolGroupId.Add(entry.GroupName, entry.GroupID);
                            var id = (uint)entry.GroupID * (uint)helloRsp.ProtocolGroupSize;
                            foreach (var msgTypeStr in entry.MessageTypes)
                            {
                                //move index
                                id++;
                                //add if valid
                                if (groupDef.Contains(msgTypeStr)) //check that we have this type
                                {
                                    var type = TypeCache.GetType(msgTypeStr, DeFriendlify: false);
                                    if (type != null)
                                    {
                                        ActiveProtocol.ProtocolLookup.Add(type, id);
                                        ActiveProtocol.ProtocolReverseLookup.Add(id, type);
                                    }
                                }
                            }
                        }
                }

                //user negotiation
                if (NegotiationHandler != null)
                {
                    if (!NegotiationHandler(this))
                        return false;
                }
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, $"Unhandle exception from derived class or event while negotiating channel open (remote={RemoteIdentifier}, name={this})");
                Close("Unhandled exception from derived class or event while negotiating channel open");
                return false;
            }

            //clear all requests
            PendingRequests.Clear();

            //negotiation completed
            return true;
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected virtual void onOpen()
        {
            //change state
            _State = ChannelStates.Open;
            DebugEx.TraceLog($"Opened channel (name={this})");

            //raise event
            try { OnOpenEvent?.Invoke(this); }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception from derived class or event while opening channel");
                Close("Unhandled exception from derived class or event while opening channel");
                return;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        protected virtual void onRedirect(string Target)
        {
            //override withOUT calling base in order to redirect
            Close("No redirection allowed");
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Close(string Message)
        {
            try
            {
                int retries = 0;
                while (!Monitor.TryEnter(syncRoot, 100))
                {
                    retries++;
                    //not connected?
                    if (_State == ChannelStates.Closed || _closing)
                        return;
                    else if (retries > 10)
                        return;
                    else
                        Thread.Sleep(100);
                }
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
                        try
                        {
                            var negResultMsg = new ChannelCloseMessage()
                            {
                                Message = Message,
                            };
                            _sendMessage(negResultMsg, null, null, false);
                        }
                        catch { }

                    //set flag to closed (after sending message)
                    _State = ChannelStates.Closed;

                    //inform keepalive
                    try
                    {
                        lock (keepAliveTaskSyncLock)
                            Monitor.PulseAll(keepAliveTaskSyncLock);
                    }
                    catch { }

                    //give some time for the message to leave before force-closing the channel
                    try { _flush(200); } catch { }
                    Thread.Sleep(100);

                    //inform
                    DebugEx.TraceLog($"Closed channel (name={this}), msg=" + Message);

                    //unlock pending request threads and clear
                    foreach (var req in PendingRequests.GetAndClear())
                        try
                        {
                            if (Monitor.TryEnter(req, 500))
                                Monitor.PulseAll(req);
                        }
                        catch { }

                    //catch derived class or event exceptions
                    try { onClose(Message); }
                    catch (Exception ex) { DebugEx.Assert(ex, $"Unhandled exception from derived class or event while closing channel (name={this}, key={ChannelKey})"); }

                    //raise event
                    TaskEx.RunSafe(() => OnClosedEvent?.Invoke(this, Message));
                }
                finally { Monitor.Exit(syncRoot); }
            }
            catch (Exception ex) { DebugEx.Assert(ex, $"Exception while closing channel (name={this}, key={ChannelKey})"); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected virtual void onClose(string Message)
        {
            //nothing.. override to add stuff
        }
        //------------------------------------------------------------------------------------------------------------------------
        internal void HeartBeatEntry()
        {
            string heartbeat_err_msg = null;
            int penalty = 0;
            try
            {
                //heartbeat
                while (IsOpen || IsNegotiating)
                {
                    //check if paused
                    if (!IsNegotiating && IsPaused)
                    {
                        var pc = pauseCompletion;
                        pc?.Task?.GetResults(); //wait for unpause
                    }

                    //heartheat delay
                    if (HeartBeatSpinDelay != TimeSpan.Zero)
                        Thread.Sleep(HeartBeatSpinDelay);

                    //custom prepare
                    HeartBeatPrepare?.Invoke(this);

                    //keep heartbeat thread id
#if NETFX
                    heartBeatThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#elif UNIVERSAL
                    heartBeatThreadID = heartbeat.Id;
#endif

                    //receive message object
                    bool ChannelError;
                    var msg = messageReceiver(out ChannelError);

                    if (ChannelError || msg == null || msg.Payload == null)
                    {
                        var err_msg = $"YPChannel ({this}) Heartbeat error on messageReceiver; ChanError:{ChannelError};";
                        if (msg == null)
                            DebugEx.TraceError(err_msg + " null incoming wrapper msg)");
                        else
                            DebugEx.TraceError(err_msg + (msg.Payload == null ? " null incoming payload" : ""));

                        if (++penalty > 10)
                            break;
                        else
                        {
                            Thread.Sleep(MathTools.GetRandomNumber(1, 50) * penalty);
                            continue;
                        }
                    }
                    //if (msg == null)
                    //    continue;
                    //if (msg.Payload == null)
                    //{
                    //    DebugEx.Assert($"YPChannel ({this}) Null payload detected");
                    //    continue;
                    //}

                    //timestamp
                    _LastActivityTimestamp = DateTime.Now;

                    //is response?
                    if (msg.Flags.HasFlag(MessageFlags.Response))
                    {
                        //get request id
                        var reqID = (UInt32)msg.RespondToRequestID;
                        //find request (and consume)
                        var req = PendingRequests.TryGetAndRemove(reqID);
                        //check for valid object
                        if (req == null)
                        {
                            DebugEx.TraceWarning($"YPChannel ({this}) Could not find request object from msg id (timed out?)");
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
                                if (typed?.SuportedProtocols?.Contains(ActiveProtocol.Version) == false)
                                {
                                    DebugEx.Assert($"YPChannel ({this}) No supported protocol version detected");
                                    Close("No supported protocol version detected");
                                }
                                else
                                {
                                    //get flags
                                    _RemoteChannelFlags = (ChannelFlags)typed.ChannelFlags;

                                    //readjust groups sizes
                                    if (!_RemoteChannelFlags.HasFlag(ChannelFlags.AdaptiveProtocol))
                                    {
                                        //default value to old default ---- for backward compatibility
                                        var old_ProtocolGroupSize = 100;

                                        //adjust protocol ids
                                        var prevLookup = ActiveProtocol.ProtocolLookup.ToArray();
                                        ActiveProtocol.ProtocolLookup.Clear();
                                        ActiveProtocol.ProtocolReverseLookup.Clear();
                                        foreach (var entry in prevLookup)
                                        {
                                            int groupID = (int)(entry.Value / (uint)ChannelProtocolGroupMessageCount);
                                            var msgID = entry.Value % ChannelProtocolGroupMessageCount;
                                            var finalID = (uint)(old_ProtocolGroupSize * groupID) + msgID;
                                            ActiveProtocol.ProtocolLookup.Add(entry.Key, finalID);
                                            ActiveProtocol.ProtocolReverseLookup.Add(finalID, entry.Key);
                                        }
                                    }

                                    //send response
                                    var helloResponse = new HELLOResponse()
                                    {
                                        Version = ActiveProtocol.Version,
                                        Message = "Hello from " + LocalIdentifier,
                                        ChannelKey = ChannelKey,
                                        ChannelFlags = (UInt32)LocalChannelFlags,
                                        ProtocolGroupSize = ChannelProtocolGroupMessageCount,
                                        ProtocolDefinitions = ActiveProtocol.ProtocolDefinitions
                                                                            .Select(kv =>
                                                                            new HELLOResponse.MessageTypeGroupPacked()
                                                                            {
                                                                                GroupName = kv.Key,
                                                                                GroupID = ActiveProtocol.ProtocolGroupId[kv.Key],
                                                                                MessageTypes = kv.Value.Select(t => t.AssemblyQualifiedName_Portable()).ToArray()
                                                                            }).ToArray(),
                                    };
                                    SendResponse(helloResponse, msg.MessageID);
                                }
                            }
                            else
                            {
                                DebugEx.Assert($"YPChannel ({this}) Server received a HELLORequest from client.. Server doesn't like this");
                                Close("Server received a HELLORequest from client.. Server doesn't like this");
                            }
                        }
                        else if (msg.Payload is RedirectionMessage)
                        {
                            if (ChannelRole == YPChannel.ChannelRole.Client)
                            {
                                var typed = msg.Payload as RedirectionMessage;
                                if (!string.IsNullOrWhiteSpace(typed?.RedirectionTarget) && AutoRedirect)
                                {
                                    _redirections++;
                                    if (_redirections >= MaximunRedirections)
                                        Close("Maximun redirections reached");
                                    else
                                    {
                                        DebugEx.TraceLog($"YPChannel ({this}) : redirecting client to " + typed?.RedirectionTarget);
                                        onRedirect(typed?.RedirectionTarget);
                                    }
                                }
                                else
                                    Close("Auto redirect not allowed");
                            }
                            else
                            {
                                DebugEx.Assert($"YPChannel ({this}) Server received a RedirectionMessage from client.. Server doesn't like this");
                                Close("Server received a RedirectionMessage from client.. Server doesn't like this");
                            }
                        }
                        else if (msg.Payload is NegotationFinishMessage)
                        {
                            if (ChannelRole == YPChannel.ChannelRole.Client)
                            {
                                var typed = msg.Payload as NegotationFinishMessage;
                                //open client-side channel
                                if (typed?.IsSuccessCode() == true)
                                {
                                    //keep channel key
                                    _ChannelKey = typed?.ChannelKey;
                                    //open channel
                                    onOpen();
                                }
                                else
                                {
                                    //close channel
                                    Close("NegotationFinishMessage report failure");
                                }
                            }
                            else
                            {
                                DebugEx.Assert($"YPChannel ({this}) Server received a negotiation finish from client.. Server doesn't like this");
                                Close("Server received a negotiation finish from client.. Server doesn't like this");
                            }
                        }
                        else if (msg.Payload is BandwidthManagementMessage)
                        {
                            RequestedBandwidthManagement = msg.Payload as BandwidthManagementMessage;
                        }
                        else if (msg.Payload is ChannelCloseMessage)
                        {
                            //closing channel
                            var closeMsg = msg.Payload as ChannelCloseMessage;
                            if (State != ChannelStates.Closed)
                                Close("ChannelCloseMessage received (Message=" + closeMsg.Message + ")");
                        }
                        else if (msg.Payload is PingRequest)
                        {
                            try
                            {
                                var pingMsg = (msg.Payload as PingRequest).Message;
                                if (msg.IsRequest)
                                    if (pingMsg == null || pingMsg.Length < PongReplySizeLimit)
                                    {
                                        var pongMsg = new PongResponse() { Message = pingMsg };
                                        SendResponse(pongMsg, msg.MessageID);
                                    }
                            }
                            catch (Exception exx) { DebugEx.TraceError(exx, $"YPChannel ({this}) Error while replying to ping"); }
                        }
                        else if (msg.Payload is ErrorMessage)
                        {
                            try
                            {
                                var errMsg = msg.Payload as ErrorMessage;
                                if (errMsg.Type < eTraceType.Info)
                                    DebugEx.TraceLog($"YPChannel ({this}) received an error message from the remote endpoint" + Environment.NewLine + "Type : Log" + Environment.NewLine + "Message : " + errMsg.Message + Environment.NewLine + Environment.NewLine);
                                else if (errMsg.Type < eTraceType.Warning)
                                    DebugEx.TraceWarning($"YPChannel ({this}) received an error message from the remote endpoint" + Environment.NewLine + "Type : Warning" + Environment.NewLine + "Message : " + errMsg.Message + Environment.NewLine + Environment.NewLine);
                                else// if (errMsg.Type >= DebugEx.eTraceType.Error)
                                    DebugEx.Assert($"YPChannel ({this}) received an error message from the remote endpoint" + Environment.NewLine + "Type : Error" + Environment.NewLine + "Message : " + errMsg.Message + Environment.NewLine + Environment.NewLine);
                            }
                            catch (Exception exx) { DebugEx.TraceError(exx, $"YPChannel ({this}) Error while processing error message"); }
                        }
                        else
                        {
                            //generic handler
                            try { OnMessageReceived?.Invoke(this, msg); }
                            catch (Exception ex) { DebugEx.Assert(ex, $"YPChannel ({this}) message handler caused an exception"); }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, $"YPChannel ({this}) Channel Heartbeat had exception");
                heartbeat_err_msg = "Channel Heartbeat had exception. " + ex.Message;
            }

            //close channel
            Close(heartbeat_err_msg == null ? "Heartbeat exited" : heartbeat_err_msg);
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected virtual void _flush(int Timeout) { }
        //------------------------------------------------------------------------------------------------------------------------
        protected abstract YPMessagePacked _readPackedMessage();
        protected abstract bool _sendPackedMessage(YPMessagePacked msg);
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
                //timestamp
                _LastActivityTimestamp = DateTime.Now;

                //unpack message
                var msg = new YPMessage();
                msg.Flags = (MessageFlags)packedMsg.Flags;
                msg.MessageID = packedMsg.MessageID;
                msg.RespondToRequestID = packedMsg.RequestID;
                if (packedMsg.MessageIE != null)
                {
                    msg.MessageIE = new Dictionary<string, object>();
                    foreach (var ie in packedMsg.MessageIE)
                        msg.MessageIE.Add(ie.Key, ie.Value);
                }

                //unpack payload
                {
                    Type objType = null;
                    //find type by protocol msg id
                    var typeID = packedMsg.Type;
                    ActiveProtocol.ProtocolReverseLookup.TryGetValue(typeID, out objType);
                    DebugEx.Assert(objType != null, $"YPChannel ({this}) : Message received is not part of protocol");
                    //try to find type by extended type
                    if (EnableExtendedTypes && objType == null && msg.MessageIE != null && msg.MessageIE.ContainsKey(MessageIEKeys.ExtendedType))
                        objType = Yodiwo.TypeCache.GetType(msg.MessageIE[MessageIEKeys.ExtendedType] as string);
                    //check
                    if (objType == null)
                    {
                        DebugEx.Assert($"YPChannel ({this}) Could not resolve type");
                        ChannelError = false;
                        return null;
                    }
                    ///unpack
                    if (ChannelSerializationMode.HasFlag(YPChannel.ChannelSerializationMode.Json))
                    {
                        string str_msg = null;
                        if (packedMsg.PayloadStr != null)
                            str_msg = packedMsg.PayloadStr;
                        else if (packedMsg.Payload != null)
                            str_msg = System.Text.Encoding.UTF8.GetString(packedMsg.Payload);
                        //deserialize
                        msg.Payload = str_msg?.FromJSON(objType);
                    }
                    else if (ChannelSerializationMode.HasFlag(YPChannel.ChannelSerializationMode.MessagePack))
                    {
                        msg.Payload = MsgPack.Unpack(objType, packedMsg.Payload);
                    }
                    else
                    {
                        DebugEx.Assert($"YPChannel ({this}) Unkown serialization method");
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
                DebugEx.Assert(ex, $"YPChannel ({this}) MessageReceiver catched unhandled exception");
                //Something went wrong here..
                Thread.Sleep(50);
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
        public Task<TResponse> SendRequestAsync<TResponse>(object Payload, TimeSpan? Timeout = null, TResponse Defaut = default(TResponse))
        {
            return TaskEx.RunSafe(() => SendRequest<TResponse>(Payload, Timeout: Timeout, Defaut: Defaut), Default: Defaut);
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
                DebugEx.Assert($"YPChannel ({this}) Only server can send responses while negotiating.");
                Thread.Sleep(50); //penalty
                return null;
            }

            //check for closed channel
            if (_State == ChannelStates.Closed)
            {
                DebugEx.TraceWarning($"YPChannel ({this}) state is CLOSED");
                Thread.Sleep(50); //penalty
                return null;
            }

            //check for open channel
            if (!IsNegotiating)
            {
                //check if channel is open
                if (!IsOpen)
                {
                    DebugEx.TraceWarning($"YPChannel ({this}) Trying to send message on a channel that is not open (channel name=" + Name + " , msg=" + Payload.GetType() + ")");
                    Thread.Sleep(50); //failsafe penalty
                    return null;
                }

                //get threadID and check it is heartbeat.. this will protect against deadlock
#if NETFX
                var tid = Thread.CurrentThread.ManagedThreadId;
#elif UNIVERSAL
                var tid = Task.CurrentId == null ? 0 : Task.CurrentId.Value;
#endif
                if (heartBeatThreadID == tid)
                {
                    DebugEx.Assert($"YPChannel ({this}) Cannot send a request from the same thread that you are handling incoming messages, as this will result in a deadlock." + Environment.NewLine + "Use a Task.Run() to send from a different thread");
                    Thread.Sleep(50); //failsafe penalty
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
                PendingRequests.Add(id, req);

            //lock on request (before sending message!)
            lock (req)
            {
                //send message
                var res = _sendMessage(Payload, id, null, true);
                if (!res)
                {
                    //if null(timeout) then remove id from dictionary
                    PendingRequests.Remove(id);
                    DebugEx.TraceWarning($"Remove request with id {id} from pending requests, timeout");
                    return null;
                }
                else if (IsNegotiating)
                {
                    //warmup channel (flush)
                    Thread.Sleep(50);
                    //wait for response in the negotiating thread (heartbeat)
                    bool ChannelError;
                    YPMessage respMsg = null;
                    while (respMsg == null || (respMsg != null && respMsg.RespondToRequestID != id))
                    {
                        respMsg = messageReceiver(out ChannelError);
                        if (ChannelError || respMsg?.Payload is ChannelCloseMessage)
                        {
                            Close("Channel error while negotiating");
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
                    {
                        DebugEx.TraceWarning($"Remove request with id {id} from pending requests, null response received (TIMEOUT)");
                        PendingRequests.Remove(id);
                    }
                    //return new response (null if timeout)                    
                    return resp;
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public Task SendResponseAsync(object Payload, UInt32 ResponseToMsg)
        {
            return TaskEx.RunSafe(() => SendResponse(Payload, ResponseToMsg));
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual bool SendResponse(object Payload, UInt32 ResponseToMsg)
        {
            try
            {
                if (IsNegotiating && ChannelRole != YPChannel.ChannelRole.Client)
                {
                    DebugEx.Assert($"YPChannel ({this}) Only client can send responses while negotiating.");
                    Thread.Sleep(50);  //penalty
                    return false;
                }
                return _sendMessage(Payload, null, ResponseToMsg, false);
            }
            catch (Exception ex) { DebugEx.Assert(ex); return false; }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual bool SendMessage(object Payload)
        {
            try
            {
                if (IsNegotiating)
                {
                    DebugEx.Assert($"YPChannel ({this}) Cannot send message while negotiating. Only Request/Respose");
                    Thread.Sleep(50);  //penalty
                    return false;
                }
                return _sendMessage(Payload, null, null, false);
            }
            catch (Exception ex) { DebugEx.Assert(ex); return false; }
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected virtual bool _sendMessage(object Payload, UInt32? MessageID, UInt32? ResponseToMsg, bool IsRequest)
        {
            try
            {
                //declares
                var flags = MessageFlags.None;
                var MessageIE = new List<MessageIE>();

                //sanity check
                DebugEx.Assert(Payload != null, $"YPChannel ({this}) Cannot send Null");
                if (Payload == null || ActiveProtocol == null)
                {
                    Thread.Sleep(50);  //failsafe penalty
                    return false;
                }

                //check if channel is open
                if (State == ChannelStates.Closed)
                {
                    DebugEx.TraceWarning($"YPChannel ({this}) Trying to send message on a channel that is not open (channel name=" + Name + ")");
                    Thread.Sleep(50);  //failsafe penalty
                    return false;
                }

                //check channel activity
                if (_State == ChannelStates.Open &&
                    !(Payload is PingRequest) && !(Payload is PongResponse) &&
                    !(Payload is ChannelCloseMessage) &&
                    (DateTime.Now - _LastActivityTimestamp) > InactiveLineWarmupTimeout)
                {
                    lock (_InactiveLineWarmupLocker)
                        if ((DateTime.Now - _LastActivityTimestamp) > InactiveLineWarmupTimeout)
                        {
                            //timestamp
                            _LastActivityTimestamp = DateTime.Now;
                            //test/warmup line
                            var pong = Ping("line warmup", Timeout: LineWarmupPingTimeout);
                            if (pong == null)
                            {
                                Close("Channel warmup ping failed");
                                return false;
                            }
                        }
                }

                //get type
                var objType = Payload.GetType();

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
                    DebugEx.Assert($"YPChannel ({this}) : Message send is not part of protocol (" + objType.AssemblyQualifiedName_Portable() + ")");
                    if (EnableExtendedTypes)
                    {
                        CheckType(Payload.GetType(), Payload.GetType().Name);
                        MessageIE.Add(new MessageIE(MessageIEKeys.ExtendedType, Payload.GetType().AssemblyQualifiedName_Portable()));
                    }
                    else
                        return false;
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
                    MessageIE = MessageIE.Count > 0 ? MessageIE.ToArray() : null,
                };

                //serialize object (payload)
                if (ChannelSerializationMode.HasFlag(YPChannel.ChannelSerializationMode.Json))
                {
                    if (use_PayloadStr)
                        msg.PayloadStr = Payload.ToJSON(HtmlEncode: false);
                    else
                        msg.Payload = Payload.ToJSON2(HtmlEncode: false);
                }
                else if (ChannelSerializationMode.HasFlag(YPChannel.ChannelSerializationMode.MessagePack))
                {
                    msg.Payload = MsgPack.Pack(Payload);
                }
                else
                    return false;

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
                            return false;
                    }

                    //timestamp
                    _LastActivityTimestamp = DateTime.Now;

                    //serialize and send message
                    return _sendPackedMessage(msg);
                }
            }
            catch (Exception ex) { DebugEx.Assert(ex); return false; }
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
#if CHECK_TYPES
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
#elif UNIVERSAL
            if (type.GetTypeInfo().IsAbstract || type.GetTypeInfo().IsInterface || type.GetType() == typeof(object) || type.GetTypeInfo().IsNotPublic)
#endif
            {
                DebugEx.Assert($"YPChannel ({this}) Cannot use Abstract/Interface/Object/NonPublic types for YPChannel message." + Environment.NewLine + "Type failed : " + type.GetFriendlyName() + Environment.NewLine + "Path: " + errorPath);
                return;
            }

            //check for StructLayout
#if NETFX
            if (!type.IsPrimitive && type != typeof(string) && !typeof(IList).IsAssignableFrom(type) && !typeof(IDictionary).IsAssignableFrom(type) && !type.IsArray && !type.IsEnum)
#elif UNIVERSAL
            if (!type.GetTypeInfo().IsPrimitive && type != typeof(string) && !typeof(IList).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()) && !typeof(IDictionary).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()) && !type.IsArray && !type.GetTypeInfo().IsEnum)
#endif
            {
                //DebugEx.Assert(type.IsLayoutSequential, "Messages should define the StructLayout Attribute with Sequential layout" + Environment.NewLine + "Hint: add [StructLayout(LayoutKind.Sequential)] on class" + Environment.NewLine + "Type failed : " + type.GetFriendlyName() + Environment.NewLine + "Path: " + errorPath);

                //check members
#if NETFX
                var members = type.GetMembers(BindingFlags.Public | BindingFlags.Instance);
#elif UNIVERSAL
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
#endif
        }
        //------------------------------------------------------------------------------------------------------------------------        
        public TimeSpan? Ping(string Message, TimeSpan? Timeout = null)
        {
            var timestamp = DateTime.Now;
            var pingMsg = new PingRequest() { Message = Message };
            var pong = SendRequest<PongResponse>(pingMsg, Timeout: Timeout);
            if (pong == null)
                return null;
            else if (pingMsg.Message == pong.Message)
                return TimeSpan.FromMilliseconds((DateTime.Now - timestamp).TotalMilliseconds.ClampFloor(0));
            else
                return null;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override string ToString()
        {
            return Name + "/" + ChannelKey;
        }
        //------------------------------------------------------------------------------------------------------------------------
        ~Channel()
        {
            try
            {
                if (State != ChannelStates.Closed)
                    Close("Channel disposed (destructor)");
            }
            catch (Exception ex)
            {
                DebugEx.TraceError(ex, $"YPChannel ({this}) Channel destructor caught unhandled exception");
            }
        }
        #endregion
    }
}
