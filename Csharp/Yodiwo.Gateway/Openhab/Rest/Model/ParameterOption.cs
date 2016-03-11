using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Gateway.Openhab.Rest.Model
{
    [Serializable]
    public class ParameterOption
    {
        public string label { get; set; }
        public string value { get; set; }
    }
}
