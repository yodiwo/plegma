package com.yodiwo.androidagent.plegma;

import java.util.ArrayList;

/**
 * Created by vaskanas on 03-Jun-16.
 */
public class GetFriendsRsp extends ApiMsg {

    public ArrayList<String> Tokens;

    public GetFriendsRsp(){}

    public GetFriendsRsp(int seqNo, ArrayList<String> tokens){
        this.SeqNo = seqNo;
        this.Tokens = tokens;
    }
}
