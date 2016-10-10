package com.yodiwo.androidagent.plegma;

import java.util.HashMap;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:35:02.
 */

public class PlegmaAPI {

    public static final int APIVersion;

    public static final char KeySeparator;

    public static final String BroadcastToken;

    public static final Class<?>[] ApiMessages;

    public static final String s_LoginReq;
    public static final String s_LoginRsp;

    public static final String s_NodeInfoReq;
    public static final String s_NodeInfoRsp;
    public static final String s_NodeUnpairedReq;

    public static final String s_ThingsGet;
    public static final String s_ThingsSet;

    public static final String s_PortEventMsg;
    public static final String s_PortStateGet;
    public static final String s_PortStateSet;
    public static final String s_ActivePortKeysMsg;

    public static final String s_PingReq;
    public static final String s_PingRsp;

    public static final String s_GenericRsp;

    public static final String s_ShareThingsRsp;
    public static final String s_ShareThingsReq;

    public static final String s_ShareNotifyMsg;

    public static final String s_GetUserTokensRsp;
    public static final String s_GetUserTokensReq;

    public static final String s_GetUserInfoRsp;
    public static final String s_GetUserInfoReq;

    public static final String s_GetFriendsRsp;
    public static final String s_GetFriendsReq;

    public static final String s_GetThingsRsp;
    public static final String s_GetThingsReq;

    public static final String s_GetNodeInfoRsp;
    public static final String s_GetNodeInfoReq;

    public static final String s_GraphActionRsp;
    public static final String s_GraphActionReq;

    public static final String s_AddFriendReq;

    public static final String s_MyHackingReq;
    public static final String s_MyHackingRsp;

    public static final String s_LiveKeysAddMsg;
    public static final String s_LiveKeysDelMsg;
    public static final String s_LiveValuesMsg;

    public static final String s_UnknownRsp;

    public static final HashMap<Class<?>, String> ApiMsgNames;

    public static final HashMap<String, Class<?>> ApiMsgNamesToTypes;


