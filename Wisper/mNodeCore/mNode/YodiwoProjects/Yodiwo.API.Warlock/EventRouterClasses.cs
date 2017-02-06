using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;

namespace Yodiwo.API.Warlock
{
    public class EvCommon
    {
        public string Message = "";
        public bool ReportIt = true;
    }
    //--------------------------------------------------
    public class EvThingCommon : EvCommon
    {
        public ThingKey ThingKey;
        public string Name; //OLD name!
    }
    //--------------------------------------------------
    public class EvThingNameUpdated : EvThingCommon { }
    //--------------------------------------------------
    public class EvThingConfigUpdated : EvThingCommon
    {
        public ConfigParameter[] OldConfig;
    }
    //--------------------------------------------------
    public class EvThingShared : EvThingCommon
    {
        public UserKey DestinationUserKey;
        public string RequesterEmail;//  Owner e-mail
        public bool IsRead; // in case this event relates to a EvThingSharePending event
    }
    //--------------------------------------------------
    public class EvThingSharePending : EvThingShared
    {
        // true|false, whether the requested user responded or not
        // default -> true
        public bool IsPending = true;
        public string PendingToken;
    }
    //--------------------------------------------------
    public class EvThingUnshared : EvThingCommon
    {
        public UserKey DestinationUserKey;
        public UserKey RequesterUserKey;
        public string RequesterEmail;
    }
    public class EvSampleGraphsRemoved : EvCommon
    {
        public List<GraphDescriptorKey> GraphDescriptorKey;
        public List<string> GraphFriendlyName;
        public string NodeType;
    }
    public class EvGraphsUndeployedDueToThingRemoval : EvCommon
    {
        public UserKey UserKey;
    }
    public class EvGraphsUndeployedDueToGroupRemoval : EvCommon
    {
        public UserKey UserKey;
    }
    public class EvNewNotification : EvCommon
    {
        public UserKey DestinationUserKey;
        public NotificationDescriptor NotificationDescriptor;
    }
    public class EvNewBackendNotification : EvNewNotification { }
    public class EvNewFrontendNotification : EvNewNotification { }

}
