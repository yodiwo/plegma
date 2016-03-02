using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.NodeLibrary.NodeDiscovery
{
    [Flags]
    public enum NodeFlags : int
    {
        None = 0,

        //General
        //Reserved = 1 << 0,
        //Reserved = 1 << 1,
        //Reserved = 1 << 2,
        //Reserved = 1 << 3,
        //Reserved = 1 << 4,
        //Reserved = 1 << 5,
        //Reserved = 1 << 6,
        //Reserved = 1 << 7,
        //Reserved = 1 << 8,
        //Reserved = 1 << 9,

        //Node Capabilities
        CanSolveGraphs = 1 << 10,
        //Reserved = 1 << 11,
        //Reserved = 1 << 12,
        //Reserved = 1 << 13,
        //Reserved = 1 << 14,
        LowPower = 1 << 15,
        LimitedCapabilities = 1 << 16,
        LongSleepCycle = 1 << 17,
        //Reserved = 1 << 18,
        //Reserved = 1 << 19,

        //Connectivity
        CloudConnection = 1 << 20,
        LimitedCloudConnection = 1 << 21,
        //Reserved = 1 << 22,
        //Reserved = 1 << 23,
        //Reserved = 1 << 24,
        //Reserved = 1 << 25,
        //Reserved = 1 << 26,
        //Reserved = 1 << 27,
        //Reserved = 1 << 28,        
        //Reserved = 1 << 29,

        //Reserved Category
        //Reserved = 1 << 30,
        //Reserved = 1 << 31,
    }
}
