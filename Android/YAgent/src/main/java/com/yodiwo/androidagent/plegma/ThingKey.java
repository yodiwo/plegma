package com.yodiwo.androidagent.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:40.
 */

/**
 * Globally unique identifier of a Yodiwo.API.Plegma.Thing
 */
public class ThingKey {

    public com.yodiwo.androidagent.plegma.NodeKey NodeKey;

    public String ThingUID;

    public String ThingCategory;

    public ThingKey() {
    }

    public ThingKey(com.yodiwo.androidagent.plegma.NodeKey NodeKey, String ThingUID) {
        this.NodeKey = NodeKey;
        this.ThingUID = ThingUID;

    }

    @Override
    public String toString() {
        return NodeKey.toString() + "-" + ThingUID;
    }

    public static String CreateKey(String NodeKey, String ThingUID ) {
        return NodeKey + "-" + ThingUID;
    }
}
