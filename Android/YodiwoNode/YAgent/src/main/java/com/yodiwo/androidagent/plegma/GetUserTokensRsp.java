package com.yodiwo.androidagent.plegma;

import java.util.Arrays;
import java.util.Collections;
import java.util.HashMap;

/**
 * Created by vaskanas on 20-May-16.
 */
public class GetUserTokensRsp extends ApiMsg {

    /**
     *  source-key-to-token Hashmap. Source key is the <see cref="GetUserTokensReq"/> entry
     *  that matches the returned token value.
     *  Tokens are transient and may stop working on further requests at any time, at which point
     *  the API client is advised to request new ones via <see cref="GetUserTokensReq"/>
     *  If multiple tokens match (i.e. there were duplicate requests) each one overwrites the previous one; so don't do that
     */
    public HashMap<String, String> Tokens;


    public GetUserTokensRsp(){
        this.SeqNo = 0;
        this.Tokens = new HashMap<>();
    }

    public GetUserTokensRsp(int SeqNo, HashMap<String, String> Tokens){
        this.SeqNo = SeqNo;
        this.Tokens = Tokens;
    }

    @Override
    public String toString() {
        return String.valueOf(Collections.singletonList(Tokens));
    }
}
