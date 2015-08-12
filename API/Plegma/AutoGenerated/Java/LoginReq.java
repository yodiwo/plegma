package com.yodiwo.plegma;

import java.util.ArrayList;

/**
 * Created by ApiGenerator Tool (Java) on 11/08/2015 18:56:25.
 */
    /** 
 * Login Request to be used only for transports that require explicit authentication via the API itself
 */
        public class LoginReq extends ApiMsg
        {
            
            public LoginReq()
            {
                this.Id = eApiType.LoginReq;
            }
                
                public LoginReq(int Version,int SeqNo,int ResponseToSeqNo)
                {
                		this.Id = eApiType.LoginReq;
		this.Version = Version;
		this.SeqNo = SeqNo;
		this.ResponseToSeqNo = ResponseToSeqNo;

                }
                
        }
