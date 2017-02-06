using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;

namespace Yodiwo.API.Warlock
{
    public class NotificationDescriptor
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public NotificationDescriptorKey NotificationDescriptorKey;
        //------------------------------------------------------------------------------------------------------------------------
        public string Description;
        //------------------------------------------------------------------------------------------------------------------------
        public NotificationDescriptorType Type;
        public NotificationDescriptorOperationType Operation;
        public DateTime Timestamp;
        //------------------------------------------------------------------------------------------------------------------------
        #region helper variables for UI Notifications' Environment
        //------------------------------------------------------------------------------------------------------------------------
        public object NotificationDescriptorRelatedObject;/* contains GraphDescriptor | NodeInfo | YThing | YNodeThingType | GraphKey
                                        UserFacebookInfo | UserSkypeInfo |  UserHangoutsInfo| UserSkypeInfo
                                        UserIrcInfo |UserSipInfo | UserLyncInfo | RestServiceInfo
                                     */
        public bool IsRead; // bool used to determine if the Notification is read
        public bool HasRelatedObject;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------------------------------------------
        #region Helper variables for Pending/Successful requests
        //------------------------------------------------------------------------------------------------------------------------
        [Newtonsoft.Json.JsonIgnore]
        public bool IsRequest { get { return !String.IsNullOrEmpty(PendingToken); } }
        public string PendingToken;
        //------------------------------------------------------------------------------------------------------------------------
        // if true, means that the user did not accepted/refused the request
        public bool IsPending;
        //------------------------------------------------------------------------------------------------------------------------
        // if true, the user accept/refused the request
        // public bool Accepted;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #endregion
        //------------------------------------------------------------------------------------------------------------------------
        public NotificationDescriptor()
        {
            Timestamp = DateTime.UtcNow;
            IsRead = false;
        }
        //------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Generate the graph model based on Logic.Graph.
        /// </summary>
        public NotificationDescriptor(NotificationDescriptorType Type, NotificationDescriptorOperationType Operation, NotificationDescriptorRelatedObject RelatedObject, string DescriptionMessage)
            : this()
        {
            this.HasRelatedObject = RelatedObject != null;
            this.NotificationDescriptorRelatedObject = RelatedObject;
            this.Type = Type;
            this.Operation = Operation;
            this.Description = DescriptionMessage;
        }

        public enum NotificationDescriptorType
        {
            Accounts,
            Graphs,
            Nodes,
            Things,
            ThingType,
            SampleGraphs,
            Friends,
            UserGraphException,
            Quota
        }

