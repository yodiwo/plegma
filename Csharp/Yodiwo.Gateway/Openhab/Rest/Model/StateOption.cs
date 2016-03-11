using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Gateway.Openhab.Rest.Model
{
    [Serializable]
    public class StateOption
    {
        public string value { get; set; }
        public string label { get; set; }
    }
}
