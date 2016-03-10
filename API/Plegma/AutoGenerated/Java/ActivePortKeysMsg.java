package com.yodiwo.plegma;

import java.util.ArrayList;

/**
 * Created by ApiGenerator Tool (Java) on 11/08/2015 18:56:36.
 */
    /** 
 * Active Port Keys Msg Informs Node of all currently active Ports (i.e. Ports that are connected and active in currently deployed graphs).  Should be used by Nodes to supress events from inactive ports, allowing more efficient use of medium
 *Direction: Cloud -> Node
 *
 */
        public class ActivePortKeysMsg extends ApiMsg
        {
            /** 
 * Array of portkeys of currently active Ports
 */
            public String[] ActivePortKeys;
            
            public ActivePortKeysMsg()
            {
                this.Id = eApiType.ActivePortKeysMsg;
            }
                
                public ActivePortKeysMsg(int Version,int SeqNo,int ResponseToSeqNo,String[] ActivePortKeys)
                {
                		this.Id = eApiType.ActivePortKeysMsg;
		this.Version = Version;
		this.SeqNo = SeqNo;
		this.ResponseToSeqNo = ResponseToSeqNo;
		this.ActivePortKeys = ActivePortKeys;

                }
                
        }