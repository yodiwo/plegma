using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Yodiwo.API.Plegma;
using System.Threading;
using Yodiwo.YPChannel;
using Yodiwo.Node.Pairing;
using System.Reflection;

namespace Yodiwo.NodeLibrary
{
    public class Node
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public object locker = new object();
        //------------------------------------------------------------------------------------------------------------------------
        public string uuid { get { return conf.uuid; } }
        public string Name { get { return conf.Name; } }
        public bool CanSolveGraphs { get { return conf.CanSolveGraphs; } }
        private NodeConfig conf;
        //------------------------------------------------------------------------------------------------------------------------
        public NodeKey NodeKey { get; private set; }
        public string NodeSecret { get; private set; }
        //------------------------------------------------------------------------------------------------------------------------
        private bool _IsPaired;
        public bool IsPaired { get { return _IsPaired; } }
        //------------------------------------------------------------------------------------------------------------------------
        //Transport
        private Transport _Transport = Transport.YPCHANNEL;
        public Transport Transport
        {
            get { return _Transport; }
            set { _Transport = value; }
        }
        private bool _IsConnectionEnabled = false;
        public bool IsConnectionEnabled { get { return _IsConnectionEnabled; } }
        //------------------------------------------------------------------------------------------------------------------------
        //transports
        private Yodiwo.YPChannel.Transport.Sockets.Client Channel;
        private Transports.ITransportMQTT mqtthandler;
        private Yodiwo.RequestQueueConsumer<Tuple<string, string>> RestRequestConsumer;
        //------------------------------------------------------------------------------------------------------------------------
        //pairing
        private IPairingModule _PairingModule;
        public IPairingModule PairingModule { get { return _PairingModule; } }
        //------------------------------------------------------------------------------------------------------------------------
        //node things and port
        private HashSetTS<PortKey> _CloudActivePortKeys = new HashSetTS<PortKey>();
        public IReadOnlySet<PortKey> CloudActivePortKeys { get { return _CloudActivePortKeys; } }

        private HashSetTS<Thing> _CloudActiveThings = new HashSetTS<Thing>();
        public IReadOnlySet<Thing> CloudActiveThings { get { return _CloudActiveThings; } }

        private DictionaryTS<ThingKey, Thing> _Things = new DictionaryTS<ThingKey, Thing>();
        public readonly ReadOnlyDictionary<ThingKey, Thing> Things;

        private HashSet<Thing> limbo_things = null; //thing not yet assigned thing id
        private List<ThingType> _thingTypes;
        private int _ThingIDCnt = 0;
        private eNodeType nodeType;
        //------------------------------------------------------------------------------------------------------------------------
        //helpers for active things/port/ changes
        object _activeChangesLocker = new object();
        //------------------------------------------------------------------------------------------------------------------------
        //Graphs
        public readonly Graphs.INodeGraphManager NodeGraphManager;
        //------------------------------------------------------------------------------------------------------------------------
        public readonly DictionaryTS<Port, Action<string>> PortEventHandlers = new DictionaryTS<Port, Action<string>>();
        //------------------------------------------------------------------------------------------------------------------------
        //delegates events that are exposed to user
        public delegate void OnNodePairedDelegate(NodeKey nodekey, string nodesecret);
        public event OnNodePairedDelegate OnNodePaired;

        public delegate void OnNodePairingFailedDelegate(string Message);
        public event OnNodePairingFailedDelegate OnNodePairingFailed;

        public delegate void OnNodeUnpairedDelegate(eUnpairReason reasonCode, string msg);
        public event OnNodeUnpairedDelegate OnNodeUnpaired;

        public delegate void OnTransportConnectedDelegate(Transport Transport, string msgStatus);
        public event OnTransportConnectedDelegate OnTransportConnected;

        public delegate void OnTransportDisconnectedDelegate(Transport Transport, string msgStatus);
        public event OnTransportDisconnectedDelegate OnTransportDisconnected;

        public delegate void OnTransportErrorDelegate(Transport Transport, TransportErrors Error, string msgStatus);
        public event OnTransportErrorDelegate OnTransportError;

        public delegate void OnThingsRegisteredDelegate();
        public event OnThingsRegisteredDelegate OnThingsRegistered;

        public delegate void OnChangedStateDelegate(Thing thing, Port port, String status);
        public event OnChangedStateDelegate OnChangedState;

        public delegate void OnPortEventMsgProcessedDelegate(PortEventMsg msg);
        public event OnPortEventMsgProcessedDelegate OnPortEventMsgProcessed;

        public delegate ApiMsg OnA2mcuActiveDriversReqDelegate(A2mcuActiveDriversReq request);
        public OnA2mcuActiveDriversReqDelegate OnA2mcuActiveDriversReq;

        public delegate ApiMsg OnA2mcuCtrlReqDelegate(A2mcuCtrlReq request);
        public OnA2mcuCtrlReqDelegate OnA2mcuCtrlReq;

        public delegate ApiMsg OnUnexpectedRequestDelegate(object request);
        public OnUnexpectedRequestDelegate OnUnexpectedRequest;

        public delegate void OnUnexpectedMessageDelegate(object message);
        public OnUnexpectedMessageDelegate OnUnexpectedMessage;

        public delegate void OnThingActivatedDelegate(Thing thing);
        public event OnThingActivatedDelegate OnThingActivated;

        public delegate void OnThingDeactivatedDelegate(Thing thing);
        public event OnThingDeactivatedDelegate OnThingDeactivated;

        public delegate void OnPortActivatedDelegate(PortKey portKey);
        public event OnPortActivatedDelegate OnPortActivated;

        public delegate void OnPortDeactivatedDelegate(PortKey portKey);
        public event OnPortDeactivatedDelegate OnPortDeactivated;

        public delegate void OnThingUpdatedDelegate(Thing Thing, Thing oldCopy);
        public event OnThingUpdatedDelegate OnThingUpdated;

