package com.yodiwo.androidagent.plegma;

import java.util.Arrays;

/**
 * Created by vaskanas on 17-May-16.
 */
public class ShareNotifyMsg extends ApiMsg {
    /**
     * Array of notification messages, each one specifying a separate, independent com.yodiwo.nebit.sharing notification towards the receiver
     */
    public ShareNotification[] Notifications;

    public ShareNotifyMsg(){
        this.SeqNo = 0;
        this.Notifications = new ShareNotification[]{};
    }

    public ShareNotifyMsg(int SeqNo, ShareNotification[] Notifications){
        this.SeqNo = SeqNo;
        this.Notifications = Notifications;
    }

    @Override
    public String toString() {
        return Arrays.toString(Notifications);
    }
}
