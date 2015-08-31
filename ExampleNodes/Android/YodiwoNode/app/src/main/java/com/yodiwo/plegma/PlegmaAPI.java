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
        ApiMsgNames = new HashMap<Class<?>, String>();
        ApiMsgNames.put(LoginReq.class, "loginreq");
        ApiMsgNames.put(LoginRsp.class, "loginrsp");
        ApiMsgNames.put(NodeInfoReq.class, "nodeinforeq");
        ApiMsgNames.put(NodeInfoRsp.class, "nodeinforsp");
        ApiMsgNames.put(ThingsReq.class, "thingsreq");
        ApiMsgNames.put(ThingsRsp.class, "thingsrsp");
        ApiMsgNames.put(PortEventMsg.class, "porteventmsg");
        ApiMsgNames.put(PortStateReq.class, "portstatereq");
        ApiMsgNames.put(PortStateRsp.class, "portstatersp");
        ApiMsgNames.put(ActivePortKeysMsg.class, "activeportkeysmsg");
        ApiMsgNamesToTypes = new HashMap<String, Class<?>>();
        ApiMsgNamesToTypes.put("loginreq", LoginReq.class);
        ApiMsgNamesToTypes.put("loginrsp", LoginRsp.class);
        ApiMsgNamesToTypes.put("nodeinforeq", NodeInfoReq.class);
        ApiMsgNamesToTypes.put("nodeinforsp", NodeInfoRsp.class);
        ApiMsgNamesToTypes.put("thingsreq", ThingsReq.class);
        ApiMsgNamesToTypes.put("thingsrsp", ThingsRsp.class);
        ApiMsgNamesToTypes.put("porteventmsg", PortEventMsg.class);
        ApiMsgNamesToTypes.put("portstatereq", PortStateReq.class);
        ApiMsgNamesToTypes.put("portstatersp", PortStateRsp.class);
        ApiMsgNamesToTypes.put("activeportkeysmsg", ActivePortKeysMsg.class);
    }
}
