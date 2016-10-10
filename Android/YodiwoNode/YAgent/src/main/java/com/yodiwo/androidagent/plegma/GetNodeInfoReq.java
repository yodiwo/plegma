package com.yodiwo.androidagent.plegma;

/**
 * Created by vaskanas on 06-Jun-16.
 */
public class GetNodeInfoReq extends ApiMsg {

    public String[] NodeKeys;

    public GetNodeInfoReq(){}

    public GetNodeInfoReq(int SeqNo, String[] NodeKeys){
        this.SeqNo = SeqNo;
        this.NodeKeys = NodeKeys;
    }
}
