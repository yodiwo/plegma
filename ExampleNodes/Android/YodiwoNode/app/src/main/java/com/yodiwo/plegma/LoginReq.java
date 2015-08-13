package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 3/8/2015 10:26:15 &#956;&#956;.
 */

/**
 * Login Request to be used only for transports that require explicit authentication via the API itself
 */
public class LoginReq extends ApiMsg {

    public LoginReq() {
        this.Id = eApiType.Invalid;
    }

    public LoginReq(int Version, int SeqNo, int ResponseToSeqNo) {
        this.Id = eApiType.Invalid;
        this.Version = Version;
        this.SeqNo = SeqNo;
        this.ResponseToSeqNo = ResponseToSeqNo;

    }

}
