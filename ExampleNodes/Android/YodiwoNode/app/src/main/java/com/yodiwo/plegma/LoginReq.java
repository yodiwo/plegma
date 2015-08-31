package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 17/8/2015 3:43:43 &#956;&#956;.
 */

/**
 * Login Request to be used only for transports that require explicit authentication via the API itself
 */
public class LoginReq extends ApiMsg {

    public LoginReq() {
        this.Id = eApiType.LoginReq;
    }

    public LoginReq(int Version, int SeqNo, int ResponseToSeqNo) {
        this.Id = eApiType.LoginReq;
        this.Version = Version;
        this.SeqNo = SeqNo;
        this.ResponseToSeqNo = ResponseToSeqNo;

    }

}
