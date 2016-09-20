package com.yodiwo.androidagent.plegma;

/**
 * Created by vaskanas on 06-Jun-16.
 */
public class GraphActionReq extends ApiMsg {

    public int ActionType;

    public String GraphDescriptorKey;

    public GraphActionReq(){}

    public GraphActionReq(int SeqNo, int ActionType, String GraphDescriptorKey){
        this.SeqNo = SeqNo;
        this.GraphDescriptorKey = GraphDescriptorKey;
        this.ActionType = ActionType;
    }
}
