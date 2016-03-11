using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Gateway.Openhab.Rest.Model
{
    [Serializable]
    public class Item
    {
        public string type { get; set; }
        public string name { get; set; }
        public string label { get; set; }
        public string category { get; set; }
        public string state { get; set; }
        public string link { get; set; }
        public List<string> tags { get; set; }
        public StateDescription stateDescription { get; set; }
        public List<string> groupNames { get; set; }

        public Item()
        {
            this.tags = new List<string>();
            this.stateDescription = new StateDescription();
            this.groupNames = new List<string>();

        }

    }

}
