package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:53.
 */

/**
 * Node Info Request If sent by cloud to a node, it is to request capabilities and supported types from the node.
 * If sent by a node to the cloud, then Yodiwo.API.Plegma.NodeInfoReq.RequestedThingType must be set
 * and can be used to perform discovery with the user's connected nodes (currently unavailable)
 * Direction: bidirectional (Node->Cloud and Cloud->Node)
 * Receiving end must reply with a Yodiwo.API.Plegma.NodeInfoRsp
 */
public class NodeInfoReq extends ApiMsg {

    public int LatestApiRev;

    public String AssignedEndpoint;

    public int ThingsRevNum;

    public NodeInfoReq() {}

    public NodeInfoReq(int SeqNo) {
        this.SeqNo = SeqNo;
    }

}
