using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Gateway.Openhab.Rest.Model
{
    public enum ThingStatus
    {
        ONLINE,
        OFFLINE
    }
    [Serializable]
    public class Thing
    {
        public string bridgeUID { get; set; }
        public Dictionary<string, object> configuration { get; set; }
        public ThingStatus status { get; set; }
        public string UID { get; set; }
        public List<Channel> channels { get; set; }
        public GroupItem item { get; set; }

        public Thing()
        {
            this.configuration = new Dictionary<string, object>();
            this.channels = new List<Channel>();
            this.item = new GroupItem();
        }
    }
}
