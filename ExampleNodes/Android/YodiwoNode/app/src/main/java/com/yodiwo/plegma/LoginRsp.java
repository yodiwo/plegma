package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 17/8/2015 3:43:43 &#956;&#956;.
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
        this.Id = eApiType.LoginRsp;
    }

    public LoginRsp(int Version, int SeqNo, int ResponseToSeqNo, String NodeKey, String SecretKey) {
        this.Id = eApiType.LoginRsp;
        this.Version = Version;
        this.SeqNo = SeqNo;
        this.ResponseToSeqNo = ResponseToSeqNo;
        this.NodeKey = NodeKey;
        this.SecretKey = SecretKey;

    }

}
