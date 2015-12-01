package com.yodiwo.plegma;

/**
 * Created by Nikos on 28/11/2015.
 */

/**
 * Node Things Request Used to request a Yodiwo.API.Plegma.Things related operation from the other end.
 * Receiving side *must* reply with a Yodiwo.API.Plegma.ThingsRsp.
 * Its ApiMsg.SyncId field *must* be set to this message's Yodiwo.API.Plegma.ApiMsg.SeqNo
 * Direction: bidirectional (Node->Cloud and Cloud->Node)
 */
public class NodeUnpairedMsg extends ApiMsg {

    public static final Integer InvalidOperation = 1;
    public static final Integer UserRequested = 2;
    public static final Integer TooManyAuthFailures = 3;

    /**
     * Reason for Node's Unpairing
     */
    public int ReasonCode;
    /**
     * Yodiwo.API.Plegma.ThingsReq.ThingKey of the Yodiwo.API.Plegma.Thing that this request refers to. If left null (invalid ThingKey)
     * then this operation refers to all of the Node's Things
     */
    public String Message;

    public NodeUnpairedMsg() {
    }

    public NodeUnpairedMsg(int SeqNo, int reasonCode, String message) {
        this.SeqNo = SeqNo;
        this.ReasonCode = reasonCode;
        this.Message = message;
    }
}
