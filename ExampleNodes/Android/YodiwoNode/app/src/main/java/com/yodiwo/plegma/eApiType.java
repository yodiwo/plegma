package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 17/8/2015 3:43:42 &#956;&#956;.
 */

/**
 * ID of the API message
 */
public enum eApiType {
    /**
     * reserved value; do not use
     */
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
    NodeInfoRsp,
    /**
     * Things Request (bidirectional)
     */
    ThingsReq,
    /**
     * Things Response or Asynchronous message (bidirectional)
     */
    ThingsRsp,
    /**
     * Asynchronous Port Event message (bidirectional)
     */
    PortEventMsg,
    /**
     * Port State Request (bidirectional)
     */
    PortStateReq,
    /**
     * Port States Response (cloud->node)
     */
    PortStateRsp,
    /**
     * Active Port Keys Message (cloud->node)
     */
    ActivePortKeysMsg,
    /**
     * Stream Open Request
     */
    StreamOpenReq,
    /**
     * Stream Open Response
     */
    StreanOpenRsp,
    /**
     * Stream Close Request
     */
    StreamCloseReq,
    /**
     * Stream Close Response
     */
    StreanCloseRsp,
    /**
     * Mjpeg Server Start Request
     */
    MjpegServerStartReq,
    /**
     * Mjpeg Server Start Response
     */
    MjpegServerStartRsp,
    /**
     * Mjpeg Server Stop Request
     */
    MjpegServerStopReq,
    /**
     * Mjpeg Server Stop Response
     */
    MjpegServerStopRsp,
}
