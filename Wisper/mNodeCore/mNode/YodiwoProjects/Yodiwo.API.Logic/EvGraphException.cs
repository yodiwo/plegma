using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;

namespace Yodiwo.API.Logic
{
    public class EvUserGraphException
    {
        public string ErrorMsg;
        public UserKey UserKey;
        public DateTime Timestamp;
        public GraphDescriptorKey grdescKey;
        public string FriendlyName;
        public BlockKey blockKey;
        public string BlockName;
    }
}
