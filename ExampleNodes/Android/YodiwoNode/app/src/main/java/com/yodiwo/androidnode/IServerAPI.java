package com.yodiwo.androidnode;

// The Interface to interact with Yodiwo server
public interface IServerAPI {

    boolean Send(Object msg);   //send async message

    //Object SendReq(Object msg);        //send RPC Request

    boolean SendRsp(Object msg, int RespToSeqNo);   //send RPC Response

    void StartRx();

    void StopRx();

}
