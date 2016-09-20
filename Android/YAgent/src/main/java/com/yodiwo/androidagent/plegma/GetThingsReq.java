package com.yodiwo.androidagent.plegma;

/**
 * Created by vaskanas on 03-Jun-16.
 */
public class GetThingsReq extends ApiMsg {

    public String key;

    public GetThingsReq(){}

    public GetThingsReq(int seqNo, String key){
        this.SeqNo = seqNo;
        this.key = key;
    }
}
