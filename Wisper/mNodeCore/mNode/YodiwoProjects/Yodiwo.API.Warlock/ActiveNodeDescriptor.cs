using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.API.Warlock
{
    public class ActiveNodeDescriptor
    {

        #region Variables
        //-------------------------------------------------------------------------------------------------------------------------
        public DictionaryTS<string, string> TransientInfo = new DictionaryTS<string, string>();
        public List<NodeLinkIpInfo> IpsInfo;
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //-------------------------------------------------------------------------------------------------------------------------
        public ActiveNodeDescriptor() { }
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion

        public class NodeLinkIpInfo
        {
            public bool IsMarkedInternal;
            public string Address;
        }
    }
}
