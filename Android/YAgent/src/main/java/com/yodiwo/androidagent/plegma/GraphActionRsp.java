package com.yodiwo.androidagent.plegma;

/**
 * Created by vaskanas on 06-Jun-16.
 */
public class GraphActionRsp extends GenericRsp {

    public String GraphDescriptorKey;

    public GraphActionRsp(){}

    public GraphActionRsp(int SeqNo, String GraphDescriptorKey, boolean IsSuccess, int StatusCode, String Message) {
        this.SeqNo = SeqNo;
        this.GraphDescriptorKey = GraphDescriptorKey;
        this.IsSuccess = IsSuccess;
        this.StatusCode = StatusCode;
        this.Message = Message;
    }
}
