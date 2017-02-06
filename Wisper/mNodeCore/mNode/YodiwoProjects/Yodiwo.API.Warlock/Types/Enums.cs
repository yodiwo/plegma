using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.API.Warlock
{
    [Flags]
    public enum eNodePermissions : int
    {
        NoPermissions = 0,

        AllowUserQueries = 1 << 0,
        AllowGraphCtrl = 1 << 1,
        AllowSharingCtrl = 1 << 2,
        AllowPermissionCtrl = 1 << 3,
        AllowCrossNodeMsgs = 1 << 4,

        //do not switch on AllowInterNodeMsgs by default
        AllPermissions = AllowUserQueries | AllowGraphCtrl | AllowSharingCtrl | AllowPermissionCtrl | AllowCrossNodeMsgs
    }

    public enum eNodeStatus
    {
        Unknown,
        Offline,
        Online
    }

    public enum NodeUpdateAction
    {
        None,
        Name,
        Permissions
    }
}
