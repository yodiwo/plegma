package com.yodiwo.androidagent.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:39.
 */

/**
 * Globally unique identifier of a Node
 */
public class NodeKey {

    public com.yodiwo.androidagent.plegma.UserKey UserKey;

    public int NodeID;

    public NodeKey() {
    }

    public NodeKey(com.yodiwo.androidagent.plegma.UserKey UserKey, int NodeID) {
        this.UserKey = UserKey;
        this.NodeID = NodeID;

    }

    @Override
    public String toString() {
        return UserKey.toString() + "-" + NodeID;
    }

    public static String CreateKey(String UserKey, String NodeID) {
        return UserKey + "-" + NodeID;
    }
}
