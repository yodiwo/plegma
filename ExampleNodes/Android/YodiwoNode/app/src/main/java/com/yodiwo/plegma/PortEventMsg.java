package com.yodiwo.plegma;

import java.util.ArrayList;
import java.util.HashMap;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:57.
 */
    /** 
 * asynchronous Port Event message The main API message to exchange events between Nodes and the Yodiwo Cloud Service
 *Direction: bidirectional (Node->Cloud and Cloud->Node)
 *
 */
        public class PortEventMsg extends ApiMsg
        {
            /** 
 * Array of Yodiwo.API.Plegma.PortEvent messages
 */
            public PortEvent[] PortEvents;
            
            public PortEventMsg()
            {
            }
                
                public PortEventMsg(int SeqNo,PortEvent[] PortEvents)
                {
                		this.SeqNo = SeqNo;
		this.PortEvents = PortEvents;

                }
                
        }
