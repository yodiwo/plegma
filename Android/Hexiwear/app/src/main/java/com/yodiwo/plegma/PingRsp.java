package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:58.
 */

/**
 * Port State Request. Will result in a response of type Yodiwo.API.Plegma.PortStateRsp
 * Direction: node->cloud
 */
public class PingRsp extends ApiMsg {
    /**
     * id being received
     */
    public int Data;

    public PingRsp() {
    }

    public PingRsp(int Data) {
        this.Data = Data;
    }
}
