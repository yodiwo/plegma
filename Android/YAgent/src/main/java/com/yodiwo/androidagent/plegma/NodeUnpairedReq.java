package com.yodiwo.androidagent.plegma;

/**
 * Created by Nikos on 28/11/2015.
 */

/**
 * Node Unpairing Request
 * Node *must* reply with Yodiwo.API.Plegma.GenericRsp.
 * Direction: Cloud->Node
 */
public class NodeUnpairedReq extends ApiMsg {

    public static final Integer InvalidOperation = 1;
    public static final Integer UserRequested = 2;
    public static final Integer TooManyAuthFailures = 3;

    /**
     * Reason for Node's Unpairing
     */
    public int ReasonCode;

    public String Message;

    public NodeUnpairedReq() {
    }

    public NodeUnpairedReq(int SeqNo, int reasonCode, String message) {
        this.SeqNo = SeqNo;
        this.ReasonCode = reasonCode;
        this.Message = message;
    }
}
