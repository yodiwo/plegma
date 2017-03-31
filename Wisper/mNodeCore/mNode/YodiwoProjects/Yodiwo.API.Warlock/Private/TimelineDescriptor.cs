using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;

namespace Yodiwo.API.Warlock.Private
{
    //------------------------------------------------------------------------------------------------------------------------
    public enum eTimelineDescriptorType
    {
        Unknown,
        Accounts,
        ApiKeys,
        BinaryResource,
        Graphs,
        Groups,
        Nodes,
        Things,
        Quota,
        User
    }
    //------------------------------------------------------------------------------------------------------------------------
    public enum eTimelineDescriptorOperationType
    {
        Unknown,
        Add,
        Remove,
        UpdateOrEdit,
        Enable,
        Disable,
        // for graphs only
        Deploy,
        UndeployExplicit,
        UndeployImplicit,
        // for things only 
        Share,
        UnshareExplicit,
        UnshareImplicit,
        ShareToMe,
        UnshareFromMe,
        UnshareToMe,
        Connected,
        Disconnected,
        ConnectFail,
        // Quota Type Only
        QuotaLimitCrossed,
        QuotaLimitUncrossed,
        QuotaLimitChanged,
        QuotaCurValueChanged,
        QuotaPeriodChanged,
        QuotaPeriodReset,
        QuotaPercentageCrossed,
        // User Type only 
        Park,
        Unpark,
        Ban,
        Unban,
        Eject,
        AccountLevelChanged,
        PrivilegeLevelChanged
        // user removal uses 'TimelineDescriptorOperationType.Remove'
    }
    //------------------------------------------------------------------------------------------------------------------------

    public class TimelineDescriptor : IEquatable<TimelineDescriptor>
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public Yodiwo.API.Warlock.Private.TimelineDescriptorKey TimelineDescriptorKey;
        public UserKey UserKey { get { return TimelineDescriptorKey.UserKey; } }
        //------------------------------------------------------------------------------------------------------------------------
        public eTimelineDescriptorType Type;
        public eTimelineDescriptorOperationType Operation;
        public DateTime DateTimeTS;
        public string Message;
        //------------------------------------------------------------------------------------------------------------------------
        // contains instance of class derived from TimelineInfo
        public object Info;
        #endregion

