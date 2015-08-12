package com.yodiwo.plegma;

import java.util.ArrayList;

/**
 * Created by ApiGenerator Tool (Java) on 11/08/2015 18:56:34.
 */
    /** 
 * Port State Request. Will result in a response of type Yodiwo.API.Plegma.PortStateRsp
 *Direction: node->cloud
 *
 */
        public class PortStateReq extends ApiMsg
        {
            /** 
 * Type of operation requested
 */
            public ePortStateOperation Operation;
            /** 
 * List of PortKeys that the server should send an update for (in conjuction with Yodiwo.API.Plegma.ePortStateOperation.SpecificKeys).
 * If set to null or an empty array then the server will send an update for all relevant PortKeys
 */
            public String[] PortKeys;
            
            public PortStateReq()
            {
                this.Id = eApiType.PortStateReq;
            }
                
                public PortStateReq(int Version,int SeqNo,int ResponseToSeqNo,ePortStateOperation Operation,String[] PortKeys)
                {
                		this.Id = eApiType.PortStateReq;
		this.Version = Version;
		this.SeqNo = SeqNo;
		this.ResponseToSeqNo = ResponseToSeqNo;
		this.Operation = Operation;
		this.PortKeys = PortKeys;

                }
                
        }
