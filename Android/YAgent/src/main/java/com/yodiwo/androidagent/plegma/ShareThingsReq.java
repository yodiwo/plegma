package com.yodiwo.androidagent.plegma;

import java.util.Arrays;

/**
 * Created by vaskanas on 11-May-16.
 */

/**
 * Message to either request a Share/Unshare action from the receiver, or to request information to be sent back
 */
public class ShareThingsReq extends ApiMsg {
    /**
     * Array of sub-requests, each one specifying a separate, independent com.yodiwo.nebit.sharing action request towards YCP
     */
    public ShareActionReq[] ShareActionReqs;


    public ShareThingsReq(){
        this.SeqNo = 0;
        this.ShareActionReqs = new ShareActionReq[]{};
    }

    public ShareThingsReq(int SeqNo, ShareActionReq[] SharedActionReqs){
        this.SeqNo = SeqNo;
        this.ShareActionReqs = SharedActionReqs;
    }

    @Override
    public String toString() {
        return Arrays.toString(ShareActionReqs);
    }
}
