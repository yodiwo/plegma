using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Gateway.Openhab.Rest.Model
{
    [Serializable]
    public class Channel
    {
        public List<string> linkedItems { get; set; }
        public string id { get; set; }
        public string itemType { get; set; }

        public Channel()
        {
            this.linkedItems = new List<string>();
        }
    }
}
