using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;

namespace Yodiwo.Logic
{
    public static class UserTrace
    {
        public enum EvUserTraceMsgType
        {
            Info,
            Warning,
            Error
        }

        public class EvUserTrace
        {
            public DateTime Timestamp;
            public Yodiwo.API.Plegma.UserKey UserKey;
            public string Msg;
            public EvUserTraceMsgType MsgType;
        }

        public static void UserTraceEx(UserKey userKey, string msg, EvUserTraceMsgType type)
        {
            Yodiwo.YEventRouter.EventRouter.TriggerEvent<EvUserTrace>(null, new EvUserTrace() { Timestamp = DateTime.UtcNow, UserKey = userKey, Msg = msg, MsgType = type });
        }
    }
}
