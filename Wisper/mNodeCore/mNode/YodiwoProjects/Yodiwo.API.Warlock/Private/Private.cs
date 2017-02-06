using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;

namespace Yodiwo.API.Warlock.Private
{
    /// <summary>
    ///WarlockAuthenticationRequest: used as request from the Warlock server for the negotiation with a WarlockClient
    /// </summary>
    [Serializable]
    public class WarlockAuthenticationRequest : WarlockApiMsg
    {
    }

    /// <summary>
    ///WarlockAuthenticationResponse: used as response from the WarlockClient so as to authenticate itself to the WarlockServer
    /// </summary>
    [Serializable]
    public class WarlockAuthenticationResponse : WarlockApiMsg
    {
        /// <summary>secret that a WarlockClient has used so as to connect to the WarlockServer</summary>
        public string Secret;
    }

    public class WarlockHandlerAttribute : Attribute
    {


    }

    public class WarlockDisplayAttribute : Attribute
    {
        public string summary;
        public Dictionary<string, string> parameters = new Dictionary<string, string>();
        public string returns;
        public WarlockDisplayAttribute(string sum, string parameters = "", string ret = "")
        {
            this.summary = sum;
            if (!string.IsNullOrEmpty(parameters))
            {
                var myparams = parameters.Split(',');
                foreach (var param in myparams)
                {
                    var name = param.Split('=')[0];
                    var namesum = param.Split('=')[1];
                    this.parameters.Add(name, namesum);
                }
            }
            this.returns = ret;
        }
    }

    public class WarlockNotExposedHandlerAttribute : Attribute
    {
    }

    public class WarlockPermissionAttribute : Attribute
    {
        public eNodePermissions Permissions;
        public WarlockPermissionAttribute(eNodePermissions Permissions)
        {
            this.Permissions = Permissions;
        }
    }

    public class RestServiceWizardDescriptor
    {
        //------------------------------------------------------------------------------------------------------------------------
        public string BlockName;
        //------------------------------------------------------------------------------------------------------------------------
        public string Icon;
        //TODO: could probably just use actual service descriptor objects instead of the serialized form
        public string JsonService;
    }

    public class ConfiguredRestServiceWizardDescriptor
    {
        //------------------------------------------------------------------------------------------------------------------------
        public string BlockName;
        //------------------------------------------------------------------------------------------------------------------------
        public string Icon;
        public string ServiceKey;
        public Dictionary<string, string> Configuration;
        public DateTime LastTokenUpdate;
        public bool IsUserService = false;
    }

    public class GlobalConfiguredServiceResp : GenericRsp
    {
        public RestServiceWizardDescriptor restservice;
        public ConfiguredRestServiceWizardDescriptor configuredrestservice;
        public string JsonrestServiceDescriptor;
    }

    //------------------------------------------------------------------------------------------------------------------------
    public class GetTimelineDescriptorsReq : WarlockApiMsg
    {
        public UserKey RequestingUser;
        public TimelineDescriptorType Type;
        public DateTime? FromTS = null;
        public DateTime? ToTS = null;
        public TimelineDescriptorOperationType Operation = Private.TimelineDescriptorOperationType.Unknown;
        public System.Int32 CurrentPage = -1;
        public System.Int32 PageSize = 0;
    }
    //------------------------------------------------------------------------------------------------------------------------
    public class GetTimelineDescriptorsRsp : Yodiwo.API.Warlock.GenericValueST<List<Yodiwo.API.Warlock.Private.TimelineDescriptor>>
    {
        public long TotalRecords;
    }
    //------------------------------------------------------------------------------------------------------------------------
    public class GetUsersRsp : Yodiwo.API.Warlock.GenericValueST<List<Yodiwo.API.Warlock.UserDescriptor>>
    {
        public long UsersCnt;
    }
    //------------------------------------------------------------------------------------------------------------------------
}