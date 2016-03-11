using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Gateway.Openhab.Rest.Model
{
    [Serializable]
    public class Binding
    {
        public string author { get; set; }
        public string description { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public List<ThingType> thingTypes { get; set; }

        public Binding()
        {
            this.thingTypes = new List<ThingType>();
        }
    }
}
