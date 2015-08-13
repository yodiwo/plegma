package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 3/8/2015 10:26:19 &#956;&#956;.
 */

/**
 * Node Info Request If sent by cloud to a node, it is to request capabilities and supported types from the node If sent by a node to the cloud, then Yodiwo.API.Plegma.NodeInfoReq.RequestedThingType must be set
 * and can be used to perform discovery with the user's connected nodes (currently unavailable)
 * Direction: bidirectional (Node->Cloud and Cloud->Node)
 * Receiving end must reply with a Yodiwo.API.Plegma.NodeInfoMsg
 */
public class NodeInfoReq extends ApiMsg {
    /**
     * Reserved for future use; ignore
     */
    public NodeThingType RequestedThingType;

    public NodeInfoReq() {
        this.Id = eApiType.Invalid;
    }

    public NodeInfoReq(NodeThingType RequestedThingType, int Version, int SeqNo, int ResponseToSeqNo) {
        this.RequestedThingType = RequestedThingType;
        this.Id = eApiType.Invalid;
        this.Version = Version;
        this.SeqNo = SeqNo;
        this.ResponseToSeqNo = ResponseToSeqNo;

    }

}
