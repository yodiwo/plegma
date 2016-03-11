using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.API.Spike
{

    public class SpikeConfig
    {
        public string Name;
        public eSpikeValueTypes Type;
        //TODO revisit, store string and convert on send? or store converted value (in object container)
        public string Value;
        //public object Value;
    }

    public class SpikeDriconf
    {
        public eSpikeDriconfTypes Type;
        public UInt32 DriconfId; //invalid in cloud context, is filled in by the spike node //alternatively, use a different type on the clouds
        public object Data;
    }

    public class SpikeDriconfInputSimple
    {
        public uint SamplingPeriodMs;
        public int IrqId;
    }

    public class SpikeDriconfInputI2c
    {
        public uint SamplingPeriodMs;
        public int IrqId;
        public byte SlaveAddress;
        public byte RegisterAddress;
        public byte Length;
    }


}
