using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Reflection;

using Newtonsoft.Json;

using Raspberry;
using Raspberry.IO;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.Interop;
using Raspberry.Timers;
using RaspberryCam;

using Yodiwo;
using Yodiwo.API.Plegma;
using Yodiwo.NodePairing;
using Yodiwo.YPChannel.Transport;



namespace Yodiwo.Projects.RaspberryPi2Node
{
    class Program
    {
        static void Main(string[] args)
        {
            var raspberryNode = new RaspNode();

            raspberryNode.Start();

            while (true)
            {
                Thread.Sleep(50);
            }
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

        private Yodiwo.YPChannel.Transport.Sockets.Client Channel;

        private Yodiwo.API.Plegma.NodeKey NodeKey;

        private YConfig<RaspNodeConfig> YConfig;
        private RaspNodeConfig ActiveCfg;
        private readonly string cfgFile = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/conf_file.json";

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

        public RaspNode()
        {
            gpioPinsConnection = new GpioConnection();

            foreach (var x in LedToPin)
            {
                gpioPinsConnection.Add(x.Value.Output());
            }

            if (!gpioPinsConnection.IsOpened)
            {
                gpioPinsConnection.Open();
            }
        }

        #endregion

        // Initialize
        public void Init()
        {
            #region Configurations, pairing

            this.YConfig = this.InitConfig();
            this.ActiveCfg = this.YConfig.GetActiveConf();

            //Start pairing if NodeKey is invalid
            if (String.IsNullOrWhiteSpace(ActiveCfg.nodeKeyS))
                NodePairingStatics.initPairing(ActiveCfg.pairingServerUrl, ActiveCfg.uuid, "RaspberryNode", onPaired, ActiveCfg.webBase);
            else
                NodeKey = ActiveCfg.nodeKeyS;

            #endregion

            #region Setup Led1 thing
            {
                var thing = Led1Thing = new Yodiwo.API.Plegma.Thing()
                {
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
                        PortKey = new Yodiwo.API.Plegma.PortKey(thing, "0")
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
                        PortKey = new Yodiwo.API.Plegma.PortKey(thing, "0")
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
                        PortKey = new Yodiwo.API.Plegma.PortKey(thing, "0")
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
                        PortKey = new Yodiwo.API.Plegma.PortKey(thing, "0")
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
                        PortKey = new Yodiwo.API.Plegma.PortKey(thing, "0")
                    }
                };
                Things.Add(thing.ThingKey, thing);
                PkeyToLed.Add(thing.Ports[0].PortKey, LedPin.Led5);
            }
            #endregion

        }

        // Start node
        public void Start()
        {
            this.Init();

            Task.Run(() =>
            {
                if (this.NodeKey.IsValid)
                {
                    this.connectYPChannel();
                }
            });

        }

        #region YPCHANNEL

        public void connectYPChannel()
        {
            Console.WriteLine("===> YPChannel connect...");
            //open channel
            if (Channel != null)
            {
                Channel.Close();
            }

            Channel = new Yodiwo.YPChannel.Transport.Sockets.Client(Yodiwo.API.Plegma.PlegmaAPI.ApiMessages);
            Channel.OnOpenEvent += Channel_OnOpenEvent;
            Channel.OnClosedEvent += Channel_OnClosedEvent;
            Channel.OnMessageReceived += Channel_OnMessageReceived;
            var status = Channel.Connect(ActiveCfg.ypchannelServer, ActiveCfg.ypchannelServerPort);
            if (!status) { Console.WriteLine("YPChannel connection failed"); }

            Console.WriteLine("===> YPChannel connected (status = {0})", status);
        }

        void Channel_OnOpenEvent(Yodiwo.YPChannel.Channel channel)
        {
            channel.Name = "Raspberry Node client";
        }

        void Channel_OnClosedEvent(Yodiwo.YPChannel.Channel channel)
        {
            channel.Close();
            this.Channel = null;
        }

        void Channel_OnMessageReceived(Yodiwo.YPChannel.Channel Channel, Yodiwo.YPChannel.YPMessage Message)
        {
            var msg = Message.Payload;
            //handle message
            if (msg is Yodiwo.API.Plegma.NodeInfoReq)
            {
                HandleIdReq(Channel, Message);
            }
            else if (msg is Yodiwo.API.Plegma.PortEventMsg)
            {
                HandlePortEventMsg(msg as Yodiwo.API.Plegma.PortEventMsg);
            }
            else if (msg is Yodiwo.API.Plegma.ThingsReq)
            {
                HandleThingsReq(Channel, Message);
            }
            else if (msg is Yodiwo.API.Plegma.PortStateRsp)
            {
                //TODO: HANDLE THIS!
            }
            else if (msg is Yodiwo.API.Plegma.Thing)
            {
                DebugEx.Assert("no.. give me port events");
            }
            else
                DebugEx.Assert("Cannot handle message");
        }

