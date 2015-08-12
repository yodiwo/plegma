package com.yodiwo.plegma;

import java.util.ArrayList;

/**
 * Created by ApiGenerator Tool (Java) on 05/08/2015 12:55:18.
 */
    /** 
 * Node Info Response Message that contains gneral information about a node including supported Node Types and Capabilities
 *Direction: bidirectional (Node->Cloud and Cloud->Node)
 *In response to an Yodiwo.API.Plegma.NodeInfoReq
 */
        public class NodeInfoMsg extends ApiMsg
        {
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
            
            public NodeInfoMsg()
            {
                this.Id = eApiType.NodeInfoMsg;
            }
                
                public NodeInfoMsg(int Version,int SeqNo,int ResponseToSeqNo,String Name,eNodeType Type,eNodeCapa Capabilities,NodeThingType[] ThingTypes)
                {
                		this.Id = eApiType.NodeInfoMsg;
		this.Version = Version;
		this.SeqNo = SeqNo;
		this.ResponseToSeqNo = ResponseToSeqNo;
		this.Name = Name;
		this.Type = Type;
		this.Capabilities = Capabilities;
		this.ThingTypes = ThingTypes;

                }
                
        }
