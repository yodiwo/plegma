using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Gateway.Openhab.Rest.Model
{
    public enum Type
    {
        TEXT,
        INTEGER,
        DECIMAL,
        BOOLEAN
    }
    [Serializable]
    public class ConfigDescriptionParameter
    {
        public string context { get; set; }
        public string defaultValue { get; set; }
        public string description { get; set; }
        public string label { get; set; }
        public string name { get; set; }
        public bool required { get; set; }
        public Type type { get; set; }
        public double minimum { get; set; }
        public double maximum { get; set; }
        public double stepsize { get; set; }
        public string pattern { get; set; }
        public bool readOnly { get; set; }
        public bool multiple { get; set; }
        public List<ParameterOption> options { get; set; }
        public List<FilterCriteria> filterCriteria { get; set; }

        public ConfigDescriptionParameter()
        {
            this.options = new List<ParameterOption>();
            this.filterCriteria = new List<FilterCriteria>();
        }
    }
}
