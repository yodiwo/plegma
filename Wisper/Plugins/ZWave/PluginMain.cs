using Newtonsoft.Json;
using NLog;
using NLog.Config;
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
using ZWaveLib;
using ZWaveLib.CommandClasses;

namespace Yodiwo.mNode.Plugins.Bridge_ZWave
{
    public class PluginMain : Plugin
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        public ZWaveLib.ZWaveController controller;
        [NonSerialized]
        Dictionary<string, Port> ZWaveThingPorts = new Dictionary<string, Port>();
        HashSetTS<string> NodeIds = new HashSetTS<string>();
        DictionaryTS<int, ZWaveDescriptor> ZWaveDevices = new DictionaryTS<int, ZWaveDescriptor>();
        public ZWaveDevicesConfig PluginCfg;
        private YConfig<ZwavePluginConfig> YConfig;
        public ZwavePluginConfig ActiveCfg;
        public static LoggingRule loggingRule = LogManager.Configuration.LoggingRules[0];
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public override bool Initialize(mNodeConfig mNodeConfig, string PluginConfig, string UserConfig)
        {
            //load config for avmedia plugin
            //var plwdir=  PluginWorkingDirectory;
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

            if (PluginCfg != null && PluginCfg.pairedDevices.Count > 0)
            {
                DebugEx.TraceLog("Start Construct ZWave Device");
                foreach (var zwavedev in PluginCfg.pairedDevices)
                {
                    var thing = ConstructZWaveDesc(zwavedev.Key, zwavedev.Value.BasicType, zwavedev.Value.GenericType);
                    zwavedev.Value.thing = AddThing(thing);
                }
            }

            //init base
            if (base.Initialize(mNodeConfig, PluginConfig, UserConfig) == false)
                return false;

            //init
            try
            {
                //init plugin
                InitZwaveBridge();

                //add things
                SetupZWaveBridgeThing();
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Could not init"); }

            //done
            return true;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override bool Deinitialize()
        {
            try
            {
                base.Deinitialize();
                //deinit client
                try { DeInitZwaveBridge(); } catch { }
                return true;
            }
            catch { return false; }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void OnTransportConnected(string msg)
        {
            base.OnTransportConnected(msg);
            //Discovery();
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void InitZwaveBridge()
        {
            DebugEx.TraceLog("========>Zwave Serial Port: <=======" + this.ActiveCfg.SerialPort);
            controller = new ZWaveController(this.ActiveCfg.SerialPort);
            controller.ControllerStatusChanged += Controller_ControllerStatusChanged;
            controller.DiscoveryProgress += Controller_DiscoveryProgress;
            controller.NodeOperationProgress += Controller_NodeOperationProgress;
            controller.NodeUpdated += Controller_NodeUpdated;
            controller.Connect();
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void Controller_NodeOperationProgress(object sender, NodeOperationProgressEventArgs args)
        {
            DebugEx.TraceLog(string.Format("NodeOperationProgress {0} {1}", args.NodeId, args.Status));

            var nodeId = args.NodeId;
            var Status = args.Status;

            if (Status == NodeQueryStatus.NodeAddDone)
            {
                try
                {
                    NodeIds.Add(nodeId.ToString());
                    var node = controller.GetNode(nodeId);
                    OnZwaveDeviceAdded(nodeId);
                    foreach (var navctx in NavigationContext.Values)
                    {
                        if (navctx.CurrentPage?.Title == "Add ZWave Devices")
                        {
                            Task.Run(() => controller.StopNodeAdd());
                            navctx.GoBack();
                            navctx.UpdateCurrentPage(createMainPage());
                        }
                    }
                }
                catch (Exception ex) { DebugEx.TraceErrorException(ex); }
            }
            else if (Status == NodeQueryStatus.NodeAddReady)
            {
                try
                {
                    foreach (var navctx in NavigationContext.Values)
                    {
                        if (navctx.CurrentPage.Title == "Prepare ZWaveBridge")
                        {
                            navctx.UpdateCurrentPage(AddZWaveDevice());
                        }
                    }
                }
                catch (Exception ex) { DebugEx.TraceErrorException(ex); }
            }
            else if (Status == NodeQueryStatus.NodeAdded)
                try
                {
                    NodeIds.Add(nodeId.ToString());
                }
                catch (Exception ex) { DebugEx.TraceErrorException(ex); }
            else if (Status == NodeQueryStatus.NodeRemoveDone)
                try
                {
                    NodeIds.Remove(nodeId.ToString());
                    OnZwaveDeviceRemoved(nodeId);
                    foreach (var navctx in NavigationContext.Values)
                    {
                        if (navctx.CurrentPage.Title == "Remove ZWave Devices")
                        {
                            Task.Run(() => controller.StopNodeRemove());
                            navctx.GoBack();
                            navctx.UpdateCurrentPage(createMainPage());
                        }
                    }
                }
                catch (Exception ex) { DebugEx.TraceErrorException(ex); }
            else if (Status == NodeQueryStatus.NodeRemoved)
                try
                {
                    NodeIds.Remove(nodeId.ToString());
                }
                catch (Exception ex) { DebugEx.TraceErrorException(ex); }
            else if (Status == NodeQueryStatus.NodeRemoveReady)
            {
                try
                {
                    foreach (var navctx in NavigationContext.Values)
                    {
                        if (navctx.CurrentPage.Title == "Prepare ZWaveBridge")
                        {
                            navctx.UpdateCurrentPage(createRemoveZWaveDevice());
                        }
                    }
                }
                catch (Exception ex) { DebugEx.TraceErrorException(ex); }
            }

        }
        //------------------------------------------------------------------------------------------------------------------------
        private void OnZwaveDeviceRemoved(int nodeId)
        {

            if (ZWaveDevices.ContainsKey(nodeId))
            {
                DeleteThing(ZWaveDevices[nodeId].thing.ThingKey);
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
        private void OnZwaveDeviceAdded(int nodeid)
        {
            TaskEx.RunSafe(() =>
            {
                ZWaveDescriptor zwavedesc;
                if (ZWaveDevices.TryGetValue(nodeid, out zwavedesc))
                {
                    DebugEx.TraceLog("Add Zwave Thing");
                    AddThing(zwavedesc.thing);
                    UpdatePluginConfig();
                }
            });
        }

        //------------------------------------------------------------------------------------------------------------------------
        private void Controller_DiscoveryProgress(object sender, DiscoveryProgressEventArgs args)
        {
            DebugEx.TraceLog(string.Format("DiscoveryProgress {0}", args.Status));
            switch (args.Status)
            {
                case DiscoveryStatus.DiscoveryStart:
                    break;
                case DiscoveryStatus.DiscoveryEnd:
                    break;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void Controller_ControllerStatusChanged(object sender, ControllerStatusEventArgs args)
        {
            DebugEx.TraceLog(string.Format("ControllerStatusChange {0}", args.Status));
            ToggleDebug(true);
            var controller = (sender as ZWaveController);
            var controllerStatus = args.Status;
            switch (controllerStatus)
            {
                case ControllerStatus.Connected:
                    // Initialize the controller and get the node list
                    controller.GetControllerInfo();
                    controller.GetControllerCapabilities();
                    controller.GetHomeId();
                    controller.GetSucNodeId();
                    controller.Initialize();
                    break;
                case ControllerStatus.Disconnected:
                    break;
                case ControllerStatus.Initializing:
                    break;
                case ControllerStatus.Ready:
                    break;
                case ControllerStatus.Error:
                    break;
            }
            ToggleDebug(false);
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void ToggleDebug(bool showDebugmsg)
        {
            LogManager.Configuration.LoggingRules.Remove(loggingRule);
            LogManager.Configuration.Reload();
            if (showDebugmsg)
            {
                LogManager.Configuration.LoggingRules.Add(loggingRule);
                LogManager.Configuration.Reload();
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void DeInitZwaveBridge()
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        #region Yodiwo Zwave Node specific methods
        //------------------------------------------------------------------------------------------------------------------------
        public static readonly string PORTID_WRITEVAL = "0";
        public static readonly string PORTID_NODEIN = "1";
        public static readonly string PORTID_NODEOUT = "2";
        public static readonly string PORTID_READTYPE = "3";
        public static readonly string PORTID_READVAL = "4";
        public static readonly string PORTID_CONFIGREG = "5";
        public static readonly string PORTID_CONFIGVAL = "6";
        public static readonly string PORTID_NODEADD = "7";
        public static readonly string PORTID_NODEDEL = "8";
        public static readonly string PORTID_NODEGET = "9";
        public static readonly string PORTID_ASSOCNODE = "10";
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

            //create ports and add to local dictionary
            ZWaveThingPorts.Add(PORTID_WRITEVAL, new Yodiwo.API.Plegma.Port()
            {
                ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                Name = "Write Value",
                State = "",
                ConfFlags = ePortConf.PropagateAllEvents,
                Type = Yodiwo.API.Plegma.ePortType.Integer,
                PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", PORTID_WRITEVAL)
            });
            ZWaveThingPorts.Add(PORTID_NODEIN, new Yodiwo.API.Plegma.Port()
            {
                ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                Name = "Node In",
                State = "",
                ConfFlags = ePortConf.PropagateAllEvents,
                Type = Yodiwo.API.Plegma.ePortType.Integer,
                PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", PORTID_NODEIN)
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
            ZWaveThingPorts.Add(PORTID_CONFIGREG, new Yodiwo.API.Plegma.Port()
            {
                ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                Name = "Config Register",
                State = "",
                ConfFlags = ePortConf.PropagateAllEvents,
                Type = Yodiwo.API.Plegma.ePortType.Integer,
                PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", PORTID_CONFIGREG)
            });
            ZWaveThingPorts.Add(PORTID_CONFIGVAL, new Yodiwo.API.Plegma.Port()
            {
                ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                Name = "Config Value",
                State = "",
                ConfFlags = ePortConf.PropagateAllEvents,
                Type = Yodiwo.API.Plegma.ePortType.Integer,
                PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", PORTID_CONFIGVAL)
            });
            ZWaveThingPorts.Add(PORTID_NODEADD, new Yodiwo.API.Plegma.Port()
            {
                ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                Name = "Add ZWave Node",
                State = "",
                ConfFlags = ePortConf.PropagateAllEvents,
                Type = Yodiwo.API.Plegma.ePortType.Integer,
                PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", PORTID_NODEADD)
            });
            ZWaveThingPorts.Add(PORTID_NODEDEL, new Yodiwo.API.Plegma.Port()
            {
                ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                Name = "Remove ZWave Node",
                State = "",
                ConfFlags = ePortConf.PropagateAllEvents,
                Type = Yodiwo.API.Plegma.ePortType.Integer,
                PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", PORTID_NODEDEL)
            });
            ZWaveThingPorts.Add(PORTID_NODEGET, new Yodiwo.API.Plegma.Port()
            {
                ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                Name = "Node Get",
                State = "",
                ConfFlags = ePortConf.PropagateAllEvents,
                Type = Yodiwo.API.Plegma.ePortType.Integer,
                PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", PORTID_NODEGET)
            });
            ZWaveThingPorts.Add(PORTID_ASSOCNODE, new Yodiwo.API.Plegma.Port()
            {
                ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                Name = "Assoc Node",
                State = "",
                ConfFlags = ePortConf.PropagateAllEvents,
                Type = Yodiwo.API.Plegma.ePortType.Integer,
                PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", PORTID_ASSOCNODE)
            });
            //also add ports to thing
            thing.Ports.AddFromSource(ZWaveThingPorts.Values);

            //add
            thing = AddThing(thing);

            return thing;
        }
        //------------------------------------------------------------------------------------------------------------------------

        public void OnZWaveDevicePortEvent(PortEvent pe, int NodeId)
        {
            try
            {
                ushort valuein;
                try { valuein = ushort.Parse(pe.State); } catch { return; }

                if (valuein != 128)                 //why this much dislike for 128?
                {
                    SendValue((byte)NodeId, valuein);
                }
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------

        public void OnZWaveBridgePortEvent(PortEventMsg msg)
        {
            ushort valuein = 0;
            ushort configregin = 0;
            ushort configvaluein = 0;
            int nodein = 0;
            bool writeexecute1 = false;
            bool writeexecute2 = false;
            bool configexecute = false;
            int assocnode = 0;
            int getexecute = 0;

            foreach (var pe in msg.PortEvents)
            {
                var pId = ((PortKey)pe.PortKey).PortUID;
                if (!ZWaveThingPorts.ContainsKey(pId))
                {
                    DebugEx.TraceError($"Received msg {pe.State} for unknown portkey {pe.PortKey}");
                }

                if (PORTID_NODEIN == pId)
                {
                    nodein = ushort.Parse(pe.State);
                    writeexecute1 = true;
                }
                else if (PORTID_WRITEVAL == pId)
                {
                    valuein = ushort.Parse(pe.State);
                    if (valuein != 128)                 //why this much dislike for 128?
                        writeexecute2 = true;
                }
                else if (PORTID_CONFIGREG == pId)
                {
                    configregin = ushort.Parse(pe.State);
                }
                else if (PORTID_CONFIGVAL == pId)
                {
                    configvaluein = ushort.Parse(pe.State);
                }
                else if (PORTID_NODEADD == pId)
                {
                    var tmp = ushort.Parse(pe.State);
                    if (tmp == 1)
                        controller.BeginNodeAdd();
                    else if (tmp == 0)
                        controller.StopNodeAdd();
                }
                else if (PORTID_NODEDEL == pId)
                {
                    var tmp = ushort.Parse(pe.State);
                    if (tmp == 1)
                        controller.BeginNodeRemove();
                    else if (tmp == 0)
                        controller.StopNodeRemove();
                }
                else if (PORTID_NODEGET == pId)
                {
                    var tmp = ushort.Parse(pe.State);
                    if (tmp != 128)
                        getexecute = tmp;
                }
                else if (PORTID_ASSOCNODE == pId)
                {
                    var tmp = ushort.Parse(pe.State);
                    if (tmp != 128 && tmp != 0)
                        assocnode = tmp;
                }
            }

            if (controller == null)
            {
                DebugEx.Assert("ZWAVE controller ref is null!");
                return;
            }
            if (writeexecute1 && writeexecute2)
            {
                SendValue((byte)nodein, valuein);
                DebugEx.TraceLog($"Sent {valuein} for {nodein} to zwave controller");
            }
            if (getexecute != 0)
            {
                var node = controller.GetNode(Convert.ToByte(getexecute));
                if (node != null)
                {
                    var zmsg = SensorMultilevel.Get(node);
                    DebugEx.TraceLog($"Sent getExec to SensorMultiLevel zwave controller for ZWave Node Id {node.Id}");
                }
                else
                    DebugEx.TraceError($"getExec fail: ZWave Node not found for id {getexecute}");
            }
            if (configexecute)
            {
                SendConfig((byte)nodein, (byte)configregin, configvaluein);
                DebugEx.TraceLog($"Sent {configregin} for {nodein} to zwave controller");
            }
            if (assocnode != 0)
            {
                var grpId = valuein == 0 ? 1 : valuein;
                DebugEx.TraceLog($"before assoc: nodeId: {assocnode}, GrpdId: {grpId}");
                AssocNodeAdd((byte)assocnode, (byte)grpId);
                DebugEx.TraceLog($"Sent assoc: nodeId {assocnode} grpId {grpId} to zwave controller");
            }
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

                    foreach (var zwave in ZWaveDevices)
                    {
                        var ports = zwave.Value.thing.Ports;
                        var targetp = ports.Where(p => p.PortKey == pe.PortKey).FirstOrDefault();
                        if (targetp != null)
                        {
                            OnZWaveDevicePortEvent(pe, Convert.ToInt16(zwave.Value.NodeId));
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
            catch (Exception ex)
            {
                DebugEx.Assert(ex);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        private void AssocNodeAdd(byte assocnode, byte grpId)
        {
            ToggleDebug(true);
            var controllernode = controller.GetNode(assocnode);
            Association.Set(controllernode, grpId, 1);
            ToggleDebug(false);
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void SendConfig(byte nodein, byte configregin, ushort configvaluein)
        {
            var nodeitem = controller.GetNode(nodein);
            Configuration.Set(nodeitem, configregin, configvaluein);
            Configuration.Get(nodeitem, configregin);
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void SendValue(byte nodein, ushort valuein)
        {
            var nodeitem = controller.GetNode(nodein);
            SwitchBinary.Set(nodeitem, valuein);
        }
        //------------------------------------------------------------------------------------------------------------------------
        void Controller_NodeUpdated(object sender, NodeUpdatedEventArgs args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            DebugEx.TraceLog(string.Format("NodeUpdated {0} Event Parameter {1} Value {2}", args.NodeId, args.Event.Parameter, args.Event.Value));
            Console.ForegroundColor = ConsoleColor.White;
            if (args.Event.Parameter == EventParameter.NodeInfo)
            {
                DebugEx.TraceLog(string.Format("NodeUpdated {0} Event Parameter {1} Node Info Frame {2}", args.NodeId.ToStringInvariant(), args.Event.Parameter, args.Event.Value));
                string nodeframe = args.Event.Value as string;
                String[] arr = nodeframe.Split(' ');
                byte[] message = new byte[arr.Length];
                for (int i = 0; i < arr.Length; i++) message[i] = Convert.ToByte(arr[i], 16);
                DebugEx.TraceLog("=======>NodeUpdated Final NodeInfoFrame<==== :" + BitConverter.ToString(message));
                var basicType = (GenericType)message[0];
                var genericType = (GenericType)message[1];
                DebugEx.TraceLog(string.Format("Node Info Frame Basic Type {0}", basicType));
                DebugEx.TraceLog(string.Format("Node Info Frame Generic Type {0}", genericType));
                ExtractNodeInfo(args.NodeId, basicType, genericType);
            }

            ZWaveDescriptor zwavedesc = null;
            var portEvents = new List<TupleS<string, string>>();

            if (ZWaveDevices.TryGetValue(args.NodeId, out zwavedesc))
            {
                DebugEx.TraceLog("ZWAVE DEVICE Found in cache");
                var ports = zwavedesc.thing.Ports.Where(p => p.ioDirection == ioPortDirection.Output)?.ToList();

                if (ports != null)
                {
                    var typepk = ports.Find(p => p.Name == "Type").PortKey;
                    DebugEx.TraceLog(string.Format("TypeKey:{0},", typepk));

                    var valuepk = ports.Find(p => p.Name == "Value").PortKey;
                    DebugEx.TraceLog(string.Format("valuekey:{0}", valuepk));

                    if (!string.IsNullOrEmpty(typepk) && !string.IsNullOrEmpty(valuepk))
                    {
                        portEvents.Add(new TupleS<string, string>(typepk, args.Event.Parameter.ToStringInvariant()));
                        portEvents.Add(new TupleS<string, string>(valuepk, args.Event.Value.ToStringInvariant()));
                    }
                }
            }

            //We want the Z-Wave Bridge Thing to also receive the events even if they match a targeted thing
            portEvents.Add(new TupleS<string, string>(ZWaveThingPorts[PORTID_NODEOUT].PortKey, args.NodeId.ToStringInvariant()));
            portEvents.Add(new TupleS<string, string>(ZWaveThingPorts[PORTID_READTYPE].PortKey, args.Event.Parameter.ToStringInvariant()));
            portEvents.Add(new TupleS<string, string>(ZWaveThingPorts[PORTID_READVAL].PortKey, args.Event.Value.ToStringInvariant()));

            SetPortState(portEvents);

            if (args.Event.NestedEvent != null)
            {
                Controller_NodeUpdated_nested(sender, args.Event.NestedEvent);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        void Controller_NodeUpdated_nested(object sender, NodeEvent args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("NodeUpdated {0} Event Parameter {1} Value {2}", args.Node.Id, args.Parameter, args.Value);
            Console.ForegroundColor = ConsoleColor.White;

            ZWaveDescriptor zwavedesc = null;
            var portEvents = new List<TupleS<string, string>>();

            if (ZWaveDevices.TryGetValue(args.Node.Id, out zwavedesc))
            {
                var typepk = zwavedesc.thing.Ports.Find(p => p.Name == "Type")?.PortKey;
                var valuepk = zwavedesc.thing.Ports.Find(p => p.Name == "Value")?.PortKey;

                if (!string.IsNullOrEmpty(typepk) && !string.IsNullOrEmpty(valuepk))
                {
                    portEvents.Add(new TupleS<string, string>(typepk, args.Parameter.ToStringInvariant()));
                    portEvents.Add(new TupleS<string, string>(valuepk, args.Value.ToStringInvariant()));
                }
            }

            //We want the Z-Wave Bridge Thing to also receive the events even if they match a targeted thing
            portEvents.Add(new TupleS<string, string>(ZWaveThingPorts[PORTID_NODEOUT].PortKey, args.Node.Id.ToStringInvariant()));
            portEvents.Add(new TupleS<string, string>(ZWaveThingPorts[PORTID_READTYPE].PortKey, args.Parameter.ToStringInvariant()));
            portEvents.Add(new TupleS<string, string>(ZWaveThingPorts[PORTID_READVAL].PortKey, args.Value.ToStringInvariant()));

            SetPortState(portEvents);

            if (args.Parameter == EventParameter.Association)
            {
                DebugEx.TraceLog("========>Association<===============");
                foreach (var nvctx in NavigationContext.Values)
                {
                    if (nvctx.CurrentPage.Title == "AssocZWaveDevice")
                    {
                        nvctx.GoBack();
                        nvctx.UpdateCurrentPage(createMainPage());
                    }
                }
            }
            if (args.NestedEvent != null)
            {
                Controller_NodeUpdated_nested(sender, args.NestedEvent);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------

        private void ExtractNodeInfo(int nodeId, GenericType basicType, GenericType genericType)
        {
            try
            {
                if (basicType != GenericType.None && basicType != GenericType.None)
                    ConstructZWaveDesc(nodeId, basicType, genericType);

            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        private Thing ConstructZWaveDesc(int nodeId, GenericType basicType, GenericType genericType)
        {
            ZWaveDescriptor zwaveDesc;
            DebugEx.TraceLog("Before check Construct ZWave Device");
            if (!ZWaveDevices.TryGetValue(nodeId, out zwaveDesc))
            {
                DebugEx.TraceLog("Construct new Zwave Device");
                var thing = CreateZWaveThing(nodeId, basicType, genericType);
                //AddThing(thing);
                zwaveDesc = new ZWaveDescriptor()
                {
                    NodeId = nodeId.ToString(),
                    BasicType = basicType,
                    GenericType = genericType,
                    thing = thing,
                };
                ZWaveDevices.Add(nodeId, zwaveDesc);
                return thing;
            }
            return null;
        }
        //------------------------------------------------------------------------------------------------------------------------
        private Thing CreateZWaveThing(int nodeId, GenericType basicType, GenericType genericType)
        {
            DebugEx.TraceLog("Create Zwave Thing " + nodeId.ToString() + " " + basicType.ToString() + " " + genericType.ToString());
            var thingkey = new ThingKey(NodeKey, PluginMain.ThingKeyPrefix + "_" + genericType.ToString() + "_" + basicType.ToString() + "_" + nodeId.ToString());

            var thing = new Thing()
            {
                ThingKey = thingkey,
                Name = "ZWave-" + genericType.ToString() + "-" + nodeId.ToString(),
                ConfFlags = eThingConf.Removable,
                UIHints = new ThingUIHints()
                {
                    Description = "ZWave Device " + genericType.ToString() + "-" + nodeId.ToString() + ".",
                    IconURI = "/Content/img/icons/Generic/icon-zwave-logo.png"
                },
            };
            if (genericType == GenericType.GarageDoor)
            {
                thing.UIHints.IconURI = "/Content/img/icons/Generic/icon-door-sensor.png";

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
            }
            else if (genericType == GenericType.Meter || genericType == GenericType.SwitchBinary || genericType == GenericType.SwitchMultilevel)
            {
                thing.UIHints.IconURI = "/Content/img/icons/Generic/icon-smart-plug.png";

                thing.Ports = new List<Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                        Name = "Value",
                        State = "",
                        ConfFlags = ePortConf.PropagateAllEvents,
                        Type = Yodiwo.API.Plegma.ePortType.Integer,
                        PortKey = new PortKey(thingkey, "WRITEVAL"),
                    },
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "Type",
                        State = "",
                        ConfFlags = ePortConf.PropagateAllEvents,
                        Type = Yodiwo.API.Plegma.ePortType.String,
                        PortKey = new PortKey(thingkey, "READTYPE"),

                    },

                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "Value",
                        State = "",
                        ConfFlags = ePortConf.PropagateAllEvents,
                        Type = Yodiwo.API.Plegma.ePortType.String,
                        PortKey = new PortKey(thingkey, "READVAL"),

                    }
                };
            }
            return thing;
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
        public Page createMainPage()
        {
            //create main page
            //onvifs
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
                stack?.Items.Add(
                    new UI.Forms.Controls.Label()
                    {
                        Text = "Paired ZWave Devices(s)",
                    });

                foreach (var zwave in ZWaveDevices)
                {
                    stack?.Items.Add(
                        new UI.Forms.Controls.Button()
                        {
                            Tag = zwave.Key.ToString(),
                            Text = zwave.Key.ToString() + "_" + zwave.Value.GenericType,
                            Clicked = btNavToAssociate_Clicked
                        });
                }
            }
            return page;
        }
        //------------------------------------------------------------------------------------------------------------------------
        void btnAdd_Clicked(NavigationContext ctx, Button control, UIEvent args)
        {
            Task.Run(() =>
            {
                //TaskEx.RunSafe(controller.StopNodeRemove).Wait(1000);
                //TaskEx.RunSafe(controller.StopNodeAdd).Wait(1000);
                controller.BeginNodeAdd();
            });
            ctx.NavigateTo(PrepareZWaveBridge("addition"));
        }
        //------------------------------------------------------------------------------------------------------------------------
        void btnRemove_Clicked(NavigationContext ctx, Button control, UIEvent args)
        {
            Task.Run(() =>
            {
                // TaskEx.RunSafe(controller.StopNodeRemove).Wait(1000);
                // TaskEx.RunSafe(controller.StopNodeAdd).Wait(1000);
                controller.BeginNodeRemove();
            });
            ctx.NavigateTo(PrepareZWaveBridge("removal"));
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
                DebugEx.TraceLog("GroupId " + groupId.Text);
                if (groupId != null)
                    try { AssocNodeAdd((byte)ushort.Parse(key), (byte)ushort.Parse(groupId.Text)); }
                    catch (Exception ex) { DebugEx.Assert(ex); }
                else
                    DebugEx.Assert("Null grouId detected");

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
                                 Clicked = (ctx,s,args) => TaskEx.RunSafe(controller.StopNodeRemove),
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
                                 Clicked = (ctx,s,args) => TaskEx.RunSafe(controller.StopNodeAdd),
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
        #endregion

    }

    public class ZWaveDevicesConfig
    {
        public DictionaryTS<int, ZWaveDescriptor> pairedDevices = new DictionaryTS<int, ZWaveDescriptor>();
    }

    public class ZWaveDescriptor
    {
        public string NodeId;
        public GenericType BasicType;
        public GenericType GenericType;
        [JsonIgnore]
        public Thing thing;
    }
}

