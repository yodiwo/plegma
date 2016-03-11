using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Gateway.Openhab.Rest.Model
{
    [Serializable]
    public class ChannelDefinition
    {
        public string description { get; set; }
        public string id { get; set; }
        public string label { get; set; }
        public List<string> tags { get; set; }
        public string category { get; set; }
        public StateDescription stateDescription { get; set; }
        public bool advanced { get; set; }

        public ChannelDefinition()
        {
            this.tags = new List<string>();
            this.stateDescription = new StateDescription();
        }
    }
}
