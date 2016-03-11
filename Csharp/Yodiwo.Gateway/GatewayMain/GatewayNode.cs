using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Yodiwo.API.Plegma;
using Yodiwo.NodeLibrary;
using System.Diagnostics;
using Yodiwo.Gateway.GatewayMain.Spike;
using Yodiwo.Gateway.GatewayMain.Nest;


namespace Yodiwo.Gateway.GatewayMain
{
    public class GatewayNode
    {
        #region Variables
        private Yodiwo.NodeLibrary.Node node;
        object locker = new object();
        HashSetTS<INodeModule> nodemodules = new HashSetTS<INodeModule>();
        #endregion

        #region Functions
        #region Init Node
        public void Start()
        {
            #region Configurations

            //assign the identified active configuration to the system
            GatewayStatics.YConfig = GatewayStatics.Init();
            GatewayStatics.ActiveCfg = GatewayStatics.YConfig.GetActiveConf();

            Console.WriteLine("Using configuration with uuid=" + GatewayStatics.ActiveCfg.Uuid);
            if (GatewayStatics.ActiveCfg.NodeKey != null)
            {
                Console.WriteLine("nodekey=" + GatewayStatics.ActiveCfg.NodeKey);
            }
            else
            {
                Console.WriteLine("not paired yet");
            }

            NodeConfig conf = new NodeConfig()
            {
                uuid = GatewayStatics.ActiveCfg.Uuid,
                Name = "Gateway",
                YpServer = GatewayStatics.ActiveCfg.ApiServer,
                YpchannelPort = GatewayStatics.ActiveCfg.YPChannelPort,
                SecureYpc = GatewayStatics.ActiveCfg.YpchannelSecure,
                FrontendServer = GatewayStatics.ActiveCfg.FrontendServer,
                CanSolveGraphs = false,
            };
            #endregion

            #region Node construction
            //prepare pairing module
            var openhubhost = new OpenHubHost();
            openhubhost.startHttpServer(GatewayStatics.ActiveCfg.LocalWebServer);

            //instatiate the modules

            //openhub module
            var ohmodule = new OpenHubModule(GatewayStatics.ActiveCfg.OpenhabBaseUrl);
            nodemodules.Add(ohmodule);

            //spike module
            var spikeModule = new SpikeModule();
            nodemodules.Add(spikeModule);

            //nest module
            var nestModule = new NestModule();
            nestModule.Init();
            nodemodules.Add(nestModule);

            //for testing purposes
            if (!string.IsNullOrWhiteSpace(GatewayStatics.ActiveCfg.SpikeSerialNodePort))
            {
                var serialLink = new SerialLink(GatewayStatics.ActiveCfg.SpikeSerialNodePort, 223400);
                serialLink.Start();
                spikeModule.AddSampleNode(serialLink);
            }

            //prepare node graph manager module
            var nodeGraphManager = new Yodiwo.NodeLibrary.Graphs.NodeGraphManager(
                                                new System.Type[]
                                                    {
                                                        typeof(Yodiwo.Logic.BlockLibrary.Basic.Librarian),
                                                        typeof(Yodiwo.Logic.BlockLibrary.Extended.Librarian),
                                                    });

            // create node
            node = new Yodiwo.NodeLibrary.Node(conf,
                                                null, //things will be provided by modules
                                                openhubhost,
                                                null, null,
                                                NodeGraphManager: nodeGraphManager,
                                                nodeType: eNodeType.Gateway,
                                                NodeModules: nodemodules
                                                //MqttTransport: typeof(Yodiwo.NodeLibrary.Transports.MQTT)
                                                );
            node.Transport = Transport.YPCHANNEL;
            #endregion

            #region Register callbacks
            node.OnTransportConnected += onConnected;
            node.OnTransportDisconnected += onDisconnected;
            node.OnTransportError += onTransportError;
            node.OnNodePaired += onPaired;
            node.OnUnexpectedMessage = onUnexpectedMessage;
            node.OnUnexpectedRequest = onUnexpectedRequest;
            node.OnThingActivated += Node_OnThingActivated;
            #endregion            

            #region Pairing/Connect
            if (String.IsNullOrWhiteSpace(GatewayStatics.ActiveCfg.NodeKey))
            {
                DebugEx.TraceLog("Starting pairing procedure.");
                var pair = node.StartPairing(GatewayStatics.ActiveCfg.FrontendServer, null, GatewayStatics.ActiveCfg.LocalWebServer);

                //open process (browser) optional
                Process.Start(GatewayStatics.ActiveCfg.LocalWebServer + @"/pairing");
            }
            else
            {
                node.SetupNodeKeys(GatewayStatics.ActiveCfg.NodeKey, GatewayStatics.ActiveCfg.NodeSecret);
                DebugEx.TraceLog("Node already paired: NodeKey = "
                    + GatewayStatics.ActiveCfg.NodeKey + ", NodeSecret = " + GatewayStatics.ActiveCfg.NodeSecret);
                node.Connect();
            }
            #endregion

        }

        #endregion

        #region HandleNodeLibraryCbs
        public void onPaired(NodeKey nodeKey, string nodeSecret)
        {
            GatewayStatics.ActiveCfg.NodeKey = nodeKey;
            GatewayStatics.ActiveCfg.NodeSecret = nodeSecret;
            GatewayStatics.YConfig.Save();
            node.Connect();
        }

        void onConnected(Transport Transport, string msg)
        {
            DebugEx.TraceLog("OnConnected transport=" + Transport.ToString() + " msg=" + msg);
        }

        void onDisconnected(Transport Transport, string msg)
        {
            DebugEx.TraceLog("OnDisconnected transport=" + Transport.ToString() + " msg=" + msg);
        }

        private ApiMsg onUnexpectedRequest(object request)
        {
            DebugEx.TraceLog("Unexpected request received.");
            return null;
        }

        private void onUnexpectedMessage(object message)
        {
            DebugEx.TraceLog("Unexpected message received.");
        }

        private void onTransportConnected(Transport Transport, string msg)
        {
            DebugEx.TraceLog("OnConnected transport=" + Transport.ToString() + " msg=" + msg);
        }

        private void onTransportDisconnected(Transport Transport, string msg)
        {
            DebugEx.TraceLog("OnDisconnected transport=" + Transport.ToString() + " msg=" + msg);
        }

        private void onTransportError(Transport Transport, TransportErrors Error, string msg)
        {
            DebugEx.TraceLog("OnTransportError transport=" + Transport.ToString() + " msg=" + msg);
        }

        private void Node_OnThingActivated(API.Plegma.Thing thing)
        {
            DebugEx.TraceLog("OnThingActivated" + thing.Name);
        }

        #endregion

        #endregion
    }
}
