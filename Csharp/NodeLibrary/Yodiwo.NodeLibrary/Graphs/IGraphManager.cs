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
        void DeployGraphs();
        void OnConnectedToCloud();
        GenericRsp HandleGraphDeploymentReq(GraphDeploymentReq req);
        void HandlePortStates(IEnumerable<TupleS<Port, string>> States);

        IEnumerable<PortKey> ActivePortKeys { get; }
        IEnumerable<Thing> ActiveThings { get; }
    }
}
