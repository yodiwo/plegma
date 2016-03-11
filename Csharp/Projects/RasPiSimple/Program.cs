using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

using Raspberry.IO.GeneralPurpose;
using Raspberry;
using Raspberry.IO;
using Raspberry.IO.Interop;
using Raspberry.Timers;
using RaspberryCam;

using Yodiwo;
using Yodiwo.API.Plegma;
using Yodiwo.NodeLibrary;
using Yodiwo.API.Plegma.NodePairing;
using Yodiwo.YPChannel.Transport;
using Yodiwo.Node.Pairing.NancyPairing;



namespace Yodiwo.Projects.RaspberryPi2Node
{
    class Program
    {
        static void Main(string[] args)
        {
            var raspberryNode = new RaspNode(Transport.YPCHANNEL);
            raspberryNode.Start();

            while (true) { Task.Delay(50).Wait(); }
        }
    }

    public class RaspNode
    {
        #region Variables

        private int _ThingIDCnt = 0;

        private Yodiwo.API.Plegma.Thing Led1Thing;
        private Yodiwo.API.Plegma.Thing Led2Thing;
        private Yodiwo.API.Plegma.Thing Led3Thing;
        private Yodiwo.API.Plegma.Thing Led4Thing;
        private Yodiwo.API.Plegma.Thing Led5Thing;

        private Yodiwo.API.Plegma.NodeKey NodeKey { get { return ActiveCfg.NodeKey; } }

        private YConfig<RaspNodeConfig> YConfig;
        private RaspNodeConfig ActiveCfg;
        private readonly string cfgFile = "conf_file.json";

        private Yodiwo.NodeLibrary.Node Node;
        Transport Transport;

        private enum LedPin
        {
            Unknown,
            Led1,
            Led2,
            Led3,
            Led4,
            Led5,
        }

        private Dictionary<ThingKey, Thing> Things = new Dictionary<ThingKey, Thing>();

        private static Dictionary<PortKey, LedPin> PkeyToLed = new Dictionary<PortKey, LedPin>();


        private static Dictionary<LedPin, ConnectorPin> LedToPin = new Dictionary<LedPin, ConnectorPin>()
        {
            {LedPin.Led1, ConnectorPin.P1Pin13},
            {LedPin.Led2, ConnectorPin.P1Pin12},
            {LedPin.Led3, ConnectorPin.P1Pin15},
            {LedPin.Led4, ConnectorPin.P1Pin16},
            {LedPin.Led5, ConnectorPin.P1Pin18},
        };

        private static GpioConnection gpioPinsConnection;


        #endregion

        #region Constructor

        public RaspNode(Transport transport)
        {
            // Hardware interface
            gpioPinsConnection = new GpioConnection();

            foreach (var x in LedToPin)
            {
                gpioPinsConnection.Add(x.Value.Output());
            }

            if (!gpioPinsConnection.IsOpened)
            {
                gpioPinsConnection.Open();
            }

            // Node transport
            this.Transport = (transport != Transport.None) ? transport : Transport.YPCHANNEL;
        }

        #endregion

        #region Functions

