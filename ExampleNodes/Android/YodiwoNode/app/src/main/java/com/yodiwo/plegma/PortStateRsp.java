package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 3/8/2015 10:26:23 &#956;&#956;.
 */

/**
 * Active Port Keys Msg Informs Node of all currently active Ports (i.e. Ports that are connected and active in currently deployed graphs).  Should be used to 1. supress events from inactive ports, allowing more efficient use of medium, 2. sync Port states with the server
 * Can be either asynchronous (e.g. at Node connection) or as a response to a PortUpdateReq
 * Direction: Cloud -> Node
 * See also Yodiwo.API.Plegma.Port.IsInGraphs
 */
public class PortStateRsp extends ApiMsg {
    /**
     * Type of operation responding to
     */
    public ePortStateOperation Operation;
    /**
     * Array of keys of currently active Ports To be used in conjuction with Yodiwo.API.Plegma.ePortStateOperation.ActivePortKeys, will be set to null otherwise
     */
    public PortKey[] ActivePortKeys;
    /**
     * Array of requested Port states when Yodiwo.API.Plegma.ePortStateOperation is set to Yodiwo.API.Plegma.ePortStateOperation.ActivePortKeys, this field is null
     */
    public PortState[] PortStates;

    public PortStateRsp() {
        this.Id = eApiType.Invalid;
    }

    public PortStateRsp(ePortStateOperation Operation, PortKey[] ActivePortKeys, PortState[] PortStates, int Version, int SeqNo, int ResponseToSeqNo) {
        this.Operation = Operation;
        this.ActivePortKeys = ActivePortKeys;
        this.PortStates = PortStates;
        this.Id = eApiType.Invalid;
        this.Version = Version;
        this.SeqNo = SeqNo;
        this.ResponseToSeqNo = ResponseToSeqNo;

    }

}
