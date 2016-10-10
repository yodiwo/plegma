package com.yodiwo.androidagent.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:46.
 */

/**
 * Configuration parameters for the thing in generic name-value pairs
 */
public class ConfigParameter {

    public String Name;

    public String Value;

    public String Description;

    public ConfigParameter() {
    }


    public ConfigParameter(String Name, String Value) {
        this.Name = Name;
        this.Value = Value;
        this.Description = "";
    }

    public ConfigParameter(String Name, String Value, String Description) {
        this.Name = Name;
        this.Value = Value;
        this.Description = Description;
    }

}
