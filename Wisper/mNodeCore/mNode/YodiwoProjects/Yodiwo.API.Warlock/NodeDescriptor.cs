using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;
using Yodiwo.API.Warlock;

namespace Yodiwo.API.Warlock
{
    public class NodeDescriptor
    {

        #region Variables
        //-------------------------------------------------------------------------------------------------------------------------
        public NodeKey NodeKey;
        //secret key added later for webappsmodule (nick should review it)!!!!
        public string SecretKey;
        public string Uuid;
        public string Name;
        public eNodeType Type;
        public string TypeStr;
        public eNodeCapa Capabilities;
        public eNodePermissions Permissions;
        public bool IsDisabled;
        public bool IgnoreQuota;
        public string PermissionsStr
        {
            get { return Permissions.ToString(); }
            set { Permissions = PermissionsFromString(value); }
        }
        public string CapabilitiesStr;
        public int ThingsCount;
        public eNodeStatus Status;
        public int SupportedApiRev;
        /// <summary>Creation (pairing) of node timestamp, in msec since the Unix Epoch</summary>
        public string PairingTimestamp;
        /// <summary>current Node's Things revision number, as defined in Plegma API</summary>
        public int ThingsRevNum;
        public List<ThingDescriptor> Things;
        public Dictionary<string, string> AdditionalInfo = new Dictionary<string, string>();
        public ActiveNodeDescriptor ActiveInfo;
        public QuotaDescriptor BytesIO;
        public QuotaDescriptor EventsIO;
        public Dictionary<string, UserApiKey> LinkedApiKeys;
        public string PrimaryEmail;
        //-------------------------------------------------------------------------------------------------------------------------
        public bool IsVirtual { get { return NodeKey.IsVirtual; } }
        #endregion

        #region Constructors
        //-------------------------------------------------------------------------------------------------------------------------
        public NodeDescriptor() { }
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        private static eNodePermissions PermissionsFromString(string value)
        {
            // assign default
            eNodePermissions temp = eNodePermissions.NoPermissions;
            try
            {
                if (!Enum.TryParse<eNodePermissions>(value, out temp))
                    // assign default
                    temp = eNodePermissions.NoPermissions;
            }
            catch (Exception ex)
            {
                DebugEx.TraceErrorException(ex);
            }
            return temp;
        }

        /// <summary>
        /// Map eNodePermissions to description
        /// </summary>
        /// <param name="Permissions"></param>
        /// <returns></returns>

        public static string PermissionsToString(eNodePermissions Permissions)
        {
            switch (Permissions)
            {
                case eNodePermissions.NoPermissions:
                    return "No Permissions";
                case eNodePermissions.AllPermissions:
                    return "All Permissions";
                default:
                    var str = Permissions.ToString();
                    return Regex.Replace(str, "(?=[A-Z][a-z])|(?<=[a-z])(?=[A-Z])", " ", RegexOptions.Compiled).Trim().Replace("Ctrl", "Control");
            }
        }
        #endregion

    }
}


