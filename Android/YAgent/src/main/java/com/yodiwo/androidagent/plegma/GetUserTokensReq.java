package com.yodiwo.androidagent.plegma;

import java.util.Arrays;

/**
 * Created by vaskanas on 20-May-16.
 */
public class GetUserTokensReq extends ApiMsg {
    /**
     * Array of emails that Req tries to convert to user tokens
     */
    public String[] Emails;

    /**
     * Array of names that Req tries to convert to user tokens
     */
    public String[] Names;

    public GetUserTokensReq(){
        this.SeqNo = 0;
        this.Emails = new String[]{};
        this.Names = new String[]{};
    }

    public GetUserTokensReq(int SeqNo, String[] Emails, String[] Names){
        this.SeqNo = SeqNo;
        this.Emails = Emails;
        this.Names = Names;
    }

    @Override
    public String toString() {
        return Arrays.toString(Emails);
    }
}
