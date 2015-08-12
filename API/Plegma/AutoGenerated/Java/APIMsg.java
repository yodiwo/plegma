package com.yodiwo.plegma;

import java.util.ArrayList;

/**
 * Created by ApiGenerator Tool (Java) on 11/08/2015 18:56:25.
 */
    /** 
 * Base class of an API message, from which all message classes inherit, holding info as the message's ID, the Node's Key, Version, Sequence number
 */
        public class ApiMsg 
        {
            /** 
 * Id of message, of type Yodiwo.API.Plegma.eApiType
 */
            public eApiType Id;
            /** 
 * Version of API, currently set to 0
 */
            public int Version;
            /** 
 * Sequence number of this message
 */
            public int SeqNo;
            /** 
 * for RPC responses only: sequence number of previous message that this message is responding to
 */
            public int ResponseToSeqNo;
        }
