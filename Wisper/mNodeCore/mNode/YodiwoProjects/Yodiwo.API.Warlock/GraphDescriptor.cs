using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.API.Warlock
{
    public class GraphWrlkDescriptor
    {
        //-------------------------------------------------------------------------------------------------------------------------
        public Yodiwo.API.Plegma.GraphDescriptorKey GraphDescriptorKey;
        //-------------------------------------------------------------------------------------------------------------------------
        public string FriendlyName;
        public String GraphKey;
        public String Path;
        //-------------------------------------------------------------------------------------------------------------------------
        public string CreatedTimestamp;
        public string UpdatedTimestamp;
        //-------------------------------------------------------------------------------------------------------------------------
        public bool IsDeployed;
        public int Revision;
        public int DeployedRevision;
        public int LatestRevision;
        //-------------------------------------------------------------------------------------------------------------------------
        public bool HasMacroBlock;
        //-------------------------------------------------------------------------------------------------------------------------

        //-------------------------------------------------------------------------------------------------------------------------
        public GraphWrlkDescriptor() { }
    }
}
