using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Gateway.Openhab.Rest.Model
{
    [Serializable]
    public class ThingType
    {
        public List<ChannelDefinition> channels { get; set; }
        public List<ChannelGroupDefinition> channelGroups { get; set; }
        public List<ConfigDescriptionParameter> configParameters { get; set; }
        public string description { get; set; }
        public string label { get; set; }
        public string UID { get; set; }

        public ThingType()
        {
            this.channels = new List<ChannelDefinition>();
            this.channelGroups = new List<ChannelGroupDefinition>();
            this.configParameters = new List<ConfigDescriptionParameter>();
        }
    }
}
