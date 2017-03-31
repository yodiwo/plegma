using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Yodiwo.mNode.Plugins.Bridge_SerialPort
{
    // --------------------------------------------------------------------------------------------
    public class SingleFill
    {
        public int TruckId;
        public float Quantity;
        public float Temp;
        public DateTime Date;
        public int SeqId;

        [JsonIgnore]
        public DateTime SendedTimestamp;  // Internal use
    }
    // --------------------------------------------------------------------------------------------
    class GAE_Config
    {
        public int SeqId;
        public DictionaryTS<int, SingleFill> Readings = new DictionaryTS<int, SingleFill>();
    }
    // --------------------------------------------------------------------------------------------
}
