package com.yodiwo.androidagent.plegma;

/**
 * Created by vaskanas on 06-Jun-16.
 */
public class MyHackingRsp extends ApiMsg {

    public String Key;

    public String AppToken;

    public Boolean IsSuccess;

    public MyHackingRsp(){
    }

    public MyHackingRsp(int SeqNo, String thingkey, String apptoken, String Key, Boolean isscuccess){
        this.SeqNo = SeqNo;
        this.AppToken = apptoken;
        this.Key = Key;
        this.IsSuccess = isscuccess;
    }
}
