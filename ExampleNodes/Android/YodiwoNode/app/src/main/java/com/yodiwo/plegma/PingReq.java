package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:58.
 */

/**
 * Port State Request. Will result in a response of type Yodiwo.API.Plegma.PortStateRsp
 * Direction: node->cloud
 */
public class PingReq extends ApiMsg {
    /**
     * id being sent
     */
    public int Data;

    public PingReq() {
    }

    public PingReq(int Data) { this.Data = Data; }
}
