package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 3/8/2015 10:26:20 &#956;&#956;.
 */

/**
 * Node Things Request Used to request a Yodiwo.API.Plegma.Things related operation from the other end
 * NOTE: This message cannot be used to send actual data to the endpoint, but to request an action to happen on the endpoint, resulting in a ThingsMsg response. If the desire is for the originator of the message to send data, then a direct Asynchronous ThingsMsg should be used instead
 * Direction: bidirectional (Node->Cloud and Cloud->Node)
 * Receiving side must reply with a Yodiwo.API.Plegma.ThingsMsg. Yodiwo.API.Plegma.ApiMsg.ResponseToSeqNo ThingReq's Yodiwo.API.Plegma.ApiMsg.SeqNo
 */
public class ThingsReq extends ApiMsg {
    /**
     * Yodiwo.API.Plegma.ThingsReq.ThingKey of the Yodiwo.API.Plegma.Thing that this request refers to. If left null (invalid ThingKey)
     * then this operation refers to all of the Node's Things
     */
    public String ThingKey;
    /**
     * Identifier of the operation requested; see Yodiwo.API.Plegma.eThingsOperation
     */
    public eThingsOperation Operation;
    /**
     * Optional: Object of class Yodiwo.API.Plegma.Thing that contains information related to the request's Operation
     */
    public Thing Data;

    public ThingsReq() {
        this.Id = eApiType.Invalid;
    }

    public ThingsReq(String ThingKey, eThingsOperation Operation, Thing Data, int Version, int SeqNo, int ResponseToSeqNo) {
        this.ThingKey = ThingKey;
        this.Operation = Operation;
        this.Data = Data;
        this.Id = eApiType.Invalid;
        this.Version = Version;
        this.SeqNo = SeqNo;
        this.ResponseToSeqNo = ResponseToSeqNo;

    }

}
