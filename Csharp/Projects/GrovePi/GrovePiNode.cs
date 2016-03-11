using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.NodeLibrary;
using Yodiwo.API.Plegma;

namespace Yodiwo.Projects.GrovePi
{
    public class GrovePiNode
    {
        private YConfig<GrovePiNodeConfig> YConfig;
        public static GrovePiNodeConfig ActiveCfg;
        Yodiwo.API.Plegma.NodeKey NodeKey { get { return ActiveCfg.NodeKey; } }
        Yodiwo.NodeLibrary.Node node;
        NodeLibrary.Transport transport = NodeLibrary.Transport.YPCHANNEL;
        private Dictionary<ThingKey, Thing> GroveThings = new Dictionary<ThingKey, Thing>();
        Transport pysharp;

        public GrovePiNode(Transport trans)
        {
            this.pysharp = trans;
        }


        public void Start()
        {
            YConfig = Config.Init();
            ActiveCfg = YConfig.GetActiveConf();

            NodeConfig conf = new NodeConfig()
            {
                uuid = ActiveCfg.Uuid,
                Name = "GrovePi Node",
                MqttBrokerHostname = ActiveCfg.MqttBrokerHostname,
                MqttUseSsl = ActiveCfg.MqttUseSsl,
                YpServer = ActiveCfg.ApiServer,
                YpchannelPort = ActiveCfg.YpchannelPort,
                SecureYpc = ActiveCfg.YpchannelSecure,
                FrontendServer = ActiveCfg.FrontendServer,
                CanSolveGraphs = false,// deactivate for GrovePi
            };

            //prepare pairing module
            var pairmodule = new Yodiwo.Node.Pairing.NancyPairing.NancyPairing();

            //prepare node graph manager module
            var nodeGraphManager = new Yodiwo.NodeLibrary.Graphs.NodeGraphManager(
                                                new Type[]
                                                    {
                                                        typeof(Yodiwo.Logic.BlockLibrary.Basic.Librarian),
                                                        typeof(Yodiwo.Logic.BlockLibrary.Extended.Librarian),
                                                    });

            //create node
            node = new Yodiwo.NodeLibrary.Node(conf,
                                               Helper.GatherThings(this.pysharp),
                                               pairmodule,
                                               null,
                                               null,
                                               NodeGraphManager: nodeGraphManager
                                               );
            Helper.node = node;
            //set TransPort
            node.Transport = transport;

            //register callbacks for grovepi node
            node.OnChangedState += OnChangedStateCb;
            node.OnNodePaired += OnPaired;
            node.OnTransportConnected += OnTransportConnectedCb;
            node.OnTransportDisconnected += OnTransportDisconnectedCb;
            node.OnTransportError += OnTransportErrorCb;
            node.OnUnexpectedMessage = OnUnexpectedMessageCb;
            node.OnPortActivated += OnPortActivatedCb;
            node.OnPortDeactivated += OnPortDeactivatedCb;
            node.OnThingUpdated += OnThingUpdated;


            //register port event handlers
            RegisterThingsHandlers();

            //start Pairing
            if (String.IsNullOrWhiteSpace(ActiveCfg.NodeKey))
            {
                DebugEx.TraceLog("Starting pairing procedure.");
                var pair = node.StartPairing(ActiveCfg.FrontendServer, null, ActiveCfg.LocalWebServer).GetResults();
            }
            else
            {
                node.SetupNodeKeys(ActiveCfg.NodeKey, ActiveCfg.NodeSecret);
                DebugEx.TraceLog("Node already paired: NodeKey = "
                    + ActiveCfg.NodeKey + ", NodeSecret = ", ActiveCfg.NodeSecret);
            }

            //connect
            node.Connect();
        }

        private void OnTransportConnectedCb(NodeLibrary.Transport Transport, string msg)
        {
            DebugEx.TraceLog("OnConnected transport=" + Transport.ToString() + " msg=" + msg);
        }

        private void OnPortDeactivatedCb(PortKey pkey)
        {
            var targetThing = Helper.Things.Find(i => i.ThingKey == pkey.ThingKey);
            if (targetThing == null)
                return;
            var targetPort = targetThing.GetPort(pkey);
            if (targetPort == null)
                return;

            DebugEx.TraceLog("DEACTIVATED: " + pkey);
            if (targetPort.ioDirection == ioPortDirection.Output ||
                targetPort.ioDirection == ioPortDirection.InputOutput)
            {
                var groveBaseSensor = Helper.Lookup.TryGetOrDefault(targetThing);
                if (groveBaseSensor != null)
                    groveBaseSensor.StopMonitoring();
            }
        }

        private void OnPortActivatedCb(PortKey pkey)
        {
            var targetThing = Helper.Things.Find(i => i.ThingKey == pkey.ThingKey);
            if (targetThing == null)
                return;
            var targetPort = targetThing.GetPort(pkey);
            if (targetPort == null)
                return;

            DebugEx.TraceLog("ACTIVATED: " + pkey);
            if (targetPort.ioDirection == ioPortDirection.Output ||
                targetPort.ioDirection == ioPortDirection.InputOutput)
            {
                var groveBaseSensor = Helper.Lookup.TryGetOrDefault(targetThing);
                if (groveBaseSensor != null)
                    groveBaseSensor.ReadContinuously();
            }
        }


        //TODO: check sensor constructor values

