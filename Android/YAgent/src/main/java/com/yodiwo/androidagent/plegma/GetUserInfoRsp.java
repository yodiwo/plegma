package com.yodiwo.androidagent.plegma;

import java.util.HashMap;

/**
 * Created by vaskanas on 26-May-16.
 */
public class GetUserInfoRsp extends ApiMsg {

    public HashMap<String, UserInfo> UserInfo;

    public GetUserInfoRsp(){}

    public GetUserInfoRsp(int seqNo, HashMap<String, UserInfo> userInfo){
        this.SeqNo = seqNo;
        this.UserInfo = userInfo;
    }
}
