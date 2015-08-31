package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 17/8/2015 3:43:48 &#956;&#956;.
 */

/**
 * Node Info Response Message that contains gneral information about a node including supported Node Types and Capabilities
 * Direction: bidirectional (Node->Cloud and Cloud->Node)
 * In response to a Yodiwo.API.Plegma.NodeInfoReq
 */
public class NodeInfoRsp extends ApiMsg {
    /**
     * Friendly name of responding Node
     */
    public String Name;
    /**
     * Type (Yodiwo.API.Plegma.eNodeType) of responding Node
     */
    public eNodeType Type;
    /**
     * Capabilities of this node
     */
    public eNodeCapa Capabilities;
    /**
     * List of Yodiwo.API.Plegma.NodeThingTypes that this Node presents and implements
     */
    public NodeThingType[] ThingTypes;

    public NodeInfoRsp() {
        this.Id = eApiType.NodeInfoRsp;
    }

    public NodeInfoRsp(int Version, int SeqNo, int ResponseToSeqNo, String Name, eNodeType Type, eNodeCapa Capabilities, NodeThingType[] ThingTypes) {
        this.Id = eApiType.NodeInfoRsp;
        this.Version = Version;
        this.SeqNo = SeqNo;
        this.ResponseToSeqNo = ResponseToSeqNo;
        this.Name = Name;
        this.Type = Type;
        this.Capabilities = Capabilities;
        this.ThingTypes = ThingTypes;

    }

}
