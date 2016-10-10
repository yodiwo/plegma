package com.yodiwo.androidagent.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 22-Mar-16 12:15:10 PM.
 */

/**
 * GCM connection message send by a GCM client to indicate that it can no longer be reached over GCM
 */
public class GcmDisconnectionMsg {
    /**
     * Registration Id issued by the GCM connection server
     */
    public String RegistrationId;

    public GcmDisconnectionMsg() {
    }

    public GcmDisconnectionMsg(String RegistrationId) {
        this.RegistrationId = RegistrationId;
    }

}
