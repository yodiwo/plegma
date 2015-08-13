package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 3/8/2015 10:26:16 &#956;&#956;.
 */

/**
 * Login Response
 * sends node and secret keys
 * to be used only for transports that require explicit authentication via the API itself
 */
public class LoginRsp extends ApiMsg {
    /**
     * NodeKey of Node
     */
    public String NodeKey;
    /**
     * Secret key of Node
     */
    public String SecretKey;

    public LoginRsp() {
        this.Id = eApiType.Invalid;
    }

    public LoginRsp(String NodeKey, String SecretKey, int Version, int SeqNo, int ResponseToSeqNo) {
        this.NodeKey = NodeKey;
        this.SecretKey = SecretKey;
        this.Id = eApiType.Invalid;
        this.Version = Version;
        this.SeqNo = SeqNo;
        this.ResponseToSeqNo = ResponseToSeqNo;

    }

}
