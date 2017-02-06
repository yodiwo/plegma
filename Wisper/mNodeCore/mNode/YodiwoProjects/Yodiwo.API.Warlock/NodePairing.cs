using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.API.Warlock
{
    public class NodePairingModelView
    {
        public string token2;
        public string user_Name;
        public string user_UserName;
        public string user_Email;
        public string user_EmailHidden;
        public string nodeName;
        public string nodeImage;
        public string nodeDescription;
        public string errorMessage;
        public bool noPINVerification;
        public List<NodeInfoModelDescriptor> recoverableNodes = new List<NodeInfoModelDescriptor>();
    }

    public class NodeInfoModelDescriptor
    {
        public string NodeKey;
        public string Name;
        public string NodeId;
    }
    public class RecoveredNodesModelView
    {
        public List<NodeInfoModelDescriptor> nodesinfo;
        public string uuid;

    }
}
