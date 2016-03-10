using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.Node.Pairing;
using Yodiwo.NodeLibrary;
using Yodiwo.Logic;
using Yodiwo.API.Plegma;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Yodiwo.PaaS.Amazon;
using System.Xml.Serialization;


namespace Yodiwo.PaaS.AWSProxyNode
{

    public class AWSProxy
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        Yodiwo.NodeLibrary.Node proxyNode;
        ListTS<Thing> Things = new ListTS<Thing>();
        NodeDescriptor ActiveCfg;
        public AWSApplicationClient awsAppClient;
        //------------------------------------------------------------------------------------------------------------------------

        //------------------------------------------------------------------------------------------------------------------------
        public static Dictionary<string, ioPortDirection> LookupPortDirection = new Dictionary<string, ioPortDirection>()
        {
            {"input", ioPortDirection.Input},
            {"output",ioPortDirection.Output},
        };
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        public static Dictionary<string, ePortType> LookupThingType = new Dictionary<string, ePortType>()
        {
            {"integer", ePortType.Integer},
            {"float",ePortType.Decimal},
            {"bool",ePortType.Boolean },
            {"string",ePortType.String}
        };
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        public static Dictionary<ePortType, string> LookupThingState = new Dictionary<ePortType, string>()
        {
            { ePortType.Integer,"0"},
            {ePortType.Decimal,"0"},
            {ePortType.Boolean,"false"},
            {ePortType.String,""}
        };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public void Init()
        {
            //load config
            XmlSerializer ser = new XmlSerializer(typeof(NodeDescriptor));
            if (!File.Exists("config.xml"))
            {
                DebugEx.TraceLog("Could not find config.xml");
                return;
            }
            else {
                using (var fs = new FileStream("config.xml", FileMode.Open))
                    ActiveCfg = ser.Deserialize(fs) as NodeDescriptor;
            }

            //first check/validation in config.xml
            if (ActiveCfg?.AWSInfo?.AWSBrokerHostName == null || ActiveCfg?.AWSInfo?.PFXClientCertPath == null || ActiveCfg?.AWSInfo?.RootCertPath == null || ActiveCfg?.AWSInfo?.DeviceName == null || ActiveCfg?.AWSInfo?.ApplicationName == null)
            {
                DebugEx.TraceLog("Not Valid Configuration in IBMInfo");
                return;
            }
            if (ActiveCfg?.YodiwoInfo?.YodiwoApiServer == null || ActiveCfg?.YodiwoInfo?.YodiwoNodeKey == null || ActiveCfg?.YodiwoInfo?.YodiwoRestUrl == null || ActiveCfg?.YodiwoInfo?.YodiwoSecretKey == null || ActiveCfg?.YodiwoInfo?.YPChannelPort == null)
            {
                DebugEx.TraceLog("Not Valid Configuration in YodiwoInfo");
                return;
            }
            if (ActiveCfg.ThingsDescription.ThingDescriptor.Count == 0)
            {
                DebugEx.TraceLog("List of Things is empty");
                return;
            }
            else
            {
                foreach (var thdesc in ActiveCfg.ThingsDescription.ThingDescriptor)
                {
                    if (thdesc?.Name == null || thdesc?.IO == null || thdesc?.Type == null)
                    {
                        DebugEx.TraceError("Check thing configurations!!! " + ((thdesc.Name != null) ? thdesc.Name : String.Empty));
                        return;
                    }
                    else
                    {
                        //reconstruct thing from thing config description
                        var th = ReconstructThing(thdesc);
                        if (th != null)
                            Things.Add(th);
                    }
                }
            }
            InitAWSConnection();
            InitYodiwoConnection();
        }
        //------------------------------------------------------------------------------------------------------------------------
        private Thing ReconstructThing(ThingDescriptor thdesc)
        {
            if (!LookupThingType.ContainsKey(thdesc.Type))
            {
                DebugEx.TraceError("Not Valid type in: " + thdesc.Name.ToLowerInvariant());
                return null;
            }
            if (!LookupPortDirection.ContainsKey(thdesc.IO.ToLowerInvariant()))
            {
                DebugEx.TraceError("Not Valid IO in: " + thdesc.Name);
                return null;
            }

            Thing thing = new Thing()
            {
                Name = thdesc?.Name,
                Config = null,
                UIHints = new ThingUIHints()
                {
                    IconURI = thdesc?.FriendlyIcon
                }
            };
            thing.Ports = new List<Port>()
            {
                new Port() {
                    ioDirection = LookupPortDirection[thdesc.IO],
                    Name =thing.Name +"Value",
                    Type = LookupThingType[thdesc.Type],
                     PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0"),
                     State = LookupThingState[LookupThingType[thdesc.Type]],
                }

            };
            return thing;
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void InitYodiwoConnection()
        {
            //create node configuration
            NodeConfig conf = new NodeConfig()
            {
                Name = "Clone of " + ActiveCfg.AWSInfo.DeviceName,
                YpServer = ActiveCfg.YodiwoInfo.YodiwoApiServer,
                YpchannelPort = Convert.ToInt32(ActiveCfg.YodiwoInfo.YPChannelPort),
                FrontendServer = ActiveCfg.YodiwoInfo.YodiwoRestUrl,
                CanSolveGraphs = false,
                SecureYpc = ActiveCfg.YodiwoInfo.SecureYPC,
            };
            //prepare node graphmanager module
            var nodeGraphManager = new Yodiwo.NodeLibrary.Graphs.NodeGraphManager(
                                                new Type[]
                                                    {
                                                        typeof(Yodiwo.Logic.BlockLibrary.Basic.Librarian)
                                                    });
            //create the proxy node
            proxyNode = new Yodiwo.NodeLibrary.Node(conf, Things, null, null, null, NodeGraphManager: nodeGraphManager);
            //set transport
            proxyNode.Transport = Transport.YPCHANNEL;

            //register cbs
            proxyNode.OnChangedState += OnChangedStateCb;
            proxyNode.OnTransportConnected += OnTransportConnectedCb;
            proxyNode.OnTransportDisconnected += OnTransportDisconnectedCb;
            proxyNode.OnTransportError += OnTransportErrorCb;
            proxyNode.OnUnexpectedMessage = OnUnexpectedMessageCb;
            proxyNode.OnThingActivated += OnThingActivatedCb;

            //use nodekey to setup things
            proxyNode.SetupNodeKeys(ActiveCfg.YodiwoInfo.YodiwoNodeKey, ActiveCfg.YodiwoInfo.YodiwoSecretKey);

            //connect
            proxyNode.Connect();
            RegisterThingStateCbs();
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void RegisterThingStateCbs()
        {
            /* proxyNode.PortEventHandlers[Things.Find(i => i.Name == "Console").Ports[0]] = data =>
             {
                 awsAppClient.SendCommands(ActiveCfg.IBMInfo.IOTFDeviceType, ActiveCfg.IBMInfo.IOTFDeviceName, "yodiwoporteventmsg", data);
             };*/
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void OnTransportConnectedCb(Yodiwo.NodeLibrary.Transport Transport, string msg)
        {
            DebugEx.TraceLog("OnConnected transport=" + Transport.ToString() + " msg=" + msg);
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void OnThingActivatedCb(Thing thing)
        {
            DebugEx.TraceLog("OnThingActivated: " + thing.ThingKey);
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void OnTransportDisconnectedCb(Yodiwo.NodeLibrary.Transport Transport, string msg)
        {
            DebugEx.TraceLog("OnDisconnected transport=" + Transport.ToString() + " msg=" + msg);
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void OnTransportErrorCb(Yodiwo.NodeLibrary.Transport Transport, TransportErrors Error, string msg)
        {
            DebugEx.TraceLog("OnTransportError transport=" + Transport.ToString() + " msg=" + msg);
        }
        //------------------------------------------------------------------------------------------------------------------------
        void OnChangedStateCb(Thing thing, Port port, string state)
        {
            DebugEx.TraceLog("On ChangedStatecb....");
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void OnUnexpectedMessageCb(object message)
        {
            DebugEx.TraceLog("Unexpected message received.");
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void InitAWSConnection()
        {
            //register IOTF application, which Receives Messages
            awsAppClient = new AWSApplicationClient(ActiveCfg.AWSInfo.AWSBrokerHostName, ActiveCfg.AWSInfo.PFXClientCertPath, ActiveCfg.AWSInfo.RootCertPath);
            //callback when an iotevent message arrives
            awsAppClient.OnRxMessagecb = OnMqttThingRxMessage;
            //start
            awsAppClient.Start(ActiveCfg.AWSInfo.ApplicationName);
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void OnMqttThingRxMessage(string data, string eventName)
        {
            try
            {
                DebugEx.TraceLog("On Mqtt Message Received");
                //deserialize message
                var payload = data.FromJSON() as Newtonsoft.Json.Linq.JObject;
                //AWS Compatible json formats
                var reported = payload["state"];
                var datapayload = reported["reported"];
                foreach (var th in Things)
                {
                    try
                    {
                        //TODO: scan all things and send one batch port event message to the YodiwoCloud
                        //is there any value for the specific thing
                        var y = datapayload[th.Name];
                        //send porteventmessage
                        proxyNode.SetState(th, th.Ports[0], y.ToString());
                    }
                    catch (Exception ex)
                    {
                        DebugEx.TraceError(ex.Message);
                    }

                }
            }
            catch (Exception ex)
            {
                DebugEx.TraceError(ex.Message);
            }

        }
        //------------------------------------------------------------------------------------------------------------------------
        public void sendC2DMessages(string topic, string data)
        {
            awsAppClient.Publish(topic, data);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
