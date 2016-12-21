package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:52.
 */

/**
 * Base class that describes a Model of a Thing Yodiwo.API.Plegma.Thing
 */
public class NodeModelType {
    /**
     * The unique identifier which identifies this model (must neither be null, nor empty)
     */
    public String Id;
    /**
     * Human readable name for this model
     */
    public String Name;
    /**
     * Human readable description for this model
     */
    public String Description;
    /**
     * Describes the configuration parameter(s) of this modelYodiwo.API.Plegma.ConfigDescription
     */
    public ConfigDescription[] Config;
    /**
     * Describes the port(s) of this modelYodiwo.API.Plegma.PortDescription
     */
    public PortDescription[] Port;

    public NodeModelType() {
    }

    public NodeModelType(String Id, String Name, String Description, ConfigDescription[] Config, PortDescription[] Port) {
        this.Id = Id;
        this.Name = Name;
        this.Description = Description;
        this.Config = Config;
        this.Port = Port;

    }

}
