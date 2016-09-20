package com.yodiwo.androidagent.plegma;

import java.util.HashMap;

/**
 * Created by vaskanas on 06-Jun-16.
 */
public class GetNodeInfoRsp extends ApiMsg {

    public HashMap<String, WNodeInfo> Nodes;

    public GetNodeInfoRsp(){}

    public GetNodeInfoRsp(int SeqNo, HashMap<String, WNodeInfo> Nodes){
        this.SeqNo = SeqNo;
        this.Nodes = Nodes;
    }

}
