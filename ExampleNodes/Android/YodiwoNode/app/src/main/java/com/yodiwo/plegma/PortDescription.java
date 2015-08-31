package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:51.
 */

/**
 * Describes restrictions and gives information of a port Yodiwo.API.Plegma.Port.
 */
public class PortDescription {
    /**
     * Human readable description for this port (can be null)
     */
    public String Description;
    /**
     * The unique identifier which identifies this port (must neither be null, nor empty)
     */
    public String Id;
    /**
     * Human readable label (can be null)
     */
    public String Label;
    /**
     * the category of this port , e.g. "TEMPERATURE"
     */
    public String Category;
    /**
     * Describes the state of this portYodiwo.API.Plegma.StateDescription
     */
    public StateDescription State;

    public PortDescription() {
    }

    public PortDescription(String Description, String Id, String Label, String Category, StateDescription State) {
        this.Description = Description;
        this.Id = Id;
        this.Label = Label;
        this.Category = Category;
        this.State = State;

    }

}
