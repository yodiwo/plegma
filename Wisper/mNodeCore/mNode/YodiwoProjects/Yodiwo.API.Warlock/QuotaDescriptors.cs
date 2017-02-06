using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.API.Warlock
{
    public enum eQuotaType : byte
    {
        BytesIn,
        BytesOut,
        BytesIO,

        EventsIn,
        EventsOut,
        EventsIO,

        ApiKeysInUse,
        ApiKeysTimesUsed,

        DeployedGraphs,
        Nodes,
        Things,
        Integrations,

        EmailsSent,
    }

    [Flags]
    public enum eQuotaFlags
    {
        None = 0,
        MonotonicallyIncreasing = 1 << 0,
        NonResettable = 1 << 1,
        Informational = 1 << 2,
        NoAutoUpdates = 1 << 3
    }

    public class QuotaDescriptor
    {
        #region Variables
        //-------------------------------------------------------------------------------------------------------------------------
        public long Limit;
        public long Overage;
        public bool IsOverLimit;
        public long TotalNumForPeriod;
        public int AlertPercentage;
        public bool IsNonResettable;
        public bool IsInformational;
        public bool NoAutoUpdates;
        public TimeSpan Period;
        public DateTime QuotaCycleStartCurrent;
        public DateTime QuotaCycleStartNext;
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //-------------------------------------------------------------------------------------------------------------------------
        public QuotaDescriptor() { }
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
    public class UserQuotaDescriptor
    {
        #region Variables
        //-------------------------------------------------------------------------------------------------------------------------
        public Dictionary<eQuotaType, QuotaDescriptor> Quota = new Dictionary<eQuotaType, QuotaDescriptor>();
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //-------------------------------------------------------------------------------------------------------------------------
        public UserQuotaDescriptor() { }
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
