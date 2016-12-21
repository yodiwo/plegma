package com.yodiwo.plegma;

import java.util.HashMap;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:35:02.
 */

public class PlegmaAPI {

    public static final int APIVersion;

    public static final char KeySeparator;

    public static final Class<?>[] ApiMessages;

    public static final String s_LoginReq;
    public static final String s_LoginRsp;

    public static final String s_NodeInfoReq;
    public static final String s_NodeInfoRsp;
    public static final String s_NodeUnpairedReq;
    public static final String s_NodeUnpairedRsp;

    public static final String s_ThingsGet;
    public static final String s_ThingsSet;

    public static final String s_PortEventMsg;
    public static final String s_PortStateGet;
    public static final String s_PortStateSet;
    public static final String s_ActivePortKeysMsg;

    public static final String s_PingReq;
    public static final String s_PingRsp;

    public static final String s_GenericRsp;

    public static final String s_UnknownRsp;

    public static final HashMap<Class<?>, String> ApiMsgNames;

    public static final HashMap<String, Class<?>> ApiMsgNamesToTypes;


    static {
        APIVersion = 1;
        KeySeparator = '-';
        ApiMessages = new Class<?>[]{LoginReq.class, LoginRsp.class, NodeInfoReq.class, NodeInfoRsp.class, ThingsGet.class, ThingsSet.class, PortEventMsg.class, PortStateGet.class, PortStateSet.class, ActivePortKeysMsg.class,};
        s_LoginReq = "loginreq";
        s_LoginRsp = "loginrsp";
        s_NodeInfoReq = "nodeinforeq";
        s_NodeInfoRsp = "nodeinforsp";
        s_NodeUnpairedReq = "nodeunpairedreq";
        s_NodeUnpairedRsp = "nodeunpairedrsp";
        s_ThingsGet = "thingsget";
        s_ThingsSet = "thingsset";
        s_PortEventMsg = "porteventmsg";
        s_PortStateGet = "portstateget";
        s_PortStateSet = "portstateset";
        s_ActivePortKeysMsg = "activeportkeysmsg";
        s_PingReq = "pingreq";
        s_PingRsp = "pingrsp";
        s_GenericRsp = "genericrsp";
        ApiMsgNames = new HashMap<Class<?>, String>();
        ApiMsgNames.put(LoginReq.class, s_LoginReq);
        ApiMsgNames.put(LoginRsp.class, s_LoginRsp);
        ApiMsgNames.put(NodeInfoReq.class, s_NodeInfoReq);
        ApiMsgNames.put(NodeInfoRsp.class, s_NodeInfoRsp);
        ApiMsgNames.put(NodeUnpairedReq.class, s_NodeUnpairedReq);
        ApiMsgNames.put(NodeUnpairedRsp.class, s_NodeUnpairedRsp);
        ApiMsgNames.put(ThingsGet.class, s_ThingsGet);
        ApiMsgNames.put(ThingsSet.class, s_ThingsSet);
        ApiMsgNames.put(PortEventMsg.class, s_PortEventMsg);
        ApiMsgNames.put(PortStateGet.class, s_PortStateGet);
        ApiMsgNames.put(PortStateSet.class, s_PortStateSet);
        ApiMsgNames.put(ActivePortKeysMsg.class, s_ActivePortKeysMsg);
        ApiMsgNames.put(PingReq.class, s_PingReq);
        ApiMsgNames.put(PingRsp.class, s_PingRsp);
        ApiMsgNamesToTypes = new HashMap<String, Class<?>>();
        ApiMsgNamesToTypes.put(s_LoginReq, LoginReq.class);
        ApiMsgNamesToTypes.put(s_LoginRsp, LoginRsp.class);
        ApiMsgNamesToTypes.put(s_NodeInfoReq, NodeInfoReq.class);
        ApiMsgNamesToTypes.put(s_NodeInfoRsp, NodeInfoRsp.class);
        ApiMsgNamesToTypes.put(s_NodeUnpairedReq, NodeUnpairedReq.class);
        ApiMsgNamesToTypes.put(s_NodeUnpairedRsp, NodeUnpairedRsp.class);
        ApiMsgNamesToTypes.put(s_ThingsGet, ThingsGet.class);
        ApiMsgNamesToTypes.put(s_ThingsSet, ThingsSet.class);
        ApiMsgNamesToTypes.put(s_PortEventMsg, PortEventMsg.class);
        ApiMsgNamesToTypes.put(s_PortStateGet, PortStateGet.class);
        ApiMsgNamesToTypes.put(s_PortStateSet, PortStateSet.class);
        ApiMsgNamesToTypes.put(s_ActivePortKeysMsg, ActivePortKeysMsg.class);
        ApiMsgNamesToTypes.put(s_PingReq, PingReq.class);
        ApiMsgNamesToTypes.put(s_PingRsp, PingRsp.class);
        ApiMsgNamesToTypes.put(s_GenericRsp, GenericRsp.class);

        s_UnknownRsp = "unknownrsp";
    }
}
