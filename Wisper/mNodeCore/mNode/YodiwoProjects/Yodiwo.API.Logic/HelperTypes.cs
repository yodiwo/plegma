using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;
using Yodiwo.Logic;

namespace Yodiwo.Logic
{
    public class ThingSolveOpResult
    {
        [Flags]
        public enum eThingSolveErrorCodes
        {
            OK = 0,
            ThingDoesNotExist = 1,
            ThingIsNotCurrentlyConnected = 2,
            NoPendingReqFound = 4,
            OtherError = 256  //exception, etc
        }
        public bool IsSuccess;
        public string ErrMsg;
        public eThingSolveErrorCodes ErrCode;
        public object SyncId;
        public NodeKey NodeKey;
        public ThingKey ThingKey;
    }
    public class ThingResponseAction
    {
        public ThingSolveOpResult Result;
        public PortEvent[] PortEvents; //unused
    }

    public class ThingUpdateAction
    {
        public PortKey PortKey;
        public string PortState;
        public NodeKey OriginNodeKey;
        public string OriginEndpointKey;
        public object SyncId;
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
        public object SyncId;
    }

    public class EvThingInSolved : EvThingSolved { }

    public class EvThingOutSolved : EvThingSolved { }
}
