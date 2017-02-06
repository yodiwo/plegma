using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;
using Yodiwo.Logic;

namespace Yodiwo.Logic
{
    public class ThingUpdateAction
    {
        public PortKey PortKey;
        public string PortState;
        public NodeKey OriginNodeKey;
        public string OriginEndpointKey;
    }

    public struct PortUpdateMessage
    {
        public PortKey PortKey;
        public string PortState;
        public uint RevNum;
        public int IoIndex;
    }

    public class EvThingSolved
    {
        public Residency Residency;
        public IEnumerable<PortUpdateMessage> PortsUpdated;
        public DateTime Timestamp;
        public bool IsWarmupSolve;
        public string TargetNodeKey;
        public string TargetEndpointKey;
    }

    public class EvThingInSolved : EvThingSolved { }

    public class EvThingOutSolved : EvThingSolved { }
}
