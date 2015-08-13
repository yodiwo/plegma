package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 3/8/2015 10:26:13 &#956;&#956;.
 */

/**
 * Basic Input/Output entity of a Thing Creates and sends messages towards the Yodiwo cloud service,  or receives and handles messages from the cloud. Both events occur via the Yodiwo.API.Plegma.PortEventMsgand PortEventBatchMsg messages
 */
public class Port {
    /**
     * Friendly name of this Port (as it will appear in the Cyan UI and graphs)
     */
    public String Name;
    /**
     * type (Yodiwo.API.Plegma.ePortType) of values that each Port sends / receives
     */
    public ePortType Type;
    /**
     * Direction (Yodiwo.API.Plegma.ioPortDirection) of Port
     */
    public ioPortDirection ioDirection;
    /**
     * Globally unique string identifying this port; Construct it using the Yodiwo.API.Plegma.Port.PortKey constructor
     */
    public String PortKey;
    /**
     * Indicates the number of graphs this port is currently active in (i.e. number of deployed graphs that use this Port).
     * On the Thing (Device) side it is meant to be read only. If 0, it means that the user has not created any stories which depend on
     * and/or trigger this Port. As such the device should not send any Yodiwo.API.Plegma.PortEventMsg messages about it
     */
    public int IsInGraphs;
    /**
     * Current (at latest update/sampling/trigger/etc) value of Port as String.
     * Contains a string representation of the port's state, encoded according to the port's Yodiwo.API.Plegma.ePortTypeOn receiving events the Cloud Server will attempt to parse the State based on its Yodiwo.API.Plegma.ePortTypeWhen sending events the Cloud Server will encode the new state into a string, again according to the Port's Yodiwo.API.Plegma.ePortType
     */
    public String State;

    public Port() {
    }

    public Port(String Name, ePortType Type, ioPortDirection ioDirection, String PortKey, int IsInGraphs, String State) {
        this.Name = Name;
        this.Type = Type;
        this.ioDirection = ioDirection;
        this.PortKey = PortKey;
        this.IsInGraphs = IsInGraphs;
        this.State = State;

    }

}
