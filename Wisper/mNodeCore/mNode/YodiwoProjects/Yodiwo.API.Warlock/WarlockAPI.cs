using System;
using System.Collections.Generic;
using System.Linq;
using Yodiwo.API.Plegma.NodePairing;

// According to recent discussions there will be 3 API umbrellas, Basic, Media Streaming, and Creative
// Plegma: communication between EndPoints (Nodes) and Cloud Server; Storage APIs; etc
// MediaStreaming: self-explanatory
// Creative: APIs to create new components and plugins to be run inside the Yodiwo Cloud (or GW) software. To be submitted for review by community

namespace Yodiwo.API.Warlock
{
    public static partial class WarlockAPI
    {
        private static Type[] PrivateApiMessages =
       {
            typeof(Yodiwo.API.Warlock.Private.WarlockAuthenticationRequest),
            typeof(Yodiwo.API.Warlock.Private.WarlockAuthenticationResponse),
            typeof(Yodiwo.API.Warlock.Private.GetTimelineDescriptorsRsp),
            typeof(Yodiwo.API.Warlock.Private.GetTimelineDescriptorsReq)
        };
        private static Type[] AdvancedApiMessages =
       {
            typeof(LiveKeysAddMsg),
            typeof(LiveKeysDelMsg),
            typeof(LiveValuesMsg),
            typeof(RegisterUserKeyReq),
            typeof(RegisterUserKeyRsp),
            typeof(UnRegisterUserKey),
            typeof(EmailRequest),
            typeof(UserLoginReq),
            typeof(UserLoginRsp),
            typeof(LiveExecutionEvent),
            typeof(LiveExecutionAddListeners),
            typeof(LiveExecutionRemoveListeners),
            typeof(WorkerEventNotification),
            typeof(EvNewNotification),
            typeof(EvNewBackendNotification),
            typeof(EvNewFrontendNotification),
            typeof(ShareNotifyMsg),
            typeof(CreateValueTriggerGraphEvent),
            typeof(CreateWatchdogGraphEvent),
            typeof(CreateGraphRsp)
        };

        public static string s_LiveKeysAddMsg = nameof(LiveKeysAddMsg).ToLower();
        public static string s_LiveKeysDelMsg = nameof(LiveKeysDelMsg).ToLower();
        public static string s_RegisterUserKeyReq = nameof(RegisterUserKeyReq).ToLower();
        public static string s_RegisterUserKeyRsp = nameof(RegisterUserKeyRsp).ToLower();
        public static string s_UnRegisterUserKey = nameof(UnRegisterUserKey).ToLower();
        public static string s_EmailRequest = nameof(EmailRequest).ToLower();
        public static string s_UserLoginReq = nameof(UserLoginReq).ToLower();
        public static string s_UserLoginRsp = nameof(UserLoginRsp).ToLower();
        public static string s_ShareNotifyMsg = nameof(ShareNotifyMsg).ToLower();
        public static string s_LiveExecutionEvent = nameof(LiveExecutionEvent).ToLower();
        public static string s_LiveExecutionAddListeners = nameof(LiveExecutionAddListeners).ToLower();
        public static string s_LiveExecutionRemoveListeners = nameof(LiveExecutionRemoveListeners).ToLower();
        public static string s_WorkerEventNotification = nameof(WorkerEventNotification).ToLower();
        public static string s_EvNewNotification = nameof(EvNewNotification).ToLower();
        public static string s_EvNewBackendNotification = nameof(EvNewBackendNotification).ToLower();
        public static string s_EvNewFrontendNotification = nameof(EvNewFrontendNotification).ToLower();
        public static string s_LiveValuesMsg = nameof(LiveValuesMsg).ToLower();
        public static string s_CreateValueTriggerGraphEvent = nameof(CreateValueTriggerGraphEvent).ToLower();
        public static string s_CreateWatchdogGraphEvent = nameof(CreateWatchdogGraphEvent).ToLower();
        public static string s_CreateGraphRsp = nameof(CreateGraphRsp).ToLower();

        public static Dictionary<String, Type> ApiMsgNamesToTypes;

        static partial void ExtendWarlockProtocol()
        {
            WarlockAPI.ApiMessages = WarlockAPI.ApiMessages.Concat(PrivateApiMessages)
                                                            .Concat(AdvancedApiMessages)
                                                            .ToArray();
            //extend api msg names
            WarlockAPI.ApiMsgNames.AddFromSource(new Dictionary<Type, string>()
            {
                {typeof(LiveKeysAddMsg),s_LiveKeysAddMsg },
                {typeof(LiveKeysDelMsg),s_LiveKeysDelMsg },
                {typeof(RegisterUserKeyReq),s_RegisterUserKeyReq },
                {typeof(RegisterUserKeyRsp),s_RegisterUserKeyRsp },
                {typeof(UnRegisterUserKey), s_UnRegisterUserKey},
                {typeof(EmailRequest), s_EmailRequest},
                {typeof(UserLoginReq), s_UserLoginReq},
                {typeof(UserLoginRsp), s_UserLoginRsp},
                {typeof(ShareNotifyMsg), s_ShareNotifyMsg},
                {typeof(LiveExecutionEvent), s_LiveExecutionEvent},
                {typeof(LiveExecutionAddListeners), s_LiveExecutionAddListeners},
                {typeof(LiveExecutionRemoveListeners), s_LiveExecutionRemoveListeners},
                {typeof(WorkerEventNotification), s_WorkerEventNotification},
                {typeof(EvNewNotification), s_EvNewNotification },
                {typeof(EvNewBackendNotification), s_EvNewBackendNotification },
                {typeof(EvNewFrontendNotification), s_EvNewFrontendNotification },
                {typeof(LiveValuesMsg), s_LiveValuesMsg},
                {typeof(CreateValueTriggerGraphEvent), s_CreateValueTriggerGraphEvent},
                {typeof(CreateWatchdogGraphEvent), s_CreateWatchdogGraphEvent},
                {typeof(CreateGraphRsp), s_CreateGraphRsp}
            });

            //extend permission matrix
            PermissionMatrix.AddFromSource(new Dictionary<Type, eNodePermissions>() { { typeof(ShareNotifyMsg), eNodePermissions.AllowSharingCtrl } });
            //look up
            ApiMsgNamesToTypes = ApiMsgNames.Select(e => new KeyValuePair<string, Type>(e.Value, e.Key)).ToDictionary();
        }
    }
}

