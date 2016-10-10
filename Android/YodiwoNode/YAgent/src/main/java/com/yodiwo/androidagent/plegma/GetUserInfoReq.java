package com.yodiwo.androidagent.plegma;

/**
 * Created by vaskanas on 26-May-16.
 */
public class GetUserInfoReq extends ApiMsg {
    /**
     * Array of tokens for users that Req tries to get info for
     */
    public String[] Tokens;

    public GetUserInfoReq(int seqNo, String[] tokens){
        this.SeqNo = seqNo;
        this.Tokens = tokens;
    }
}
