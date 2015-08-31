package com.yodiwo.plegma;

import java.util.HashMap;

/**
 * Created by ApiGenerator Tool (Java) on 17/8/2015 3:43:54 &#956;&#956;.
 */

public class PlegmaAPI {

    public static final int APIVersion;

    public static final char KeySeparator;

    public static final Class<?>[] ApiMessages;

    public static final HashMap<Class<?>, String> ApiMsgNames;


    static {
        APIVersion = 1;
        KeySeparator = '-';
        ApiMessages = new Class<?>[]{LoginReq.class, LoginRsp.class, NodeInfoReq.class, NodeInfoRsp.class, ThingsReq.class, ThingsRsp.class, PortEventMsg.class, PortStateReq.class, PortStateRsp.class, ActivePortKeysMsg.class,};
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
    }
}
