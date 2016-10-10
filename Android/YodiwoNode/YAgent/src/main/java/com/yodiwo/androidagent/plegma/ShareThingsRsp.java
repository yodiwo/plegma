package com.yodiwo.androidagent.plegma;

/**
 * Created by vaskanas on 11-May-16.
 */

import java.util.Arrays;

/**
 * Response to a <see cref="ShareThingsReq"/> message
 */
public class ShareThingsRsp extends ApiMsg {

    /**
     * indication of whether message as whole got handled or not
     */
    public boolean Handled;

    /**
     * Array of sub-responses to each of <see cref="ShareThingsReq"/>'s sub-requests
     */
    public ShareActionRsp[] ShareActionRsps;

    public ShareThingsRsp(){
        this.SeqNo = 0;
        this.Handled = false;
        this.ShareActionRsps = new ShareActionRsp[]{};
    }

    public ShareThingsRsp(int SeqNo, boolean Handled, ShareActionRsp[] SharedActionRsps){
        this.SeqNo = SeqNo;
        this.Handled = Handled;
        this.ShareActionRsps = SharedActionRsps;
    }

    @Override
    public String toString() {
        return Arrays.toString(ShareActionRsps);
    }
}
