package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:39.
 */

/**
 * Globally unique identifier of a Node
 */
public class NodeKey {

    public UserKey UserKey;

    public int NodeID;

    public NodeKey() {
    }

    public NodeKey(UserKey UserKey, int NodeID) {
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
