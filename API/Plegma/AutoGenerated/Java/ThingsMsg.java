package com.yodiwo.plegma;

import java.util.ArrayList;

/**
 * Created by ApiGenerator Tool (Java) on 05/08/2015 12:55:19.
 */
    /** 
 * Node Things Message Can be asynchronous (self-triggered) or in response to a Yodiwo.API.Plegma.ThingsReq request
 *If responding to a ThingReq, a ThingsMsg message should have:  - Yodiwo.API.Plegma.ThingsMsg.Operation set to ThingReq's operation
 * - Yodiwo.API.Plegma.ApiMsg.ResponseToSeqNo set to ThingReq's Yodiwo.API.Plegma.ApiMsg.SeqNo- Yodiwo.API.Plegma.ThingsMsg.Status set to True if ThingsReq was successfully handled and this Msg has valid data, False otherwise
 * - if Yodiwo.API.Plegma.ThingsMsg.Status is True, Yodiwo.API.Plegma.ThingsMsg.Data set to correspond to requested Req's operation, set to Null otherwise
 *If an asynchronous message, then:  - Yodiwo.API.Plegma.ThingsMsg.Operation is set to indicate what the receiving endpoint should do
 * - Yodiwo.API.Plegma.ApiMsg.ResponseToSeqNo is set to 0
 * - Yodiwo.API.Plegma.ThingsMsg.Status set to True
 * - Yodiwo.API.Plegma.ThingsMsg.Data set to correspond to requested Req's operation
 *Direction: bidirectional (Node->Cloud and Cloud->Node).
 * Nodes can asynchronously trigger updates towards the Cloud with this message
 *
 */
        public class ThingsMsg extends ApiMsg
        {
            /** 
 * Identifier of this message's operation of type Yodiwo.API.Plegma.eThingsOperationIf responding to a Yodiwo.API.Plegma.ThingsReq request, Operation fields must much between Req and Rsp.
 *If an asynchronous message (via a Yodiwo.API.Plegma.ThingsMsg message) this indicates the message's contents
 */
            public eThingsOperation Operation;
            /** 
 * If responding to a Yodiwo.API.Plegma.ThingsReq request, Status indicates if the request was successful and this response contains actual data
 *If an asynchronous message (via a Yodiwo.API.Plegma.ThingsMsg message) this is to be ignored
 */
            public Boolean Status;
            /** 
 * Array of Yodiwo.API.Plegma.Things that contain data related to the selected Operation
 */
            public Thing[] Data;
            
            public ThingsMsg()
            {
                this.Id = eApiType.ThingsMsg;
            }
                
                public ThingsMsg(int Version,int SeqNo,int ResponseToSeqNo,eThingsOperation Operation,Boolean Status,Thing[] Data)
                {
                		this.Id = eApiType.ThingsMsg;
		this.Version = Version;
		this.SeqNo = SeqNo;
		this.ResponseToSeqNo = ResponseToSeqNo;
		this.Operation = Operation;
		this.Status = Status;
		this.Data = Data;

                }
                
        }
