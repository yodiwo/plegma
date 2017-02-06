using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.API.Warlock
{
    public enum ePrivilegeLevel
    {
        Guest = 0,
        User,

        PartialReadAdmin = 10,
        PartialWriteAdmin = 11,

        YodiReadAdmin = 99,
        YodiQuotaAdmin = 100,
        YodiWriteAdmin = 101,
    }

    public enum eUserPrivilegeLevel
    {
        None,
        ReadOnly,
        WriteOnly,
        All,
    }
}