        //public delegate void OnPortStateRxDelegate();
        //public event OnPortStateRxDelegate OnPortStateRx = delegate { };

        public delegate IEnumerable<Thing> OnThingScanRequestDelegate(ThingKey key);
        public OnThingScanRequestDelegate OnThingScanRequest;

        public delegate bool OnThingDeleteRequestDelegate(Thing Thing);
        public OnThingDeleteRequestDelegate OnThingDeleteRequest;

        //------------------------------------------------------------------------------------------------------------------------
        //used to protect against IO race contitions
        DictionaryTS<string, object> IOLocker = new DictionaryTS<string, object>();

        //Strorage callbacks
        public delegate byte[] DataLoadDelegate(string Identifier, bool Secure);
        private readonly DataLoadDelegate DataLoad = null;

        public delegate bool DataSaveDelegate(string Identifier, byte[] data, bool Secure);
        private readonly DataSaveDelegate DataSave = null;
        //------------------------------------------------------------------------------------------------------------------------
        //Discovery
        public readonly NodeDiscovery.NodeDiscoverManager NodeDiscovery;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        public Node(NodeConfig conf,
                    IEnumerable<Thing> things,
                    IPairingModule pairingmodule,
                    DataLoadDelegate DataLoad,
                    DataSaveDelegate DataSave,
                    Graphs.INodeGraphManager NodeGraphManager = null,
                    Type MqttTransport = null,
                    List<ThingType> thingTypes = null, eNodeType nodeType = eNodeType.Unknown)
        {
            DebugEx.Assert(conf != null, "Cannot have null conf");
            this.conf = conf.Clone() as NodeConfig;
            this.limbo_things = things?.ToHashSet();
            this.Things = new ReadOnlyDictionary<ThingKey, Thing>(this._Things);
            this._thingTypes = thingTypes;
            this.nodeType = nodeType;
            this._PairingModule = pairingmodule;
            this.DataLoad = DataLoad;
            this.DataSave = DataSave;
            this.NodeGraphManager = NodeGraphManager;
            //Initialize Graph stuff
            if (NodeGraphManager != null)
                NodeGraphManager.Initialize(this);

            //create mqtt transport
            if (MqttTransport != null)
            {
                //check interface
#if NETFX
                if (!MqttTransport.GetInterfaces().Contains(typeof(Transports.ITransportMQTT)))
#else
                if (!MqttTransport.GetTypeInfo().ImplementedInterfaces.Contains(typeof(Transports.ITransportMQTT)))
#endif
                    DebugEx.AssertAndThrow("MqttTransport transport type does not implement ITransportMQTT");
                //instanciate
                mqtthandler = Activator.CreateInstance(MqttTransport, new object[] { this }) as Transports.ITransportMQTT;
                if (mqtthandler == null)
                    DebugEx.AssertAndThrow("Could not create mqtt transport from provided type");
            }

            //init node discovery module
            if (conf.EnableNodeDiscovery)
            {
                var port = MathTools.GetRandomNumber(conf.NodeDiscovery_YPCPort_Start, conf.NodeDiscovery_YPCPort_End);
                this.NodeDiscovery = new NodeDiscovery.NodeDiscoverManager(this, YPCPort: port);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------

        public async Task<bool> Pairing(string postUrl, string redirectUrl, string selfUrl = null)
        {
            var res = this._PairingModule.StartPair(postUrl, redirectUrl, conf, selfUrl, onPaired, onPairFailed);
            if (!res)
            {
                DebugEx.Assert("Could not start pairing");
                return false;
            }
            //wait for pairing completion
            while (!_IsPaired)
                await Task.Delay(300);
            return true;
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void Pairing(string postUrl, string redirectUrl, IEnumerable<Cookie> cookies)
        {
            try
            {
                var backend = new Yodiwo.Node.Pairing.NodePairingBackend(postUrl, conf, onPaired, onPairFailed);
                string token2 = backend.pairGetTokens(redirectUrl);
                //send token request
                var paramet = new Dictionary<string, string>()
                {
                    {"token2",token2},
                };
                var t = Tools.Http.RequestGET(backend.userUrl, paramet, cookies);

                //wait
                Task.Delay(2000).Wait();

                //send uuidentry request
                var paramet2 = new Dictionary<string, string>()
            {
                { "uuid", conf.uuid },
                { "token2", token2},
            };
                var t1 = Tools.Http.RequestGET(backend.postUrl + "/uuidentry", paramet2, cookies);
                if (t1.StatusCode == HttpStatusCode.NotFound || t1.StatusCode == HttpStatusCode.RedirectMethod)
                    backend.pairGetKeys();
                else
                    DebugEx.Assert("Pairing failed (could not enter uuid)");
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception caught");
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        private void onPaired(NodeKey nodeKey, string nodeSecret)
        {
            try
            {
                //setup node
                SetupNodeKeys(nodeKey, nodeSecret);

                //inform user
                try
                {
                    OnNodePaired?.Invoke(this.NodeKey, this.NodeSecret);
                }
                catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught from user callback"); }

                //finished pairing
                this._PairingModule.EndPair();
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception caught");
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        private void onPairFailed(string Message)
        {
            try
            {
                this._IsPaired = false;

                //inform user
                try
                {
                    OnNodePairingFailed?.Invoke(Message);
                }
                catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught from user callback"); }

                //finished pairing
                this._PairingModule.EndPair();
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception caught");
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void SetupNodeKeys(NodeKey nk, string secret)
        {
            try
            {
                lock (locker)
                {
                    this.NodeKey = nk;
                    this.NodeSecret = secret;
                    this._IsPaired = true;

                    //setup thing keys
                    _setupThings();

                    //now we can load local graphs
                    NodeGraphManager?.DeployGraphs();

                    //start node discovery
                    NodeDiscovery?.Initialize();
                }
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception caught");
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        void _setupThings()
        {
            try
            {
                lock (locker)
                {
                    if (limbo_things != null)
                    {
                        foreach (var thing in this.limbo_things)
                        {
                            //setup thing (keys etc)
                            _setupThing(thing);

                            //add to collection
                            this._Things.ForceAdd(thing.ThingKey, thing);
                        }
                        //no longer needed
                        this.limbo_things = null;
                    }
                }
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception caught");
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        void _setupThing(Thing thing)
        {
            try
            {
                //replace macro
                thing.ThingKey = thing.ThingKey.Replace("$NodeKey$", NodeKey);

                //reconstruct key
                if (((ThingKey)thing.ThingKey).IsInvalid)
                {
                    thing.ThingKey = new ThingKey(this.NodeKey, Interlocked.Increment(ref _ThingIDCnt).ToStringInvariant());
                }
                else
                {
                    //make sure nodekeyis correct (keep thing id)
                    var key = (ThingKey)thing.ThingKey;
                    key.NodeKey = NodeKey;
                    thing.ThingKey = key;
                }

                foreach (var port in thing.Ports)
                {
                    //replace macros
                    port.PortKey = port.PortKey.Replace("$NodeKey$", NodeKey);
                    port.PortKey = port.PortKey.Replace("$ThingKey$", thing.ThingKey);

                    //reconstruct portkey
                    var pk = (PortKey)port.PortKey;
                    if (pk.IsValid)
                        port.PortKey = new PortKey(thing, pk.PortUID);
                    else
                    {
                        var ind = port.PortKey.LastIndexOf('-');
                        if (ind != -1 && ind + 1 < port.PortKey.Length)
                            port.PortKey = new PortKey(thing, port.PortKey.Substring(ind + 1));
                        else
                        {
                            DebugEx.Assert("Could not decide on portUID");
                            port.PortKey = new PortKey(thing, MathTools.GetRandomNumber(100, 1000000).ToString());
                        }
                    }
                }

                //fix thing states
                foreach (var port in thing.Ports)
                {
                    if (port.Type == ePortType.String && port.State != null)
                    {
                        port.State = port.State.Replace("$NodeKey$", NodeKey);
                        port.State = port.State.Replace("$ThingKey$", thing.ThingKey);
                        port.State = port.State.Replace("$PortKey$", port.PortKey);
                    }
                }
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception caught");
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void UpdateThing(Thing thing, bool SendToCloud = true) { AddThing(thing, SendToCloud: SendToCloud); }

        public void AddThing(Thing thing, bool SendToCloud = true)
        {
            try
            {
                lock (locker)
                {
                    //still in limbo?
                    if (limbo_things != null)
                    {
                        limbo_things.Add(thing);
                        return;
                    }

                    //setup thing
                    _setupThing(thing);

                    //add to collection
                    this._Things.ForceAdd(thing.ThingKey, thing);

                    //send update message
                    if (SendToCloud)
                    {
                        var msg = new ThingsSet()
                        {
                            Operation = eThingsOperation.Update,
                            Data = new[] { thing },
                            Status = true,
                        };
                        SendMessage(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception caught");
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void RemoveThing(ThingKey thingkey, bool SendToCloud = true)
        {
            try
            {
                lock (locker)
                {
                    //still in limbo?
                    if (limbo_things != null)
                    {
                        limbo_things.RemoveWhere(t => t.ThingKey == thingkey);
                        return;
                    }

                    //find thing
                    var thing = this._Things.TryGetOrDefault(thingkey);
                    if (thing == null)
                        return;

                    //remove from collection
                    this._Things.Remove(thingkey);

                    //send update message
                    if (SendToCloud)
                    {
                        var msg = new ThingsSet()
                        {
                            Operation = eThingsOperation.Delete,
                            Data = new[] { thing },
                            Status = true,
                        };
                        SendMessage(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception caught");
            }
        }

        //------------------------------------------------------------------------------------------------------------------------


        public void RemoveAllThings(bool SendToCloud = true)
        {
            try
            {
                lock (locker)
                {
                    if (this._Things.Count == 0)
                        return;

                    //gather things
                    var things = this._Things.Values.ToArray();

                    //remove from collection
                    this._Things.Clear();

                    //send update message
                    if (SendToCloud)
                    {
                        var msg = new ThingsSet()
                        {
                            Operation = eThingsOperation.Delete,
                            Data = things,
                            Status = true,
                        };
                        SendMessage(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception caught");
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void SetState(Thing thing, Port port, string state)
        {
            try
            {
                DebugEx.TraceLog("=====>Set State<====== " + port.PortKey.ToString() + " state: " + state);
                SetState(new TupleS<Port, string>[] { new TupleS<Port, string>(port, state) });
            }
            catch (Exception ex)
            {
                DebugEx.TraceErrorException(ex);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void SetState(Port port, string state)
        {
            try
            {
                DebugEx.TraceLog("=====>Set State<====== " + port.PortKey.ToString() + " state: " + state);
                SetState(new TupleS<Port, string>[] { new TupleS<Port, string>(port, state) });
            }
            catch (Exception ex)
            {
                DebugEx.TraceErrorException(ex);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void SetState(PortKey portKey, string state)
        {
            try
            {
                DebugEx.TraceLog("=====>Set State<====== " + portKey.ToString() + " state: " + state);
                //find thing
                var tk = portKey.ThingKey;
                var thing = _Things.TryGetOrDefault(tk);
                if (thing == null)
                {
                    DebugEx.TraceError("Trying to set state to a thing that does not exists in nodelibrary things");
                    return;
                }
                //find port
                var portkKeyStr = portKey.ToStringInvariant();
                var port = thing.Ports.Find(p => p.PortKey == portkKeyStr);
                if (port == null)
                {
                    DebugEx.TraceError($"Trying to set state to a port(Key:{portKey}) that does not exists in thing");
                    return;
                }
                //set state
                SetState(new TupleS<Port, string>[] { new TupleS<Port, string>(port, state) });
            }
            catch (Exception ex)
            {
                DebugEx.TraceErrorException(ex);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------

        public void SetState(IEnumerable<TupleS<Port, string>> states)
        {
            try
            {
                lock (locker)
                {
                    //first pass them from NodeGraphManager
                    var ngm = NodeGraphManager;
                    if (ngm != null)
                        ngm.HandlePortStates(states);

                    //Process request
                    List<PortEvent> events = null;
                    foreach (var state in states)
                    {
                        //get keys
                        var pk = (PortKey)state.Item1.PortKey;
                        var thk = pk.ThingKey;

                        //find thing
                        Thing thing = null;
                        if (_Things.TryGetValue(thk, out thing))
                            if (thing != null)
                            {
                                //update port state
                                thing.GetPort(pk).State = state.Item2;
                                //check if this port is active in a cloud graph
                                if (_CloudActivePortKeys.Contains(pk))
                                {
                                    //create list if null
                                    if (events == null)
                                        events = new List<PortEvent>();
                                    //add to event list
                                    events.Add(new PortEvent(pk, state.Item2));
                                }
                            }
                    }

                    //send message
                    if (events != null && events.Count > 0)
                    {
                        DebugEx.TraceLog("========>Actual Set State<====== " + events[0].State);
                        var msg = new Yodiwo.API.Plegma.PortEventMsg(0);
                        msg.PortEvents = events.ToArray();
                        SendMessage(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception caught");
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        /// <summary> Ping server. Returns the request Round-Trip-Time. If failed, it will return Timespan.Zero </summary>
        public TimeSpan Ping(int cnt)
        {
            try
            {
                var sendTimestamp = DateTime.Now;
                var req = new Yodiwo.API.Plegma.PingReq() { Data = cnt };
                var rsp = SendRequest<PingRsp>(req);
                if (rsp != null && rsp.Data == req.Data)
                    return DateTime.Now - sendTimestamp;
                else
                    return TimeSpan.Zero;
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception caught");
                return TimeSpan.Zero;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void HandlePortEventMsg(PortEventMsg msg)
        {
            try
            {
                foreach (var portevent in msg.PortEvents)
                {
                    var pk = (PortKey)portevent.PortKey;
                    var thingKey = pk.ThingKey;
                    var thing = _Things[thingKey];
                    var port = thing.GetPort(pk);
                    port.State = portevent.State;
                    OnChangedState?.Invoke(thing, port, portevent.State);
                    DebugEx.TraceLog("On Changed State:" + portevent.State);
                    if (PortEventHandlers.ContainsKey(port))
                        PortEventHandlers[port](portevent.State);
                }
                OnPortEventMsgProcessed?.Invoke(msg);
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception caught");
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public Thing GetThingByName(string name)
        {
            try
            {
                return this._Things.FirstOrDefault(t => t.Value.Name == name).Value;
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception caught");
                return null;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public IEnumerable<Thing> GetAllThings()
        {
            try
            {
                return this._Things.Values;
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception caught");
                return Enumerable.Empty<Thing>();
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        private void OnConnectedToCloud()
        {
            try
            {
                //inform node manager
                if (NodeGraphManager != null)
                    NodeGraphManager.OnConnectedToCloud();
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception caught");
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        private void OnDisconnectedFromCloud()
        {
            //TODO..
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void Connect()
        {
            try
            {
                //set flag
                _IsConnectionEnabled = true;

                //connect YPChannel?
                if (this._Transport == Transport.YPCHANNEL)
                {
                    var ret = _YPChannelConnectWithWorker(this.conf.YpServer, this.conf.YpchannelPort, this.conf.SecureYpc);
                    if (ret.IsSuccessful)
                    {
                        OnConnectedToCloud();
                        OnTransportConnected?.Invoke(Transport.YPCHANNEL, ret.Message);
                    }
                    else
                    {
                        DebugEx.TraceError("Could not connect YPChannel (Message:" + ret.Message + ")");
                        OnTransportError?.Invoke(Transport.YPCHANNEL, TransportErrors.ConnectionEstablishFailed, ret.Message);
                    }
                }
                //connect Mqtt?
                if (this._Transport == Transport.MQTT)
                {
                    var ret = mqtthandler.ConnectWithWorker(this.conf.MqttBrokerHostname, this.conf.MqttUseSsl);
                    if (ret.IsSuccessful)
                    {
                        OnConnectedToCloud();
                        OnTransportConnected?.Invoke(Transport.MQTT, ret.Message);
                    }
                    else
                    {
                        DebugEx.TraceError("Could not connect YPChannel (Message:" + ret.Message + ")");
                        OnTransportError?.Invoke(Transport.MQTT, TransportErrors.ConnectionEstablishFailed, ret.Message);
                    }
                }
                //"connect"(enable) Rest?
                if (this._Transport == Transport.REST)
                {
                    var ret = _RestConnectWithWorker();
                    if (ret.IsSuccessful)
                    {
                        OnConnectedToCloud();
                        OnTransportConnected?.Invoke(Transport.REST, ret.Message);
                    }
                    else
                    {
                        DebugEx.TraceError("Could not connect YPChannel (Message:" + ret.Message + ")");
                        OnTransportError?.Invoke(Transport.REST, TransportErrors.ConnectionEstablishFailed, ret.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception caught");
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void Disconnect()
        {
            try
            {
                //set flag
                _IsConnectionEnabled = false;

                //disconnect all
                if (mqtthandler != null)
                {
                    mqtthandler.Disconnect();
                }
                if (RestRequestConsumer != null)
                {
                    RestRequestConsumer.Stop();
                    RestRequestConsumer.Clear();
                    RestRequestConsumer = null;
                }
                if (Channel != null)
                {
                    Channel.Close();
                    Channel = null;
                }
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception caught");
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public bool SendMessage(API.Plegma.ApiMsg msg)
        {
            try
            {
                //check
                if (msg == null)
                {
                    DebugEx.Assert("Cannot send null msg");
                    return false;
                }

                //get type
                var msgType = msg.GetType();

                //get vars on stack
                var __ypc = Channel;
                var __mqtt = mqtthandler;

                //send Message
                if (this._Transport.HasFlag(Transport.YPCHANNEL) && __ypc != null && __ypc.IsOpen)
                {
                    __ypc.SendMessage(msg);
                    return true;
                }
                else if (this._Transport.HasFlag(Transport.MQTT) && __mqtt != null && __mqtt.IsConnected)
                {
                    __mqtt.SendMessage(msg);
                    return true;
                }
                else if (this._Transport.HasFlag(Transport.REST))
                {
                    string route;
                    if (PlegmaAPI.ApiMsgNames.TryGetValue(msgType, out route))
                    {
                        PostMsg(route, msg);
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception caught");
                return false;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public TResponse SendRequest<TResponse>(API.Plegma.ApiMsg msg)
        {
            try
            {
                //check
                if (msg == null)
                {
                    DebugEx.Assert("Cannot send null request msg");
                    return default(TResponse);
                }

                //get type
                var msgType = msg.GetType();

                //get vars on stack
                var __ypc = Channel;
                var __mqtt = mqtthandler;

                //send Message
                if (this._Transport.HasFlag(Transport.YPCHANNEL) && __ypc != null && __ypc.IsOpen)
                {
                    return __ypc.SendRequest<TResponse>(msg);
                }
                else if (this._Transport.HasFlag(Transport.MQTT) && __mqtt != null && __mqtt.IsConnected)
                {
                    return __mqtt.SendRequest<TResponse>(msg, timeout: TimeSpan.FromSeconds(10));
                }
                else if (this._Transport.HasFlag(Transport.REST))
                {
                    //TODO: send request from rest
                    DebugEx.Assert("Cannot send request using REST (Not implemented yet)");
                    return default(TResponse);
                }
                else
                    return default(TResponse);
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception caught");
                return default(TResponse);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public ApiMsg HandleApiReq(object msg)
        {
            try
            {
                if (msg is Yodiwo.API.Plegma.LoginReq)
                {
                    var rsp = new LoginRsp()
                    {
                        NodeKey = this.NodeKey,
                        SecretKey = this.NodeSecret,
                    };
                    return rsp;
                }
                else if (msg is Yodiwo.API.Plegma.NodeInfoReq)
                {
                    NodeInfoRsp rsp = new NodeInfoRsp()
                    {
                        Name = conf.Name,
                        Type = (nodeType == eNodeType.Unknown) ? eNodeType.TestEndpoint : nodeType,
                        Capabilities = eNodeCapa.None |
                                       (CanSolveGraphs ? eNodeCapa.SupportsGraphSolving : eNodeCapa.None),
                        ThingTypes = (this._thingTypes == null) ? null : this._thingTypes.ToArray(),
                        BlockLibraries = NodeGraphManager != null && CanSolveGraphs ? NodeGraphManager.BlockLibrariesNames : null,
                    };
                    return rsp;
                }
                else if (msg is NodeUnpairedReq)
                {
                    var m = msg as NodeUnpairedReq;
                    OnNodeUnpaired?.Invoke(m.ReasonCode, m.Message);
                    this._IsPaired = false;
                    this.NodeKey = null;
                    this.NodeSecret = null;

                    //just return empty rsp, acknowledging the unpairing
                    return new NodeUnpairedRsp();
                }
                else if (msg is Yodiwo.API.Plegma.ThingsGet)
                {
                    var msgTyped = msg as ThingsGet;
                    var op = msgTyped.Operation;
                    var tkey = (ThingKey)msgTyped.ThingKey;

                    var rsp = new ThingsSet() { Operation = eThingsOperation.Invalid };

                    //Handle request
                    if (op == eThingsOperation.Get)
                    {
                        if (tkey.IsValid)
                        {
                            var thing = _Things.TryGetOrDefault(tkey);
                            if (thing == null)
                                return rsp;
                            else
                            {
                                rsp.Operation = eThingsOperation.Update;
                                rsp.Status = true;
                                rsp.Data = new[] { thing };
                                return rsp;
                            }
                        }
                        else
                        {
                            if (this._Things == null)
                                return rsp;
                            else
                            {
                                rsp.Operation = eThingsOperation.Update;
                                rsp.Status = true;
                                rsp.Data = this._Things.Values.ToArray();
                                OnThingsRegistered?.Invoke();
                            }
                        }
                    }
                    else if (op == eThingsOperation.Scan)
                    {
                        if (OnThingScanRequest == null)
                            return rsp;
                        else
                        {
                            var things = OnThingScanRequest?.Invoke(tkey);
                            if (things == null)
                                return rsp;
                            rsp.Operation = eThingsOperation.Update;
                            rsp.Data = things.ToArray();
                            rsp.Status = true;
                        }
                    }
                    else if (op == eThingsOperation.Delete)
                    {
                        //find thing
                        var thing = Things.TryGetOrDefaultReadOnly(msgTyped.ThingKey);
                        if (thing == null)
                            return rsp;
                        //do callback
                        var success = OnThingDeleteRequest?.Invoke(null);
                        if (!success.HasValue || !success.Value)
                            return rsp;
                        //remove thing from nodelib
                        RemoveThing(msgTyped.ThingKey, SendToCloud: false);
                        rsp.Operation = eThingsOperation.Delete;
                        rsp.Status = true;
                    }
                    //return response
                    return rsp;
                }
                else if (msg is Yodiwo.API.Plegma.ThingsSet)
                {
                    var msgTyped = msg as ThingsSet;
                    var op = msgTyped.Operation;

                    var rsp = new GenericRsp()
                    {
                        IsSuccess = false
                    };

                    //TODO: Dummy handler for thing deletion, implemenent this properly
                    if (op == eThingsOperation.Delete)
                    {
                        rsp.IsSuccess = true;
                    }
                    else if (op == eThingsOperation.Update)
                    {
                        var msgThings = msgTyped.Data;
                        if (msgThings != null)
                        {
                            bool hadError = false;
                            foreach (var msgThing in msgThings)
                            {
                                //get key
                                var tk = (ThingKey)msgThing.ThingKey;
                                if (tk.IsInvalid || tk.NodeKey != NodeKey)
                                {
                                    hadError = true;
                                    continue;
                                }
                                //find thing
                                var thing = Things.TryGetOrDefaultReadOnly(tk);
                                if (thing == null)
                                {
                                    hadError = true;
                                    continue;
                                }
                                //create a "previous" copy backup
                                var before = thing.DeepClone();
                                //update thing
                                thing.Update(msgThing);
                                //raise event
                                OnThingUpdated?.Invoke(thing, before);
                            }

                            //fill response
                            rsp.IsSuccess = !hadError;
                        }
                    }
                    //return response
                    return rsp;
                }
                else if (msg is Yodiwo.API.Plegma.PortStateReq)
                {
                    var msgRx = msg as PortStateReq;
                    var states = HandlePortStateReq(msgRx.PortKeys);
                    var rsp = new PortStateRsp()
                    {
                        Operation = (msg as PortStateReq).Operation,
                        PortStates = states
                    };
                    //if (OnPortStateRx != null)
                    //  OnPortStateRx();
                    return rsp;
                }
                else if (msg is Yodiwo.API.Plegma.PingReq)
                {
                    var rsp = new PingRsp()
                    {
                        Data = (msg as Yodiwo.API.Plegma.PingReq).Data
                    };
                    return rsp;
                }
                else if (msg is Yodiwo.API.Plegma.GraphDeploymentReq)
                {
                    //forward to node graph manager
                    if (NodeGraphManager != null)
                        return NodeGraphManager.HandleGraphDeploymentReq(msg as GraphDeploymentReq);
                    else
                        return new GenericRsp() { IsSuccess = false, Message = "Node has no NodeGraphManager" };
                }
                else if (msg is Yodiwo.API.Plegma.A2mcuActiveDriversReq)
                {
                    if (OnA2mcuActiveDriversReq != null)
                    {
                        return OnA2mcuActiveDriversReq(msg as Yodiwo.API.Plegma.A2mcuActiveDriversReq);
                    }
                    else
                    {
                        return new GenericRsp() { IsSuccess = false, Message = "Node does not handle A2mcuActiveDriversReq" };
                    }
                }
                else if (msg is Yodiwo.API.Plegma.A2mcuCtrlReq)
                {
                    if (OnA2mcuCtrlReq != null)
                    {
                        return OnA2mcuCtrlReq(msg as Yodiwo.API.Plegma.A2mcuCtrlReq);
                    }
                    else
                    {
                        return new GenericRsp() { IsSuccess = false, Message = "Node does not handle A2mcuCtrlReq" };
                    }
                }
                else
                {
                    return OnUnexpectedRequest?.Invoke(msg);
                }
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception in node HandleApiReq()");
                return null;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        private PortState[] HandlePortStateReq(string[] pks)
        {
            try
            {
                ListTS<PortState> portstates = new ListTS<PortState>();
                foreach (var pk in pks)
                {
                    var portkey = (PortKey)pk;
                    var port = this._Things[portkey.ThingKey].Ports.Find(i => i.PortKey == portkey);
                    var portState = new PortState()
                    {
                        PortKey = pk,
                        IsDeployed = true,
                        State = port.State
                    };
                    portstates.Add(portState);
                }
                return portstates.ToArray();
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception in node HandlePortStateReq()");
                return null;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public Tuple<HashSet<PortKey>, HashSet<Thing>> BeginActiveThingsUpdate()
        {
            //enter lock
            Monitor.Enter(_activeChangesLocker);
            try
            {
                //create sets
                var prevActivePortKeys = new HashSet<PortKey>();
                var prevActiveThings = new HashSet<Thing>();

                //prepare sets
                //copy from cloud
                prevActivePortKeys.AddFromSource(_CloudActivePortKeys);
                prevActiveThings.AddFromSource(_CloudActiveThings);
                //copy from local
                if (NodeGraphManager != null)
                {
                    prevActivePortKeys.AddFromSource(NodeGraphManager.ActivePortKeys);
                    prevActiveThings.AddFromSource(NodeGraphManager.ActiveThings);
                }
                return new Tuple<HashSet<PortKey>, HashSet<Thing>>(prevActivePortKeys, prevActiveThings);
            }
            catch (Exception ex) { DebugEx.TraceErrorException(ex); Monitor.Exit(_activeChangesLocker); return null; }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void EndActiveThingsUpdate(Tuple<HashSet<PortKey>, HashSet<Thing>> sets)
        {
            //check
            if (sets == null)
                return;
            try
            {
                //get old sets
                var prevActivePortKeys = sets.Item1;
                var prevActiveThings = sets.Item2;

                //create sets
                var newActivePortKeys = new HashSet<PortKey>();
                var newActiveThings = new HashSet<Thing>();

                //Prepare new sets
                //copy from cloud
                newActivePortKeys.AddFromSource(_CloudActivePortKeys);
                newActiveThings.AddFromSource(_CloudActiveThings);
                //copy from local
                if (NodeGraphManager != null)
                {
                    newActivePortKeys.AddFromSource(NodeGraphManager.ActivePortKeys);
                    newActiveThings.AddFromSource(NodeGraphManager.ActiveThings);
                }

                //inform PORTS for deactivations
                foreach (var pkey in prevActivePortKeys)
                    if (!newActivePortKeys.Contains(pkey))
                        try { OnPortDeactivated?.Invoke(pkey); }
                        catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught in user code for OnPortDeactivated (PortID=" + pkey.PortUID + ")"); }

                //inform PORTS for activations
                foreach (var pkey in newActivePortKeys)
                    if (!prevActivePortKeys.Contains(pkey))
                        try { OnPortActivated?.Invoke(pkey); }
                        catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught in user code for OnPortActivated (PortID=" + pkey.PortUID + ")"); }

                //inform THINGS for deactivations
                foreach (var thing in prevActiveThings)
                    if (!newActiveThings.Contains(thing))
                        try { OnThingDeactivated?.Invoke(thing); }
                        catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught in user code for OnThingDeactivated (ThingName=" + thing.Name + ")"); }

                //inform THINGS for activation
                foreach (var thing in newActiveThings)
                    if (!prevActiveThings.Contains(thing))
                        try { OnThingActivated?.Invoke(thing); }
                        catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught in user code for OnThingActivated (ThingName=" + thing.Name + ")"); }

                //clear sets
                prevActivePortKeys.Clear(); prevActivePortKeys = null;
                prevActiveThings.Clear(); prevActiveThings = null;
            }
            finally
            {
                //exit lock
                try { Monitor.Exit(_activeChangesLocker); } catch { }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void PerformActiveThingsUpdate(Action Updater)
        {
            //collect sets
            var sets = BeginActiveThingsUpdate();

            //run updater
            Updater();

            //finish
            EndActiveThingsUpdate(sets);
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void HandleApiMsg(object msg)
        {
            try
            {
                if (msg is Yodiwo.API.Plegma.ActivePortKeysMsg)
                {
                    var rsp = msg as Yodiwo.API.Plegma.ActivePortKeysMsg;

                    //collect sets
                    var sets = BeginActiveThingsUpdate();

                    //update active port keys
                    _CloudActivePortKeys.Clear();
                    _CloudActivePortKeys.AddFromSource(rsp.ActivePortKeys.Select(k => (PortKey)k));
                    //update active things
                    _CloudActiveThings.Clear();
                    _CloudActiveThings.AddFromSource(_CloudActivePortKeys.Select(k => Things.TryGetOrDefaultReadOnly(k.ThingKey)).WhereNotNull());

                    //finish
                    EndActiveThingsUpdate(sets);
                }
                else if (msg is PortEventMsg)
                {
                    HandlePortEventMsg(msg as Yodiwo.API.Plegma.PortEventMsg);
                }
                else
                {
                    OnUnexpectedMessage?.Invoke(msg);
                }
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception in node HandleApiMsg()");
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        #region YPChannel
        //------------------------------------------------------------------------------------------------------------------------

        SimpleActionResult _YPChannelConnectWithWorker(string ypserver, int port, bool isSecure)
        {
            try
            {
                //ypchannel
                if (this._IsPaired && this.NodeKey != null)
                {
                    if (this.NodeKey.IsValid)
                    {
                        //check if current channel is valid
                        if (Channel != null && (Channel.RemoteHost != ypserver || Channel.RemotePort != port.ToStringInvariant()))
                        {
                            Channel.Close();
                            Channel = null;
                        }
                        //create new channel if needed
                        if (Channel == null)
                        {
                            //create protocol
                            var proto = new YPChannel.Protocol()
                            {
                                Version = API.Plegma.PlegmaAPI.APIVersion,
                                ProtocolDefinitions = new List<YPChannel.Protocol.MessageTypeGroup>()
                                {
                                    new YPChannel.Protocol.MessageTypeGroup() { GroupName = API.Plegma.PlegmaAPI.ApiGroupName,              MessageTypes = API.Plegma.PlegmaAPI.ApiMessages             },
                                    new YPChannel.Protocol.MessageTypeGroup() { GroupName = API.Plegma.PlegmaAPI.ApiLogicGroupName,         MessageTypes = API.Plegma.PlegmaAPI.LogicApiMessages        },
                                    new YPChannel.Protocol.MessageTypeGroup() { GroupName = Yodiwo.API.MediaStreaming.Video.ApiGroupName,   MessageTypes= Yodiwo.API.MediaStreaming.Video.ApiMessages   },
                                    new YPChannel.Protocol.MessageTypeGroup() { GroupName = Yodiwo.API.MediaStreaming.Audio.ApiGroupName,   MessageTypes= Yodiwo.API.MediaStreaming.Audio.ApiMessages   },
                                },
                            };
                            //create channel
                            Channel = new Yodiwo.YPChannel.Transport.Sockets.Client(proto);
                            Channel.NoDelay = true; //Disable Nagle Algorithm since we care about latency more than throughput
                            Channel.Name = conf.Name;
                            Channel.OnMessageReceived += Channel_OnMessageReceived;
                            Channel.OnClosedEvent += Channel_OnClosedEvent;
                        }
                        //close existing channel
                        if (Channel.IsOpen)
                            Channel.Close();
                        //connect
                        return Channel.Connect(ypserver, port, isSecure, "*.yodiwo.com");
                    }
                    else
                        return new SimpleActionResult() { IsSuccessful = false, Message = "Nodekey" + this.NodeKey + " is not valid" };
                }
                else
                    return new SimpleActionResult() { IsSuccessful = false, Message = "Node has not been paired or nodekey is null" };
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception in node _YPChannelConnectWithWorker()");
                return new SimpleActionResult() { IsSuccessful = false, Message = "Unhandled exception caught." + ex.Message };
            }
        }
        //------------------------------------------------------------------------------------------------------------------------

        private void Channel_OnClosedEvent(Channel Channel)
        {
            try
            {
                OnTransportDisconnected?.Invoke(Transport.YPCHANNEL, "Disconnected");
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception caught");
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        private void Channel_OnMessageReceived(YPChannel.Channel Channel, YPChannel.YPMessage Message)
        {
            try
            {
                var api_msg = Message.Payload;

                //check if it is a new Request
                if (Message.IsRequest)
                {
                    var rsp = HandleApiReq(api_msg);
                    if (rsp != null)
                    {
                        //send response if handler created one
                        Channel.SendResponse(rsp, Message.MessageID);
                    }
                }
                //otherwise, handle as a normal new Plegma API Message
                else
                {
                    HandleApiMsg(api_msg);
                }
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception caught");
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Rest
        //------------------------------------------------------------------------------------------------------------------------
        SimpleActionResult _RestConnectWithWorker()
        {
            try
            {
                if (this._IsPaired && this.NodeKey != null)
                {
                    if (this.NodeKey.IsValid)
                    {
                        if (RestRequestConsumer == null)
                        {
                            RestRequestConsumer = new RequestQueueConsumer<Tuple<string, string>>(RESTConsumerHandler);
                            RestRequestConsumer.Start();

                            //send things
                            var msg = new ThingsSet()
                            {
                                Operation = eThingsOperation.Update,
                                Data = this._Things.Values.ToArray(),
                            };
                            PostMsg(PlegmaAPI.ApiMsgNames[typeof(ThingsSet)], msg);
                        }
                        return new SimpleActionResult() { IsSuccessful = true, Message = string.Empty };
                    }
                    else
                        return new SimpleActionResult() { IsSuccessful = false, Message = "Nodekey" + this.NodeKey + " is not valid" };
                }
                else
                    return new SimpleActionResult() { IsSuccessful = false, Message = "Node has not been paired or nodekey is null" };
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception caught");
                return new SimpleActionResult() { IsSuccessful = false, Message = "Unhandled exception caught." + ex.Message };
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        private void RESTConsumerHandler(Tuple<string, string> target_msg)
        {
            try
            {
                //add base route
                string restRoute = this.conf.FrontendServer + "/" + PlegmaAPI.RestAPIBaseRoute;
                //add API ver
                restRoute += "/" + PlegmaAPI.APIVersion;
                //add nodekey
                restRoute += "/" + this.NodeKey;
                //add secretkey
                restRoute += "/" + this.NodeSecret;
                //add msg name
                restRoute += "/" + target_msg.Item1;

                var res = Yodiwo.Tools.Http.Request(HttpMethods.Post, restRoute, target_msg.Item2, HttpRequestDataFormat.Json);
                if (res.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    DebugEx.Assert("REST resp(" + res.StatusCode + "): " + res.ResponseBodyText);
                }
            }
            catch (Exception ex) { DebugEx.TraceErrorException(ex); }
        }

        //------------------------------------------------------------------------------------------------------------------------

        private void PostMsg<T>(string target, T msg)
        {
            try
            {
                var body = msg.ToJSON(HtmlEncode: false);
                RestRequestConsumer.Enqueue(new Tuple<string, string>(target, body));
            }
            catch (Exception ex) { DebugEx.Assert(ex, "REST PortMsg failed"); }
        }

        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        //------------------------------------------------------------------------------------------------------------------------

        public bool SaveStr(string Identifier, string Data, bool Secure = true)
        {
            if (Data == null)
                return false;
            else
                return Save(Identifier, Encoding.UTF8.GetBytes(Data), Secure: Secure);
        }

        //------------------------------------------------------------------------------------------------------------------------

        public bool SaveObject(string Identifier, object Data, bool Secure = true)
        {
            if (Data == null)
                return false;
            else
                return SaveStr(Identifier, Data.ToJSON(), Secure: Secure);
        }

        //------------------------------------------------------------------------------------------------------------------------

        public bool Save(string Identifier, byte[] Data, bool Secure = true)
        {
            if (DataSave == null)
                return false;
            else
                try
                {
                    //find IO locker
                    object waiter;
                    lock (IOLocker)
                    {
                        waiter = IOLocker.TryGetOrDefault(Identifier);
                        if (waiter == null)
                        {
                            waiter = new object();
                            IOLocker.Add(Identifier, waiter);
                        }
                    }

                    //lock and write
                    lock (waiter)
                        return DataSave(Identifier, Data, Secure);
                }
                catch (Exception ex)
                {
                    DebugEx.Assert(ex, "Unhandled exception during data save");
                    return false;
                }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public string LoadStr(string Identifier, bool Secure = true)
        {
            var data = Load(Identifier, Secure: Secure);
            if (data == null)
                return null;
            else
#if NETFX
                return Encoding.UTF8.GetString(data);
#else
                return Encoding.UTF8.GetString(data, 0, data.Length);
#endif
        }

        //------------------------------------------------------------------------------------------------------------------------

        public T LoadObject<T>(string Identifier, bool Secure = true, T Default = default(T))
        {
            var data = LoadStr(Identifier, Secure: Secure);
            if (data == null)
                return Default;
            else
                return data.FromJSON<T>();
        }

        //------------------------------------------------------------------------------------------------------------------------

        public byte[] Load(string Identifier, bool Secure = true)
        {
            if (DataLoad == null)
                return null;
            else
                try
                {
                    //find IO locker
                    object waiter;
                    lock (IOLocker)
                    {
                        waiter = IOLocker.TryGetOrDefault(Identifier);
                        if (waiter == null)
                        {
                            waiter = new object();
                            IOLocker.Add(Identifier, waiter);
                        }
                    }

                    //lock and read
                    lock (waiter)
                        return DataLoad?.Invoke(Identifier, Secure);
                }
                catch (Exception ex)
                {
                    DebugEx.Assert(ex, "Unhandled exception during data load");
                    return null;
                }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public bool IsPortActive(PortKey key)
        {
            //check cloud
            if (_CloudActivePortKeys.Contains(key))
                return true;

            //check local
            //if (_CloudActivePortKeys.Contains(key))
            //return true;

            return false;
        }

        //------------------------------------------------------------------------------------------------------------------------

        public bool IsThingActive(ThingKey key)
        {
            var thing = _Things.TryGetOrDefault(key);
            if (thing == null)
                return false;

            //check cloud
            if (_CloudActiveThings.Contains(thing))
                return true;

            //check local
            //if (_CloudActivePortKeys.Contains(thing))
            //return true;

            return false;
        }

        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}


