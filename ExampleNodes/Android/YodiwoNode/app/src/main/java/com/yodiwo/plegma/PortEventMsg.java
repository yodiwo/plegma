package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 17/8/2015 3:43:51 &#956;&#956;.
 */

/**
 * asynchronous Port Event message The main API message to exchange events between Nodes and the Yodiwo Cloud Service
 * Direction: bidirectional (Node->Cloud and Cloud->Node)
 */
public class PortEventMsg extends ApiMsg {
    /**
     * Array of Yodiwo.API.Plegma.PortEvent messages
     */
    public PortEvent[] PortEvents;

    public PortEventMsg() {
        this.Id = eApiType.PortEventMsg;
    }

    public PortEventMsg(int Version, int SeqNo, int ResponseToSeqNo, PortEvent[] PortEvents) {
        this.Id = eApiType.PortEventMsg;
        this.Version = Version;
        this.SeqNo = SeqNo;
        this.ResponseToSeqNo = ResponseToSeqNo;
        this.PortEvents = PortEvents;

    }

}
