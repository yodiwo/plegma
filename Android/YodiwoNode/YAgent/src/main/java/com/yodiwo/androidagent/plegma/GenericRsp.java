package com.yodiwo.androidagent.plegma;

/**
 * Created by r00tb00t on 2/3/16.
 */
public class GenericRsp extends ApiMsg {

    public boolean IsSuccess;

    public int StatusCode;

    public String Message;

    public GenericRsp() {}

    public GenericRsp(int SeqNo, boolean IsSuccess, int StatusCode, String Message) {
        this.SeqNo = SeqNo;
        this.IsSuccess = IsSuccess;
        this.StatusCode = StatusCode;
        this.Message = Message;
    }
}
