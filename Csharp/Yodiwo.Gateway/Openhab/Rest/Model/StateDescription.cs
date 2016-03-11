using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Gateway.Openhab.Rest.Model
{
    [Serializable]
    public class StateDescription
    {
        public double minimum { get; set; }
        public double maximum { get; set; }
        public double step { get; set; }
        public string pattern { get; set; }
        public bool readOnly { get; set; }
        public List<StateOption> options { get; set; }

        public StateDescription()
        {
            this.options = new List<StateOption>();
        }
    }
}
