package com.yodiwo.androidagent.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 22-Mar-16 12:15:09 PM.
 */

/**
 * GCM connection message send by a GCM client to indicate that it can be reached over GCM This is also used by the server to create the NodeKey - (GCM) Registration Id association, and authenticate Registration Ids
 */
public class GcmConnectionMsg {
    /**
     * NodeKey
     */
    public String NodeKey;
    /**
     * Registration Id issued by the GCM connection server
     */
    public String RegistrationId;

    public GcmConnectionMsg() {
    }

    public GcmConnectionMsg(String NodeKey, String RegistrationId) {
        this.NodeKey = NodeKey;
        this.RegistrationId = RegistrationId;
    }

}
