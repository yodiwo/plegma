using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Yodiwo;
using Yodiwo.API.Plegma;
using Yodiwo.NodeLibrary;
using System.Security;
using System.Diagnostics;
using System.Linq;
using Raspberry.IO.GeneralPurpose;

namespace Yodiwo.Projects.RaspberryPi.GPIO
{
    class Program
    {
        static void Main(string[] args)
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

            (new RaspPiGpioNode()).Start();

            while (true) { Task.Delay(5000).Wait(); }
        }
    }

    public class RaspPiGpioNode
    {
        #region Variables

        static Yodiwo.NodeLibrary.Node Node;

        private NodeKey NodeKey { get { return ActiveCfg.NodeKey; } }

        private YConfig<RaspPiGpioNodeConfig> YConfig;
        private RaspPiGpioNodeConfig ActiveCfg;
        private readonly string cfgFile = "conf_file.json";

        bool _IsFirstConnectionAfterPairing = false;

        private GpioConnection gpioPinsConnection;

        private Dictionary<string, GpioPin> ThingIdToPin = new Dictionary<string, GpioPin>();

        private Dictionary<string, Thing> ThingIdToThing = new Dictionary<string, Thing>();

        private enum GpioPin
        {
            Unknown,
            Pin20,
            Pin21,
        }

        // GPIO pin to P1-header pin-numbering mapping
        private Dictionary<GpioPin, ConnectorPin> GpioToPin = new Dictionary<GpioPin, ConnectorPin>()
        {
            {GpioPin.Pin20, ConnectorPin.P1Pin38},
            {GpioPin.Pin21, ConnectorPin.P1Pin40},
        };

        #endregion

        #region Constructor

        public RaspPiGpioNode()
        {
            // GPIO init hardware interface
            gpioPinsConnection = new GpioConnection();

            foreach (var x in GpioToPin)
            {
                gpioPinsConnection.Add(x.Value.Output());
            }

            if (!gpioPinsConnection.IsOpened)
            {
                gpioPinsConnection.Open();
            }

            // Done
            DebugEx.TraceLog("RaspberryPIGPIO plugin up and running !! ");
        }

        #endregion

        public static string CreateThingKey(string pinId)
        {
            return "GPIO:" + pinId;
        }

        public void Start()
        {
            #region Configurations

            this.YConfig = this.InitConfig();
            this.ActiveCfg = this.YConfig.GetActiveConf();
            NodeConfig conf = new NodeConfig()
            {
                uuid = ActiveCfg.Uuid,
                Name = "RaspPiGpioNode",
                YpServer = ActiveCfg.ApiServer,
                YpchannelPort = ActiveCfg.YpchannelPort,
                SecureYpc = ActiveCfg.YpchannelSecure,
                FrontendServer = ActiveCfg.FrontendServer,
                CanSolveGraphs = false,
                Pairing_NoUUIDAuthentication = true
            };

            if (String.IsNullOrWhiteSpace(ActiveCfg.NodeKey))
            {
                _IsFirstConnectionAfterPairing = true;
            }

            #endregion

            #region Node construction
            //create node
            Node = new Yodiwo.NodeLibrary.Node(conf,
                                               new Yodiwo.NodeLibrary.Pairing.NancyPairing.NancyPairing(),
                                               DataLoad: null,
                                               DataSave: null,
                                               nodeType: eNodeType.EndpointSingle
                                               );
            #endregion

            #region Register callbacks
            Node.OnNodePaired += Node_OnPairedCb;
            Node.OnNodeUnpaired += Node_OnNodeUnpairedCb;
            Node.OnTransportConnected += Node_OnTransportConnectedCb;
            Node.OnTransportDisconnected += Node_OnTransportDisconnectedCb;
            Node.OnTransportError += Node_OnTransportErrorCb;
            #endregion

            #region Do Pairing/Connection
            if (String.IsNullOrWhiteSpace(ActiveCfg.NodeKey))
            {
                //if nodekey null, start pairing
                DebugEx.TraceLog("Starting pairing procedure.");
                var task = Node.StartPairing(ActiveCfg.FrontendServer, null, ActiveCfg.LocalWebServer);
                task.Wait();
            }
            #endregion

            //already paired, connect
            Node.SetupNodeKeys(ActiveCfg.NodeKey, ActiveCfg.NodeSecret.ToSecureString());
            DebugEx.TraceLog($"Node already paired: NodeKey={ActiveCfg.NodeKey}, NodeSecret={ActiveCfg.NodeSecret.Substring(0, 8)}...");

            _SetupThings();

            foreach (var thing in ThingIdToThing.Values)
            {
                Node.PortEventHandlers[thing.Ports[0]] = (data, isEvent) =>
                {
                    Console.WriteLine("PorteventMsg Rx (isEvent={2}) for thingUID {0} with data {1}", thing.ThingKey.Split('-').Last(), data, isEvent);
                    if (isEvent)
                    {
                        var gpiopin = ThingIdToPin.TryGetOrDefault(thing.ThingKey.Split('-').Last(), GpioPin.Unknown);
                        if (gpiopin != GpioPin.Unknown)
                        {
                            Console.WriteLine("Setting pin {0} to {1}", gpiopin, data);
                            gpioPinsConnection[GpioToPin.TryGetOrDefault(gpiopin)] = data.ParseToBool();
                        }
                    }
                };
            }

            //connect
            Node.Connect();
        }

        #region Things setup
        private void _SetupThings()
        {
            #region thing for pin20
            var thingPin20Id = CreateThingKey("Pin20");
            Console.WriteLine("Adding thing with ID: {0}", thingPin20Id);
            ThingIdToPin.Add(thingPin20Id, GpioPin.Pin20);
            ThingIdToThing.Add(thingPin20Id, new Yodiwo.API.Plegma.Thing()
            {
                ThingKey = ThingKey.BuildFromArbitraryString(NodeKey, thingPin20Id),
                Type = ThingTypeLibrary.Lights.Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.Gpio_Type,
                Name = "GPIO pin 20",
                Config = null,
                UIHints = new ThingUIHints()
                {
                    IconURI = "/Content/img/icons/Generic/gpio.png",
                },
                Ports = new List<Yodiwo.API.Plegma.Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        PortKey = PortKey.BuildFromArbitraryString(ThingKey.BuildFromArbitraryString(NodeKey, thingPin20Id), "0"),
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                        Name = "PinState",
                        State = "0",
                        PortModelId = ModelTypeLibrary.GpioModel_Id,
                        Type = Yodiwo.API.Plegma.ePortType.Boolean,
                    }
                }
            });
            #endregion

            #region thing for pin21
            var thingPin21Id = CreateThingKey("Pin21");
            Console.WriteLine("Adding thing with ID: {0}", thingPin21Id);
            ThingIdToPin.Add(thingPin21Id, GpioPin.Pin21);
            ThingIdToThing.Add(thingPin21Id, new Yodiwo.API.Plegma.Thing()
            {
                ThingKey = ThingKey.BuildFromArbitraryString(NodeKey, thingPin21Id),
                Type = ThingTypeLibrary.Lights.Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.Gpio_Type,
                Name = "GPIO pin 21",
                Config = null,
                UIHints = new ThingUIHints()
                {
                    IconURI = "/Content/img/icons/Generic/gpio.png",
                },
                Ports = new List<Yodiwo.API.Plegma.Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        PortKey = PortKey.BuildFromArbitraryString(ThingKey.BuildFromArbitraryString(NodeKey, thingPin21Id), "0"),
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                        Name = "PinState",
                        State = "0",
                        PortModelId = ModelTypeLibrary.GpioModel_Id,
                        Type = Yodiwo.API.Plegma.ePortType.Boolean,
                    }
                }
            });
            #endregion
        }

        #endregion

        #region Node Library callbacks

        public YConfig<RaspPiGpioNodeConfig> InitConfig()
        {
            var absoluteCfgFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/" + this.cfgFile;

            var yconfig = new YConfig<RaspPiGpioNodeConfig>(absoluteCfgFile);

            try
            {
                if (yconfig.Retrieve())
                {
                    Console.WriteLine("Config file found: {0}", absoluteCfgFile);
                    return yconfig;
                }
                Console.WriteLine("Config retrieval failed; falling back to defaults");
            }
            catch
            {
                Console.WriteLine("Loading default configuration");
            }

            //create default conf
            RaspPiGpioNodeConfig cfg = new RaspPiGpioNodeConfig();
            cfg.FrontendServer = "https://tcyan.yodiwo.com";
            cfg.ApiServer = "api.yodiwo.com";
            cfg.LocalWebServer = "http://localhost:3339";
            cfg.Uuid = "RaspPiGpioNode";
            cfg.YpchannelPort = Constants.YPChannelPort;
            cfg.YpchannelSecure = true;

            //add new active conf and save to disk
            yconfig.AddActiveConf("default", cfg);
            yconfig.Save();

            return yconfig;
        }

        private void Node_OnPairedCb(NodeKey nodeKey, SecureString nodeSecret)
        {
            ActiveCfg.NodeKey = nodeKey;
            ActiveCfg.NodeSecret = nodeSecret.SecureStringToString();
            YConfig.Save();
        }

        private void Node_OnNodeUnpairedCb(eUnpairReason reasonCode, string msg)
        {
            ActiveCfg.NodeKey = null;
            ActiveCfg.NodeSecret = null;
            YConfig.Save();
        }

        private void Node_OnTransportConnectedCb(Transport Transport, string msg)
        {
            DebugEx.TraceLog("OnConnected transport=" + Transport.ToString() + " msg=" + msg);

            // Add things and send to cloud
            foreach (var thing in ThingIdToThing.Values)
            {
                Console.WriteLine("==============>  Adding thing {0}", thing.ThingKey);

                Node.AddThing(thing);
            }
        }

        private void Node_OnTransportDisconnectedCb(Transport Transport, string msg)
        {
            DebugEx.TraceLog("OnDisconnected transport=" + Transport.ToString() + " msg=" + msg);
        }

        private void Node_OnTransportErrorCb(Transport Transport, TransportErrors Error, string msg)
        {
            DebugEx.TraceError("OnTransportError transport=" + Transport.ToString() + " msg=" + msg);
        }

        #endregion

        #region Helpers

        public class RaspPiGpioNodeConfig : IYConfigEntry
        {
            public string Uuid;
            public string NodeKey;
            public string NodeSecret;
            public string FrontendServer;
            public string ApiServer;
            public int YpchannelPort;
            public bool YpchannelSecure;
            public string CertificateServerName;
            public string LocalWebServer;
        }

        #endregion
    }
}
