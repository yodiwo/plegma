using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;

namespace Yodiwo.NodeLibrary.Graphs
{
    public interface INodeGraphManager
    {
        string[] BlockLibrariesNames { get; }
        void Initialize(Node ParentNode);
        void DeInitialize();
        void DeployGraphs();
        void OnConnectedToCloud();
        void OnDisconnectedFromCloud();
        GenericRsp HandleGraphDeploymentReq(GraphDeploymentReq req);
        void HandlePortStates(IEnumerable<TupleS<Port, string>> States);
        void HandleIncomingVirtualBlockEventMsg(VirtualBlockEventMsg msg);

        IReadOnlySet<PortKey> ActivePortKeys { get; }
        IReadOnlySet<Thing> ActiveThings { get; }

        bool IsPortActive(PortKey pk);
        bool IsThingActive(ThingKey tk);

        void Purge();
    }
}
