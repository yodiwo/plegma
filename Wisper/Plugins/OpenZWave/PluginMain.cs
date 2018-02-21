using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yodiwo;
using Yodiwo.API.Plegma;
using Yodiwo.mNode.Plugins.UI.ContextMenu;
using Yodiwo.mNode.Plugins.UI.Forms;
using Yodiwo.mNode.Plugins.UI.Forms.Controls;

namespace Yodiwo.mNode.Plugins.Bridge_OZWave
{
    public class PluginMain : Plugin
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        Dictionary<string, Port> ZWaveThingPorts = new Dictionary<string, Port>();
        HashSetTS<string> NodeIds = new HashSetTS<string>();
        DictionaryTS<Byte, ZWaveDescriptor> ZWaveDevices = new DictionaryTS<Byte, ZWaveDescriptor>();
        public ZWaveDevicesConfig PluginCfg;
        private YConfig<ZwavePluginConfig> YConfig;
        public ZwavePluginConfig ActiveCfg;
        private AutoResetEvent _isDriverReady = new AutoResetEvent(false);
        Thread CommandRetrieverThread;
        bool isRunning;
        UInt32 homeID;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public override bool Initialize(mNodeConfig mNodeConfig, string PluginConfig, string UserConfig)
        {
            #region Init Plugin
            YConfig = Config.Init(PluginWorkingDirectory);
            ActiveCfg = YConfig.GetActiveConf();

            // plugin config
            if (string.IsNullOrEmpty(PluginConfig))
                PluginCfg = new ZWaveDevicesConfig();
            else
            {
                DebugEx.TraceLog("PluginConfig" + PluginConfig);
                try
                {
                    PluginCfg = PluginConfig.FromJSON<ZWaveDevicesConfig>();
                }
                catch (Exception ex)
                {
                    DebugEx.Assert(ex);
                }
            }

            //init base
            if (base.Initialize(mNodeConfig, PluginConfig, UserConfig) == false)
                return false;
            #endregion

            #region Init OZW
            try
            {
                if (!InitOpenZwaveBridge())
                    return false;

                //add zwave bridge things
                SetupZWaveBridgeThing();
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Could not init, ex.HResult" + ex.HResult + ", ex.Source:" + ex.Source + ", ex.Message: " + ex.Message + ", ex.InnerException: " + ex.InnerException); }
            #endregion

            //done
            return true;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void OnTransportConnected(string msg)
        {
            base.OnTransportConnected(msg);
            //Discovery();
        }
        //------------------------------------------------------------------------------------------------------------------------
        private bool InitOpenZwaveBridge()
        {
            #region Start ozw manager
            if (Native.init(ActiveCfg.OpenZWaveDirectory) == 0)
            {
                DebugEx.TraceError("openzwave failure to start Manager");
                return false;
            }
            #endregion

            #region Start waiting for messages from ozw
            Task.Delay(5000).Wait();
            isRunning = true;
            CommandRetrieverThread = new Thread(commandRetrieverHeartbeatEntryPoint);
            CommandRetrieverThread.IsBackground = true;
            CommandRetrieverThread.Start();
            #endregion

            #region Connect to controller
            Task.Delay(5000).Wait();
            if (Native.Connect(ActiveCfg.SerialPort) == 0)
            {
                DebugEx.TraceError("openzwave failure to connect to controller");
                return false;
            }

            // wait for ozw driver ready msg
            if (_isDriverReady.WaitOne(10000))
                return true;
            else
            {
                DebugEx.TraceError($"Driver Ready notification didn't recv, could not connect to Controller at port {ActiveCfg.SerialPort}");
                return false;
            }
            #endregion
        }
        //------------------------------------------------------------------------------------------------------------------------
        void commandRetrieverHeartbeatEntryPoint()
        {
            while (isRunning)
            {
                try
                {
                    var cmd = Native.GetMessage();
                    if (cmd != null)
                    {
                        try { processCommand(cmd); }
                        catch (Exception ex) { DebugEx.TraceError(ex, "processCommand failed"); }
                    }
                    else
                    {
                        DebugEx.TraceError("We get null message.");
                        Thread.Sleep(100); //something went wrong.. sleep for a while to avoid catastrophic spinning
                    }
                }
                catch (Exception ex)
                {
                    DebugEx.Assert(ex, "Could not init, ex.HResult" + ex.HResult + ", ex.Source:" + ex.Source + ", ex.Message: " + ex.Message + ", ex.InnerException: " + ex.InnerException);
                    return;
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        void processCommand(string jsonMsg)
        {
            try
            {
                var msg = jsonMsg.FromJSON<MsgFromCpp>();
                var nodeId = msg.nodeID;
                // remove special characters from thing product name
                msg.name = msg.name?.Replace(" ", "").Replace("+", "").Replace("/", "").Replace("-", "_").Replace("(", "").Replace(")", "");
                msg.label = msg.label?.Replace(" ", "").Replace("+", "").Replace("/", "").Replace("-", "_").Replace("(", "").Replace(")", "");
                switch (msg.NotificationType)
                {
                    case NotificationType.Type_DriverReady:
                        _isDriverReady.Set();
                        homeID = msg.homeID;
                        DebugEx.TraceLog("Controller HomeID" + homeID);
                        break;

                    case NotificationType.Type_DriverRemoved:
                        _isDriverReady.Reset();
                        DebugEx.TraceLog("Controller Removed");
                        break;

                    case NotificationType.Type_NodeNew:
                        if (!NodeIds.Contains(nodeId.ToString()))
                            ZWaveDevices.ForceAdd(nodeId, new ZWaveDescriptor() { NodeId = nodeId });
                        break;

                    case NotificationType.Type_NodeAdded:
                        try
                        {
                            if (!NodeIds.Contains(nodeId.ToString()))
                                ZWaveDevices.ForceAdd(nodeId, new ZWaveDescriptor() { NodeId = nodeId });
                            string stateInfo = msg.NotificationCode.ToString();
                            foreach (var navctx in NavigationContext.Values)
                                if (navctx.CurrentPage?.Title == "Add ZWave Devices") { navctx.GoBack(); navctx.UpdateCurrentPage(createMainPage(stateInfo)); }
                        }
                        catch (Exception ex) { DebugEx.TraceErrorException(ex, ex.StackTrace); }
                        break;

                    case NotificationType.Type_NodeQueriesComplete:
                    case NotificationType.Type_NodeNaming:
                        try
                        {
                            if (!NodeIds.Contains(nodeId.ToString()))
                            {
                                NodeIds.Add(nodeId.ToString());
                                // create thing
                                OZWAddDeviceComplete(msg, nodeId);
                                // inform mNode and cloud for the new thing
                                OnZwaveDeviceAdded(nodeId);
                                // Update remote control app pages
                                foreach (var navctx in NavigationContext.Values)
                                    if (navctx.CurrentPage?.Title == "Main page") { navctx.UpdateCurrentPage(createMainPage()); }
                                // save zwave devices
                                if (!Native.saveZWaveDevices(homeID)) { DebugEx.TraceLog("OZW fails to save zwave devices"); }
                            }
                        }
                        catch (Exception ex) { DebugEx.TraceErrorException(ex, ex.StackTrace); }
                        break;

                    case NotificationType.Type_ValueAdded:
                    case NotificationType.Type_ValueRefreshed:
                        DebugEx.TraceLog($"New value for node {msg.nodeID}");
                        AddUpdateNewValueDevice(msg);
                        break;

                    case NotificationType.Type_ValueChanged:
                        DebugEx.TraceLog($"New Changed added for node {msg.nodeID}");
                        AddUpdateNewValueDevice(msg);
                        OZWController_NodeUpdated(msg);
                        break;

                    case NotificationType.Type_NodeRemoved:
                        try
                        {
                            NodeIds.Remove(nodeId.ToString());
                            OnZwaveDeviceRemoved(msg.nodeID);
                            // update remote control app 
                            foreach (var navctx in NavigationContext.Values)
                            {
                                if (navctx.CurrentPage.Title == "Remove ZWave Devices") { navctx.GoBack(); navctx.UpdateCurrentPage(createMainPage()); }
                                else if (navctx.CurrentPage?.Title == "Main page") { navctx.UpdateCurrentPage(createMainPage()); }
                            }
                        }
                        catch (Exception ex) { DebugEx.TraceErrorException(ex); }
                        break;

                    case NotificationType.Type_AwakeNodesQueried:
                    case NotificationType.Type_AllNodesQueriedSomeDead:
                    case NotificationType.Type_AllNodesQueried:
                        // save zwave devices
                        if (!Native.saveZWaveDevices(homeID)) { DebugEx.TraceLog("OZW failed to save zwave devices"); }
                        break;
                    default:
                        DebugEx.TraceWarning($"Unhandle msg received from OZW of type {msg.NotificationType}");
                        break;
                }
            }
            catch (Exception ex) { DebugEx.TraceError(ex, "processCommand failed"); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        void AddUpdateNewValueDevice(MsgFromCpp msg)
        {
            try
            {
                ZWaveDescriptor zwD;
                if (ZWaveDevices.Count > 0 && ZWaveDevices.TryGetValue(msg.nodeID, out zwD))
                {
                    Dictionary<string, ValueID> previousVIDs = new Dictionary<string, ValueID>();
                    var valueid = new ValueID
                    {
                        homeID = msg.homeID,
                        nodeID = msg.nodeID,
                        genre = msg.genre,
                        commandClassId = msg.commandClassId,
                        instance = msg.instance,
                        valueIndex = msg.valueIndex,
                        type = msg.type,
                        label = msg.label
                    };
                    if (zwD.values.ContainsKey(msg.commandClassId))
                    {
                        previousVIDs = zwD.values[msg.commandClassId];
                        previousVIDs.ForceAdd(valueid.label, valueid);
                        zwD.values.ForceAdd(msg.commandClassId, previousVIDs);
                    }
                    else
                    {
                        previousVIDs.Add(valueid.label, valueid);
                        zwD.values.Add(msg.commandClassId, previousVIDs);
                    }
                }
                else { DebugEx.TraceWarning($"No valid zwave device with id {msg.nodeID}"); return; }

                Byte id = msg.nodeID;
                ZWaveDevices.ForceAdd(id, zwD);
                DebugEx.TraceLog($"New command class {msg.commandClassId} added for node {msg.nodeID}, total values count {ZWaveDevices[msg.nodeID].values.Count}");

            }
            catch (Exception ex) { DebugEx.TraceErrorException(ex, ex.StackTrace + ex.HResult + ex.Source + ex.Data); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void OZWAddDeviceComplete(MsgFromCpp msg, Byte nodeId)
        {
            try
            {
                ZWaveDescriptor zwD;
                ZWaveDevices.TryGetValue(nodeId, out zwD);
                // now that node pairing is complete add the new node
                DebugEx.TraceLog("Construct new Zwave Device");
                var thing = CreateOpenZWaveThing(nodeId, msg.name, ref zwD);
                var zwaveDesc = new ZWaveDescriptor()
                {
                    NodeId = nodeId,
                    ProductName = msg.name,
                    Thing = thing,
                };
                if (zwD == null)
                    ZWaveDevices.Add(nodeId, zwaveDesc);
                else
                {
                    zwD.ProductName = msg.name;
                    zwD.Thing = thing;
                    zwD.NodeId = nodeId;
                    ZWaveDevices.ForceAdd(nodeId, zwD);
                }
            }
            catch (Exception ex) { DebugEx.TraceErrorException(ex, ex.StackTrace); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void OnZwaveDeviceRemoved(Byte nodeId)
        {
            if (ZWaveDevices.ContainsKey(nodeId))
            {
                DeleteThing(ZWaveDevices[nodeId].Thing.ThingKey);
                ZWaveDevices.Remove(nodeId);
                DebugEx.TraceLog("Remove Zwave Thing");
                UpdatePluginConfig();
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void UpdatePluginConfig()
        {
            PluginCfg.pairedDevices = ZWaveDevices;
            UpdateConfig(PluginCfg.ToJSON());
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void OnZwaveDeviceAdded(Byte nodeid)
        {
            try
            {
                ZWaveDescriptor zwavedesc;
                if (ZWaveDevices.TryGetValue(nodeid, out zwavedesc))
                {
                    DebugEx.TraceLog("Add Zwave Thing");
                    AddThing(zwavedesc.Thing);
                    UpdatePluginConfig();
                }
            }
            catch (Exception ex) { DebugEx.TraceErrorException(ex, ex.StackTrace); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override bool Deinitialize()
        {
            try
            {
                base.Deinitialize();

                // deinitialize ozw manager
                Native.deInit(homeID);

                //stop thread
                isRunning = false;
                CommandRetrieverThread?.Join(1000);
                CommandRetrieverThread = null;
                return true;
            }
            catch (Exception ex) { DebugEx.TraceErrorException(ex); return false; }
        }
        //------------------------------------------------------------------------------------------------------------------------

        #region Yodiwo Zwave Node specific methods
        //------------------------------------------------------------------------------------------------------------------------
        public static readonly string PORTID_WRITEVAL = "0";
        public static readonly string PORTID_NODEIN = "1";
        public static readonly string PORTID_NODEOUT = "2";
        public static readonly string PORTID_READTYPE = "3";
        public static readonly string PORTID_READVAL = "4";
        public static readonly string PORTID_NODEADD = "5";
        public static readonly string PORTID_NODEDEL = "6";
        public static readonly string PORTID_NODEGET = "7";
        public static readonly string PORTID_ASSOCNODE = "8";
        public static readonly string PORTID_HASNODEFAILED = "9";
        public static readonly string PORTID_RESPONSE = "10";
        public static readonly string PORTID_COMMANDCLASSID = "11";
        public static readonly string PORTID_COMMANDCLASSIDLABEL = "12";
        public static readonly string PORTID_TARGETNODEID = "13";
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        public static string CreateThingKey(string bridgeID)
        {
            return ThingKeyPrefix + "ZWave:" + bridgeID;
        }
        //------------------------------------------------------------------------------------------------------------------------
        private Thing SetupZWaveBridgeThing()
        {
            var thing = new Yodiwo.API.Plegma.Thing()
            {
                ThingKey = ThingKey.BuildFromArbitraryString("$NodeKey$", CreateThingKey("1")),
                Type = "com.yodiwo.zwave",
                Name = "Yodiwo Z-Wave",
                UIHints = new ThingUIHints()
                {
                    IconURI = "/Content/img/icons/Generic/icon-zwave-logo.png",
                },
            };

            #region create ports and add to local dictionary
            ZWaveThingPorts.Add(PORTID_WRITEVAL, new Yodiwo.API.Plegma.Port()
            {
                ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                Name = "Write Value",
                State = "",
                ConfFlags = ePortConf.PropagateAllEvents,
                Type = Yodiwo.API.Plegma.ePortType.String,
                PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", PORTID_WRITEVAL),
                Description = "The new Z-Wave device value"

            });
            ZWaveThingPorts.Add(PORTID_NODEIN, new Yodiwo.API.Plegma.Port()
            {
                ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                Name = "Node In",
                State = "",
                ConfFlags = ePortConf.PropagateAllEvents,
                Type = Yodiwo.API.Plegma.ePortType.Integer,
                PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", PORTID_NODEIN),
                Description = "Z-Wave Device Node id (in which changes are applied)"
            });
            ZWaveThingPorts.Add(PORTID_NODEOUT, new Yodiwo.API.Plegma.Port()
            {
                ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                Name = "Node Out",
                State = "",
                ConfFlags = ePortConf.PropagateAllEvents,
                Type = Yodiwo.API.Plegma.ePortType.String,
                PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", PORTID_NODEOUT)
            });
            ZWaveThingPorts.Add(PORTID_READTYPE, new Yodiwo.API.Plegma.Port()
            {
                ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                Name = "Read Type",
                State = "",
                ConfFlags = ePortConf.PropagateAllEvents,
                Type = Yodiwo.API.Plegma.ePortType.String,
                PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", PORTID_READTYPE)
            });
            ZWaveThingPorts.Add(PORTID_READVAL, new Yodiwo.API.Plegma.Port()
            {
                ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                Name = "Read Value",
                State = "",
                ConfFlags = ePortConf.PropagateAllEvents,
                Type = Yodiwo.API.Plegma.ePortType.String,
                PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", PORTID_READVAL)
            });
            ZWaveThingPorts.Add(PORTID_NODEADD, new Yodiwo.API.Plegma.Port()
            {
                ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                Name = "Add ZWave Node",
                State = "",
                ConfFlags = ePortConf.PropagateAllEvents,
                Type = Yodiwo.API.Plegma.ePortType.Integer,
                PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", PORTID_NODEADD),
                Description = "Start (1) or cancel (0) adding new Z-Wave device."
            });
            ZWaveThingPorts.Add(PORTID_NODEDEL, new Yodiwo.API.Plegma.Port()
            {
                ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                Name = "Remove ZWave Node",
                State = "",
                ConfFlags = ePortConf.PropagateAllEvents,
                Type = Yodiwo.API.Plegma.ePortType.Integer,
                PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", PORTID_NODEDEL),
                Description = "Start (1) or cancel (0) removing new Z-Wave device."
            });
            ZWaveThingPorts.Add(PORTID_NODEGET, new Yodiwo.API.Plegma.Port()
            {
                ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                Name = "Node Get",
                State = "",
                ConfFlags = ePortConf.PropagateAllEvents,
                Type = Yodiwo.API.Plegma.ePortType.Integer,
                PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", PORTID_NODEGET),
                Description = "Node id for Z-Wave device retrieval"
            });
            ZWaveThingPorts.Add(PORTID_ASSOCNODE, new Yodiwo.API.Plegma.Port()
            {
                ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                Name = "Assoc Group",
                State = "",
                ConfFlags = ePortConf.PropagateAllEvents,
                Type = Yodiwo.API.Plegma.ePortType.Integer,
                PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", PORTID_ASSOCNODE),
                Description = "Association Group id for Z-Wave Device"
            });
            ZWaveThingPorts.Add(PORTID_HASNODEFAILED, new Yodiwo.API.Plegma.Port()
            {
                ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                Name = "Node Status",
                State = "",
                ConfFlags = ePortConf.PropagateAllEvents,
                Type = Yodiwo.API.Plegma.ePortType.Integer,
                PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", PORTID_HASNODEFAILED),
                Description = "With \"0\" get status of all Z-Wave nodes, with appropriate node id get status of a specific node"
            });
            ZWaveThingPorts.Add(PORTID_RESPONSE, new Yodiwo.API.Plegma.Port()
            {
                ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                Name = "Response",
                State = "",
                ConfFlags = ePortConf.PropagateAllEvents,
                Type = Yodiwo.API.Plegma.ePortType.String,
                PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", PORTID_RESPONSE)
            });
            ZWaveThingPorts.Add(PORTID_COMMANDCLASSID, new Yodiwo.API.Plegma.Port()
            {
                ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                Name = "Command Class ID",
                State = "",
                ConfFlags = ePortConf.PropagateAllEvents,
                Type = Yodiwo.API.Plegma.ePortType.Integer,
                PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", PORTID_COMMANDCLASSID),
                Description = "In order to write a new value in Z-Wave Device, enter in this port CommandClassId value. From \"Node Get\" find out which are the available CommandClassIDs."
            });
            ZWaveThingPorts.Add(PORTID_COMMANDCLASSIDLABEL, new Yodiwo.API.Plegma.Port()
            {
                ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                Name = "Command Class ID Label",
                State = "",
                ConfFlags = ePortConf.PropagateAllEvents,
                Type = Yodiwo.API.Plegma.ePortType.String,
                PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", PORTID_COMMANDCLASSIDLABEL),
                Description = "In order to write a new value in Z-Wave Device, enter in this port CommandClassId \"Label\" value.  From \"Node Get\" find out which are the available CommandClassIDs Labels."
            });
            ZWaveThingPorts.Add(PORTID_TARGETNODEID, new Yodiwo.API.Plegma.Port()
            {
                ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                Name = "Target Node ID",
                State = "",
                ConfFlags = ePortConf.PropagateAllEvents,
                Type = Yodiwo.API.Plegma.ePortType.String,
                PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", PORTID_TARGETNODEID),
                Description = "Enter the target node id for node group association (value \"1\" refers to the Z-Wave Controller)."
            });
            #endregion

            //also add ports to thing
            thing.Ports.AddFromSource(ZWaveThingPorts.Values);

            //add
            thing = AddThing(thing);

            return thing;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void OnZWaveBridgePortEvent(PortEventMsg msg)
        {
            try
            {
                bool WriteDevice = false;
                bool ChangeAssociationGroups = false;
                byte Nodeid = 0;
                byte TargetNodeid = 0;
                byte GroupID = 0;
                string NewDeviceValue = "";
                string CommandClassID = "";
                string CommandClassIDLabel = "";
                var portEvents = new List<TupleS<string, string>>();

                foreach (var pe in msg.PortEvents)
                {
                    var pId = ((PortKey)pe.PortKey).PortUID;
                    if (!ZWaveThingPorts.ContainsKey(pId)) { DebugEx.TraceError($"Received msg {pe.State} for unknown portkey {pe.PortKey}"); return; }

                    if (PORTID_NODEIN == pId)
                    {
                        try
                        {
                            Nodeid = (byte)uint.Parse(pe.State);
                        }
                        catch (Exception ex) { DebugEx.TraceErrorException(ex); }
                    }
                    else if (PORTID_WRITEVAL == pId)
                    {
                        try
                        {
                            NewDeviceValue = pe.State;
                            WriteDevice = true;
                        }
                        catch (Exception ex) { DebugEx.TraceErrorException(ex); }
                    }
                    else if (PORTID_COMMANDCLASSID == pId)
                    {
                        try
                        {
                            CommandClassID = pe.State;
                            WriteDevice = true;
                        }
                        catch (Exception ex) { DebugEx.TraceErrorException(ex); }
                    }
                    else if (PORTID_COMMANDCLASSIDLABEL == pId)
                    {
                        try
                        {
                            CommandClassIDLabel = pe.State;
                            WriteDevice = true;
                        }
                        catch (Exception ex) { DebugEx.TraceErrorException(ex); }
                    }
                    else if (PORTID_NODEADD == pId)
                    {
                        var tmp = ushort.Parse(pe.State);
                        if (tmp == 1)
                        {
                            DebugEx.TraceLog($"OnZWaveBridgeEvent: Begin node add");
                            Native.AddDevice(false, homeID);
                        }
                        else if (tmp == 0)
                        {
                            DebugEx.TraceLog($"OnZWaveBridgeEvent: Begin node add stop");
                            Native.CancelOperation(homeID);
                        }
                    }
                    else if (PORTID_NODEDEL == pId)
                    {
                        var tmp = ushort.Parse(pe.State);
                        if (tmp == 1)
                        {
                            DebugEx.TraceLog($"OnZWaveBridgeEvent: Begin node remove");
                            Native.RemoveDevice(homeID);
                        }
                        else if (tmp == 0)
                        {
                            DebugEx.TraceLog($"OnZWaveBridgeEvent: Begin node remove stop");
                            Native.CancelOperation(homeID);
                        }
                    }
                    else if (PORTID_NODEGET == pId)
                    {
                        var zwDescr = new ZWaveDescriptor();
                        if (ZWaveDevices.TryGetValue((byte)uint.Parse(pe.State), out zwDescr))
                        {
                            portEvents.Add(new TupleS<string, string>(ZWaveThingPorts[PORTID_RESPONSE].PortKey, zwDescr.values.ToJSON()));
                            SetPortState(portEvents);
                        }
                    }
                    else if (PORTID_ASSOCNODE == pId)
                    {
                        try
                        {
                            GroupID = (byte)uint.Parse(pe.State);
                            ChangeAssociationGroups = true;
                        }
                        catch (Exception ex) { DebugEx.TraceErrorException(ex); }
                    }
                    else if (PORTID_TARGETNODEID == pId)
                    {
                        try
                        {
                            TargetNodeid = (byte)uint.Parse(pe.State);
                        }
                        catch (Exception ex) { DebugEx.TraceErrorException(ex); }
                    }
                    else if (PORTID_HASNODEFAILED == pId)
                    {
                        DebugEx.TraceLog($"Has Node failed request: {pe.State}");
                        var resp = new Dictionary<string, string>();
                        if (pe.State == "0")
                        {
                            DebugEx.TraceLog($"Get all ZWave Devices!");
                            foreach (var zdev in ZWaveDevices)
                            {
                                var response = Native.GetNodeState(homeID, zdev.Key);
                                resp.Add(zdev.Key.ToString(), response);
                            }
                        }
                        else
                        {
                            var response = Native.GetNodeState(homeID, (byte)uint.Parse(pe.State));
                            resp.Add(pe.State, response);
                        }
                        portEvents.Add(new TupleS<string, string>(ZWaveThingPorts[PORTID_RESPONSE].PortKey, resp.ToJSON()));
                        SetPortState(portEvents);
                    }
                }
                if (Nodeid != 0)
                {
                    // check if node id is valid
                    ZWaveDescriptor zdev = new ZWaveDescriptor();
                    if (!ZWaveDevices.TryGetValue(Nodeid, out zdev))
                    {
                        DebugEx.TraceError($"Invalid node id requested {Nodeid}");
                        return;
                    }

                    #region Change device value
                    if (WriteDevice
                        && !NewDeviceValue.IsNullOrEmpty()
                        && !CommandClassID.IsNullOrEmpty()
                        && !CommandClassIDLabel.IsNullOrEmpty())
                    {
                        CommandClassId cmdClass;
                        Enum.TryParse(CommandClassID, out cmdClass);
                        var vids = new Dictionary<string, ValueID>();
                        ValueID vid;
                        if (zdev.values.Count > 0 && zdev.values.TryGetValue(cmdClass, out vids))
                        {
                            vids.TryGetValue(CommandClassIDLabel, out vid);
                            if (!Native.ChangeDeviceValue(vid.ToJSON(), NewDeviceValue, homeID))
                            {
                                DebugEx.TraceError("Could not change device state");
                                portEvents.Add(new TupleS<string, string>(ZWaveThingPorts[PORTID_RESPONSE].PortKey, $"ZWave Device {Nodeid} writeval failed."));
                                SetPortState(portEvents);
                            }
                        }
                    }
                    #endregion

                    #region Update Association Groups 
                    if (ChangeAssociationGroups
                        && TargetNodeid != 0)
                    {
                        byte instance = 1;
                        if (!Native.ChangeGroupAssociation(homeID, Nodeid, TargetNodeid, instance, GroupID))
                        {
                            portEvents.Add(new TupleS<string, string>(ZWaveThingPorts[PORTID_RESPONSE].PortKey, $"ZWave Device {Nodeid} failed to update association group {GroupID} for target node {TargetNodeid}."));
                            SetPortState(portEvents);
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex) { DebugEx.TraceErrorException(ex); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void OnPortEvent(PortEventMsg msg)
        {
            //check read
            try
            {
                List<PortEvent> bridgePevs = new List<PortEvent>();

                foreach (var pe in msg.PortEvents)
                {
                    bool IsForZWaveDevice = false;
                    foreach (var zdev in ZWaveDevices.Values)
                    {
                        var targetp = zdev?.Thing?.Ports?.Where(p => p.PortKey == pe.PortKey).FirstOrDefault();
                        if (targetp != null)
                        {
                            var commandID = targetp.PortKey.RightOf("WRITEVAL_").LeftOf("...");
                            DebugEx.TraceLog($"Port event for port {targetp.Name} with port key {targetp.PortKey} for commandClassID {commandID}");
                            OnOZWDevicePortEvent(pe, zdev, commandID);
                            IsForZWaveDevice = true;
                            break;
                        }
                    }
                    if (!IsForZWaveDevice)
                    {
                        bridgePevs.Add(pe);
                    }
                }
                if (bridgePevs.Count > 0)
                {
                    OnZWaveBridgePortEvent(new PortEventMsg() { PortEvents = bridgePevs.ToArray() });
                }
            }
            catch (Exception ex) { DebugEx.TraceErrorException(ex, ex.StackTrace); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void OnOZWDevicePortEvent(PortEvent pe, ZWaveDescriptor zwDesc, string commandClass)
        {
            try
            {
                CommandClassId cmdClass;
                Enum.TryParse(commandClass, out cmdClass);
                var vids = new Dictionary<string, ValueID>();
                ValueID vid;
                if (zwDesc.values.Count > 0 && zwDesc.values.TryGetValue(cmdClass, out vids))
                {
                    var label = pe.PortKey.RightOf("...");
                    vids.TryGetValue(label, out vid);
                    DebugEx.TraceLog($"Send ChangeDeviceValue Request {vid.ToJSON()}");
                    if (!Native.ChangeDeviceValue(vid.ToJSON(), pe.State, homeID))
                        DebugEx.TraceError("Could not change device state");
                }
            }
            catch (Exception ex) { DebugEx.TraceErrorException(ex, ex.StackTrace); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        void OZWController_NodeUpdated(MsgFromCpp args)
        {
            if (NodeIds.Contains(args.nodeID.ToString()))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                DebugEx.TraceLog(string.Format("NodeUpdated {0} Event Parameter {1} Value {2}", args.nodeID, args.commandClassId, args.currentValue));
                Console.ForegroundColor = ConsoleColor.White;
                DebugEx.TraceLog(string.Format("NodeUpdated {0} Event Parameter {1} Node Info Frame {2}", args.nodeID.ToStringInvariant(), args.commandClassId, args.currentValue));
                string nodeframe = args.currentValue as string;

                ZWaveDescriptor zwavedesc = null;
                var portEvents = new List<TupleS<string, string>>();

                // set port states value
                string typeState = (args.label == null) ? args.commandClassId.ToString() : args.label;
                string valueState = args.currentValue.ToStringInvariant();

                if (ZWaveDevices.TryGetValue(args.nodeID, out zwavedesc))
                {
                    // if zwave device is a plug filter its notifications before send them as port event
                    var plugMeterType = PlugMeteringType.none;
                    if (zwavedesc.ProductName.Contains(DevicesName.MicroSmartPlug.ToString())
                        || zwavedesc.ProductName.Contains(DevicesName.Powerplug12A.ToString()))
                        if (!FilteringSmartPlug(args, out plugMeterType))
                            return;

                    var ports = zwavedesc.Thing?.Ports.Where(p => p.ioDirection == ioPortDirection.Output)?.ToList();

                    if (ports != null)
                    {
                        var typepk = ports.Find(p => p.Name == "Type").PortKey;
                        DebugEx.TraceLog(string.Format("TypeKey:{0},", typepk));

                        var valuepk = ports.Find(p => p.Name == "Value").PortKey;
                        DebugEx.TraceLog(string.Format("valuekey:{0}", valuepk));

                        if (!string.IsNullOrEmpty(typepk) && !string.IsNullOrEmpty(valuepk))
                        {
                            // if its energy or power or current or voltage metering include this info in value state 
                            if (plugMeterType != 0)
                                typeState = typeState + $" ({plugMeterType.ToString()})";
                            portEvents.Add(new TupleS<string, string>(typepk, typeState));
                            portEvents.Add(new TupleS<string, string>(valuepk, valueState));
                        }
                    }
                }

                //We want the Z-Wave Bridge Thing to also receive the events even if they dont match a targeted thing
                portEvents.Add(new TupleS<string, string>(ZWaveThingPorts[PORTID_NODEOUT].PortKey, args.nodeID.ToStringInvariant()));
                portEvents.Add(new TupleS<string, string>(ZWaveThingPorts[PORTID_READTYPE].PortKey, typeState));
                portEvents.Add(new TupleS<string, string>(ZWaveThingPorts[PORTID_READVAL].PortKey, valueState));

                SetPortState(portEvents);
            }
            else
                DebugEx.TraceLog("Value update msg received for not valid device");
        }
        //------------------------------------------------------------------------------------------------------------------------
        private Thing CreateOpenZWaveThing(Byte nodeId, string name, ref ZWaveDescriptor zwD)
        {
            DebugEx.TraceLog("Create Zwave Thing " + nodeId.ToString() + " " + name);
            var thingkey = new ThingKey(NodeKey, PluginMain.ThingKeyPrefix + name + "_" + nodeId.ToString());

            #region Generic ZWave Thing Description
            var thing = new Thing()
            {
                ThingKey = thingkey,
                Name = "ZWave-" + name + "-" + nodeId.ToString(),
                ConfFlags = eThingConf.None,
                UIHints = new ThingUIHints()
                {
                    Description = "ZWave Device " + name + "-" + nodeId.ToString() + ".",
                    IconURI = "/Content/img/icons/Generic/icon-zwave-logo.png"
                },
            };

            // every Yodiwo ZWave Thing has output ports to export information send from ozw for the associated sensor
            thing.Ports = new List<Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        PortKey = new PortKey(thingkey, "READTYPE"),
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "Type",
                        State = "",
                        ConfFlags = ePortConf.PropagateAllEvents,
                        Type = Yodiwo.API.Plegma.ePortType.String,
                    },
                    new Yodiwo.API.Plegma.Port()
                    {
                        PortKey = new PortKey(thingkey, "READVAL"),
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "Value",
                        State = "",
                        ConfFlags = ePortConf.PropagateAllEvents,
                        Type = Yodiwo.API.Plegma.ePortType.String,
                    }
                };
            #endregion

            #region Specific Thing Description

            #region Special Icon Uri & Thing Type
            // if its known Yodikit sensor set the appropriate icon
            if (name.Contains(DevicesName.FGK10xDoorOpeningSensor.ToString())
                || name.Contains(DevicesName.DoorWindowDetector.ToString()))
            {
                thing.UIHints.IconURI = "/Content/img/icons/Generic/icon-door-sensor.png";
                thing.Type = ThingTypeLibrary.DoorSensor_ModelType + PlegmaAPI.ThingModelTypeSeparatorPlusDefault;
            }
            else if (name.Contains(DevicesName.MicroSmartPlug.ToString())
                    || name.Contains(DevicesName.Powerplug12A.ToString())
                    || name.Contains(DevicesName.FGWPEFWallPlug.ToString()))
            {
                thing.UIHints.IconURI = "/Content/img/icons/Generic/icon-smart-plug.png";
                thing.Type = ThingTypeLibrary.SmartPlugSensor_ModelType + PlegmaAPI.ThingModelTypeSeparatorPlusDefault;
            }
            else if (name.Contains(DevicesName.DanalockV2BTZE.ToString()))
            {
                thing.UIHints.IconURI = "/Content/img/icons/Generic/doorlock.png";
                thing.Type = ThingTypeLibrary.DoorlockSensor_ModelType + PlegmaAPI.ThingModelTypeSeparatorPlusDefault;
            }
            else if (name.Contains(DevicesName.ZXT_120EU.ToString()))
            {
                thing.UIHints.IconURI = "/Content/img/icons/Generic/aircondition.png";
                thing.Type = ThingTypeLibrary.AirConditionSensor_ModelType + PlegmaAPI.ThingModelTypeSeparatorPlusDefault;
            }
            else
                thing.Type = ThingTypeLibrary.ZWaveSensor_ModelType + PlegmaAPI.ThingModelTypeSeparatorPlusDefault;
            #endregion

            #region Special Thing Configuration Parameters

            if (name.Contains(DevicesName.ZXT_120EU.ToString()))
            {
                DebugEx.TraceLog("Try to add configuration");
                thing.Config = new List<ConfigParameter>
                {
                    new ConfigParameter()
                    {
                        Name = "ConfCode:27",
                        Description = "IR code number for built-in code library",
                        Value = "0",
                    }
                };
                DebugEx.TraceLog($"Configuration added {thing.Config.ToJSON()}");
                zwD.ConfigIDs.Add(27);
            }

            #endregion

            #region Special Input Ports
            // based on thing write enabled command classes create and a thing port
            foreach (var vid in zwD.values)
            {
                if (vid.Key == CommandClassId.SWITCH_BINARY
                    || vid.Key == CommandClassId.SWITCH_ALL
                    || vid.Key == CommandClassId.SWITCH_TOGGLE_BINARY
                    || vid.Key == CommandClassId.SWITCH_MULTILEVEL
                    || vid.Key == CommandClassId.SWITCH_TOGGLE_MULTILEVEL
                    || vid.Key == CommandClassId.THERMOSTAT_FAN_MODE
                    || vid.Key == CommandClassId.THERMOSTAT_MODE
                    || vid.Key == CommandClassId.THERMOSTAT_SETPOINT
                    || vid.Key == CommandClassId.DOOR_LOCK)
                {
                    foreach (var v in vid.Value)
                    {
                        DebugEx.TraceLog($"New label {v.Key} for command class {vid.Key}");
                        thing.Ports.Add(
                            new Yodiwo.API.Plegma.Port()
                            {
                                ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                                Name = v.Key,
                                State = "",
                                ConfFlags = ePortConf.PropagateAllEvents,
                                Type = YodiwoOZWaveTypesMatching(v.Value.type),
                                PortKey = new PortKey(thingkey, "WRITEVAL_" + vid.Key.ToString() + "..." + v.Key),
                            });
                    }
                }
            }
            #endregion

            #endregion

            return thing;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public ePortType YodiwoOZWaveTypesMatching(ValueType type)
        {
            switch (type)
            {
                case ValueType.ValueType_Bool:
                    return ePortType.Boolean;
                case ValueType.ValueType_Int:
                    return ePortType.Integer;
                case ValueType.ValueType_List:
                case ValueType.ValueType_Raw:
                case ValueType.ValueType_Schedule:
                case ValueType.ValueType_String:
                case ValueType.ValueType_Byte:
                case ValueType.ValueType_Button:
                    return ePortType.String;
                case ValueType.ValueType_Short:
                case ValueType.ValueType_Decimal:
                    return ePortType.Decimal;
                default:
                    return ePortType.String;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void OnUpdateThing(Thing thing, Thing oldCopy)
        {
            base.OnUpdateThing(thing, oldCopy);
            try
            {
                foreach (var zdev in ZWaveDevices.Values)
                {
                    if (zdev?.Thing.ThingKey == thing.ThingKey)
                    {
                        // check if zwave thing configuration has changed
                        var differences = thing.Config.Except(oldCopy.Config).ToList();
                        if (!differences.IsNullOrEmpty())
                        {
                            DebugEx.TraceLog("Changes in thing configurations");
                            // update plugin thing
                            zdev.Thing.Config = thing.Config;
                            ZWaveDevices.ForceAdd(zdev.NodeId, zdev);
                            // apply to real device the changes
                            ChangeOZWDeviceConfiguration(zdev, differences);
                        }
                        break;
                    }
                }
            }
            catch (Exception ex) { DebugEx.TraceErrorException(ex); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void ChangeOZWDeviceConfiguration(ZWaveDescriptor zwD, List<ConfigParameter> newThingConfig)
        {
            try
            {
                foreach (var diff in newThingConfig)
                {
                    var configCode = diff.Name.RightOf("ConfCode:").ParseToByte();
                    ApplyNewConfiguration(zwD, configCode, diff.Value);
                }
            }
            catch (Exception ex) { DebugEx.TraceErrorException(ex); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public bool ApplyNewConfiguration(ZWaveDescriptor zwD, float ConfCode, string ConfValue)
        {
            try
            {
                var valueid = zwD.values[CommandClassId.CONFIGURATION].Values.Where(vid => vid.valueIndex == ConfCode);
                if (!valueid.IsNullOrEmpty() && !Native.ChangeDeviceValue(valueid.First().ToJSON(), ConfValue, homeID))
                {
                    DebugEx.TraceError("Could not change configuration");
                    return false;
                }
                return true;
            }
            catch (Exception ex) { DebugEx.TraceErrorException(ex); return false; }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Gui
        //------------------------------------------------------------------------------------------------------------------------
        public override void UI_mNodeForms_OnNewNavigationContext(NavigationContext navCtx, string StartPageUri)
        {
            base.UI_mNodeForms_OnNewNavigationContext(navCtx, StartPageUri);
            //navigate to start page
            navCtx.NavigateTo(createMainPage());
        }
        //------------------------------------------------------------------------------------------------------------------------
        public Page createMainPage(string stateinfo = "")
        {
            //create main page
            Page page = new Page()
            {
                Uri = "main",
                Name = "main",
                Title = "Main page",
                Controls = new ListTS<Control>()
                {
                    new UI.Forms.Controls.StackPanel()
                    {
                        Items = new ListTS<Control>()
                        {
                             new UI.Forms.Controls.Button()
                             {
                                 Text = "Add new ZWaveDevices",
                                 Clicked = btnAdd_Clicked,
                             },
                             new UI.Forms.Controls.Button()
                             {
                                 Text = "Secure Add new ZWaveDevices",
                                 Clicked = btnSecAdd_Clicked,
                             },
                             new UI.Forms.Controls.Button()
                             {
                                 Text = "Remove Already Paired ZWaveDevices",
                                 Clicked = btnRemove_Clicked,
                             },
                        }
                    }
                },
            };

            if (ZWaveDevices.Any())
            {
                var stack = page.Controls[0] as StackPanel;
                //stack?.Items.Add(new UI.Forms.Controls.Label()
                //{
                //    Text = "Web Control Panel",
                //});
                //stack?.Items.Add(new UI.Forms.Controls.Button()
                //{
                //    Text = "Control Panel",
                //    Clicked = btOZWCP_Clicked
                //});

                stack?.Items.Add(new UI.Forms.Controls.Label()
                {
                    Text = "Paired ZWave Devices(s)",
                });

                foreach (var zwave in ZWaveDevices)
                {
                    string text;
                    if (zwave.Value.ProductName.IsNullOrEmpty())
                    {
                        var msg = stateinfo.IsNullOrEmpty() ? " (getting info)" : $" ({stateinfo} state)";
                        text = "Node " + zwave.Key.ToString() + msg;
                    }
                    else
                        text = zwave.Key.ToString() + "_" + zwave.Value.ProductName;

                    stack?.Items.Add(new UI.Forms.Controls.Button()
                    {
                        Tag = zwave.Key.ToString(),
                        Text = text,
                        Clicked = btNavToAssociate_Clicked
                    });
                }
            }
            return page;
        }
        //------------------------------------------------------------------------------------------------------------------------
        void btnAdd_Clicked(NavigationContext ctx, Button control, UIEvent args)
        {
            RemoveAddZWaveDevice(ctx, "add", false);
        }
        //------------------------------------------------------------------------------------------------------------------------
        void btnSecAdd_Clicked(NavigationContext ctx, Button control, UIEvent args)
        {
            RemoveAddZWaveDevice(ctx, "add", true);
        }
        //------------------------------------------------------------------------------------------------------------------------
        void btnRemove_Clicked(NavigationContext ctx, Button control, UIEvent args)
        {
            RemoveAddZWaveDevice(ctx, "remove", false);
        }
        //------------------------------------------------------------------------------------------------------------------------
        void RemoveAddZWaveDevice(NavigationContext ctx, string operation, bool isSecure)
        {
            try
            {
                DebugEx.TraceLog("Button " + operation + " clicked");
                // remote control UI
                if (operation == "remove")
                    ctx.NavigateTo(PrepareZWaveBridge("removal"));
                else
                    ctx.NavigateTo(PrepareZWaveBridge("addition"));

                // cancel current ozw bridge operations and start the new commanded process
                Native.CancelOperation(homeID);

                var res = false;
                if (operation == "remove")
                    res = Native.RemoveDevice(homeID) == 1;
                else
                    res = Native.AddDevice(isSecure, homeID) == 1;

                if (res)
                {
                    // requested operation started
                    foreach (var navctx in NavigationContext.Values)
                    {
                        if (navctx.CurrentPage.Title == "Prepare ZWaveBridge")
                        {
                            if (operation == "remove")
                                navctx.UpdateCurrentPage(createRemoveZWaveDevice());
                            else
                                navctx.UpdateCurrentPage(AddZWaveDevice());
                        }
                    }
                }
                else
                {
                    // requested operation could not start, return to main zwave plugin page
                    foreach (var navctx in NavigationContext.Values)
                    {
                        if (navctx.CurrentPage.Title == "Prepare ZWaveBridge")
                        {
                            navctx.GoBack();
                            navctx.UpdateCurrentPage(createMainPage());
                        }
                    }
                }
            }
            catch (Exception ex) { DebugEx.TraceErrorException(ex, ex.StackTrace); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        void btNavToAssociate_Clicked(NavigationContext ctx, Button control, UIEvent args)
        {
            DebugEx.TraceLog("Button Navigate to Associate clicked");
            var key = control?.TagStr;
            DebugEx.TraceLog("Button Nabigate to Associate clicked: " + key);
            if (key != null && ctx != null)
                ctx.NavigateTo(AssocZWaveDevice(key));
        }
        //------------------------------------------------------------------------------------------------------------------------
        private Page AssocZWaveDevice(string key)
        {
            return new Page()
            {
                Uri = "AssocZWaveDevice",
                Name = "AssocZWaveDevice",
                Title = "AssocZWaveDevice",
                Controls = new ListTS<Control>()
                {
                    new UI.Forms.Controls.StackPanel()
                    {
                        Items = new ListTS<Control>()
                        {
                             new UI.Forms.Controls.Label()
                             {
                                 Text= " Group Id",
                             },
                             new UI.Forms.Controls.TextBox()
                             {
                                 //TODO: 
                                 Name= "groupId",
                                 Text= "",
                             },
                             new UI.Forms.Controls.Label()
                             {
                                 Text= " Target Node Id",
                             },
                             new UI.Forms.Controls.TextBox()
                             {
                                 //TODO: 
                                 Name= "targetNodeId",
                                 Text= "",
                             },
                             new UI.Forms.Controls.Button()
                             {
                                 Tag = key,
                                 Text = "Associate",
                                 Clicked =  btnAssociate_Clicked ,
                             },
                        }
                    }
                },
            };
        }
        //------------------------------------------------------------------------------------------------------------------------
        void btnAssociate_Clicked(NavigationContext ctx, Button control, UIEvent args)
        {
            var key = control?.TagStr;
            DebugEx.TraceLog("Button Navigate to Associate clicked: " + key);
            if (key != null && ctx != null)
            {
                TextBox groupId = ctx.CurrentPage.FindControl<TextBox>("groupId");
                TextBox targetnodeid = ctx.CurrentPage.FindControl<TextBox>("targetNodeId");
                if (groupId != null && targetnodeid != null)
                    try
                    {
                        byte nodeid = (byte)uint.Parse(key);
                        byte groupid = (byte)uint.Parse(groupId.Text);
                        byte targetNID = (byte)uint.Parse(targetnodeid.Text);
                        byte instance = 1;

                        DebugEx.TraceLog("GroupId " + groupId.Text);
                        Native.ChangeGroupAssociation(homeID, nodeid, targetNID, instance, groupid);
                    }
                    catch (Exception ex) { DebugEx.Assert(ex); }
                else
                    DebugEx.Assert("grouId was " + groupId + " and target node id was " + targetnodeid);

                //return to main
                ctx.GoBack();
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        private Page createRemoveZWaveDevice()
        {
            return new Page()
            {
                Uri = "Remove ZWave Devices",
                Name = "Remove ZWave Devices",
                Title = "Remove ZWave Devices",
                Controls = new ListTS<Control>()
                {
                    new UI.Forms.Controls.StackPanel()
                    {
                        Items = new ListTS<Control>()
                        {
                             new UI.Forms.Controls.Label()
                             {
                                 Text= "The Z-Wave Bridge is in Discovery mode, press the button three times and wait some seconds",
                             },
                             new UI.Forms.Controls.Button()
                             {
                                 Text = "Stop Removing Nodes",
                                 Clicked = (ctx,s,args) =>  {
                                    try
                                     {
                                         if (Native.CancelOperation(homeID))
                                             foreach (var navctx in NavigationContext.Values)
                                             {
                                                 if (navctx.CurrentPage.Title == "Remove ZWave Devices")
                                                 {
                                                     navctx.GoBack();
                                                     navctx.UpdateCurrentPage(createMainPage());
                                                 }
                                             }
                                     }
                                     catch (Exception ex) { DebugEx.TraceErrorException(ex, ex.StackTrace); }
                                 }
                             },
                        }
                    }
                }
            };
        }
        //------------------------------------------------------------------------------------------------------------------------
        private Page AddZWaveDevice()
        {
            return new Page()
            {
                Uri = "Add ZWave Devices",
                Name = "Add ZWave Devices",
                Title = "Add ZWave Devices",
                Controls = new ListTS<Control>()
                {
                    new UI.Forms.Controls.StackPanel()
                    {
                        Items = new ListTS<Control>()
                        {
                             new UI.Forms.Controls.Label()
                             {
                                 Text= "The Z-Wave Bridge is on Discovery mode, press the button three times and wait some seconds",
                             },
                             new UI.Forms.Controls.Button()
                             {
                                 Text = "Stop Adding Nodes",
                                 Clicked = (ctx,s,args) => {
                                     try
                                     {
                                         Native.CancelOperation(homeID);
                                         foreach (var navctx in NavigationContext.Values)
                                         {
                                             if (navctx.CurrentPage?.Title == "Add ZWave Devices")
                                             {
                                                 navctx.GoBack();
                                                 navctx.UpdateCurrentPage(createMainPage());
                                             }
                                         }
                                     }
                                     catch (Exception ex) { DebugEx.TraceErrorException(ex, ex.StackTrace); }
                                 }
                            },
                        }
                    }
                }
            };
        }
        //------------------------------------------------------------------------------------------------------------------------
        private Page PrepareZWaveBridge(string op)
        {
            return new Page()
            {
                Uri = "Prepare ZWaveBridge",
                Name = "Prepare ZWaveBridge",
                Title = "Prepare ZWaveBridge",
                Controls = new ListTS<Control>()
                {
                    new UI.Forms.Controls.StackPanel()
                    {
                        Items = new ListTS<Control>()
                        {
                             new UI.Forms.Controls.Label()
                             {
                                 Text= "The Z-Wave Bridge is getting ready for operation " + op,
                             },
                        }
                    }
                }
            };
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Filtering Spammy Devices
        //------------------------------------------------------------------------------------------------------------------------
        private bool FilteringSmartPlug(MsgFromCpp msg, out PlugMeteringType MeterType)
        {
            /* Send For metering reports only:
             * index 0 : Energy - kWh
             * index 8 : Power - Watt
             * index 16: Voltage - V
             * index 20: Current - A
             */
            MeterType = PlugMeteringType.none;
            if (msg.commandClassId == CommandClassId.METER)
            {
                switch (msg.valueIndex)
                {
                    case 8:
                        MeterType = PlugMeteringType.W;
                        break;
                    case 16:
                        MeterType = PlugMeteringType.V;
                        break;
                    case 20:
                        MeterType = PlugMeteringType.A;
                        break;
                    case 0:
                        MeterType = PlugMeteringType.kWh;
                        break;
                    default:
                        return false;
                }
            }
            return true;
        }
        //------------------------------------------------------------------------------------------------------------------------

        #endregion

        #endregion
    }

    public class ZWaveDevicesConfig
    {
        public DictionaryTS<Byte, ZWaveDescriptor> pairedDevices = new DictionaryTS<Byte, ZWaveDescriptor>();
    }

    public class ZWaveDescriptor
    {
        public Byte NodeId;
        public string ProductName;
        public Dictionary<CommandClassId, Dictionary<string, ValueID>> values;
        public ZWaveDescriptor() { values = new Dictionary<CommandClassId, Dictionary<string, ValueID>>(); ConfigIDs = new List<byte>(); }
        public List<Byte> ConfigIDs;

        [JsonIgnore]
        public Thing Thing;
    }
}