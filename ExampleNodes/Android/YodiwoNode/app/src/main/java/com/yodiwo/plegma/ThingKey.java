package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 3/8/2015 10:26:09 &#956;&#956;.
 */

/**
 * Globally unique identifier of a Yodiwo.API.Plegma.Thing
 */
public class ThingKey {

    public NodeKey NodeKey;

    public String ThingUID;

    public ThingKey() {
    }

    public ThingKey(NodeKey NodeKey, String ThingUID) {
        this.NodeKey = NodeKey;
        this.ThingUID = ThingUID;

    }

    @Override
    public String toString() {
        return NodeKey.toString() + "-" + ThingUID;
    }

    public static String CreateKey(String NodeKey, String ThingUID) {
        return NodeKey + "-" + ThingUID;
    }
}
