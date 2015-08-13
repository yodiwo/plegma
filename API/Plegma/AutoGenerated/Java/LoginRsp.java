package com.yodiwo.plegma;

import java.util.ArrayList;

/**
 * Created by ApiGenerator Tool (Java) on 11/08/2015 18:56:26.
 */
    /** 
 * Login Response
 *sends node and secret keys
 *to be used only for transports that require explicit authentication via the API itself
 */
        public class LoginRsp extends ApiMsg
        {
            /** 
 * NodeKey of Node
 */
            public String NodeKey;
            /** 
 * Secret key of Node
 */
            public String SecretKey;
            
            public LoginRsp()
            {
                this.Id = eApiType.LoginRsp;
            }
                
                public LoginRsp(int Version,int SeqNo,int ResponseToSeqNo,String NodeKey,String SecretKey)
                {
                		this.Id = eApiType.LoginRsp;
		this.Version = Version;
		this.SeqNo = SeqNo;
		this.ResponseToSeqNo = ResponseToSeqNo;
		this.NodeKey = NodeKey;
		this.SecretKey = SecretKey;

                }
                
        }
