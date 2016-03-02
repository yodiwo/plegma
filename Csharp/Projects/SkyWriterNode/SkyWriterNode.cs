using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.NodeLibrary;
using Yodiwo.NodeLibrary.Graphs;
using Yodiwo.API.Plegma;

namespace Yodiwo.Projects.SkyWriter
{
    public class SkyWriterNode
    {
        private YConfig<SkyWriterNodeConfig> YConfig;
        public static SkyWriterNodeConfig ActiveCfg;
        Yodiwo.API.Plegma.NodeKey NodeKey { get { return ActiveCfg.NodeKey; } }
        Yodiwo.NodeLibrary.Node node;
        NodeLibrary.Transport transport = NodeLibrary.Transport.YPCHANNEL;
        private Dictionary<ThingKey, Thing> SkyWriterThings = new Dictionary<ThingKey, Thing>();
        //transport is common for all things (standardinput,standard output)
        Transport pysharp;


        public SkyWriterNode(Transport trans)
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
                Name = "SkyWriter Node",
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
                                                null, null,
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
            node.OnThingActivated += OnThingActivatedCb;
            node.OnThingDeactivated += OnThingDeactivatedCb;

            //register port event handlers


            //start Pairing
            if (String.IsNullOrWhiteSpace(ActiveCfg.NodeKey))
            {
                DebugEx.TraceLog("Starting pairing procedure.");
                var pair = node.Pairing(ActiveCfg.FrontendServer + @"/pairing", null, ActiveCfg.LocalWebServer).GetResults();
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

        private void OnThingActivatedCb(Thing thing)
        {
            var targetThing = Helper.Things.Find(i => i.ThingKey == thing.ThingKey);
            var skywritersensor = Helper.Lookup.TryGetOrDefault(targetThing);
            if (skywritersensor != null)
            {
                Console.WriteLine("ACTIVATED: " + thing.ThingKey + " " + targetThing.Name);
                skywritersensor.watcher.Active = true;
                skywritersensor.Read();
            }
        }

        private void OnThingDeactivatedCb(Thing thing)
        {
            var targetThing = Helper.Things.Find(i => i.ThingKey == thing.ThingKey);
            var skywritersensor = Helper.Lookup.TryGetOrDefault(targetThing);
            if (skywritersensor != null)
            {
                Console.WriteLine("DEACTIVATED: " + thing.ThingKey + " " + targetThing.Name);
                skywritersensor.watcher.Active = false;
            }
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


        //cb when node is paired
        void OnPaired(NodeKey nodekey, string secret)
        {
            ActiveCfg.NodeKey = nodekey;
            ActiveCfg.NodeSecret = secret;
            YConfig.Save();
        }
    }


}
