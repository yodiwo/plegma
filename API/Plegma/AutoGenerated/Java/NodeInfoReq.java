package com.yodiwo.plegma;

import java.util.ArrayList;

/**
 * Created by ApiGenerator Tool (Java) on 11/08/2015 18:56:30.
 */
    /** 
 * Node Info Request If sent by cloud to a node, it is to request capabilities and supported types from the node If sent by a node to the cloud, then Yodiwo.API.Plegma.NodeInfoReq.RequestedThingType must be set
 *and can be used to perform discovery with the user's connected nodes (currently unavailable)
 *Direction: bidirectional (Node->Cloud and Cloud->Node)
 *Receiving end must reply with a Yodiwo.API.Plegma.NodeInfoRsp
 *
 */
        public class NodeInfoReq extends ApiMsg
        {
            /** 
 * Reserved for future use; ignore
 */
            public NodeThingType RequestedThingType;
            
            public NodeInfoReq()
            {
                this.Id = eApiType.NodeInfoReq;
            }
                
                public NodeInfoReq(int Version,int SeqNo,int ResponseToSeqNo,NodeThingType RequestedThingType)
                {
                		this.Id = eApiType.NodeInfoReq;
		this.Version = Version;
		this.SeqNo = SeqNo;
		this.ResponseToSeqNo = ResponseToSeqNo;
		this.RequestedThingType = RequestedThingType;

                }
                
        }
