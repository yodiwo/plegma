using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Yodiwo.API.Plegma;
using System.Threading;
using Yodiwo;
using Yodiwo.YPChannel;
using Yodiwo.NodeLibrary.Pairing;
using System.Reflection;
using System.Security;

namespace Yodiwo.NodeLibrary
{
    public class Node
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public const string ThingKeyUIDModuleIdSeparator = ":";
        public const string DataIdentifier_Things = "Things.json";
        //------------------------------------------------------------------------------------------------------------------------
        bool _IsInitialized = false;
        public bool IsInitialized => _IsInitialized;
        //------------------------------------------------------------------------------------------------------------------------
        public object locker = new object();
        //------------------------------------------------------------------------------------------------------------------------
        public string uuid { get { return conf.uuid; } }
        public string Name { get { return conf.Name; } }
        public bool CanSolveGraphs { get { return conf.CanSolveGraphs; } }
        public bool IsWarlock { get { return conf.IsWarlock; } }
        public bool IsShellNode { get { return conf.IsShellNode; } }
        private NodeConfig conf;
        //------------------------------------------------------------------------------------------------------------------------
        public NodeKey NodeKey { get; private set; }
        public SecureString NodeSecret { get; private set; }
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
        public bool AutoReconnect { get; set; } = true;
        public TimeSpan AutoReconnectDelay { get; set; } = TimeSpan.FromSeconds(1);
        public TimeSpan keepAliveSpinDelay { get; set; }
        //------------------------------------------------------------------------------------------------------------------------
        public bool IsLinkUp { set; get; }
        //------------------------------------------------------------------------------------------------------------------------
        public bool IsConnected
        {
            get
            {
                if (Transport == Transport.YPCHANNEL)
                    return Channel?.IsOpen ?? false;
                else
                    throw new NotSupportedException();
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        //transports
        private Yodiwo.YPChannel.Transport.Sockets.Client Channel;
        private Transports.ITransportMQTT mqtthandler;
        private Yodiwo.RequestQueueConsumer<Tuple<string, string>> RestRequestConsumer;
        public Action<Yodiwo.YPChannel.Transport.Sockets.Client> NewChannelSetup = null;
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

        private DictionaryTS<ThingKey, Thing> _OrphanThings = new DictionaryTS<ThingKey, Thing>();
        private DictionaryTS<ThingKey, Thing> _Things = new DictionaryTS<ThingKey, Thing>();
        public readonly ReadOnlyDictionary<ThingKey, Thing> Things;

        private ListTS<ThingType> _thingTypes = new ListTS<ThingType>();

        private eNodeType nodeType;
        private eConnectionFlags connectionFlags;
        //------------------------------------------------------------------------------------------------------------------------
        //helpers for active things/port/ changes
        object _activeChangesLocker = new object();
        //------------------------------------------------------------------------------------------------------------------------
        //Graphs
        public readonly Graphs.INodeGraphManager NodeGraphManager;
        //------------------------------------------------------------------------------------------------------------------------
        public readonly DictionaryTS<Port, Action<string, bool>> PortEventHandlers = new DictionaryTS<Port, Action<string, bool>>();
        public readonly DictionaryTS<Thing, Action<TupleS<Port, string>[], bool>> ThingEventHandlers = new DictionaryTS<Thing, Action<TupleS<Port, string>[], bool>>();
        //------------------------------------------------------------------------------------------------------------------------
        HashSetTS<INodeModule> _NodeModules = new HashSetTS<INodeModule>();
        public IReadOnlySet<INodeModule> NodeModules => _NodeModules;
        DictionaryTS<INodeModule, HashSetTS<Yodiwo.API.Plegma.Thing>> _NodeModuleThings = new DictionaryTS<INodeModule, HashSetTS<Yodiwo.API.Plegma.Thing>>();
        DictionaryTS<ThingKey, INodeModule> thingkey2module = new DictionaryTS<ThingKey, INodeModule>();
        //------------------------------------------------------------------------------------------------------------------------
        #region Delegates events that are exposed to user

        #region Plegma and Transport
        public delegate void OnNodePairedDelegate(NodeKey nodekey, SecureString nodesecret);
        public event OnNodePairedDelegate OnNodePaired;

        public delegate void OnNodePairingFailedDelegate(string Message);
        public event OnNodePairingFailedDelegate OnNodePairingFailed;

        public delegate void OnNodeUnpairedDelegate(eUnpairReason reasonCode, string msg);
        public event OnNodeUnpairedDelegate OnNodeUnpaired;

        public delegate void OnLinkActivatedDelegate();
        public event OnLinkActivatedDelegate OnLinkActivated;

        public delegate void OnTransportConnectedDelegate(Transport Transport, string msgStatus);
        public event OnTransportConnectedDelegate OnTransportConnected;

        public delegate void OnTransportConnectionStartDelegate(Transport Transport);
        public event OnTransportConnectionStartDelegate OnTransportConnectionStart;

        public delegate void OnTransportDisconnectedDelegate(Transport Transport, string msgStatus);
        public event OnTransportDisconnectedDelegate OnTransportDisconnected;

        public delegate void OnTransportErrorDelegate(Transport Transport, TransportErrors Error, string msgStatus);
        public event OnTransportErrorDelegate OnTransportError;

        public delegate void OnThingsRegisteredDelegate();
        public event OnThingsRegisteredDelegate OnThingsRegistered;

        public delegate void OnChangedStateDelegate(Thing thing, Port port, String status, bool isEvent);
        public event OnChangedStateDelegate OnChangedState;

        public delegate void OnChangedStateGrouppedDelegate(Thing thing, List<TupleS<Port, String>> states);
        public event OnChangedStateGrouppedDelegate OnChangedStateGroupped;

        public delegate void OnPortEventMsgProcessedDelegate(PortEventMsg msg);
        public event OnPortEventMsgProcessedDelegate OnPortEventMsgProcessed;

        public delegate PlegmaApiMsg OnA2mcuActiveDriversReqDelegate(A2mcuActiveDriversReq request);
        public OnA2mcuActiveDriversReqDelegate OnA2mcuActiveDriversReq;

        public delegate PlegmaApiMsg OnA2mcuCtrlReqDelegate(A2mcuCtrlReq request);
        public OnA2mcuCtrlReqDelegate OnA2mcuCtrlReq;

        public delegate PlegmaApiMsg OnUnexpectedRequestDelegate(object request);
        public event OnUnexpectedRequestDelegate OnUnexpectedRequest;

        public delegate void OnUnexpectedMessageDelegate(object message);
        public event OnUnexpectedMessageDelegate OnUnexpectedMessage;

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

        public delegate IEnumerable<Thing> OnThingScanRequestDelegate(ThingKey key);
        public OnThingScanRequestDelegate OnThingScanRequest;

        public delegate bool OnThingDeleteRequestDelegate(Thing Thing);
        public OnThingDeleteRequestDelegate OnThingDeleteRequest;

        public delegate void OnForgetMeDelegate();
        public event OnForgetMeDelegate OnForgetMeCb;
        #endregion

        #region Warlock integration
        public delegate void OnReceivedWarlockApiMsgDelegate(Yodiwo.API.Warlock.WarlockApiMsg msg);
        public event OnReceivedWarlockApiMsgDelegate OnReceivedWarlockApiMsg;
        #endregion

        #endregion
        //------------------------------------------------------------------------------------------------------------------------
        //used to protect against IO race contitions
        DictionaryTS<string, object> IOLocker = new DictionaryTS<string, object>();

        //Storage callbacks
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
                    IPairingModule pairingmodule,
                    DataLoadDelegate DataLoad,
                    DataSaveDelegate DataSave,
                    Graphs.INodeGraphManager NodeGraphManager = null,
                    Type MqttTransport = null,
                    List<ThingType> thingTypes = null,
                    eNodeType nodeType = eNodeType.Unknown,
                    TimeSpan? keepAliveSpinDelay = null,
                    eConnectionFlags connFlags = eConnectionFlags.None)
        {
            DebugEx.Assert(conf != null, "Cannot have null conf");
            this.conf = conf.Clone() as NodeConfig;
            this.Things = new ReadOnlyDictionary<ThingKey, Thing>(this._Things);
            this._thingTypes.AddFromSource(thingTypes);
            this.nodeType = nodeType;
            this._PairingModule = pairingmodule;
            this.DataLoad = DataLoad;
            this.DataSave = DataSave;
            this.NodeGraphManager = NodeGraphManager;
            this.connectionFlags = connFlags;
            this.keepAliveSpinDelay = (keepAliveSpinDelay == null) ? TimeSpan.FromMinutes(5) : keepAliveSpinDelay.Value;
            //clampfloor keepalivespin delay
            if (this.keepAliveSpinDelay.TotalMilliseconds < 1000)
                this.keepAliveSpinDelay = TimeSpan.FromSeconds(1);

            // Note: Prevent erroneous behaviour
            // TODO: Needs further internal investigation/fixes 
            //       CanSolveGraphs flag does not fully disable NodeGraphManager/Splitting functionality
            if ((this.conf.CanSolveGraphs && (DataLoad == null || DataSave == null || NodeGraphManager == null)) ||
                (!this.conf.CanSolveGraphs && NodeGraphManager != null))
            {
                this.conf.CanSolveGraphs = false;
                this.NodeGraphManager = null;
                this.DataLoad = null;
                this.DataSave = null;
                DebugEx.Assert("Error configuring graph solving functionality - Fog support is disabled");
                DebugEx.TraceError("Error configuring graph solving functionality - Fog support is disabled");
            }

#if DEBUG
            DebugEx.TraceLog("Starting NodeLibrary in DEBUG mode");
#else
            DebugEx.TraceLog("Starting NodeLibrary in RELEASE mode");
#endif

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

            //Initialize Graph stuff
            NodeGraphManager?.Initialize(this);

            //inited
            _IsInitialized = true;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public void DeInitialize()
        {
            try
            {
                //deinit
                _IsInitialized = false;

                //disconnect from server
                Disconnect();

                //deinit NodeDiscovery stuff
                try { NodeDiscovery?.Deinitialize(); } catch (Exception ex2) { DebugEx.Assert(ex2, "NodeDiscovery DeInitialize failed"); }

                //deinit Graph stuff
                try { NodeGraphManager?.DeInitialize(); } catch (Exception ex2) { DebugEx.Assert(ex2, "NodeDiscovery DeInitialize failed"); }

                //disconnect
                Disconnect();
            }
            catch (Exception ex) { DebugEx.Assert(ex, "NodeLibrary DeInitialize failed"); }
        }
        //------------------------------------------------------------------------------------------------------------------------

        public Task<bool> StartPairingAsync(string frontendUrl, string redirectUrl, string selfUrl = null)
        {
            return Task<bool>.Run(() => { return StartPairing(frontendUrl, redirectUrl, selfUrl: selfUrl); });
        }

        public async Task<bool> StartPairing(string frontendUrl, string redirectUrl, string selfUrl = null)
        {
            //null check
            if (this._PairingModule == null)
                return false;
            //start pairing module
            var res = this._PairingModule.StartPair(frontendUrl, redirectUrl, conf, selfUrl, onPaired, onPairFailed);
            if (!res)
            {
                DebugEx.Assert("Could not start pairing");
                return false;
            }
            //wait for pairing completion
            while (!_IsPaired)
                await Task.Delay(300).ConfigureAwait(false);
            return true;
        }

        //------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Auto submit pairing request, wait for completion and retrieve nodekey/secretkey (used for Testing)
        /// </summary>
        public void AutoPairing(string frontendUrl, IEnumerable<Cookie> cookies, string antiCSRFtoken)
        {
            try
            {
                var backend = new Yodiwo.NodeLibrary.Pairing.NodePairingBackend(frontendUrl, conf, onPaired, onPairFailed);
                string token2 = backend.pairGetTokens(null);

                //send uuidentry request
                var paramet2 = new Dictionary<string, string>()
                {
                    { "uuid", conf.uuid },
                    { "token2", token2},
                    { "_token2", token2},
                    { "NCSRF", antiCSRFtoken },
                };
                var t1 = Tools.Http.RequestPost(backend.pairingPostUrl + "/complete", paramet2, cookies);
                if (t1.IsSuccessStatusCode)
                    backend.pairGetKeys();
                else
                    DebugEx.Assert("Pairing failed (could not enter uuid)");
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught"); }
        }

        //------------------------------------------------------------------------------------------------------------------------

        private void onPaired(NodeKey nodeKey, SecureString nodeSecret)
        {
            try
            {
                //setup node
                SetupNodeKeys(nodeKey, nodeSecret);

                //inform user
                try
                {
                    try { OnNodePaired?.Invoke(this.NodeKey, this.NodeSecret); } catch (Exception ex) { DebugEx.TraceError(ex, "UserCode exception caught"); }
                }
                catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught from user callback"); }

                //finished pairing
                this._PairingModule.EndPair();
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught"); }
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
                    try { OnNodePairingFailed?.Invoke(Message); } catch (Exception ex) { DebugEx.TraceError(ex, "UserCode exception caught"); }
                }
                catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught from user callback"); }

                //finished pairing
                this._PairingModule.EndPair();
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught"); }
        }

        //------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// set node and secret keys for Node
        /// </summary>
        /// <param name="nk"></param>
        /// <param name="secret"></param>
        /// <param name="setupThings">set to false to manually handle thing syncing</param>
        public void SetupNodeKeys(NodeKey nk, SecureString secret)
        {
            try
            {
                lock (locker)
                {
                    this.NodeKey = nk;
                    this.NodeSecret = secret;
                    this._IsPaired = nk.IsValid && secret != null && secret.Length > 0;

                    //check
                    if (!IsPaired)
                    {
                        DebugEx.TraceError("Invalid keys");
                        return;
                    }

                    //set key on modules
                    lock (this._NodeModules)
                        NodeModules.ForEach(m => { try { m.NodeKey = nk; } catch (Exception ex) { DebugEx.Assert(ex, "NodeModule exception while trying to set key"); } });

                    //load things
                    if (DataLoad != null)
                        try
                        {
                            var thingsJSON_Buffer = DataLoad(DataIdentifier_Things, true);
                            if (thingsJSON_Buffer != null && thingsJSON_Buffer.Length > 0)
                            {
                                var things = thingsJSON_Buffer.FromJSON<Thing[]>();
                                foreach (var thing in things)
                                    _AddThing(thing, null, sendToCloud: false, writeToDisk: false);
                            }
                        }
                        catch (Exception ex) { DebugEx.Assert(ex, "Thing loading failed"); }

                    //now we can load local graphs
                    try { NodeGraphManager?.DeployGraphs(); } catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught"); }

                    //start node discovery
                    try { NodeDiscovery?.Initialize(); } catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught"); }
                }
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught"); }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void AssignOrphanThingsToOwner(INodeModule owner)
        {
            foreach (var kv in _OrphanThings)
            {
                if (kv.Key.ThingUID.StartsWith(owner.ModuleID))
                {
                    _AddThing(kv.Value, owner, sendToCloud: false, writeToDisk: false, triggerEvents: false);
                    _OrphanThings.Remove(kv.Key);
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        bool _setupThing(Thing thing)
        {
            try
            {
                //replace macro
                thing.ThingKey = thing.ThingKey.Replace("$NodeKey$", NodeKey);

                //reconstruct key
                if (((ThingKey)thing.ThingKey).IsInvalid)
                {
                    DebugEx.Assert("Invalid ThingKey specified for thing: " + thing.Name);
                    return false;
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
                        DebugEx.Assert("No portkey specified for port");
                        return false;
                    }
                }

                //fix thing states
                foreach (var port in thing.Ports)
                {
                    port.State = port.State.Replace("$NodeKey$", NodeKey)
                                            .Replace("$ThingKey$", thing.ThingKey)
                                            .Replace("$PortKey$", port.PortKey);
                }
                return true;
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught"); return false; }
        }

        //------------------------------------------------------------------------------------------------------------------------

        /// <summary> This will overwrite all things from cloud  </summary>
        public bool OverwriteThings()
        {
            try
            {
                lock (locker)
                {
                    //still in limbo?
                    if (!IsPaired)
                    {
                        DebugEx.Assert("Cannot use node whilst not paired (call setupkeys first)");
                        return false;
                    }

                    //send update message
                    var req = new ThingsSet()
                    {
                        Operation = eThingsOperation.Overwrite,
                        Data = this._Things.Values.ToArray(),
                        Status = true,
                    };
                    var rsp = SendRequest<GenericRsp>(req);
                    if (rsp != null && rsp.IsSuccess)
                    {
                        DebugEx.TraceLog($"Successful overwrite of things");
                        return true;
                    }
                    else
                    {
                        DebugEx.TraceError($"Overwrite of things failed with: {rsp?.Message}");
                        return false;
                    }
                }
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught"); return false; }
        }

        //------------------------------------------------------------------------------------------------------------------------
        /*
        public IEnumerable<Thing> AddThings(IEnumerable<Thing> things, bool SendToCloud = true) { return AddThings(things, null, SendToCloud: SendToCloud); }
        public IEnumerable<Thing> AddThings(IEnumerable<Thing> things, INodeModule owner, bool SendToCloud = true)
        {
            bool error = false;
            var _things = new List<Thing>();
            try
            {
                if (things != null)
                    foreach (var thing in things.ToList())
                        try
                        {
                            var _t = AddThing(thing, SendToCloud: SendToCloud, owner: owner);
                            if (_t != null)
                                _things.Add(_t);
                        }
                        catch { error |= true; }
            }
            catch { error |= true; }
            return _things;
        }
        */
        //------------------------------------------------------------------------------------------------------------------------

        public Thing UpdateThing(Thing thing, bool SendToCloud = true) { return UpdateThing(thing, null, SendToCloud: SendToCloud); }
        public Thing UpdateThing(Thing thing, INodeModule owner, bool SendToCloud = true) { return AddThing(thing, owner, SendToCloud: SendToCloud); }

        public Thing AddThing(Thing thing, bool SendToCloud = true) { return _AddThing(thing, null, sendToCloud: SendToCloud); }
        public Thing AddThing(Thing thing, INodeModule owner, bool SendToCloud = true) { return _AddThing(thing, owner, sendToCloud: SendToCloud); }

        private Thing _AddThing(Thing thing, INodeModule owner, bool sendToCloud = true, bool writeToDisk = true, bool triggerEvents = true)
        {
            try
            {
                lock (locker)
                {
                    //still in limbo?
                    if (!IsPaired)
                    {
                        DebugEx.Assert("Cannot use node whilst not paired (call setupkeys first)");
                        return null;
                    }

                    //fix macro
                    thing.ThingKey = thing.ThingKey.Replace("$NodeKey$", NodeKey);

                    //setup thing
                    if (!_setupThing(thing))
                        return null;

                    //check ownership
                    var _existingOwner = thingkey2module.TryGetOrDefault(thing.ThingKey);
                    if (owner != null && _existingOwner != null && _existingOwner != owner)
                    {
                        DebugEx.Assert("Module adding thing that already belongs to another module");
                        return null; //invalid owner
                    }

                    //find original thing
                    Thing orThing = this._Things.TryGetOrDefault(thing.ThingKey, Default: thing);
                    if (orThing != thing)
                        orThing.Update(thing);

                    //add to collection
                    this._Things.ForceAdd(orThing.ThingKey, orThing);

                    //check thing ID if following rules
                    if (owner != null)
                    {
                        var tk = (ThingKey)thing.ThingKey;
                        if (!tk.ThingUID.StartsWith(owner.ModuleID + ThingKeyUIDModuleIdSeparator))
                        {
                            DebugEx.Assert("ThingUID must start with " + owner.ModuleID + ThingKeyUIDModuleIdSeparator);
                            return null;
                        }
                        //if has owner setup mappings
                        _NodeModuleThings.TryGetOrDefault(owner)?.Add(orThing);
                        thingkey2module.Add(orThing.ThingKey, owner);
                    }
                    else
                        _OrphanThings.Add(orThing.ThingKey, orThing);

                    //Save things
                    if (writeToDisk && DataSave != null)
                        try
                        {
                            var things = Things.Values.ToArray();
                            var json = things.ToJSON2();
                            if (json != null && json.Length > 0)
                                DataSave(DataIdentifier_Things, json, true);
                        }
                        catch (Exception ex) { DebugEx.Assert(ex, "Thing save failed"); }

                    //send update message
                    if (sendToCloud)
                    {
                        var req = new ThingsSet()
                        {
                            Operation = eThingsOperation.Update,
                            Data = new[] { orThing },
                            Status = true,
                        };
                        var rsp = SendRequest<GenericRsp>(req);
                        if (rsp != null && rsp.IsSuccess)
                            DebugEx.TraceLog($"Successful addition/updating of thing {thing.ThingKey}");
                        else if (rsp == null)
                            DebugEx.TraceError($"Addition/updating of thing {thing.ThingKey} failed (null response)");
                        else
                            DebugEx.TraceError($"Addition/updating of thing {thing.ThingKey} failed with: {rsp?.Message}");
                    }

                    if (triggerEvents)
                    {
                        //inform of active thing
                        if (owner != null && orThing != thing && IsThingActive(thing.ThingKey))
                            TaskEx.RunSafe(() => owner?.OnThingActivated(thing.ThingKey));

                        //inform of active ports
                        if (owner != null && orThing != thing)
                            foreach (var p in orThing.Ports)
                                if (IsPortActive(p.PortKey))
                                    TaskEx.RunSafe(() => owner?.OnPortActivated(p.PortKey));
                    }
                    return orThing;
                }
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught"); return thing; }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public bool RemoveThing(ThingKey thingkey, bool SendToCloud = true) { return RemoveThing(thingkey, null, SendToCloud: SendToCloud); }
        public bool RemoveThing(ThingKey thingkey, INodeModule owner, bool SendToCloud = true)
        {
            try
            {
                lock (locker)
                {
                    //still in limbo?
                    if (!IsPaired)
                    {
                        DebugEx.Assert("Cannot use node whilst not paired (call setupkeys first)");
                        return false;
                    }

                    //fix macro
                    if (thingkey.NodeKey == "$NodeKey$")
                        thingkey.NodeKey = NodeKey;

                    //find thing
                    var thing = this._Things.TryGetOrDefault(thingkey);
                    if (thing == null)
                        return false;

                    //check ownership
                    var _existingOwner = thingkey2module.TryGetOrDefault(thingkey);
                    if (_existingOwner != null && owner != null && _existingOwner != owner)
                        return false; //invalid owner

                    //remove from collection
                    this._Things.Remove(thingkey);

                    //remove owner setup mappings
                    if (_existingOwner != null)
                        _NodeModuleThings.TryGetOrDefault(_existingOwner)?.Remove(thing);
                    thingkey2module.Remove(thingkey);

                    //Save things
                    if (DataSave != null)
                        try
                        {
                            var things = Things.Values.ToArray();
                            var json = things.ToJSON2();
                            if (json != null && json.Length > 0)
                                DataSave(DataIdentifier_Things, json, true);
                        }
                        catch (Exception ex) { DebugEx.Assert(ex, "Thing save failed"); }

                    //send update message
                    if (SendToCloud)
                    {
                        var req = new ThingsSet()
                        {
                            Operation = eThingsOperation.Delete,
                            Data = new[] { thing },
                            Status = true,
                        };
                        var rsp = SendRequest<GenericRsp>(req);
                        if (rsp != null && rsp.IsSuccess)
                        {
                            DebugEx.TraceLog($"Successful removal of thing {thing.ThingKey} from cloud server");
                            return true;
                        }
                        else
                        {
                            DebugEx.TraceError($"Removal of thing {thing.ThingKey} from cloud server failed with: {rsp?.Message}");
                            return false;
                        }
                    }
                    else
                        return true;
                }
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught"); return false; }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public bool RemoveAllThings(bool SendToCloud = true, INodeModule owner = null)
        {
            try
            {
                lock (locker)
                {
                    //still in limbo?
                    if (!IsPaired)
                    {
                        DebugEx.Assert("Cannot use node whilst not paired (call setupkeys first)");
                        return false;
                    }

                    //nothing to do
                    if (this._Things.Count == 0)
                        return true;

                    //if has owner get from owner else get orphans
                    var toRemove = new HashSet<Thing>();
                    if (owner != null)
                    {
                        var owned = _NodeModuleThings.TryGetOrDefault(owner);
                        foreach (var thing in owned)
                        {
                            thingkey2module.Remove(thing.ThingKey);
                            toRemove.Add(thing);
                        }
                    }
                    else
                        toRemove.AddFromSource(_Things.Where(tkv => !thingkey2module.ContainsKey(tkv.Key)).Select(tkv => tkv.Value));

                    //remove them
                    foreach (var t in toRemove)
                        _Things.Remove(t.ThingKey);

                    //Save things
                    if (DataSave != null)
                        try
                        {
                            var things = Things.Values.ToArray();
                            var json = things.ToJSON2();
                            if (json != null && json.Length > 0)
                                DataSave(DataIdentifier_Things, json, true);
                        }
                        catch (Exception ex) { DebugEx.Assert(ex, "Thing save failed"); }

                    //send update message
                    if (SendToCloud)
                    {
                        var req = new ThingsSet()
                        {
                            Operation = eThingsOperation.Delete,
                            Data = toRemove.ToArray(),
                            Status = true,
                        };
                        var rsp = SendRequest<GenericRsp>(req);
                        if (rsp != null && rsp.IsSuccess)
                        {
                            DebugEx.TraceLog($"Successful delete of things");
                            return true;
                        }
                        else
                        {
                            DebugEx.TraceError($"Delete of things failed with: {rsp?.Message}");
                            return false;
                        }
                    }
                    else
                        return true;
                }
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught"); return false; }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void SetState(Thing thing, Port port, string state, bool IgnoreActivePortKeys = false)
        {
            try
            {
                DebugEx.TraceLog("=====>Set State<====== " + port.PortKey.ToString() + " state: " + state);
                SetState(new TupleS<Port, string>[] { new TupleS<Port, string>(port, state) }, IgnoreActivePortKeys: IgnoreActivePortKeys);
            }
            catch (Exception ex)
            {
                DebugEx.TraceErrorException(ex);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void SetState(Port port, string state, bool IgnoreActivePortKeys = false)
        {
            try
            {
                DebugEx.TraceLog("=====>Set State<====== " + port.PortKey.ToString() + " state: " + state);
                SetState(new TupleS<Port, string>[] { new TupleS<Port, string>(port, state) }, IgnoreActivePortKeys: IgnoreActivePortKeys);
            }
            catch (Exception ex)
            {
                DebugEx.TraceErrorException(ex);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public Port GetPort(PortKey portKey)
        {
            try
            {
                //find thing
                var tk = portKey.ThingKey;
                var thing = _Things.TryGetOrDefault(tk);
                if (thing == null)
                {
                    DebugEx.TraceError("Trying to set state to a thing that does not exists in nodelibrary things");
                    return null;
                }
                //find port
                var portkKeyStr = portKey.ToStringInvariant();
                return thing.Ports.Find(p => p.PortKey == portkKeyStr);
            }
            catch (Exception ex)
            {
                DebugEx.TraceErrorException(ex);
                return null;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        void _setStateMacroFromModule(INodeModule module, string portKey, string state, bool IgnoreActivePortKeys = false)
        {
            //replace macros
            portKey = portKey.Replace("$NodeKey$", NodeKey);
            //set state
            _setStateFromModule(module, portKey, state, IgnoreActivePortKeys: IgnoreActivePortKeys);
        }

        public void SetStateMacro(string portKey, string state, bool IgnoreActivePortKeys = false)
        {
            //replace macros
            portKey = portKey.Replace("$NodeKey$", NodeKey);
            //set state
            SetState(portKey, state, IgnoreActivePortKeys: IgnoreActivePortKeys);
        }
        //------------------------------------------------------------------------------------------------------------------------

        void _setStateMacroFromModule(INodeModule module, IEnumerable<TupleS<string, string>> states, bool IgnoreActivePortKeys = false)
        {
            //replace macros
            var transformer = states.Select(kv => new TupleS<PortKey, string>((PortKey)(kv.Item1.Replace("$NodeKey$", NodeKey)), kv.Item2));
            //set state
            _setStateFromModule(module, transformer, IgnoreActivePortKeys: IgnoreActivePortKeys);
        }

        public void SetStateMacro(IEnumerable<TupleS<string, string>> states, bool IgnoreActivePortKeys = false)
        {
            //replace macros
            var transformer = states.Select(kv => new TupleS<PortKey, string>((PortKey)(kv.Item1.Replace("$NodeKey$", NodeKey)), kv.Item2));
            //set state
            SetState(transformer, IgnoreActivePortKeys: IgnoreActivePortKeys);
        }

        //------------------------------------------------------------------------------------------------------------------------

        bool _checkPortKeyBelongsToModule(INodeModule module, PortKey portKey)
        {
            //get stuff
            if (module == null)
                return false;
            var thing = _Things.TryGetOrDefault(portKey.ThingKey);
            if (thing == null)
                return false;
            var moduleThings = _NodeModuleThings.TryGetOrDefault(module);
            if (moduleThings == null)
                return false;

            //check that module tried to set state to it's own thing
            return moduleThings.Contains(thing);
        }

        //------------------------------------------------------------------------------------------------------------------------

        void _setStateFromModule(INodeModule module, PortKey portKey, string state, bool IgnoreActivePortKeys = false)
        {
            if (_checkPortKeyBelongsToModule(module, portKey))
                SetState(portKey, state, IgnoreActivePortKeys: IgnoreActivePortKeys);
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void SetState(PortKey portKey, string state, bool IgnoreActivePortKeys = false)
        {
            try
            {
                //get port
                DebugEx.TraceDebug("=====>Set State<====== " + portKey.ToString() + " state: " + state);
                var port = GetPort(portKey);
                if (port == null)
                {
                    DebugEx.TraceError($"Trying to set state to a port(Key:{portKey}) that does not exists in thing");
                    return;
                }
                //set state
                SetState(new TupleS<Port, string>[] { new TupleS<Port, string>(port, state) }, IgnoreActivePortKeys: IgnoreActivePortKeys);
            }
            catch (Exception ex)
            {
                DebugEx.TraceErrorException(ex);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        void _setStateFromModule(INodeModule module, IEnumerable<TupleS<PortKey, string>> states, bool IgnoreActivePortKeys = false)
        {
            SetState(states.Where(t => _checkPortKeyBelongsToModule(module, t.Item1)), IgnoreActivePortKeys: IgnoreActivePortKeys);
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void SetState(IEnumerable<TupleS<PortKey, string>> states, bool IgnoreActivePortKeys = false)
        {
            SetState(states.Select(t => new TupleS<Port, string>(GetPort(t.Item1), t.Item2)), IgnoreActivePortKeys: IgnoreActivePortKeys);
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void SetState(IEnumerable<TupleS<Port, string>> states, bool IgnoreActivePortKeys = false)
        {
            if (states == null)
                return;

            try
            {
                //get to list
                var stateList = states.Where(s => s.Item1 != null).ToList();

                //filter out all ports with no state changed that did not flag themselves as ReceiveAllEvents
                stateList.RemoveAll(t => t.Item1.State == t.Item2 && t.Item1.ConfFlags.HasFlag(ePortConf.SupressIdenticalEvents));

                //first pass them from NodeGraphManager
                var ngm = NodeGraphManager;
                if (ngm != null)
                    ngm.HandlePortStates(stateList);

                //Process request
                List<PortEvent> events = null;
                foreach (var state in stateList)
                {
                    if (state.Item1 != null)
                    {
                        //get keys
                        var pk = (PortKey)state.Item1.PortKey;
                        var thk = pk.ThingKey;

                        //find thing
                        Thing thing = null;
                        if (_Things.TryGetValue(thk, out thing))
                        {
                            if (thing != null)
                            {
                                //get port
                                var port = thing.GetPort(pk);
                                if (port == null)
                                    continue;

                                //update port state and increase revision
                                port.SetState(state.Item2);
                                DebugEx.TraceVerbose("=======>IgnoreActivePortKeys:" + IgnoreActivePortKeys + "<=======");
                                DebugEx.TraceVerbose("=======>_CloudActivePortKeys.Contains(pk):" + _CloudActivePortKeys.Contains(pk) + "<=======");
                                DebugEx.TraceVerbose("Display Cloud Active PortKeys");
                                foreach (var pkey in _CloudActivePortKeys)
                                    DebugEx.TraceVerbose("=====>ActivePortKey:" + pkey.ToString() + "<=======");
                                //check if this port is active in a cloud graph
                                if (IgnoreActivePortKeys || _CloudActivePortKeys.Contains(pk))
                                {
                                    //create list if null
                                    if (events == null)
                                        events = new List<PortEvent>();
                                    //add to event list
                                    events.Add(new PortEvent(pk, state.Item2));
                                }
                            }
                        }
                    }
                }
                //send message
                if (events != null && events.Count > 0)
                {
                    DebugEx.TraceLog("========>Actual Set State<====== " + events[0].State);
                    var msg = new Yodiwo.API.Plegma.PortEventMsg(0);
                    events.Where(e => e.Timestamp == 0).ForEach(e => e.Timestamp = DateTime.UtcNow.ToUnixMilli());
                    msg.PortEvents = events.ToArray();
                    SendMessage(msg);
                }
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught"); }
        }

        //------------------------------------------------------------------------------------------------------------------------

        /// <summary> Ping server. Returns the request Round-Trip-Time. If failed, it will return Timespan.Zero </summary>
        public TimeSpan Ping(int cnt = 123)
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
                //create groupping dictionary (if necessary)
                var thingGroupped_module = _NodeModules.Count > 0 ? new Dictionary<ThingKey, List<TupleS<PortKey, string>>>() : null;
                var thingGroupped_user = OnChangedStateGroupped != null || ThingEventHandlers.Count > 0 ? new Dictionary<Thing, List<TupleS<Port, string>>>() : null;

                //raise events
                foreach (var portevent in msg.PortEvents)
                {
                    var pk = (PortKey)portevent.PortKey;
                    var thingKey = pk.ThingKey;
                    var thing = _Things.TryGetOrDefault(thingKey);
                    var port = thing?.GetPort(pk);

                    //port not found?
                    if (thing == null || port == null)
                        continue;

                    //add to groups1
                    if (thingGroupped_module != null)
                    {
                        var group1 = thingGroupped_module.TryGetOrDefault(thingKey);
                        if (group1 == null)
                            thingGroupped_module.Add(thingKey, group1 = new List<TupleS<PortKey, string>>());
                        group1.Add(TupleS.Create(pk, portevent.State));
                    }

                    //add to groups2
                    if (thingGroupped_user != null)
                    {
                        var group2 = thingGroupped_user.TryGetOrDefault(thing);
                        if (group2 == null)
                            thingGroupped_user.Add(thing, group2 = new List<TupleS<Port, string>>());
                        group2.Add(TupleS.Create(port, portevent.State));
                    }

                    //update state
                    port.State = portevent.State;

                    //raise generic event
                    try { OnChangedState?.Invoke(thing, port, portevent.State, isEvent: true); } catch (Exception ex) { DebugEx.TraceError(ex, "UserCode exception caught"); }

                    //raise port handlers
                    DebugEx.TraceLog("On Changed State:" + portevent.State.Substring(0, Math.Min(portevent.State.Length, 80)));
                    try { PortEventHandlers.TryGetOrDefault(port)?.Invoke(portevent.State, true); }
                    catch (Exception ex) { DebugEx.TraceError(ex, "PortEventHandlers failed"); }

                    //send to module (wait for up to 1 sec before movong on..)
                    Task.Run(() => { try { thingkey2module.TryGetOrDefault(thing.ThingKey)?.SetThingsState(port.PortKey, portevent.State, true); } catch { } }).Wait(1000);
                }

                //send groupped to user
                if (thingGroupped_user != null)
                    foreach (var entry in thingGroupped_user)
                    {
                        //raise thing grouped events
                        ThingEventHandlers.TryGetOrDefault(entry.Key)?.Invoke(entry.Value.ToArray(), true);
                        //raise generic event
                        try { OnChangedStateGroupped?.Invoke(entry.Key, entry.Value); } catch (Exception ex) { DebugEx.TraceError(ex, "UserCode exception caught"); }
                    }

                //send groupped to module
                if (thingGroupped_module != null)
                    foreach (var entry in thingGroupped_module)
                        thingkey2module.TryGetOrDefault(entry.Key)?.SetThingsState(entry.Key, entry.Value, true);

                //send to modules
                {
                    var modules = new HashSet<INodeModule>();
                    foreach (var ev in msg.PortEvents)
                    {
                        var module = thingkey2module.TryGetOrDefault(((PortKey)ev.PortKey).ThingKey);
                        if (module != null)
                            modules.Add(module);
                    }
                    foreach (var module in modules)
                        Task.Run(() =>
                        {
                            try { module.OnPortEvent(msg); }
                            catch (Exception ex) { DebugEx.TraceError(ex, "NodeModule OnPortEventProcessed failed"); }
                        }).Wait(1000);
                }

                //other processing
                try { OnPortEventMsgProcessed?.Invoke(msg); }
                catch (Exception ex) { DebugEx.TraceError(ex, "OnPortEventMsgProcessed failed"); }
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught"); }
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

        public IEnumerable<Thing> GetAllThings(bool forceUpdateFromCloud = false)
        {
            try
            {
                if (forceUpdateFromCloud)
                {
                    var req = new ThingsGet()
                    {
                        Operation = eThingsOperation.Get,
                        Key = null, //all things
                                    //RevNum = 0
                    };
                    var rsp = SendRequest<ThingsSet>(req);
                    if (rsp != null && rsp.Status)
                    {
                        if (rsp.Operation == eThingsOperation.Update)
                        {
                            foreach (var t in rsp.Data)
                            {
                                _Things.ForceAdd(t.ThingKey, t);
                                var moduleId = ((ThingKey)t.ThingKey).NodeModule;
                                INodeModule owner = null;
                                if (!string.IsNullOrEmpty(moduleId))
                                    owner = _NodeModules.FirstOrDefault(m => m.ModuleID == moduleId);
                                if (owner != null)
                                {
                                    _NodeModuleThings.TryGetOrDefault(owner)?.Add(t);
                                    thingkey2module.Add(t.ThingKey, owner);
                                }
                            }
                            DebugEx.TraceLog($"Successful update of things {string.Join(",", rsp.Data.Select(t => t.ThingKey))}");
                        }
                        else if (rsp.Operation == eThingsOperation.Overwrite)
                        {
                            _Things.Clear();
                            _Things.AddFromSource(rsp.Data.ToDictionary(t => (ThingKey)t.ThingKey, t => t));
                            DebugEx.TraceLog($"Successful overwrote existing things with: {string.Join(",", rsp.Data.Select(t => t.ThingKey))}");
                        }
                    }
                    else
                        DebugEx.TraceError($"Update of all things from cloud failed");
                }
                return this._Things.Values;
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception caught");
                return Enumerable.Empty<Thing>();
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        private void OnConnectedToCloud(Transport transport, string message)
        {
            try
            {
                DebugEx.TraceLog($"Connect to cloud via {transport}");
                //inform node manager
                if (NodeGraphManager != null)
                {
                    DebugEx.TraceLog($"Inform node manager that {message}");
                    NodeGraphManager.OnConnectedToCloud();
                }
                //inform user
                Task.Run(() => { try { OnTransportConnected?.Invoke(transport, message); } catch (Exception ex) { DebugEx.TraceError(ex, "UserCode exception caught"); } }).Wait(500);

                //inform modules
                var tasks = new List<Task>();
                foreach (var module in NodeModules)
                    tasks.Add(Task.Run(() =>
                    {
                        try { module.OnTransportConnected(message); }
                        catch (Exception ex) { DebugEx.TraceError(ex, "NodeModule OnTransportConnected failed for module " + module.Name); }
                    }));
                //wait for a sec
                Task.WaitAll(tasks.ToArray(), 1000);
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught"); }
        }

        //------------------------------------------------------------------------------------------------------------------------

        private void OnDisconnectedFromCloud(Transport transport, string message)
        {
            try
            {
                IsLinkUp = false;

                //inform node manager
                if (NodeGraphManager != null)
                    NodeGraphManager.OnDisconnectedFromCloud();

                //inform user
                Task.Run(() => { try { OnTransportDisconnected?.Invoke(transport, message); } catch (Exception ex) { DebugEx.TraceError(ex, "UserCode exception caught"); } }).Wait(500);

                //inform modules
                var tasks = new List<Task>();
                foreach (var module in NodeModules)
                    tasks.Add(Task.Run(() =>
                    {
                        try { module.OnTransportDisconnected(message); }
                        catch (Exception ex) { DebugEx.TraceError(ex, "NodeModule OnTransportConnected failed for module " + module.Name); }
                    }));
                //wait for a sec
                Task.WaitAll(tasks.ToArray(), 1000);
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught"); }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public bool Connect()
        {
            //set flag
            _IsConnectionEnabled = true;
            return _connect(false);
        }

        bool _connect(bool isReconnection)
        {
            try
            {
                //check keys
                if (!_IsPaired || this.NodeKey.IsInvalid || this.NodeSecret == null)
                {
                    DebugEx.TraceError("Connection request while node is not paired");
                    return false;
                }

                //stop auto-reconnection
                if (isReconnection && !IsConnectionEnabled)
                    return false;

                //stop auto-reconnection
                if (isReconnection && !AutoReconnect)
                    return false;

                //inform user
                try { OnTransportConnectionStart?.Invoke(this._Transport); } catch { }

                //connect YPChannel?
                if (this._Transport == Transport.YPCHANNEL)
                {
                    var ret = _YPChannelConnectWithWorker(this.conf.YpServer, this.conf.YpchannelPort, this.conf.SecureYpc);
                    if (ret.IsSuccessful)
                    {
                        OnConnectedToCloud(Transport.YPCHANNEL, ret.Message);
                    }
                    else
                    {
                        DebugEx.TraceError("Could not connect YPChannel (Message:" + ret.Message + ")");
                        Task.Run(() => { try { OnTransportError?.Invoke(Transport.YPCHANNEL, TransportErrors.ConnectionFailed, ret.Message); } catch (Exception ex) { DebugEx.TraceError(ex, "UserCode exception caught"); } });
                    }
                    return ret;
                }
                //connect Mqtt?
                else if (this._Transport == Transport.MQTT)
                {
                    var ret = mqtthandler.ConnectWithWorker(this.conf.MqttBrokerHostname, this.conf.MqttUseSsl);
                    if (ret.IsSuccessful)
                    {
                        OnConnectedToCloud(Transport.MQTT, ret.Message);
                    }
                    else
                    {
                        DebugEx.TraceError("Could not connect to MQTT (Message:" + ret.Message + ")");
                        Task.Run(() => { try { OnTransportError?.Invoke(Transport.MQTT, TransportErrors.ConnectionFailed, ret.Message); } catch (Exception ex) { DebugEx.TraceError(ex, "UserCode exception caught"); } });
                    }
                    return ret;
                }
                //"connect"(enable) Rest?
                else if (this._Transport == Transport.REST)
                {
                    var ret = _RestConnectWithWorker();
                    if (ret.IsSuccessful)
                    {
                        OnConnectedToCloud(Transport.REST, ret.Message);
                    }
                    else
                    {
                        DebugEx.TraceError("Could not connect REST (Message:" + ret.Message + ")");
                        Task.Run(() => { try { OnTransportError?.Invoke(Transport.REST, TransportErrors.ConnectionFailed, ret.Message); } catch (Exception ex) { DebugEx.TraceError(ex, "UserCode exception caught"); } });
                    }
                    return ret;
                }
                else
                    return false;
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught"); return false; }
        }

        //------------------------------------------------------------------------------------------------------------------------

        void _reConnectWithWorker()
        {
            if (AutoReconnect && _IsConnectionEnabled)
            {
                Task.Delay((int)AutoReconnectDelay.TotalMilliseconds).ContinueWith(_ =>
                {
                    try { _connect(true); } catch (Exception ex) { DebugEx.TraceError(ex, "Reconnection exception caught"); }
                });

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
                    Channel.Close("Node disconnection");
                    Channel = null;
                }
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught"); }
        }

        //------------------------------------------------------------------------------------------------------------------------

        private void _TriggerLinkActivatedCallback()
        {
            Task.Run(() => { try { OnLinkActivated?.Invoke(); } catch (Exception ex) { DebugEx.TraceError(ex, "UserCode exception caught"); } });

            foreach (var module in NodeModules)
                TaskEx.RunSafe(module.OnLinkActivated);
        }

        //------------------------------------------------------------------------------------------------------------------------

        public bool SendMessage(API.Plegma.PlegmaApiMsg msg)
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
                if (this._Transport == Transport.YPCHANNEL)
                    return __ypc.SendMessage(msg);
                else if (this._Transport == Transport.MQTT)
                {
                    __mqtt.SendMessage(msg);
                    return true;
                }
                else if (this._Transport == Transport.REST)
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

        public TResponse SendRequest<TResponse>(API.ApiMsg msg)
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
        public PlegmaApiMsg HandleApiReq(object msg, uint requestID)
        {
            try
            {
                DebugEx.TraceLog($"Node just msg of type {msg.GetType()}");
                if (msg is Yodiwo.API.Plegma.LoginReq)
                {
                    var rsp = new LoginRsp()
                    {
                        NodeKey = this.NodeKey,
                        SecretKey = this.NodeSecret.SecureStringToString(),
                    };
                    return rsp;
                }
                else if (msg is Yodiwo.API.Plegma.NodeInfoReq)
                {
                    var internalIP = Yodiwo.Tools.Network.FindLocalIpAddress();
                    var foundIP = !string.IsNullOrEmpty(internalIP) && internalIP != "127.0.0.1";

                    NodeInfoRsp rsp = new NodeInfoRsp()
                    {
                        Name = conf.Name,
                        Type = (nodeType == eNodeType.Unknown) ? eNodeType.Generic : nodeType,
                        Capabilities = eNodeCapa.None |
                                       (CanSolveGraphs ? eNodeCapa.SupportsGraphSolving : eNodeCapa.None) |
                                       (IsWarlock ? eNodeCapa.IsWarlock : eNodeCapa.None) |
                                       (IsShellNode ? eNodeCapa.IsShellNode : eNodeCapa.None),
                        ThingTypes = (this._thingTypes == null) ? null : this._thingTypes.ToArray(),
                        BlockLibraries = NodeGraphManager != null && CanSolveGraphs ? NodeGraphManager.BlockLibrariesNames : null,
                        ThingsRevNum = 0,
                        TransientInfo = foundIP ? new Dictionary<string, string>() { { Constants.IntIpAddressKey, internalIP } } : null,
                        SupportedApiRev = 1
                    };
                    var tmp = rsp.ToJSON();

                    //link up event
                    IsLinkUp = true;
                    _TriggerLinkActivatedCallback();

                    return rsp;
                }
                else if (msg is NodeUnpairedReq)
                {
                    DebugEx.TraceLog("NodeUnpairedReq just received!!!! Unpair node");
                    var m = msg as NodeUnpairedReq;
                    //invoke event
                    Task.Run(() => { try { OnNodeUnpaired?.Invoke(m.ReasonCode, m.Message); } catch (Exception ex) { DebugEx.TraceError(ex, "UserCode exception caught"); } });
                    //just return simple rsp, acknowledging the unpairing
                    Channel?.SendResponse(new GenericRsp() { IsSuccess = true }, requestID);
                    //forget and forgive
                    ForgetMe();
                    return null;
                }
                else if (msg is Yodiwo.API.Plegma.ThingsGet)
                {
                    var msgTyped = msg as ThingsGet;
                    var op = msgTyped.Operation;
                    var tkey = (ThingKey)msgTyped.Key;

                    var rsp = new ThingsSet() { Operation = eThingsOperation.Invalid };

                    DebugEx.TraceLog($"Msg of type Yodiwo.API.Plegma.ThingsGet recvd");

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
                        else if (string.IsNullOrEmpty(msgTyped.Key))
                        {
                            if (this._Things != null)
                            {
                                rsp.Operation = eThingsOperation.Update;
                                rsp.Status = true;
                                rsp.Data = this._Things.Values.ToArray();
                                try { OnThingsRegistered?.Invoke(); } catch (Exception ex) { DebugEx.TraceError(ex, "UserCode exception caught"); }
                            }
                        }
                    }
                    else if (op == eThingsOperation.Scan)
                    {
                        //raise event?
                        if (OnThingScanRequest == null || _NodeModules.Count == 0)
                            return rsp;
                        else
                        {
                            var things = new List<Thing>();
                            //add from event
                            try { things.AddFromSource(OnThingScanRequest?.Invoke(tkey)); } catch (Exception ex) { DebugEx.TraceError(ex, "UserCode exception caught"); }
                            //add from modules
                            if (tkey.IsInvalid)
                                foreach (var module in _NodeModules)
                                    try { things.AddFromSource(module.Scan(default(ThingKey))); } catch (Exception ex) { DebugEx.TraceError(ex, "UserCode exception caught"); }
                            else
                                try { things.AddFromSource(thingkey2module.TryGetOrDefault(tkey)?.Scan(tkey)); } catch (Exception ex) { DebugEx.TraceError(ex, "UserCode exception caught"); }
                            //return result
                            rsp.Operation = eThingsOperation.Update;
                            rsp.Data = things.ToArray();
                            rsp.Status = true;
                        }
                    }
                    else if (op == eThingsOperation.Sync)
                    {
                    }

                    //return response
                    return rsp;
                }
                else if (msg is Yodiwo.API.Plegma.ThingsSet)
                {
                    var msgTyped = msg as ThingsSet;
                    var op = msgTyped.Operation;
                    var msgThings = msgTyped.Data;

                    var rsp = new GenericRsp()
                    {
                        IsSuccess = false
                    };

                    if (op == eThingsOperation.Delete)
                    {
                        if (msgThings != null)
                        {
                            bool hadError = false;
                            foreach (var thing in msgThings)
                            {
                                //remove from module
                                var module = thingkey2module.TryGetOrDefault(thing.ThingKey);
                                bool? modres = null;
                                try { modres = module?.Delete(new[] { (ThingKey)thing.ThingKey }); } catch (Exception ex) { modres = false; DebugEx.TraceError(ex, "UserCode exception caught"); }
                                if (modres == true && module != null)
                                {
                                    //remove from lookups
                                    _NodeModuleThings.TryGetOrDefault(module)?.Remove(thing);
                                    thingkey2module.Remove(thing.ThingKey);
                                }
                                //do callback
                                bool? eventres = null;
                                try { eventres = OnThingDeleteRequest?.Invoke(thing); } catch (Exception ex) { eventres = false; DebugEx.TraceError(ex, "UserCode exception caught"); }
                                //respond
                                if (modres != false && eventres != false)
                                    //remove thing from nodelib
                                    RemoveThing(thing.ThingKey, SendToCloud: false);
                                else
                                    hadError = true;
                            }
                            //fill response
                            rsp.IsSuccess = !hadError;
                        }
                        else
                            return rsp;
                    }
                    else if (op == eThingsOperation.Update)
                    {
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
                                Thing before = null;
                                var thing = Things.TryGetOrDefaultReadOnly(tk);
                                if (thing == null)
                                {
                                    //create an orphan
                                    thing = msgThing;
                                    _Things.Add(tk, thing);
                                }
                                else
                                {
                                    //create a "previous" copy backup
                                    before = thing.DeepClone();
                                    //update thing
                                    thing.Update(msgThing);
                                    //create a portstate update (implicitr portstate message)
                                    HandlePortStateSet(new PortStateSet()
                                    {
                                        Operation = ePortStateOperation.SpecificKeys,
                                        PortStates = thing.Ports.Select(p => new PortState()
                                        {
                                            IsDeployed = IsPortActive(p.PortKey),
                                            PortKey = p.PortKey,
                                            RevNum = p.RevNum,
                                            State = p.State
                                        }).ToArray(),
                                    });
                                }
                                //inform module
                                try { thingkey2module.TryGetOrDefault(tk)?.OnUpdateThing(thing, before); } catch (Exception ex) { DebugEx.TraceError(ex, "UserCode exception caught"); }
                                //raise event
                                try { OnThingUpdated?.Invoke(thing, before); } catch (Exception ex) { DebugEx.TraceError(ex, "UserCode exception caught"); }
                            }
                            //fill response
                            rsp.IsSuccess = !hadError;
                        }
                    }
                    //return response
                    return rsp;
                }
                else if (msg is Yodiwo.API.Plegma.PortStateGet)
                {
                    var msgRx = msg as PortStateGet;
                    var states = HandlePortStateReq(msgRx);
                    var rsp = new PortStateSet()
                    {
                        Operation = (msg as PortStateGet).Operation,
                        PortStates = states
                    };
                    return rsp;
                }
                else if (msg is Yodiwo.API.Plegma.PortStateSet)
                {
                    var msgRx = msg as PortStateSet;
                    HandlePortStateSet(msgRx);
                    var rsp = new GenericRsp();
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
                    var ev = OnUnexpectedRequest;
                    if (ev == null)
                        return null;
                    else
                    {
                        try
                        {
                            PlegmaApiMsg ret;
                            var dels = ev.GetInvocationList();
                            foreach (var del in dels)
                            {
                                try
                                {
                                    ret = del.DynamicInvoke(msg) as PlegmaApiMsg;
                                    if (ret != null)
                                        return ret;
                                }
                                catch (Exception ex) { DebugEx.TraceError(ex, "UserCode exception caught"); continue; }
                            }
                        }
                        catch { }
                        //failed
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception in node HandleApiReq()");
                return null;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        private void HandlePortStateSet(PortStateSet msg)
        {
            try
            {
                //create groupping dictionary (if necessary)
                var thingGroupped_module = _NodeModules.Count > 0 ? new Dictionary<ThingKey, List<TupleS<PortKey, string>>>() : null;
                var thingGroupped_user = OnChangedStateGroupped != null || ThingEventHandlers.Count > 0 ? new Dictionary<Thing, List<TupleS<Port, string>>>() : null;

                //raise events
                foreach (var portstate in msg.PortStates)
                {
                    var pk = (PortKey)portstate.PortKey;
                    var thingKey = pk.ThingKey;
                    var thing = _Things.TryGetOrDefault(thingKey);
                    var port = thing?.GetPort(pk);

                    //port not found?
                    if (thing == null || port == null)
                        continue;

                    //add to groups1
                    if (thingGroupped_module != null)
                    {
                        var group1 = thingGroupped_module.TryGetOrDefault(thingKey);
                        if (group1 == null)
                            thingGroupped_module.Add(thingKey, group1 = new List<TupleS<PortKey, string>>());
                        group1.Add(TupleS.Create(pk, portstate.State));
                    }

                    //add to groups2
                    if (thingGroupped_user != null)
                    {
                        var group2 = thingGroupped_user.TryGetOrDefault(thing);
                        if (group2 == null)
                            thingGroupped_user.Add(thing, group2 = new List<TupleS<Port, string>>());
                        group2.Add(TupleS.Create(port, portstate.State));
                    }

                    //update state
                    port.State = portstate.State;

                    //raise generic event
                    try { OnChangedState?.Invoke(thing, port, portstate.State, isEvent: false); } catch (Exception ex) { DebugEx.TraceError(ex, "UserCode exception caught"); }

                    //raise port handlers
                    DebugEx.TraceLog("On Changed State:" + portstate.State.Substring(0, Math.Min(portstate.State.Length, 80)));
                    try { PortEventHandlers.TryGetOrDefault(port)?.Invoke(portstate.State, false); }
                    catch (Exception ex) { DebugEx.TraceError(ex, "PortEventHandlers failed"); }

                    //send to module (wait for up to 1 sec before movong on..)
                    Task.Run(() => { try { thingkey2module.TryGetOrDefault(thing.ThingKey)?.SetThingsState(port.PortKey, portstate.State, false); } catch { } }).Wait(1000);
                }

                //send groupped to user
                if (thingGroupped_user != null)
                    foreach (var entry in thingGroupped_user)
                    {
                        //raise thing grouped events
                        ThingEventHandlers.TryGetOrDefault(entry.Key)?.Invoke(entry.Value.ToArray(), false);
                        //raise generic event
                        try { OnChangedStateGroupped?.Invoke(entry.Key, entry.Value); } catch (Exception ex) { DebugEx.TraceError(ex, "UserCode exception caught"); }
                    }

                //send groupped to module
                if (thingGroupped_module != null)
                    foreach (var entry in thingGroupped_module)
                        thingkey2module.TryGetOrDefault(entry.Key)?.SetThingsState(entry.Key, entry.Value, false);
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught"); }
        }

        //------------------------------------------------------------------------------------------------------------------------

        private PortState[] HandlePortStateReq(PortStateGet req)
        {
            try
            {
                ListTS<PortState> portstates = new ListTS<PortState>();
                if (req.Operation == ePortStateOperation.AllPortStates)
                {
                    portstates.AddFromSource(_Things.Values.SelectMany(thing =>
                        thing.Ports.Select(port =>
                        new PortState()
                        {
                            PortKey = port.PortKey,
                            RevNum = port.RevNum,
                            State = port.State,
                            IsDeployed = IsPortActive(port.PortKey),
                        })));
                }
                else
                {
                    if (req.PortKeys != null)
                        foreach (var pk in req.PortKeys)
                        {
                            //get key
                            var portkey = (PortKey)pk;
                            if (portkey.IsInvalid)
                                continue;

                            //find port
                            var port = this._Things.TryGetOrDefault(portkey.ThingKey)?.Ports.Find(i => i.PortKey == portkey);
                            if (port == null)
                                continue;

                            //add to states
                            var portState = new PortState()
                            {
                                PortKey = pk,
                                RevNum = port.RevNum,
                                State = port.State,
                                IsDeployed = IsPortActive(portkey),
                            };
                            portstates.Add(portState);
                        }
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
                    {
                        try { OnPortDeactivated?.Invoke(pkey); }
                        catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught in user code for OnPortDeactivated (PortID=" + pkey.PortUID + ")"); }
                        //inform module
                        TaskEx.RunSafe(() => thingkey2module.TryGetOrDefault(pkey.ThingKey)?.OnPortDeactivated(pkey));
                    }

                //inform PORTS for activations
                foreach (var pkey in newActivePortKeys)
                    if (!prevActivePortKeys.Contains(pkey))
                    {
                        try { OnPortActivated?.Invoke(pkey); }
                        catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught in user code for OnPortActivated (PortID=" + pkey.PortUID + ")"); }
                        //inform module
                        TaskEx.RunSafe(() => thingkey2module.TryGetOrDefault(pkey.ThingKey)?.OnPortActivated(pkey));
                    }

                //inform THINGS for deactivations
                foreach (var thing in prevActiveThings)
                    if (!newActiveThings.Contains(thing))
                    {
                        try { OnThingDeactivated?.Invoke(thing); }
                        catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught in user code for OnThingDeactivated (ThingName=" + thing.Name + ")"); }
                        //inform module
                        TaskEx.RunSafe(() => thingkey2module.TryGetOrDefault(thing.ThingKey)?.OnThingDeactivated(thing.ThingKey));
                    }

                //inform THINGS for activation
                foreach (var thing in newActiveThings)
                    if (!prevActiveThings.Contains(thing))
                    {
                        try { OnThingActivated?.Invoke(thing); }
                        catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught in user code for OnThingActivated (ThingName=" + thing.Name + ")"); }
                        //inform module
                        TaskEx.RunSafe(() => thingkey2module.TryGetOrDefault(thing.ThingKey)?.OnThingActivated(thing.ThingKey));
                    }

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
            if (msg is Yodiwo.API.Plegma.PlegmaApiMsg)
                HandlePlegmaApiMsg(msg);
            else if (msg is Yodiwo.API.Warlock.WarlockApiMsg)
                HandleWarlockApiMsg(msg);
            else
                try { OnUnexpectedMessage?.Invoke(msg); } catch (Exception ex) { DebugEx.TraceError(ex, "UserCode exception caught"); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void HandleWarlockApiMsg(object msg)
        {
            try
            {
                OnReceivedWarlockApiMsg?.Invoke(msg as Yodiwo.API.Warlock.WarlockApiMsg);
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception in HandleWarlockApiMsg()");
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void HandlePlegmaApiMsg(object msg)
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
                    if (rsp.ActivePortKeys.Length > 0)
                    {
                        foreach (var activepkey in rsp.ActivePortKeys)
                            DebugEx.TraceLog("=====>Node just got ActivePortKeysMsg for portkey:" + activepkey);
                    }
                    else
                        DebugEx.TraceLog("Empty ActivePortKeys");

                    //finish
                    EndActiveThingsUpdate(sets);
                }
                else if (msg is PortEventMsg)
                {
                    HandlePortEventMsg(msg as Yodiwo.API.Plegma.PortEventMsg);
                }
                else if (msg is VirtualBlockEventMsg)
                {
                    //forward to node graph mngr
                    NodeGraphManager?.HandleIncomingVirtualBlockEventMsg(msg as VirtualBlockEventMsg);
                }
                else
                {
                    try { OnUnexpectedMessage?.Invoke(msg); } catch (Exception ex) { DebugEx.TraceError(ex, "UserCode exception caught"); }
                }
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception in node HandlePlegmaApiMsg()"); }
        }

        //------------------------------------------------------------------------------------------------------------------------

        #region YPChannel
        //------------------------------------------------------------------------------------------------------------------------
        bool _ypcConnecting = false;

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
                        if (Channel != null) //) && (Channel.RemoteHost != ypserver || Channel.RemotePort != port.ToStringInvariant()))
                        {
                            try
                            {
                                Channel.OnMessageReceived -= Channel_OnMessageReceived;
                                Channel.OnClosedEvent -= Channel_OnClosedEvent;
                                try { Channel.Close("Node disconnection (2)"); } catch { }
                                try { Channel.Dispose(); } catch { }
                            }
                            catch { }
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
                                    new YPChannel.Protocol.MessageTypeGroup() { GroupName = API.Plegma.PlegmaAPI.ApiGroupName,      MessageTypes = API.Plegma.PlegmaAPI.ApiMessages      },
                                    new YPChannel.Protocol.MessageTypeGroup() { GroupName = API.Plegma.PlegmaAPI.ApiLogicGroupName, MessageTypes = API.Plegma.PlegmaAPI.LogicApiMessages },
                                    new YPChannel.Protocol.MessageTypeGroup() { GroupName = API.MediaStreaming.Video.ApiGroupName,  MessageTypes = API.MediaStreaming.Video.ApiMessages  },
                                    new YPChannel.Protocol.MessageTypeGroup() { GroupName = API.MediaStreaming.Audio.ApiGroupName,  MessageTypes = API.MediaStreaming.Audio.ApiMessages  },
                                    new YPChannel.Protocol.MessageTypeGroup() { GroupName = API.Warlock.WarlockAPI.ApiGroupName,    MessageTypes = API.Warlock.WarlockAPI.ApiMessages    },
                                },
                            };
                            //create channel
                            var supportedPackers = ChannelSerializationMode.Json;
                            var preferredPackers = ChannelSerializationMode.Json;
                            Channel = new Yodiwo.YPChannel.Transport.Sockets.Client(proto, SupportedChannelSerializationModes: supportedPackers, PreferredChannelSerializationModes: preferredPackers, keepAliveSpinDelay: this.keepAliveSpinDelay);
                            Channel.NoDelay = true; //Disable Nagle Algorithm since we care about latency more than throughput
                            Channel.Name = conf.Name;
                            Channel.OnMessageReceived += Channel_OnMessageReceived;
                            Channel.OnClosedEvent += Channel_OnClosedEvent;
                            Channel.KeepAliveSpinDelay = TimeSpan.FromSeconds(30);
                            NewChannelSetup?.Invoke(Channel); //user setup
                        }

                        //close existing channel
                        if (Channel.IsOpen)
                            try { Channel.Close("Node disconnection (3)"); } catch { }

                        //connect
                        _ypcConnecting = true;
                        try
                        {
                            var res = Channel.Connect(ypserver, port, isSecure, conf.CertificationServerName);
                            return res;
                        }
                        finally { _ypcConnecting = false; }
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

        private void Channel_OnClosedEvent(Channel Channel, string Message)
        {
            try
            {
                if (!_ypcConnecting)
                {
                    //inform
                    OnDisconnectedFromCloud(Transport.YPCHANNEL, Message);
                }

                if (AutoReconnect)
                    _reConnectWithWorker();
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught"); }
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
                    var rsp = HandleApiReq(api_msg, Message.MessageID);
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
            catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught"); }
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

                var res = Yodiwo.Tools.Http.RequestPost(restRoute, target_msg.Item2, HttpRequestDataFormat.Json);
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
            if (key.IsInvalid)
                return false;

            //check cloud
            if (_CloudActivePortKeys?.Contains(key) == true)
                return true;

            //check local
            if (NodeGraphManager?.IsPortActive(key) == true)
                return true;

            return false;
        }

        //------------------------------------------------------------------------------------------------------------------------

        public bool IsThingActive(ThingKey key)
        {
            if (key.IsInvalid)
                return false;

            var thing = _Things.TryGetOrDefault(key);
            if (thing == null)
                return false;

            //check cloud
            if (_CloudActiveThings?.Contains(thing) == true)
                return true;

            //check local
            if (NodeGraphManager?.IsThingActive(key) == true)
                return true;

            return false;
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void RequestStateUpdate(bool OnlyActivePorts = false)
        {
            try
            {
                //fetch latest states from server
                var req = new PortStateGet()
                {
                    Operation = OnlyActivePorts ? ePortStateOperation.ActivePortStates : ePortStateOperation.AllPortStates,
                };
                var rsp = SendRequest<PortStateSet>(req);
                if (rsp != null)
                {
                    //handle state update
                    HandlePortStateSet(rsp);
                }
                else
                    DebugEx.Assert("Null response when trying to decide port states");
            }
            catch (Exception ex) { DebugEx.TraceError(ex, "Error while requesting state updates"); }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void AddModule(INodeModule module)
        {
            try
            {
                lock (this._NodeModules)
                {
                    //add to modules
                    this._NodeModules.Add(module);

                    //set nodekey
                    if (NodeKey.IsValid)
                        try { module.NodeKey = NodeKey; } catch (Exception ex) { DebugEx.Assert(ex, "NodeModule exception while trying to set key"); }

                    //register node hooks
                    module.OnNodeModuleData1 = (p, s) => _setStateFromModule(module, p, s);
                    module.OnNodeModuleData2 = (s) => _setStateFromModule(module, s);
                    module.OnNodeModuleData3 = (p, s) => _setStateMacroFromModule(module, p, s, true);
                    module.OnNodeModuleData4 = (s) => _setStateMacroFromModule(module, s, true);
                    module.OnNodeModuleThingAdd = _moduleOnThingAddHandler;
                    module.OnNodeModuleThingDelete = _moduleOnThingDeleteHandler;
                    module.OnNodeModuleThingUpdate = _moduleOnThingUpdateHandler;

                    //init thing dictionary
                    _NodeModuleThings.Add(module, new HashSetTS<Thing>());

                    //add types from modules
                    this._thingTypes.AddFromSource(module.EnumerateThingTypes());
                }
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught"); }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void RemoveModule(INodeModule module, bool sendToCloud = true)
        {
            try
            {
                lock (this._NodeModules)
                {
                    //add to modules
                    if (this._NodeModules.Remove(module))
                    {
                        //remove hooks from node
                        module.OnNodeModuleData1 = null;
                        module.OnNodeModuleData2 = null;
                        module.OnNodeModuleData3 = null;
                        module.OnNodeModuleData4 = null;
                        module.OnNodeModuleThingAdd = null;
                        module.OnNodeModuleThingDelete = null;
                        module.OnNodeModuleThingUpdate = null;

                        //enumerate things and keep for quick lookup
                        var modThings = _NodeModuleThings[module];
                        _NodeModuleThings.Remove(module);

                        //Add module things
                        foreach (var t in modThings)
                            RemoveThing(t.ThingKey, SendToCloud: sendToCloud, owner: module);
                    }
                }
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught"); }
        }

        //------------------------------------------------------------------------------------------------------------------------

        void _moduleOnThingAddHandler(INodeModule module, IEnumerable<Thing> things)
        {
            if (things != null)
                foreach (var tk in things)
                    try { AddThing(tk, module); } catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception"); }
        }

        void _moduleOnThingDeleteHandler(INodeModule module, IEnumerable<ThingKey> things)
        {
            if (things != null)
                foreach (var tk in things)
                    try { RemoveThing(tk, module); } catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception"); }
        }

        void _moduleOnThingUpdateHandler(INodeModule module, IEnumerable<Thing> things, bool sendToCloud = true)
        {
            if (things != null)
                foreach (var tk in things)
                    try { UpdateThing(tk, module, sendToCloud); } catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception"); }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void ForgetMe(bool disconnect = true, bool purge = true)
        {
            try
            {
                //forget keys
                this._IsPaired = false;
                this.NodeKey = default(NodeKey);
                this.NodeSecret = null;

                //disconnect channels
                if (disconnect)
                    Disconnect();

                //purge?
                if (purge)
                    try { Purge(); } catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught in Purge()"); }

                //invoke event
                try { OnForgetMeCb?.Invoke(); } catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught OnForgetMeCb"); }
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught"); }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void Unpair(bool disconnect = false, bool purge = true)
        {
            try
            {
                DebugEx.TraceLog($"NodeUnpairedReq event is send");
                var unpairReq = new NodeUnpairedReq() { ReasonCode = eUnpairReason.UserRequested };
                var rsp = SendRequest<GenericRsp>(unpairReq);
                ForgetMe(disconnect: disconnect, purge: purge);
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught"); }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void Purge()
        {
            try
            {
                //purge graph manager
                try { NodeGraphManager?.Purge(); } catch (Exception ex) { DebugEx.Assert(ex); }

                //Purge saved information
                try { DataSave?.Invoke(DataIdentifier_Things, new byte[0], true); } catch (Exception ex) { DebugEx.Assert(ex, "Thing purge failed"); }

                //Purge nodekeys
                try { OnNodePaired?.Invoke(default(NodeKey), "".ToSecureString()); } catch (Exception ex) { DebugEx.TraceError(ex, "Node keys purge failed"); }

                //purge modules
                foreach (var module in NodeModules)
                    try { module.Purge(); } catch (Exception ex) { DebugEx.TraceError(ex, "UserCode exception caught"); }
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception caught"); }
        }

        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}


