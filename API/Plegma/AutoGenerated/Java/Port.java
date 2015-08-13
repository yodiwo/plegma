package com.yodiwo.plegma;

import java.util.ArrayList;

/**
 * Created by ApiGenerator Tool (Java) on 11/08/2015 18:56:21.
 */
    /** 
 * Basic Input/Output entity of a Thing Creates and sends messages towards the Yodiwo cloud service,  or receives and handles messages from the cloud. Both events occur via the Yodiwo.API.Plegma.PortEventMsgand PortEventBatchMsg messages
 */
        public class Port 
        {
            /** 
 * Globally unique string identifying this port; Construct it using the Yodiwo.API.Plegma.Port.PortKey constructor
 */
            public String PortKey;
            /** 
 * Friendly name of this Port (as it will appear in the Cyan UI and graphs)
 */
            public String Name;
            /** 
 * Direction (Yodiwo.API.Plegma.ioPortDirection) of Port
 */
            public ioPortDirection ioDirection;
            /** 
 * type (Yodiwo.API.Plegma.ePortType) of values that each Port sends / receives
 */
            public ePortType Type;
            /** 
 * Indicates the number of graphs this port is currently active in (i.e. number of deployed graphs that use this Port).
 *On the Thing (Device) side it is meant to be read only. If 0, it means that the user has not created any stories which depend on
 * and/or trigger this Port. As such the device should not send any Yodiwo.API.Plegma.PortEventMsg messages about it
 */
            public int NumOfActiveGraphs;
            /** 
 * Current (at latest update/sampling/trigger/etc) value of Port as String.
 *Contains a string representation of the port's state, encoded according to the port's Yodiwo.API.Plegma.ePortTypeOn receiving events the Cloud Server will attempt to parse the State based on its Yodiwo.API.Plegma.ePortTypeWhen sending events the Cloud Server will encode the new state into a string, again according to the Port's Yodiwo.API.Plegma.ePortType
 */
            public String State;
            /** 
 * Port state sequence number: incremented by the Cloud server at every state update, so that Node and servers stay in sync
 */
            public int RevNum;
            
            public Port()
            {
            }
                
                public Port(String PortKey,String Name,ioPortDirection ioDirection,ePortType Type,int NumOfActiveGraphs,String State,int RevNum)
                {
                		this.PortKey = PortKey;
		this.Name = Name;
		this.ioDirection = ioDirection;
		this.Type = Type;
		this.NumOfActiveGraphs = NumOfActiveGraphs;
		this.State = State;
		this.RevNum = RevNum;

                }
                
        }
