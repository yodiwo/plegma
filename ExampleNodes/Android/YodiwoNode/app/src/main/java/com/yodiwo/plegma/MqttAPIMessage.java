package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:35:07.
 */

/**
 * Mqtt message encapsulation class.
 */
public class MqttAPIMessage {
    /**
     * for RPC responses only: sequence number of previous message that this message is responding to
     */
    public int ResponseToSeqNo;
    /**
     * The API message (payload)
     */
    public String Payload;

    public MqttAPIMessage() {
    }

    public MqttAPIMessage(int ResponseToSeqNo, String Payload) {
        this.ResponseToSeqNo = ResponseToSeqNo;
        this.Payload = Payload;
    }

}