        public enum NotificationDescriptorOperationType
        {
            UnDeployAfterThingRemove,
            SharedThing,
            UnsharedThing,
            RemovedSampleGraphs,
            UserGraphException,
            UnDeployAfterSocialRemove,
            QuotaLimitCrossed,
            QuotaPercentageCrossed,
            QuotaLimitUncrossed,
            UnDeployAfterGroupRemove,
            SharedGraph
        }
    }

    #region Notification classes

    [Newtonsoft.Json.JsonSafeType]
    public abstract class NotificationDescriptorRelatedObject
    {
    }

    #region SampleGraphDescriptorNotificationInfo
    //SampleGraphDescriptor Details for the NotificationDescriptor 
    [Newtonsoft.Json.JsonSafeType]
    public class SampleGraphDescriptorNotificationInfo : NotificationDescriptorRelatedObject
    {
        // store SampleGraphDescriptorKey,GraphFriendlyName, along with nodeType
        public List<Yodiwo.API.Plegma.GraphDescriptorKey> GraphDescriptorKey;
        public List<string> GraphFriendlyName;
        public string NodeType;
    }
    #endregion

    #region GraphDescriptorNotificationInfo
    //GraphDescriptor Details for the NotificationDescriptor 
    [Newtonsoft.Json.JsonSafeType]
    public class GraphDescriptorNotificationInfo : NotificationDescriptorRelatedObject
    {
        // store GraphDescriptorKey,FriendlyName and Path only
        public Yodiwo.API.Plegma.GraphDescriptorKey GraphDescriptorKey;
        public string FriendlyName;
        public String Path;
    }
    #endregion

    #region GraphSharedNotificationInfo
    [Newtonsoft.Json.JsonSafeType]
    public class GraphSharedNotificationInfo : GraphDescriptorNotificationInfo
    {
        public string RequesterEmail;
    }
    #endregion

    #region NodeInfoNotificationInfo
    //NodeInfo Details for the NotificationDescriptor 
    [Newtonsoft.Json.JsonSafeType]
    public class NodeInfoNotificationInfo : NotificationDescriptorRelatedObject
    {
        // store NodeKey and Name only
        public NodeKey NodeKey;
        public string Name;
    }
    #endregion

    #region YThingNotificationInfo
    //YThing Details for the NotificationDescriptor 
    [Newtonsoft.Json.JsonSafeType]
    public class YThingNotificationInfo : NotificationDescriptorRelatedObject
    {
        // store ThingKey and Name only
        public ThingKey ThingKey;
        public string Name;
        public string RequesterEmail; // Owner e-mail
        public string Owner; // Remove after NotificationMgmt migration to version 3
    }
    #endregion

    #region ThingNotificationInfo
    //Thing Details for the NotificationDescriptor 
    [Newtonsoft.Json.JsonSafeType]
    public class ThingNotificationInfo : NotificationDescriptorRelatedObject
    {
        // store ThingKey and Name only
        public ThingKey ThingKey;
        public string Name;
        public string Owner; // Owner e-mail
    }
    #endregion

    #region GraphDescriptorAndThingNotificationInfo
    //ThingKey and GraphDescriptor for the NotificationDescriptor 
    [Newtonsoft.Json.JsonSafeType]
    public class GraphDescriptorAndThingNotificationInfo : NotificationDescriptorRelatedObject
    {
        // store ThingKey and Name only
        public ThingKey ThingKey;
        public GraphDescriptorKey GraphDescriptorKey;
        public string FriendlyName;
        public string Path;
        public string ThingName;
    }
    #endregion

    #region GraphDescriptorAndGroupNotificationInfo
    //ThingKey and GraphDescriptor for the NotificationDescriptor 
    [Newtonsoft.Json.JsonSafeType]
    public class GraphDescriptorAndGroupNotificationInfo : NotificationDescriptorRelatedObject
    {
        // store ThingKey and Name only
        public GroupKey GroupKey;
        public GraphDescriptorKey GraphDescriptorKey;
        public string FriendlyName;
        public string Path;
        public string GroupName;
    }
    #endregion

    #region YThingTypeNotificationInfo
    //YThingType Details for the NotificationDescriptor 
    [Newtonsoft.Json.JsonSafeType]
    public class YThingTypeNotificationInfo : NotificationDescriptorRelatedObject
    {
        // store NodeKey only
        public NodeKey NodeKey;
    }
    #endregion

    #region Accounts
    //UserFacebookInfo Details for the NotificationDescriptor 
    [Newtonsoft.Json.JsonSafeType]
    public class UserFacebookInfoNotificationInfo : NotificationDescriptorRelatedObject
    {
        // store jid, email and name only
        public string jid;
        public string email;
        public string name;
    }

    //UserSkypeInfo Details for the NotificationDescriptor 
    [Newtonsoft.Json.JsonSafeType]
    public class UserSkypeInfoNotificationInfo : NotificationDescriptorRelatedObject
    {
        // store msmail only
        //public string skypeId;
        public string msmail;
    }

    //UserHangoutsInfo Details for the NotificationDescriptor 
    [Newtonsoft.Json.JsonSafeType]
    public class UserHangoutsInfoNotificationInfo : NotificationDescriptorRelatedObject
    {
        // store jid, email and name only
        public string jid;
        public string email;
        public string name;
    }

    //UserIrcInfo Details for the NotificationDescriptor 
    [Newtonsoft.Json.JsonSafeType]
    public class UserIrcInfoNotificationInfo : NotificationDescriptorRelatedObject
    {
        // store nickname and subscribedchannels only
        public List<string> subscribedchannels;
        public string nickname;
    }

    //UserSipInfo Details for the NotificationDescriptor 
    [Newtonsoft.Json.JsonSafeType]
    public class UserSipInfoNotificationInfo : NotificationDescriptorRelatedObject
    {
        // store sipusername and IOPLync only
        public string sipusername;
        public bool IOPLync;
    }

    //UserLyncInfo Details for the NotificationDescriptor 
    [Newtonsoft.Json.JsonSafeType]
    public class UserLyncInfoNotificationInfo : NotificationDescriptorRelatedObject
    {
        // store lyncusername and IOPSip only
        public string lyncusername;
        public bool IOPSip;
    }

    //RestServiceInfo Details for the NotificationDescriptor 
    [Newtonsoft.Json.JsonSafeType]
    public class RestServiceInfoNotificationInfo : NotificationDescriptorRelatedObject
    {
        // store BlockName, Icon and JsonService only
        public string BlockName;
        public string Icon;
        public string JsonService;
    }
    #endregion

    #region UserGraphExceptionNotificationInfo
    [Newtonsoft.Json.JsonSafeType]
    //UserGraphException Details for the NotificationDescriptor 
    public class UserGraphExceptionNotificationInfo : NotificationDescriptorRelatedObject
    {
        public string ErrorMsg;
    }
    #endregion

    #region Quota
    [Newtonsoft.Json.JsonSafeType]
    public class QuotaNotificationInfo : NotificationDescriptorRelatedObject
    {
        public string Name;
        public bool AfterManualLimitLift;
        public bool AfterManualPeriodReset;
        public int Percentage;
        public long Limit;
        public TimeSpan Period;
    }
    #endregion

    #endregion

}
