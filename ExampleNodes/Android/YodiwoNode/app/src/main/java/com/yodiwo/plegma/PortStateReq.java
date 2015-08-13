package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 3/8/2015 10:26:22 &#956;&#956;.
 */

/**
 * Port Update Request. Will result in a response of type PortEventBatchMsg
 * Direction: node->cloud
 */
public class PortStateReq extends ApiMsg {
    /**
     * Type of operation requested
     */
    public ePortStateOperation Operation;
    /**
     * List of PortKeys that the server should send an update for (in conjuction with Yodiwo.API.Plegma.ePortStateOperation.SpecificKeys).
     * If set to null or an empty array then the server will send an update for all relevant PortKeys
     */
    public PortKey[] Keys;

    public PortStateReq() {
        this.Id = eApiType.Invalid;
    }

    public PortStateReq(ePortStateOperation Operation, PortKey[] Keys, int Version, int SeqNo, int ResponseToSeqNo) {
        this.Operation = Operation;
        this.Keys = Keys;
        this.Id = eApiType.Invalid;
        this.Version = Version;
        this.SeqNo = SeqNo;
        this.ResponseToSeqNo = ResponseToSeqNo;

    }

}
