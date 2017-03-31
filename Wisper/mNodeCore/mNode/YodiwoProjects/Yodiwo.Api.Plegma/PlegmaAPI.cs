using System;
using System.Collections.Generic;
using System.Linq;

// According to recent discussions there will be 3 API umbrellas, Basic, Media Streaming, and Creative
// Plegma: communication between EndPoints (Nodes) and Cloud Server; Storage APIs; etc
// MediaStreaming: self-explanatory
// Creative: APIs to create new components and plugins to be run inside the Yodiwo Cloud (or GW) software. To be submitted for review by community

namespace Yodiwo.API.Plegma
{
    public static class PlegmaAPI
    {
        public const int APIVersion = 1;

        public const char KeySeparator = '-';
        public const string BroadcastToken = "*";

        public const string RestAPIBaseRoute = "api";

        public const string ApiGroupName = "PlegmaAPI";
        public const string ApiLogicGroupName = "PlegmaAPI.Logic";
        public const char ThingModelTypeSeparator = '.';
        public const string ThingModelTypeSeparatorStr = ".";
        public const string ThingModelTypeDefault = "default";
        public const string ThingModelTypeSeparatorPlusDefault = ThingModelTypeSeparatorStr + ThingModelTypeDefault;


        /// <summary>
        /// List of all possible API messages that are exchanged between Nodes and the Yodiwo Cloud Service
        /// </summary>
        public static Type[] ApiMessages =
        {
            //Plegma v1 messages
            typeof(LoginReq),
            typeof(LoginRsp),
            typeof(NodeInfoReq),
            typeof(NodeInfoRsp),
            typeof(NodeUnpairedReq),
            typeof(EndpointSyncReq),
            typeof(EndpointSyncRsp),
            typeof(ThingsGet),
            typeof(ThingsSet),
            typeof(PortEventMsg),
            typeof(PortStateGet),
            typeof(PortStateSet),
            typeof(ActivePortKeysMsg),
            typeof(PingReq),
            typeof(PingRsp),
            typeof(VirtualBlockEventMsg),
            typeof(A2mcuActiveDriversReq),
            typeof(A2mcuCtrlReq),
            typeof(GenericRsp),     //Only add new messaged to the botton of the list.. do not insert between previous types (breaks ypc protocol)

            //Plegma v2 messages
            typeof(NodeStatusChangedReq),
        };

        public static Type[] LogicApiMessages =
        {
            typeof(LocallyDeployedGraphsMsg),
            typeof(GraphDeploymentReq),
        };

        //Literal API names
        public const string s_LoginReq = "loginreq";
        public const string s_LoginRsp = "loginrsp";
        public const string s_NodeInfoReq = "nodeinforeq";
        public const string s_NodeInfoRsp = "nodeinforsp";
        public const string s_NodeUnpairedReq = "nodeunpairedreq";
        public const string s_NodeStatusChangedReq = "nodestatuschangedreq";
        public const string s_EndpointSyncReq = "endpointsyncreq";
        public const string s_EndpointSyncRsp = "endpointsyncrsp";
        public const string s_ThingsGet = "thingsget";
        public const string s_ThingsSet = "thingsset";
        public const string s_PortEventMsg = "porteventmsg";
        public const string s_PortStateGet = "portstateget";
        public const string s_PortStateSet = "portstateset";
        public const string s_ActivePortKeysMsg = "activeportkeysmsg";
        public const string s_PingReq = "pingreq";
        public const string s_PingRsp = "pingrsp";
        public const string s_VirtualBlockEventMsg = "virtualblockeventmsg";

        public const string s_LocallyDeployedGraphsMsg = "locallydeployedgraphsmsg";
        public const string s_GraphDeploymentReq = "graphdeploymentreq";

        public const string s_A2mcuActiveDriversReq = "A2mcuactivedriversreq";
        public const string s_A2mcuCtrlReq = "A2mcuctrlreq";

        public const string s_GenericRsp = "genericrsp";

        public const string s_FcmConnectionMsg = "fcmconnectionmsg";

        public const string s_FcmDisconnectionMsg = "fcmdisconnectionmsg";


        /// <summary>
        /// Dictionary that maps API classes to names. These names are the ones used for REST routes, MQTT topics, or RabbitMQ queue names
        /// </summary>
        public static Dictionary<Type, String> ApiMsgNames = new Dictionary<Type, string>()
        {
            { typeof(LoginReq),             s_LoginReq                      },
            { typeof(LoginRsp),             s_LoginRsp                      },
            { typeof(NodeInfoReq) ,         s_NodeInfoReq                   },
            { typeof(NodeInfoRsp),          s_NodeInfoRsp                   },
            { typeof(NodeUnpairedReq),      s_NodeUnpairedReq               },
            { typeof(NodeStatusChangedReq), s_NodeStatusChangedReq          },
            { typeof(EndpointSyncReq),      s_EndpointSyncReq               },
            { typeof(EndpointSyncRsp),      s_EndpointSyncRsp               },
            { typeof(ThingsGet),            s_ThingsGet                     },
            { typeof(ThingsSet),            s_ThingsSet                     },
            { typeof(PortEventMsg),         s_PortEventMsg                  },
            { typeof(PortStateGet),         s_PortStateGet                  },
            { typeof(PortStateSet),         s_PortStateSet                  },
            { typeof(ActivePortKeysMsg),    s_ActivePortKeysMsg             },
            { typeof(PingReq),              s_PingReq                       },
            { typeof(PingRsp),              s_PingRsp                       },

            { typeof(VirtualBlockEventMsg), s_VirtualBlockEventMsg          },
            { typeof(GraphDeploymentReq),   s_GraphDeploymentReq            },
            { typeof(LocallyDeployedGraphsMsg), s_LocallyDeployedGraphsMsg  },

            { typeof(A2mcuActiveDriversReq),s_A2mcuActiveDriversReq         },
            { typeof(A2mcuCtrlReq),         s_A2mcuCtrlReq                  },

            { typeof(FcmConnectionMsg),     s_FcmConnectionMsg              },
            { typeof(FcmDisconnectionMsg),  s_FcmDisconnectionMsg           },

            { typeof(GenericRsp),           s_GenericRsp                    },
        };

        /// <summary>
        /// Dictionary that maps API classes to retain values. 
        /// If true, then messages should be retained (either for protocols that support this natively or if implemented as extra)
        /// If false, the broker is instructed to not retain the message
        /// If an API msg is not found on this dictionary, the caller should assume "false"
        /// </summary>
        public static Dictionary<Type, bool> ApiMsgRetained = new Dictionary<Type, bool>()
        {
        };

        /// <summary>
        /// Dictionary that maps API names to classes. These names are the ones used for REST routes, MQTT topics, or RabbitMQ queue names
        /// </summary>
        public static Dictionary<String, Type> ApiMsgNamesToTypes = ApiMsgNames.Select(e => new KeyValuePair<string, Type>(e.Value, e.Key)).ToDictionary();
    }
}