    static {
        APIVersion = 1;
        KeySeparator = '-';
        BroadcastToken = "*";
        ApiMessages = new Class<?>[]{LoginReq.class, LoginRsp.class,
                NodeInfoReq.class, NodeInfoRsp.class,
                ThingsGet.class, ThingsSet.class,
                PortEventMsg.class,
                PortStateGet.class, PortStateSet.class,
                ActivePortKeysMsg.class,
                ShareThingsReq.class, ShareThingsRsp.class,
                ShareNotifyMsg.class,
                GetUserTokensReq.class, GetUserTokensRsp.class,
                GetUserInfoReq.class, GetUserInfoRsp.class,
                GetFriendsRsp.class, GetFriendsReq.class,
                GetThingsReq.class, GetThingsRsp.class,
                GetNodeInfoReq.class, GetNodeInfoRsp.class,
                GraphActionRsp.class, GraphActionRsp.class,
                GraphActionReq.class, GraphActionReq.class,
                AddFriendReq.class,
                MyHackingReq.class, MyHackingRsp.class,
                LiveKeysAddMsg.class, LiveKeysDelMsg.class, LiveValuesMsg.class};
        s_LoginReq = "loginreq";
        s_LoginRsp = "loginrsp";
        s_NodeInfoReq = "nodeinforeq";
        s_NodeInfoRsp = "nodeinforsp";
        s_NodeUnpairedReq = "nodeunpairedreq";
        s_ThingsGet = "thingsget";
        s_ThingsSet = "thingsset";
        s_PortEventMsg = "porteventmsg";
        s_PortStateGet = "portstateget";
        s_PortStateSet = "portstateset";
        s_ActivePortKeysMsg = "activeportkeysmsg";
        s_PingReq = "pingreq";
        s_PingRsp = "pingrsp";
        s_GenericRsp = "genericrsp";
        s_ShareThingsRsp = "sharethingsrsp";
        s_ShareThingsReq = "sharethingsreq";
        s_GetUserTokensRsp = "getusertokensrsp";
        s_GetUserTokensReq = "getusertokensreq";
        s_ShareNotifyMsg = "sharenotifymsg";
        s_GetUserInfoReq = "getuserinforeq";
        s_GetUserInfoRsp = "getuserinforsp";
        s_GetFriendsReq = "getfriendsreq";
        s_GetFriendsRsp = "getfriendsrsp";
        s_GetThingsReq = "getthingsreq";
        s_GetThingsRsp = "getthingsrsp";
        s_GetNodeInfoReq = "getnodeinforeq";
        s_GetNodeInfoRsp = "getnodeinforsp";
        s_GraphActionReq = "graphactionreq";
        s_GraphActionRsp = "graphactionrsp";
        s_AddFriendReq = "addfriendreq";
        s_LiveKeysAddMsg = "livekeysaddmsg";
        s_LiveKeysDelMsg = "livekeysdelmsg";
        s_LiveValuesMsg = "livevaluesmsg";

        s_MyHackingReq = "myhackingreq";
        s_MyHackingRsp = "myhackingrsp";

        ApiMsgNames = new HashMap<>();
        ApiMsgNames.put(LoginReq.class, s_LoginReq);
        ApiMsgNames.put(LoginRsp.class, s_LoginRsp);
        ApiMsgNames.put(NodeInfoReq.class, s_NodeInfoReq);
        ApiMsgNames.put(NodeInfoRsp.class, s_NodeInfoRsp);
        ApiMsgNames.put(NodeUnpairedReq.class, s_NodeUnpairedReq);
        ApiMsgNames.put(ThingsGet.class, s_ThingsGet);
        ApiMsgNames.put(ThingsSet.class, s_ThingsSet);
        ApiMsgNames.put(PortEventMsg.class, s_PortEventMsg);
        ApiMsgNames.put(PortStateGet.class, s_PortStateGet);
        ApiMsgNames.put(PortStateSet.class, s_PortStateSet);
        ApiMsgNames.put(ActivePortKeysMsg.class, s_ActivePortKeysMsg);
        ApiMsgNames.put(PingReq.class, s_PingReq);
        ApiMsgNames.put(PingRsp.class, s_PingRsp);
        ApiMsgNames.put(ShareThingsRsp.class, s_ShareThingsRsp);
        ApiMsgNames.put(ShareThingsReq.class, s_ShareThingsReq);
        ApiMsgNames.put(ShareNotifyMsg.class, s_ShareNotifyMsg);
        ApiMsgNames.put(GetUserTokensReq.class, s_GetUserTokensReq);
        ApiMsgNames.put(GetUserTokensRsp.class, s_GetUserTokensRsp);
        ApiMsgNames.put(GetUserInfoReq.class, s_GetUserInfoReq);
        ApiMsgNames.put(GetUserInfoRsp.class, s_GetUserInfoRsp);
        ApiMsgNames.put(GetFriendsReq.class, s_GetFriendsReq);
        ApiMsgNames.put(GetFriendsRsp.class, s_GetFriendsRsp);
        ApiMsgNames.put(GetThingsReq.class, s_GetThingsReq);
        ApiMsgNames.put(GetThingsRsp.class, s_GetThingsRsp);
        ApiMsgNames.put(GetNodeInfoReq.class, s_GetNodeInfoReq);
        ApiMsgNames.put(GetNodeInfoRsp.class, s_GetNodeInfoRsp);
        ApiMsgNames.put(GraphActionReq.class, s_GraphActionReq);
        ApiMsgNames.put(GraphActionRsp.class, s_GraphActionRsp);
        ApiMsgNames.put(AddFriendReq.class, s_AddFriendReq);
        ApiMsgNames.put(LiveKeysAddMsg.class, s_LiveKeysAddMsg);
        ApiMsgNames.put(LiveKeysDelMsg.class, s_LiveKeysDelMsg);
        ApiMsgNames.put(LiveValuesMsg.class, s_LiveValuesMsg);

        ApiMsgNames.put(GenericRsp.class, s_GenericRsp);

        ApiMsgNames.put(MyHackingReq.class, s_MyHackingReq);
        ApiMsgNames.put(MyHackingRsp.class, s_MyHackingRsp);

        ApiMsgNamesToTypes = new HashMap<>();
        ApiMsgNamesToTypes.put(s_LoginReq, LoginReq.class);
        ApiMsgNamesToTypes.put(s_LoginRsp, LoginRsp.class);
        ApiMsgNamesToTypes.put(s_NodeInfoReq, NodeInfoReq.class);
        ApiMsgNamesToTypes.put(s_NodeInfoRsp, NodeInfoRsp.class);
        ApiMsgNamesToTypes.put(s_NodeUnpairedReq, NodeUnpairedReq.class);
        ApiMsgNamesToTypes.put(s_ThingsGet, ThingsGet.class);
        ApiMsgNamesToTypes.put(s_ThingsSet, ThingsSet.class);
        ApiMsgNamesToTypes.put(s_PortEventMsg, PortEventMsg.class);
        ApiMsgNamesToTypes.put(s_PortStateGet, PortStateGet.class);
        ApiMsgNamesToTypes.put(s_PortStateSet, PortStateSet.class);
        ApiMsgNamesToTypes.put(s_ActivePortKeysMsg, ActivePortKeysMsg.class);
        ApiMsgNamesToTypes.put(s_PingReq, PingReq.class);
        ApiMsgNamesToTypes.put(s_PingRsp, PingRsp.class);
        ApiMsgNamesToTypes.put(s_GenericRsp, GenericRsp.class);
        ApiMsgNamesToTypes.put(s_ShareThingsRsp, ShareThingsRsp.class);
        ApiMsgNamesToTypes.put(s_ShareThingsReq, ShareThingsReq.class);
        ApiMsgNamesToTypes.put(s_ShareNotifyMsg, ShareNotifyMsg.class);
        ApiMsgNamesToTypes.put(s_GetUserTokensReq, GetUserTokensReq.class);
        ApiMsgNamesToTypes.put(s_GetUserTokensRsp, GetUserTokensRsp.class);
        ApiMsgNamesToTypes.put(s_GetUserInfoReq, GetUserInfoReq.class);
        ApiMsgNamesToTypes.put(s_GetUserInfoRsp, GetUserInfoRsp.class);
        ApiMsgNamesToTypes.put(s_GetFriendsReq, GetFriendsReq.class);
        ApiMsgNamesToTypes.put(s_GetFriendsRsp, GetFriendsRsp.class);
        ApiMsgNamesToTypes.put(s_GetThingsReq, GetThingsReq.class);
        ApiMsgNamesToTypes.put(s_GetThingsRsp, GetThingsRsp.class);
        ApiMsgNamesToTypes.put(s_GetNodeInfoReq, GetNodeInfoReq.class);
        ApiMsgNamesToTypes.put(s_GetNodeInfoRsp, GetNodeInfoRsp.class);
        ApiMsgNamesToTypes.put(s_GraphActionReq, GraphActionReq.class);
        ApiMsgNamesToTypes.put(s_GraphActionRsp, GraphActionRsp.class);
        ApiMsgNamesToTypes.put(s_AddFriendReq, AddFriendReq.class);
        ApiMsgNamesToTypes.put(s_LiveKeysAddMsg, LiveKeysAddMsg.class);
        ApiMsgNamesToTypes.put(s_LiveKeysDelMsg, LiveKeysDelMsg.class);
        ApiMsgNamesToTypes.put(s_LiveValuesMsg, LiveValuesMsg.class);

        ApiMsgNamesToTypes.put(s_GenericRsp, GenericRsp.class);

        ApiMsgNamesToTypes.put(s_MyHackingReq, MyHackingReq.class);
        ApiMsgNamesToTypes.put(s_MyHackingRsp, MyHackingRsp.class);

        s_UnknownRsp = "unknownrsp";
    }
}
