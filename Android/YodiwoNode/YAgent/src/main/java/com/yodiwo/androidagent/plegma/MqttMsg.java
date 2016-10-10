package com.yodiwo.androidagent.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:35:07.
 */

/**
 * Mqtt message encapsulation class.
 */
public class MqttMsg extends WrapperMsg {

    public MqttMsg() {
    }

    public MqttMsg(int SyncId, String Payload) {

    }

    public MqttMsg(String Payload, int SyncId, int Flags) {
        this.Flags = Flags;
        this.SyncId = SyncId;
        this.Payload = Payload;
    }
}
