package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:55.
 */

/**
 * Node Things Request Used to request a Yodiwo.API.Plegma.Things related operation from the other end.
 * Receiving side *must* reply with a Yodiwo.API.Plegma.ThingsRsp.
 * Its ApiMsg.ResponseToSeqNo field *must* be set to this message's Yodiwo.API.Plegma.ApiMsg.SeqNo
 * Direction: bidirectional (Node->Cloud and Cloud->Node)
 */
public class ThingsReq extends ApiMsg {

    public static final Integer Update = 1;
    public static final Integer Overwrite = 2;
    public static final Integer Delete = 3;
    public static final Integer Get = 4;
    public static final Integer Scan = 5;

    /**
     * Identifier of the operation requested; see Yodiwo.API.Plegma.eThingsOperation
     */
    public int Operation;
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
    }

    public ThingsReq(int SeqNo, int Operation, String ThingKey, Thing[] Data) {
        this.SeqNo = SeqNo;
        this.Operation = Operation;
        this.ThingKey = ThingKey;
        this.Data = Data;
    }
}
