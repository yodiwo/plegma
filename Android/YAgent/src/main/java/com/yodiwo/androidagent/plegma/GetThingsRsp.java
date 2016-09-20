package com.yodiwo.androidagent.plegma;

import java.util.ArrayList;
import java.util.HashMap;

/**
 * Created by vaskanas on 03-Jun-16.
 */
public class GetThingsRsp extends ApiMsg {

    public HashMap<String, ArrayList<Thing>> ThingsPerNode;

    public GetThingsRsp(){}

    public GetThingsRsp(int seqNo, HashMap<String, ArrayList<Thing>> thingsPerNode){
        this.SeqNo = seqNo;
        this.ThingsPerNode = thingsPerNode;
    }
}
