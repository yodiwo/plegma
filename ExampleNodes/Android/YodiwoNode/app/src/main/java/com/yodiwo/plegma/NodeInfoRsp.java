package com.yodiwo.plegma;

import java.util.ArrayList;
import java.util.HashMap;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:54.
 */
    /** 
 * Node Info Response Message that contains gneral information about a node including supported Node Types and Capabilities
 *Direction: bidirectional (Node->Cloud and Cloud->Node)
 *In response to a Yodiwo.API.Plegma.NodeInfoReq
 */
        public class NodeInfoRsp extends ApiMsg
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
            
            public NodeInfoRsp()
            {
            }
                
                public NodeInfoRsp(int SeqNo,String Name,eNodeType Type,eNodeCapa Capabilities,NodeThingType[] ThingTypes)
                {
                		this.SeqNo = SeqNo;
		this.Name = Name;
		this.Type = Type;
		this.Capabilities = Capabilities;
		this.ThingTypes = ThingTypes;

                }
                
        }
