package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:35:07.
 */

/**
 * Mqtt message encapsulation class.
 */
public class MqttMsg {

    /* Message Flags */
    public static final int Message = 0;
    public static final int Request = 1;
    public static final int Response = 2;

    /**
     * flags: req, rsp, msg
     */
    public int Flags;

    /**
     * for RPC sync only: Id of this message or of previous message that this message is responding to
     */
    public int SyncId;
    /**
     * The API message (payload)
     */
    public String Payload;
    /**
     * The size of the API message payload. CAN BE LEFT EMPTY
     */
    public int PayloadSize;

    public MqttMsg() {
    }

    public MqttMsg(int SyncId, String Payload) {}
    public MqttMsg(String Payload, int SyncId, int Flags) {
        this.Flags = Flags;
        this.SyncId = SyncId;
        this.Payload = Payload;
    }

}