        private void OnThingUpdated(Thing thing, Thing oldCopy)
        {
            var changed = false;
            var sensor = Helper.Lookup.TryGetOrDefault(thing);
            if (sensor == null)
            {
                DebugEx.TraceError("did not find sensor to update");
                return;
            }
            for (int i = 0; i < thing.Config.Count; i++)
            {
                if (oldCopy.Config.ElementAt(i).Value != thing.Config.ElementAt(i).Value)
                {
                    if (thing.Config.ElementAt(i).Name == "Pin")
                    {
                        //Pin changed; find and assign new one
                        var newPin = GrovePiSensor.PinNameToPin.TryGetOrDefault(thing.Config.ElementAt(i).Value, Pin.Unknown);
                        if (newPin == Pin.Unknown)
                            continue;
                        if (sensor.Pin == newPin)
                            //already checked!
                            continue;

                        sensor.Pin = newPin;
                        changed |= true;
                    }
                    else if (thing.Config.ElementAt(i).Name == "SamplePeriod")
                    {
                        long period;

                        //VALIDATIONS
                        //invalid value
                        if (!long.TryParse(thing.Config.ElementAt(i).Value, out period))
                            continue;
                        if (sensor.Watcher.SamplingPeriod == period)
                            //unchanged value
                            continue;
                        if (!((int)period).isBetweenValues(1, 10000))
                            //value off limits
                            continue;

                        //GO
                        changed |= true;

                        if (sensor is GPIO)
                        {
                            //sampling period changed; find and assign new one for all GPIOs
                            foreach (var gpio in Helper.GPIOs.Values)
                            {
                                gpio.Watcher.SamplingPeriod = period;
                            }
                        }
                        else
                        {
                            //sampling period changed; find and assign new one
                            sensor.Watcher.SamplingPeriod = period;
                        }
                    }
                }
            }
#if false
            //NOTE: use this one if config parameters order is not guarranteed
            foreach (var conf in thing.Config)
            {
                var oldConfParam = oldCopy.Config.Find(cp => cp.Name == conf.Name);
                if (oldConfParam.Value != conf.Value)
                {
                    //config parameter changed
                    if (conf.Name == "Pin")
                    {
                        //Pin changed; find and assign new one
                        var newPin = GrovePiSensor.PinNameToPin.TryGetOrDefault(conf.Value, Pin.Unknown);
                        if (sensor.Pin != newPin && newPin != Pin.Unknown)
                            sensor.Pin = newPin;
                    }
                    else if (conf.Name == "SamplePeriod")
                    {
                        //sampling period changed; find and assign new one
                        long period;
                        if (long.TryParse(conf.Value, out period))
                            if (sensor.Watcher.SamplingPeriod != period && ((int)period).isBetweenValues(1, 10000))
                                sensor.Watcher.SamplingPeriod = period;
                    }
                }
            }
#endif
            if (changed)
                Helper.CommitConfig();
        }

        private void OnTransportDisconnectedCb(NodeLibrary.Transport Transport, string msg)
        {
            DebugEx.TraceLog("OnDisconnected transport=" + Transport.ToString() + " msg=" + msg);
        }

        private void OnTransportErrorCb(NodeLibrary.Transport Transport, TransportErrors Error, string msg)
        {
            DebugEx.TraceLog("OnTransportError transport=" + Transport.ToString() + " msg=" + msg);
        }

        void OnChangedStateCb(Thing thing, Port port, string state)
        {
        }

        private ApiMsg OnUnexpectedRequestCb(object request)
        {
            DebugEx.TraceLog("Unexpected request received.");
            return null;
        }

        private void OnUnexpectedMessageCb(object message)
        {
            DebugEx.TraceLog("Unexpected message received.");
        }


        //initialize GrovePi Platform
        public void InitializePlatform()
        {


        }

        public void RegisterThingsHandlers()
        {
            /* A2MCU */
            foreach (var gpio in Helper.GPIOs)
            {
                foreach (var port in gpio.Key.Ports)
                    if (port.ioDirection == ioPortDirection.Input || port.ioDirection == ioPortDirection.InputOutput)
                    {

                        node.PortEventHandlers[port] = data =>
                        {
#if DEBUG
                            DebugEx.TraceLog("buzzerData: " + data);
#endif
                            bool value;
                            if (Boolean.TryParse(data, out value))
                                gpio.Value.DigitalWrite(value ? "1" : "0");
                        };
                        break; //cheat
                    }
            }
            /* /A2MCU */

            node.PortEventHandlers[Helper.BuzzerThing.Ports[0]] = data =>
            {
#if DEBUG
                DebugEx.TraceLog("buzzerData: " + data);
#endif
                Buzzer buzzerSensor = (Buzzer)Helper.Lookup[Helper.BuzzerThing];
                //Buzzer buzzerSensor = (Buzzer)Helper.GroveSensors[Helper.BuzzerThing.Name];
                buzzerSensor.DigitalWrite(data);
            };
            node.PortEventHandlers[Helper.RgbLedThing.Ports[0]] = data =>
            {
#if DEBUG
                DebugEx.TraceLog("rgbLedData: " + data);
#endif
                RgbLed rgbLedSensor = (RgbLed)Helper.Lookup[Helper.RgbLedThing];
                //RgbLed rgbLedSensor = (RgbLed)Helper.GroveSensors[Helper.RgbLedThing.Name];
                rgbLedSensor.DigitalWrite(data);
            };

            node.PortEventHandlers[Helper.LCDThing.Ports[0]] = data =>
            {
#if DEBUG
                DebugEx.TraceLog("lcdData: " + data);
#endif
                LCD lcd = (LCD)Helper.Lookup[Helper.LCDThing];
                //LCD lcd = (LCD)Helper.GroveSensors[Helper.LCDThing.Name];
                lcd.Display(data);
            };

        }


        //cb when node is paired
        void OnPaired(NodeKey nodekey, string secret)
        {
            ActiveCfg.NodeKey = nodekey;
            ActiveCfg.NodeSecret = secret;
            YConfig.Save();
        }
    }

}
