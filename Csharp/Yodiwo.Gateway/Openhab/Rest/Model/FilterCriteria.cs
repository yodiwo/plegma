using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Gateway.Openhab.Rest.Model
{
    [Serializable]
    public class FilterCriteria
    {
        public string value { get; set; }
        public string name { get; set; }
    }
}