        private void HandleThingsReq(Yodiwo.YPChannel.Channel Channel, Yodiwo.YPChannel.YPMessage Message)
        {
            Console.WriteLine("===> HandleNodeThingsReq received");

            var msg = Message.Payload as ThingsReq;

            var id = msg.Id;
            var tkey = msg.ThingKey;
            var op = msg.Operation;

            var msgResp = new ThingsRsp()
            {
                Operation = op,
                Status = false,
                Data = null,
            };

            if (op == eThingsOperation.Get)
            {
                var data = GatherThings();
                if (data != null)
                {
                    msgResp.Status = true;
                    msgResp.Data = data;
                }
                else
                {
                    Console.WriteLine("==> Things:: {0}", data.First().Name);
                }
            }
            Channel.SendResponse(msgResp, Message.MessageID);
        }

        private void HandleIdReq(Yodiwo.YPChannel.Channel Channel, Yodiwo.YPChannel.YPMessage Message)
        {
            Console.WriteLine("===> HandleIdReq received");

            var msg = Message.Payload;
            var msgTyped = msg as Yodiwo.API.Plegma.NodeInfoReq;


            var idMsg = new Yodiwo.API.Plegma.NodeInfoRsp();

            idMsg.Name = "Raspberry Node";
            idMsg.Type = Yodiwo.API.Plegma.eNodeType.TestGateway;
            idMsg.ThingTypes = null;

            Channel.SendResponse(idMsg, Message.MessageID);

        }

        private void HandlePortEventMsg(Yodiwo.API.Plegma.PortEventMsg msg)
        {
            foreach (var portEvent in msg.PortEvents)
            {
                var ledState = portEvent.State.ParseToBool();
                var led = PkeyToLed.TryGetOrDefault(portEvent.PortKey, LedPin.Unknown);
                Console.WriteLine("==> Rx port event msg for led {0}", led);
                if (led != LedPin.Unknown)
                {
                    SetLedState(led, ledState);
                }
            }
        }

        #endregion

        #region Functions

        [Flags]
        private enum Transport : byte
        {
            None = 0,
            Ypc = 1 << 0,
            Rest = 1 << 1,
            Mqtt = 1 << 2
        }

        public YConfig<RaspNodeConfig> InitConfig()
        {
            Console.WriteLine("Config file:: {0}", cfgFile);
            var yconfig = new YConfig<RaspNodeConfig>(cfgFile);

            try
            {
                if (yconfig.Retrieve())
                {
                    return yconfig;
                }
                Console.WriteLine("Config retrieval failed; falling back to defaults");
            }
            catch
            {
                Console.WriteLine("Loading default demo configuration");
            }
            //create default conf
            RaspNodeConfig cfg = new RaspNodeConfig();
            cfg.active = true;
            cfg.pairingServerUrl = "http://10.30.254.130:3334/pairing";
            cfg.webBase = "http://localhost:6666";
            cfg.ypchannelServer = "10.30.254.130";
            cfg.uuid = "1337Raspberry";
            cfg.ypchannelServerPort = Yodiwo.API.Plegma.Constants.YPChannelPort;

            //add new active conf and save to disk
            yconfig.AddActiveConf(cfg);
            yconfig.Save();

            return yconfig;
        }

        private void SetLedState(LedPin eled, bool state)
        {
            /*
            Console.WriteLine("Setting led {0} {1}", eled, state);

            var led = LedToPin.TryGetOrDefault(eled).ToProcessor();

            var ledDriver = GpioConnectionSettings.DefaultDriver;

            ledDriver.Allocate(led, PinDirection.Output);

            ledDriver.Write(led, state);

            ledDriver.Release(led);
             * */

            Console.WriteLine("Setting led {0} {1}", eled, state);

            gpioPinsConnection[LedToPin.TryGetOrDefault(eled)] = state;

        }

        private Yodiwo.API.Plegma.Thing[] GatherThings()
        {
            return Things.Values.ToArray();
        }

        private void SendMsg(Yodiwo.API.Plegma.PortEventMsg msg, Transport useTransport)
        {
            if (useTransport.HasFlag(Transport.Ypc))
                if (Channel != null && Channel.IsOpen)
                {
                    Channel.SendMessage(msg);
                }
        }

        string GenerateThingID()
        {
            return Interlocked.Increment(ref _ThingIDCnt).ToStringInvariant();
        }

        public void onPaired(NodeKey nodeKey, string nodeSecret)
        {
            this.NodeKey = nodeKey;
            ActiveCfg.nodeKeyS = nodeKey;
            ActiveCfg.nodeSecret = nodeSecret;
            this.YConfig.Save();
        }

        #endregion
    }

    #region Helpers

    public class RaspNodeConfig
    {
        public bool active;
        public string uuid;
        public string nodeKeyS;
        public string nodeSecret;
        public string pairingServerUrl;
        public string ypchannelServer;
        public int ypchannelServerPort;
        public string webBase;
    }

    #endregion
}
