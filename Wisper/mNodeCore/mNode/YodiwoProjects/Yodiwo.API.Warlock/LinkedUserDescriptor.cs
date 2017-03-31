using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yodiwo.API.Plegma;

namespace Yodiwo.API.Warlock
{
    public class LinkedUserDescriptor
    {
        public SubUserKey SubUserKey;
        public string Name;
        public string Email;
        public string AvatarUri;
        public bool IsPrimary;

        public string CreatedTimeStr;
        public DateTime CreatedTime
        {
            get { return CreatedTimeStr.ParseToDateTime(); }
            set { CreatedTimeStr = value.ToString(); }
        }

        public string LastLoginDateStr;
        public DateTime LastLoginDate
        {
            get { return LastLoginDateStr.ParseToDateTime(); }
            set { LastLoginDateStr = value.ToString(); }
        }

        public string PrivilegeLevelStr;
        public eUserPrivilegeLevel PrivilegeLevel
        {
            get { return PrivilegeFromString(PrivilegeLevelStr); }
            set { PrivilegeLevelStr = value.ToString(); }
        }

        private static eUserPrivilegeLevel PrivilegeFromString(string value)
        {
            var privilege = eUserPrivilegeLevel.None;
            try
            {
                if (!Enum.TryParse(value, out privilege))
                {
                    privilege = eUserPrivilegeLevel.None;
                }
            }
            catch (Exception ex)
            {
                DebugEx.TraceErrorException(ex);
            }
            return privilege;
        }
    }
}