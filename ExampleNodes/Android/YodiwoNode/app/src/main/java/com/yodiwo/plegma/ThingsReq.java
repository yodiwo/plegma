package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 17/8/2015 3:43:49 &#956;&#956;.
 */

/**
 * Node Things Request Used to request a Yodiwo.API.Plegma.Things related operation from the other end.
 * Receiving side *must* reply with a Yodiwo.API.Plegma.ThingsRsp.
 * Its Yodiwo.API.Plegma.ApiMsg.ResponseToSeqNo field *must* be set to this message's Yodiwo.API.Plegma.ApiMsg.SeqNo
 * Direction: bidirectional (Node->Cloud and Cloud->Node)
 */
public class ThingsReq extends ApiMsg {
    /**
     * Identifier of the operation requested; see Yodiwo.API.Plegma.eThingsOperation
     */
    public eThingsOperation Operation;
    /**
     * Yodiwo.API.Plegma.ThingsReq.ThingKey of the Yodiwo.API.Plegma.Thing that this request refers to. If left null (invalid ThingKey)
     * then this operation refers to all of the Node's Things
     */
    public String ThingKey;
    /**
     * Optional: Array of Yodiwo.API.Plegma.Thing that contain information related to the request's Operation
     */
    public Thing[] Data;

    public ThingsReq() {
        this.Id = eApiType.ThingsReq;
    }

    public ThingsReq(int Version, int SeqNo, int ResponseToSeqNo, eThingsOperation Operation, String ThingKey, Thing[] Data) {
        this.Id = eApiType.ThingsReq;
        this.Version = Version;
        this.SeqNo = SeqNo;
        this.ResponseToSeqNo = ResponseToSeqNo;
        this.Operation = Operation;
        this.ThingKey = ThingKey;
        this.Data = Data;

    }

}
