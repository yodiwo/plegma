using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo;
using Yodiwo.NodeLibrary;
using Yodiwo.API.Plegma;
using Yodiwo.API.MediaStreaming;
using Yodiwo.Services.Media.Video;

namespace Yodiwo.Projects.RasPiCamera
{
    public class RaspberryNode
    {
        private YConfig<RasPiCameraNodeConfig> YConfig;
        public static RasPiCameraNodeConfig ActiveCfg;
        Yodiwo.API.Plegma.NodeKey NodeKey { get { return ActiveCfg.NodeKey; } }
        Yodiwo.NodeLibrary.Node node;
        Yodiwo.NodeLibrary.Transport transport = Yodiwo.NodeLibrary.Transport.YPCHANNEL;
        //transport is common for all things (standardinput,standard output)
        Transport pysharp;
        static Dictionary<string, YVideoClient> videosources = new Dictionary<string, YVideoClient>();




        public RaspberryNode(Transport trans)
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
                Name = "Raspberry Node",
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
            /*var nodeGraphManager = new Yodiwo.NodeLibrary.Graphs.NodeGraphManager(
                                                new Type[]
                                                    {
                                                        typeof(Yodiwo.Logic.BlockLibrary.Basic.Librarian),
                                                        typeof(Yodiwo.Logic.BlockLibrary.Extended.Librarian),
                                                    });*/

            //create node
            node = new Yodiwo.NodeLibrary.Node(conf,
                                                Helper.GatherThings(this.pysharp),
                                                pairmodule,
                                                null, null

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
            node.OnUnexpectedRequest = HandleUnknownRequest;
            node.OnThingActivated += OnThingActivatedCb;

            //register port event handlers
            RegisterHandlers();


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

        private void OnTransportConnectedCb(Yodiwo.NodeLibrary.Transport Transport, string msg)
        {
            DebugEx.TraceLog("OnConnected transport=" + Transport.ToString() + " msg=" + msg);
        }

        private void OnThingActivatedCb(Thing thing)
        {

        }

        private void OnTransportDisconnectedCb(Yodiwo.NodeLibrary.Transport Transport, string msg)
        {
            DebugEx.TraceLog("OnDisconnected transport=" + Transport.ToString() + " msg=" + msg);
        }

        private void OnTransportErrorCb(Yodiwo.NodeLibrary.Transport Transport, TransportErrors Error, string msg)
        {
            DebugEx.TraceLog("OnTransportError transport=" + Transport.ToString() + " msg=" + msg);
        }

        void OnChangedStateCb(Thing thing, Port port, string state)
        {
        }



        private void OnUnexpectedMessageCb(object message)
        {
            DebugEx.TraceLog("Unexpected message received.");
        }

        private ApiMsg HandleUnknownRequest(object msg)
        {
            ApiMsg rsp = null;
            if (msg is Yodiwo.API.MediaStreaming.VideoServerConnectRequest)
            {
                rsp = HandleVideoServerConnectRequest(msg);
            }
            if (msg is Yodiwo.API.MediaStreaming.VideoServerDisconnectRequest)
            {
                rsp = HandleVideoServerDisconnectRequest(msg);
            }
            return rsp;
        }


        private ApiMsg HandleVideoServerConnectRequest(object Message)
        {
            DebugEx.TraceLog("Handle Video Server Connect Request ");
            YVideoClient yclient = null;
            VideoServerConnectResponse rsp = null;
            var msg = Message as Yodiwo.API.MediaStreaming.VideoServerConnectRequest;
            var raspicamera = new Camera(this.pysharp);
            yclient = new YVideoClient(raspicamera);
            if (yclient != null)
            {
                DebugEx.TraceLog("Video client is connecting on " + msg.serverip + " port: " + msg.port + " msgvideotoken: " + msg.videotoken + " " + ActiveCfg.YpchannelSecure);
                var res = yclient.Connect(msg.serverip, msg.videotoken, msg.port, ActiveCfg.YpchannelSecure);
                if (!res)
                {
                    DebugEx.TraceError($"Could not connect yclient ({res.Message})");
                    rsp = new VideoServerConnectResponse()
                    {
                        status = StatusCode.Error,
                    };
                }
                else
                {
                    lock (videosources)
                        videosources.Add(msg.videotoken, yclient);
                    rsp = new VideoServerConnectResponse()
                    {
                        status = StatusCode.Success,
                    };
                }
            }
            else
            {
                rsp = new VideoServerConnectResponse()
                {
                    status = StatusCode.Error,
                };
            }
            return rsp;
        }


        private ApiMsg HandleVideoServerDisconnectRequest(object Message)
        {
            var msg = Message as Yodiwo.API.MediaStreaming.VideoServerDisconnectRequest;
            YVideoClient yclient;
            StatusCode status = StatusCode.None;
            VideoServerDisconnectResponse rsp = null;
            if (msg != null)
            {
                lock (videosources)
                {
                    if (videosources.TryGetValue(msg.videotoken, out yclient))
                        status = yclient.TearDown();
                    else
                        DebugEx.Assert("No such videotoken connected with a micclient");
                }
                rsp = new VideoServerDisconnectResponse()
                {
                    status = status,
                };
            }
            else
                rsp = new VideoServerDisconnectResponse() { status = StatusCode.Error };

            return rsp;
        }


        //cb when node is paired
        void OnPaired(NodeKey nodekey, string secret)
        {
            ActiveCfg.NodeKey = nodekey;
            ActiveCfg.NodeSecret = secret;
            YConfig.Save();
        }

        private void RegisterHandlers()
        {
            node.PortEventHandlers[Helper.CameraThing.Ports[1]] = data =>
            {
                var msg = new SharpPy()
                {
                    operation = CMD.Filter,
                    payload = data,
                };
                DebugEx.TraceLog("===============>Change Filter");
                this.pysharp.Send2python(msg);
            };
        }
    }
}
