package com.yodiwo.androidagent.plegma;

import java.util.HashMap;

/**
 * Created by vaskanas on 18-May-16.
 */
public class MyHackingReq extends ApiMsg {

    public String AppToken;

    public String AppId;

    public String ThingKey;

    public String NodeKey;

    public HashMap<String, String> Data;

    public HashMap<String, Boolean> IsDataString;


    public MyHackingReq(){
        this.SeqNo = 0;
        this.AppToken = "";
        this.AppId = "";
        this.NodeKey = "";
        this.ThingKey = "";
        this.Data = new HashMap<>();
        this.IsDataString = new HashMap<>();
    }

    public MyHackingReq(int SeqNo, String thingkey, String nodekey, String apptoken, String appid, HashMap<String, String> data, HashMap<String, Boolean> isDataString){
        this.SeqNo = SeqNo;
        this.AppToken = apptoken;
        this.ThingKey = thingkey;
        this.NodeKey = nodekey;
        this.AppId = appid;
        this.Data = data;
        this.IsDataString = isDataString;
    }
}
