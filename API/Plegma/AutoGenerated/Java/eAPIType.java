package com.yodiwo.plegma;

import java.util.ArrayList;

/**
 * Created by ApiGenerator Tool (Java) on 11/08/2015 18:56:24.
 */
    /** 
 * ID of the API message
 */
        public enum eApiType
        {
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
