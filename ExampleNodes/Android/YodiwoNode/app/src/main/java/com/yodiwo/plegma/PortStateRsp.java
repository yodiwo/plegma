package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 17/8/2015 3:43:53 &#956;&#956;.
 */

/**
 * Active Port Keys Msg Informs Node of all currently active Ports (i.e. Ports that are connected and active in currently deployed graphs).  Should be used to 1. supress events from inactive ports, allowing more efficient use of medium, 2. sync Port states with the server
 * Can be either asynchronous (e.g. at Node connection) or as a response to a PortUpdateReq
 * Direction: Cloud -> Node
 */
public class PortStateRsp extends ApiMsg {
    /**
     * Type of operation responding to
     */
    public ePortStateOperation Operation;
    /**
     * Array of requested Port states.
     */
    public PortState[] PortStates;

    public PortStateRsp() {
        this.Id = eApiType.PortStateRsp;
    }

    public PortStateRsp(int Version, int SeqNo, int ResponseToSeqNo, ePortStateOperation Operation, PortState[] PortStates) {
        this.Id = eApiType.PortStateRsp;
        this.Version = Version;
        this.SeqNo = SeqNo;
        this.ResponseToSeqNo = ResponseToSeqNo;
        this.Operation = Operation;
        this.PortStates = PortStates;

    }

}
