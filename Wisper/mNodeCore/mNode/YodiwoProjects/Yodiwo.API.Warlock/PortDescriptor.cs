using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;

namespace Yodiwo.API.Warlock
{

    public class PortDescriptor
    {
        #region Variables
        //-------------------------------------------------------------------------------------------------------------------------
        public string PortKey;
        public string Name;
        public string Description;
        public string IoDirection;
        public string PortType;
        public string Semantics;
        public string State;
        public volatile string RevNum;
        public string LastUpdatedTimestamp;
        public bool HasRules;
        public int Size;
        public string Color;
        public string Extra;
        public List<PortRule> Rules;
        public Port Port;
        public bool IsInput;
        public bool IsOutput;
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion
        public PortDescriptor() { }
    }
}
