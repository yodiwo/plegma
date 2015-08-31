package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:40.
 */

/**
 * Globally unique identifier of a Yodiwo.API.Plegma.Thing's Yodiwo.API.Plegma.Port
 */
public class PortKey {

    public ThingKey ThingKey;

    public String PortUID;

    public PortKey() {
    }

    public PortKey(ThingKey ThingKey, String PortUID) {
        this.ThingKey = ThingKey;
        this.PortUID = PortUID;

    }

    @Override
    public String toString() {
        return ThingKey.toString() + "-" + PortUID;
    }

    public static String CreateKey(String ThingKey, String PortUID) {
        return ThingKey + "-" + PortUID;
    }
}
