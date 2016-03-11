using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Gateway.Openhab.Rest.Model
{
    [Serializable]
    public class ChannelGroupDefinition
    {
        public string id { get; set; }
        public string description { get; set; }
        public string label { get; set; }
        public List<ChannelDefinition> channels { get; set; }

        public ChannelGroupDefinition()
        {
            this.channels = new List<ChannelDefinition>();
        }
    }
}
