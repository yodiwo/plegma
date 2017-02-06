using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;
using Yodiwo.API.Warlock;
using Yodiwo.API.Warlock.Private;
using Yodiwo;


namespace Yodiwo.API.Warlock.Code
{
    public static class WarlockMsgHandler
    {

        [WarlockHandlerAttribute]
        public static void HandleNotificationofDeployedGraph(UserKey userkey, Dictionary<int, ValidationResultDescriptor> results)
        {
            //todo:handle notification of deployed graph
        }
    }
}
