using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Gateway.Openhab.Rest.Model
{
    public enum DiscoveryResultFlag
    {
        NEW,
        IGNORED
    }
    [Serializable]
    public class DiscoveryResult
    {
        public string bridgeUID { get; set; }
        public DiscoveryResultFlag flag { get; set; }
        public string label { get; set; }
        public Dictionary<string, object> properties { get; set; }
        public string thingUID { get; set; }

        public DiscoveryResult()
        {
            this.properties = new Dictionary<string, object>();
        }
    }
}
