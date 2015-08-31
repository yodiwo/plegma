package com.yodiwo.plegma;

import java.util.ArrayList;
import java.util.HashMap;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:35:00.
 */
    /** 
 * Active Port Keys Msg Informs Node of all currently active Ports (i.e. Ports that are connected and active in currently deployed graphs).  Should be used to 1. supress events from inactive ports, allowing more efficient use of medium, 2. sync Port states with the server
 *Can be either asynchronous (e.g. at Node connection) or as a response to a PortUpdateReq
 *Direction: Cloud -> Node
 *
 */
        public class PortStateRsp extends ApiMsg
        {
            /** 
 * Type of operation responding to
 */
            public ePortStateOperation Operation;
            /** 
 * Array of requested Port states.
 */
            public PortState[] PortStates;
            
            public PortStateRsp()
            {
            }
                
                public PortStateRsp(int SeqNo,ePortStateOperation Operation,PortState[] PortStates)
                {
                		this.SeqNo = SeqNo;
		this.Operation = Operation;
		this.PortStates = PortStates;

                }
                
        }
