package com.yodiwo.androidagent.plegma;

/**
 * Created by vaskanas on 03-Mar-16.
 */
public class GcmMsg extends WrapperMsg {

    /**
     * type of payload
     */
    public String PayloadType;

    public GcmMsg() {
    }

    public GcmMsg(String Payload, int SyncId, int Flags, String payloadType) {
        this.Flags = Flags;
        this.SyncId = SyncId;
        this.Payload = Payload;
        this.PayloadType = payloadType;
    }
}
