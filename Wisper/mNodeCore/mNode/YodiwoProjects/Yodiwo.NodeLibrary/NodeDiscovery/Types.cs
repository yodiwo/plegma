using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.NodeLibrary.NodeDiscovery
{
    [Flags]
    public enum NodeFlags : UInt64
    {
        None = 0,

        //General
        Mobile = 1L << 0,    //portable devices (such as mobiles, wearables etc)
        //Reserved = 1L << 1,
        //Reserved = 1L << 2,
        //Reserved = 1L << 3,
        //Reserved = 1L << 4,
        //Reserved = 1L << 5,
        //Reserved = 1L << 6,
        //Reserved = 1L << 7,
        //Reserved = 1L << 8,
        //Reserved = 1L << 9,

        //Node Capabilities
        CanSolveGraphs = 1L << 10,
        //Reserved = 1L << 11,
        //Reserved = 1L << 12,
        //Reserved = 1L << 13,
        //Reserved = 1L << 14,
        LowPower = 1L << 15,
        LimitedCapabilities = 1L << 16,
        LongSleepCycle = 1L << 17,
        BatteryPowered = 1L << 18,
        //Reserved = 1L << 19,

        //Connectivity
        CloudConnection = 1L << 20,
        LimitedCloudConnection = 1L << 21,
        DataPlanConnectivity = 1L << 22,  //3G, 4G or other pay per byte connections
        //Reserved = 1L << 23,
        //Reserved = 1L << 24,
        //Reserved = 1L << 25,
        //Reserved = 1L << 26,
        //Reserved = 1L << 27,
        //Reserved = 1L << 28,        
        //Reserved = 1L << 29,

        //Reserved Category
        //Reserved = 1L << 30,
        //Reserved = 1L << 31,
    }
}
