package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 3/8/2015 10:26:15 &#956;&#956;.
 */

/**
 * ID of the API message
 */
public enum eApiType {

    Invalid,
    /**
     * Login Request
     */
    LoginReq,
    /**
     * Login Response
     */
    LoginRsp,
    /**
     * Node Info Request (bidirectional)
     */
    NodeInfoReq,
    /**
     * Node Info Response or Asynchronous message (bidirectional)
     */
    NodeInfoMsg,

    ThingsReq,

    ThingsMsg,

    PortEventMsg,

    PortStateReq,

    PortStateRsp,

    StreamOpenReq,
}
