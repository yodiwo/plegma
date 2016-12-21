package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:49.
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
    }

    public LoginRsp(int SeqNo, String NodeKey, String SecretKey) {
        this.SeqNo = SeqNo;
        this.NodeKey = NodeKey;
        this.SecretKey = SecretKey;

    }

}
