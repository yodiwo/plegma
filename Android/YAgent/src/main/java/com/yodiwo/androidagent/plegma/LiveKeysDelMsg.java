package com.yodiwo.androidagent.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:35:00.
 */

/**
 * Live Port Keys Payload Informs Node to delete portkeys for LiveView
 * Direction: Node -> Cloud
 */
public class LiveKeysDelMsg extends ApiMsg {
    /**
     * Array of portkeys of currently live(view) Ports
     */
    public String[] LivePortKeys;

    public LiveKeysDelMsg() {
    }

    public LiveKeysDelMsg(int SeqNo, String[] LivePortKeys) {
        this.SeqNo = SeqNo;
        this.LivePortKeys = LivePortKeys;
    }
}