        //------------------------------------------------------------------------------------------------------------------------
        public TimelineDescriptor() { DateTimeTS = DateTime.UtcNow; }
        //------------------------------------------------------------------------------------------------------------------------
        public TimelineDescriptor(TimelineDescriptorKey tldKey, eTimelineDescriptorType Type, eTimelineDescriptorOperationType Operation, DateTime Timestamp, TimelineInfo Info, string Message = "")
        {
            this.TimelineDescriptorKey = tldKey;
            this.Type = Type;
            this.Operation = Operation;
            this.DateTimeTS = Timestamp;
            this.Info = Info;
            this.Message = Message;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override int GetHashCode()
        {
            int DateTimeHash = DateTimeTS != null ? DateTimeTS.GetHashCode() : 0;
            int TypeHash = Type.GetHashCode();
            int OperationHash = Operation.GetHashCode();
            int UserKeyHash = TimelineDescriptorKey.UserKey.GetHashCode();
            int MessageHash = Message != null ? Message.GetHashCode() : 0;
            return TypeHash ^ OperationHash ^ UserKeyHash ^ DateTimeHash ^ MessageHash;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public bool Equals(TimelineDescriptor other)
        {
            //Check whether the compared object is null.
            if (Object.ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data.
            if (Object.ReferenceEquals(this, other)) return true;

            return other.DateTimeTS == DateTimeTS &&
                other.Type == Type &&
                other.Operation == Operation &&
                other.TimelineDescriptorKey.UserKey == TimelineDescriptorKey.UserKey &&
                other.Message == Message;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            TimelineDescriptor objAsDesc = obj as TimelineDescriptor;
            if (objAsDesc == null) return false;
            else return Equals(objAsDesc);
        }
        //------------------------------------------------------------------------------------------------------------------------
    }

    #region TimelineInfo

    public abstract class TimelineInfo
    {
        public string Key;
        public TimelineInfo() { }
    }

    #region Accounts

    //------------------------------------------------------------------------------------------------------------------------
    public class AccountInfoTimelineInfo : TimelineInfo
    {
        public string Type;
        //------------------------------------------------------------------------------------------------------------------------
        public AccountInfoTimelineInfo() { }
        //------------------------------------------------------------------------------------------------------------------------
        public AccountInfoTimelineInfo(string key, string type) { Key = key; Type = type; }
        //------------------------------------------------------------------------------------------------------------------------
    }
    //------------------------------------------------------------------------------------------------------------------------

    #endregion

    #region API Keys

    //------------------------------------------------------------------------------------------------------------------------
    public class ApiKeyInfoTimelineInfo : TimelineInfo
    {
        //------------------------------------------------------------------------------------------------------------------------
        public UserApiKey UserApiKey
        {
            get { return this.Key; }
            set { this.Key = value; }
        }
        public string Name;
        public API.Warlock.eApiKeyType Type;
        //------------------------------------------------------------------------------------------------------------------------
        public ApiKeyInfoTimelineInfo() : base() { }
        //------------------------------------------------------------------------------------------------------------------------
        public ApiKeyInfoTimelineInfo(API.Warlock.ApiKeyDescriptor desc)
        {
            UserApiKey = desc.UserApiKey;
            Name = desc.Name;
            Type = desc.Type;
        }
        //------------------------------------------------------------------------------------------------------------------------
    }
    //------------------------------------------------------------------------------------------------------------------------

    #endregion

    #region GraphDescriptor

    //------------------------------------------------------------------------------------------------------------------------
    public class GraphDescriptorTimelineInfo : TimelineInfo
    {
        //------------------------------------------------------------------------------------------------------------------------
        // store GraphDescriptorKey,FriendlyName and Path only
        public Yodiwo.API.Plegma.GraphDescriptorKey GraphDescriptorKey
        {
            get { return this.Key; }
            set { this.Key = value; }
        }
        public string Name;
        public string Path;
        //------------------------------------------------------------------------------------------------------------------------
        public GraphDescriptorTimelineInfo() : base() { }
        //------------------------------------------------------------------------------------------------------------------------
        public GraphDescriptorTimelineInfo(API.Warlock.GraphWrlkDescriptor desc)
        {
            GraphDescriptorKey = desc.GraphDescriptorKey;
            Name = desc.FriendlyName;
            Path = desc.Path;
        }
        //------------------------------------------------------------------------------------------------------------------------
    }
    //------------------------------------------------------------------------------------------------------------------------
    #endregion

    #region Groups

    //------------------------------------------------------------------------------------------------------------------------
    public class GroupInfoTimelineInfo : TimelineInfo
    {
        //------------------------------------------------------------------------------------------------------------------------
        public GroupKey GroupKey
        {
            get { return this.Key; }
            set { this.Key = value; }
        }
        public string Name;
        //------------------------------------------------------------------------------------------------------------------------
        public GroupInfoTimelineInfo() : base() { }
        //------------------------------------------------------------------------------------------------------------------------
        public GroupInfoTimelineInfo(API.Warlock.GroupDescriptor desc)
        {
            GroupKey = desc.GroupKey;
            Name = desc.Name;
        }
        //------------------------------------------------------------------------------------------------------------------------
    }
    //------------------------------------------------------------------------------------------------------------------------

    #endregion

    #region Nodes

    //------------------------------------------------------------------------------------------------------------------------
    public class NodeInfoTimelineInfo : TimelineInfo
    {
        //------------------------------------------------------------------------------------------------------------------------
        // store NodeKey and Name only
        public NodeKey NodeKey
        {
            get { return this.Key; }
            set { this.Key = value; }
        }
        public string Name;
        //------------------------------------------------------------------------------------------------------------------------
        public NodeInfoTimelineInfo() : base() { }
        //------------------------------------------------------------------------------------------------------------------------
        public NodeInfoTimelineInfo(Yodiwo.API.Warlock.NodeDescriptor desc)
        {
            NodeKey = desc.NodeKey;
            Name = desc.Name;
        }
    }
    //------------------------------------------------------------------------------------------------------------------------

    #endregion

    #region Things

    //------------------------------------------------------------------------------------------------------------------------
    public class ThingTimelineInfo : TimelineInfo
    {
        //------------------------------------------------------------------------------------------------------------------------
        // store ThingKey and Name only
        public ThingKey ThingKey
        {
            get { return this.Key; }
            set { this.Key = value; }
        }
        public string Name;
        //------------------------------------------------------------------------------------------------------------------------
        public ThingTimelineInfo() : base() { }
        //------------------------------------------------------------------------------------------------------------------------
        public ThingTimelineInfo(Yodiwo.API.Warlock.ThingDescriptor desc)
        {
            ThingKey = desc.ThingKey;
            Name = desc.Name;
        }
        //------------------------------------------------------------------------------------------------------------------------
    }
    //------------------------------------------------------------------------------------------------------------------------

    #endregion

    #region BinaryResourceDescriptor

    //------------------------------------------------------------------------------------------------------------------------
    public class BinaryResourceDescriptorTimelineInfo : TimelineInfo
    {
        //------------------------------------------------------------------------------------------------------------------------
        // store ThingKey and Name only
        public BinaryResourceDescriptorKey BinaryResourceDescriptorKey
        {
            get { return this.Key; }
            set { this.Key = value; }
        }
        public string Name;
        //------------------------------------------------------------------------------------------------------------------------
        public BinaryResourceDescriptorTimelineInfo() : base() { }
        //------------------------------------------------------------------------------------------------------------------------
        public BinaryResourceDescriptorTimelineInfo(Yodiwo.API.Plegma.BinaryResourceDescriptor desc)
        {
            BinaryResourceDescriptorKey = desc.Key;
            Name = desc.FriendlyName;
        }
    }
    //------------------------------------------------------------------------------------------------------------------------

    #endregion

    #region Quota

    //------------------------------------------------------------------------------------------------------------------------
    public class QuotaTimelineInfo : TimelineInfo
    {
        //------------------------------------------------------------------------------------------------------------------------
        public UserKey UserKey
        {
            get { return this.Key; }
            set { this.Key = value; }
        }
        public string Name;
        public long? LimitOld, LimitCurrent;
        public long? TotalNumForPeriodOld, TotalNumForPeriodCurrent;
        public long? TotalNumOld, TotalNumCurrent;
        public TimeSpan? PeriodOld, PeriodCurrent;
        public bool? ManualResetByUser;
        public bool? AfterManualLimitLift;
        public bool? AfterManualPeriodReset;
        public DateTime? StartOfPeriod;
        public int? Percentage;
        public long? Overage;

        //------------------------------------------------------------------------------------------------------------------------
        public QuotaTimelineInfo() : base() { }
    }
    //------------------------------------------------------------------------------------------------------------------------

    #endregion

    #region User

    //------------------------------------------------------------------------------------------------------------------------
    public class UserTimelineInfo : TimelineInfo
    {
        //------------------------------------------------------------------------------------------------------------------------
        public string UserEmail
        {
            get { return this.Key; }
            set { this.Key = value; }
        }
        public string ReasonMsg;
        public int? ReasonCode;
        public int? AccountLevel;
        public ePrivilegeLevel? PrivilegeLevel;
        //------------------------------------------------------------------------------------------------------------------------
        public UserTimelineInfo(string UserEmail) { this.Key = UserEmail; }
        //------------------------------------------------------------------------------------------------------------------------
    }
    //------------------------------------------------------------------------------------------------------------------------

    #endregion

    #endregion
}
