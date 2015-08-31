package com.yodiwo.plegma;

import java.util.ArrayList;
import java.util.HashMap;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:35:07.
 */
    /** 
 * Mqtt message encapsulation class.
 */
        public class MqttAPIMessage 
        {
            /** 
 * for RPC responses only: sequence number of previous message that this message is responding to
 */
            public int ResponseToSeqNo;
            /** 
 * The API message (payload)
 */
            public String Msg;
            
            public MqttAPIMessage()
            {
            }
                
                public MqttAPIMessage(int ResponseToSeqNo,String Msg)
                {
                		this.ResponseToSeqNo = ResponseToSeqNo;
		this.Msg = Msg;

                }
                
        }
