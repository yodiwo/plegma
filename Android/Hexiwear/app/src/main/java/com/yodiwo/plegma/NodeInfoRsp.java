package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:54.
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
    public int Type;
    /**
     * Capabilities of this node (Yodiwo.API.Plegma.eNodeCapa)
     */
    public int Capabilities;
    /**
     * List of Yodiwo.API.Plegma.NodeThingTypes that this Node presents and implements
     */
    public NodeThingType[] ThingTypes;

    public int ThingsRevNum;

    public String[] BlockLibraries;

    public NodeInfoRsp() {
    }

    public NodeInfoRsp(int SeqNo, String Name, int Type, int Capabilities, NodeThingType[] ThingTypes, int ThingsRevNum) {
        this.SeqNo = SeqNo;
        this.Name = Name;
        this.Type = Type;
        this.Capabilities = Capabilities;
        this.ThingTypes = ThingTypes;
        this.ThingsRevNum = ThingsRevNum;
    }

}
