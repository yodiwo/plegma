package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:56.
 */

/**
 * Port Event class: used to describe a new event that should trigger en endpoint, either towards a node or the Cloud Services
 */
public class PortEvent {
    /**
     * Yodiwo.API.Plegma.PortEvent.PortKey of the Yodiwo.API.Plegma.Port this message refers to (either generating the event, or receiving the event)
     */
    public String PortKey;
    /**
     * Contents of the event in string form. See Yodiwo.API.Plegma.Port.State
     */
    public String State;
    /**
     * Revision number of this update; matches the Port State's internal sequence numbering. See Yodiwo.API.Plegma.Port.State
     */
    public int RevNum;

    public PortEvent() {
    }

    public PortEvent(String PortKey, String State, int RevNum) {
        this.PortKey = PortKey;
        this.State = State;
        this.RevNum = RevNum;

    }

}
