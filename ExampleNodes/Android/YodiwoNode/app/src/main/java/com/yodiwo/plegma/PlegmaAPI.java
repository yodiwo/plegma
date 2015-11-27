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

    public static final String s_ThingsReq;

    public static final String s_ThingsRsp;

    public static final String s_PortEventMsg;

    public static final String s_PortStateReq;

    public static final String s_PortStateRsp;

    public static final String s_ActivePortKeysMsg;

    public static final String s_PingReq;

    public static final String s_PingRsp;

    public static final HashMap<Class<?>, String> ApiMsgNames;

    public static final HashMap<String, Class<?>> ApiMsgNamesToTypes;


    static {
        APIVersion = 1;
        KeySeparator = '-';
        ApiMessages = new Class<?>[]{LoginReq.class, LoginRsp.class, NodeInfoReq.class, NodeInfoRsp.class, ThingsReq.class, ThingsRsp.class, PortEventMsg.class, PortStateReq.class, PortStateRsp.class, ActivePortKeysMsg.class,};
        s_LoginReq = "loginreq";
        s_LoginRsp = "loginrsp";
        s_NodeInfoReq = "nodeinforeq";
        s_NodeInfoRsp = "nodeinforsp";
        s_ThingsReq = "thingsreq";
        s_ThingsRsp = "thingsrsp";
        s_PortEventMsg = "porteventmsg";
        s_PortStateReq = "portstatereq";
        s_PortStateRsp = "portstatersp";
        s_ActivePortKeysMsg = "activeportkeysmsg";
        s_PingReq = "pingreq";
        s_PingRsp = "pingrsp";
        ApiMsgNames = new HashMap<Class<?>, String>();
        ApiMsgNames.put(LoginReq.class, s_LoginReq);
        ApiMsgNames.put(LoginRsp.class, s_LoginRsp);
        ApiMsgNames.put(NodeInfoReq.class, s_NodeInfoReq);
        ApiMsgNames.put(NodeInfoRsp.class, s_NodeInfoRsp);
        ApiMsgNames.put(ThingsReq.class, s_ThingsReq);
        ApiMsgNames.put(ThingsRsp.class, s_ThingsRsp);
        ApiMsgNames.put(PortEventMsg.class, s_PortEventMsg);
        ApiMsgNames.put(PortStateReq.class, s_PortStateReq);
        ApiMsgNames.put(PortStateRsp.class, s_PortStateRsp);
        ApiMsgNames.put(ActivePortKeysMsg.class, s_ActivePortKeysMsg);
        ApiMsgNames.put(PingReq.class, s_PingReq);
        ApiMsgNames.put(PingRsp.class, s_PingRsp);
        ApiMsgNamesToTypes = new HashMap<String, Class<?>>();
        ApiMsgNamesToTypes.put(s_LoginReq, LoginReq.class);
        ApiMsgNamesToTypes.put(s_LoginRsp, LoginRsp.class);
        ApiMsgNamesToTypes.put(s_NodeInfoReq, NodeInfoReq.class);
        ApiMsgNamesToTypes.put(s_NodeInfoRsp, NodeInfoRsp.class);
        ApiMsgNamesToTypes.put(s_ThingsReq, ThingsReq.class);
        ApiMsgNamesToTypes.put(s_ThingsRsp, ThingsRsp.class);
        ApiMsgNamesToTypes.put(s_PortEventMsg, PortEventMsg.class);
        ApiMsgNamesToTypes.put(s_PortStateReq, PortStateReq.class);
        ApiMsgNamesToTypes.put(s_PortStateRsp, PortStateRsp.class);
        ApiMsgNamesToTypes.put(s_ActivePortKeysMsg, ActivePortKeysMsg.class);
        ApiMsgNamesToTypes.put(s_PingReq, PingReq.class);
        ApiMsgNamesToTypes.put(s_PingRsp, PingRsp.class);
    }
}
