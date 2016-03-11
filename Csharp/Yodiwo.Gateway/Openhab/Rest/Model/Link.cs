using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Gateway.Openhab.Rest.Model
{
    [Serializable]
    public class Link
    {
        public string channelUID { get; set; }
        public string itemName { get; set; }
    }
}