        public void Start()
        {
            #region Configurations

            this.YConfig = this.InitConfig();
            this.ActiveCfg = this.YConfig.GetActiveConf();
            NodeConfig conf = new NodeConfig()
            {
                uuid = ActiveCfg.Uuid,
                Name = "RaspberryNode",
                MqttBrokerHostname = ActiveCfg.MqttBrokerHostname,
                MqttUseSsl = ActiveCfg.MqttUseSsl,
                YpServer = ActiveCfg.ApiServer,
                YpchannelPort = ActiveCfg.YpchannelPort,
                SecureYpc = ActiveCfg.YpchannelSecure,
                FrontendServer = ActiveCfg.FrontendServer
            };

            #endregion

            #region Things setup

            #region Setup Led1 thing
            {
                var thing = Led1Thing = new Yodiwo.API.Plegma.Thing()
                {
                    Type = "yodiwo.input.leds.simple",
                    ThingKey = new ThingKey(NodeKey, GenerateThingID()),
                    Name = "Raspberry Led 1",
                    Config = null,
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "/Content/RaspberryNode/img/icon-thing-led.png",
                    },
                };
                thing.Ports = new List<Yodiwo.API.Plegma.Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                        Name = "LedState",
                        State = "0",
                        Type = Yodiwo.API.Plegma.ePortType.Boolean,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0")
                    }
                };
                Things.Add(thing.ThingKey, thing);
                PkeyToLed.Add(thing.Ports[0].PortKey, LedPin.Led1);
            }
            #endregion

            #region Setup Led2 thing
            {
                var thing = Led2Thing = new Yodiwo.API.Plegma.Thing()
                {
                    Type = "yodiwo.input.leds.simple",
                    ThingKey = new ThingKey(NodeKey, GenerateThingID()),
                    Name = "Raspberry Led 2",
                    Config = null,
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "/Content/RaspberryNode/img/icon-thing-led.png",
                    },
                };
                thing.Ports = new List<Yodiwo.API.Plegma.Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                        Name = "LedState",
                        State = "0",
                        Type = Yodiwo.API.Plegma.ePortType.Boolean,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0")
                    }
                };
                Things.Add(thing.ThingKey, thing);
                PkeyToLed.Add(thing.Ports[0].PortKey, LedPin.Led2);
            }
            #endregion

            #region Setup Led3 thing
            {
                var thing = Led3Thing = new Yodiwo.API.Plegma.Thing()
                {
                    Type = "yodiwo.input.leds.simple",
                    ThingKey = new ThingKey(NodeKey, GenerateThingID()),
                    Name = "Raspberry Led 3",
                    Config = null,
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "/Content/RaspberryNode/img/icon-thing-led.png",
                    },
                };
                thing.Ports = new List<Yodiwo.API.Plegma.Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                        Name = "LedState",
                        State = "0",
                        Type = Yodiwo.API.Plegma.ePortType.Boolean,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0")
                    }
                };
                Things.Add(thing.ThingKey, thing);
                PkeyToLed.Add(thing.Ports[0].PortKey, LedPin.Led3);
            }
            #endregion

            #region Setup Led4 thing
            {
                var thing = Led4Thing = new Yodiwo.API.Plegma.Thing()
                {
                    Type = "yodiwo.input.leds.simple",
                    ThingKey = new ThingKey(NodeKey, GenerateThingID()),
                    Name = "Raspberry Led 4",
                    Config = null,
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "/Content/RaspberryNode/img/icon-thing-led.png",
                    },
                };
                thing.Ports = new List<Yodiwo.API.Plegma.Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                        Name = "LedState",
                        State = "0",
                        Type = Yodiwo.API.Plegma.ePortType.Boolean,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0")
                    }
                };
                Things.Add(thing.ThingKey, thing);
                PkeyToLed.Add(thing.Ports[0].PortKey, LedPin.Led4);
            }
            #endregion

            #region Setup Led5 thing
            {
                var thing = Led5Thing = new Yodiwo.API.Plegma.Thing()
                {
                    Type = "yodiwo.input.leds.simple",
                    ThingKey = new ThingKey(NodeKey, GenerateThingID()),
                    Name = "Raspberry Led 5",
                    Config = null,
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "/Content/RaspberryNode/img/icon-thing-led.png",
                    },
                };
                thing.Ports = new List<Yodiwo.API.Plegma.Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                        Name = "LedState",
                        State = "0",
                        Type = Yodiwo.API.Plegma.ePortType.Boolean,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0")
                    }
                };
                Things.Add(thing.ThingKey, thing);
                PkeyToLed.Add(thing.Ports[0].PortKey, LedPin.Led5);
            }
            #endregion

            #endregion

            #region Node construction
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
            Node = new Yodiwo.NodeLibrary.Node(conf,
                                                Things.Values.ToList(),
                                                pairmodule,
                                                null, null,
                                                NodeGraphManager: nodeGraphManager
                                                //MqttTransport: typeof(Yodiwo.NodeLibrary.Transports.MQTT)
                                                );

            #endregion

            #region Register port event handlers

            Node.PortEventHandlers[Led1Thing.Ports[0]] = data =>
            {
                var ledState = data.ParseToBool();
                var led = PkeyToLed.TryGetOrDefault(Led1Thing.Ports[0].PortKey, LedPin.Unknown);
                Console.WriteLine("==> Rx port event msg for led {0}", led);
                if (led != LedPin.Unknown)
                {
                    SetLedState(led, ledState);
                }
            };

            Node.PortEventHandlers[Led2Thing.Ports[0]] = data =>
            {
                var ledState = data.ParseToBool();
                var led = PkeyToLed.TryGetOrDefault(Led2Thing.Ports[0].PortKey, LedPin.Unknown);
                Console.WriteLine("==> Rx port event msg for led {0}", led);
                if (led != LedPin.Unknown)
                {
                    SetLedState(led, ledState);
                }
            };

            Node.PortEventHandlers[Led3Thing.Ports[0]] = data =>
            {
                var ledState = data.ParseToBool();
                var led = PkeyToLed.TryGetOrDefault(Led3Thing.Ports[0].PortKey, LedPin.Unknown);
                Console.WriteLine("==> Rx port event msg for led {0}", led);
                if (led != LedPin.Unknown)
                {
                    SetLedState(led, ledState);
                }
            };

            Node.PortEventHandlers[Led4Thing.Ports[0]] = data =>
            {
                var ledState = data.ParseToBool();
                var led = PkeyToLed.TryGetOrDefault(Led4Thing.Ports[0].PortKey, LedPin.Unknown);
                Console.WriteLine("==> Rx port event msg for led {0}", led);
                if (led != LedPin.Unknown)
                {
                    SetLedState(led, ledState);
                }
            };

            Node.PortEventHandlers[Led5Thing.Ports[0]] = data =>
            {
                var ledState = data.ParseToBool();
                var led = PkeyToLed.TryGetOrDefault(Led5Thing.Ports[0].PortKey, LedPin.Unknown);
                Console.WriteLine("==> Rx port event msg for led {0}", led);
                if (led != LedPin.Unknown)
                {
                    SetLedState(led, ledState);
                }
            };

            #endregion

            #region Register callbacks

            Node.OnChangedState += OnChangedStateCb;
            Node.OnNodePaired += OnPairedCb;
            Node.OnTransportConnected += OnTransportConnectedCb;
            Node.OnTransportDisconnected += OnTransportDisconnectedCb;
            Node.OnTransportError += OnTransportErrorCb;
            Node.OnUnexpectedMessage = OnUnexpectedMessageCb;

            #endregion

            #region Pairing/Connect

            if (String.IsNullOrWhiteSpace(ActiveCfg.NodeKey))
            {
                DebugEx.TraceLog("Starting pairing procedure.");
                var task = Node.StartPairing(ActiveCfg.FrontendServer, null, ActiveCfg.LocalWebServer);
            }
            else
            {
                Node.SetupNodeKeys(ActiveCfg.NodeKey, ActiveCfg.NodeSecret);
                DebugEx.TraceLog("Node already paired: NodeKey = "
                    + ActiveCfg.NodeKey + ", NodeSecret = ", ActiveCfg.NodeSecret);

                Node.Connect();
            }

            #endregion
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

        public YConfig<RaspNodeConfig> InitConfig()
        {
            var absoluteCfgFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/" + this.cfgFile;

            var yconfig = new YConfig<RaspNodeConfig>(absoluteCfgFile);

            try
            {
                if (yconfig.Retrieve())
                {
                    Console.WriteLine("Config file found: {0}", absoluteCfgFile);
                    return yconfig;
                }
                Console.WriteLine("Config retrieval failed; falling back to defaults");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Loading default configuration");
            }

            //create default conf
            RaspNodeConfig cfg = new RaspNodeConfig();
            cfg.FrontendServer = "https://cyan.yodiwo.com";
            cfg.ApiServer = "api.yodiwo.com";
            cfg.LocalWebServer = "http://localhost:3339";
            cfg.Uuid = "1337Raspberry";
            cfg.MqttBrokerHostname = "api.yodiwo.com";
            cfg.YpchannelPort = Yodiwo.API.Plegma.Constants.YPChannelPort;
            cfg.MqttUseSsl = false;
            cfg.YpchannelSecure = false;

            //add new active conf and save to disk
            yconfig.AddActiveConf("default", cfg);
            yconfig.Save();

            return yconfig;
        }

        private void SetLedState(LedPin eled, bool state)
        {
            Console.WriteLine("Setting led {0} {1}", eled, state);
            gpioPinsConnection[LedToPin.TryGetOrDefault(eled)] = state;
        }

        string GenerateThingID()
        {
            return Interlocked.Increment(ref _ThingIDCnt).ToStringInvariant();
        }

        void OnPairedCb(NodeKey nodekey, string secret)
        {
            ActiveCfg.NodeKey = nodekey;
            ActiveCfg.NodeSecret = secret;
            YConfig.Save();

            //connect
            Node.Connect();
        }

        private void OnTransportConnectedCb(Transport Transport, string msg)
        {
            DebugEx.TraceLog("OnConnected transport=" + Transport.ToString() + " msg=" + msg);
        }

        private void OnTransportDisconnectedCb(Transport Transport, string msg)
        {
            DebugEx.TraceLog("OnDisconnected transport=" + Transport.ToString() + " msg=" + msg);
        }

        private void OnTransportErrorCb(Transport Transport, TransportErrors Error, string msg)
        {
            DebugEx.TraceLog("OnTransportError transport=" + Transport.ToString() + " msg=" + msg);
        }

        void OnChangedStateCb(Thing thing, Port port, string state)
        {
        }

        #endregion
    }

    #region Helpers

    public class RaspNodeConfig : IYConfigEntry
    {
        public string webBase;
        public string Uuid;
        public string NodeKey;
        public string NodeSecret;
        public string FrontendServer;
        public string ApiServer;
        public string MqttBrokerHostname;
        public bool MqttUseSsl;
        public int YpchannelPort;
        public bool YpchannelSecure;
        public string CertificateServerName;
        public string LocalWebServer;
        public string MqttApiPasswd;
        public string ffmpegexe;
    }

    #endregion
}
