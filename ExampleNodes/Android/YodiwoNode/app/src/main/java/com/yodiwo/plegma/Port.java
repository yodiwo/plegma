package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:45.
 */

/**
 * Basic Input/Output entity of a Thing Creates and sends messages towards the Yodiwo cloud service,  or receives and handles messages from the cloud. Both events occur via the Yodiwo.API.Plegma.PortEventMsg message
 */
public class Port {
    /**
     * Globally unique string identifying this port; Construct it using the Yodiwo.API.Plegma.Port.PortKey constructor
     */
    public String PortKey;
    /**
     * Friendly name of this Port (as it will appear in the Cyan UI and blocks)
     */
    public String Name;
    /**
     * Description of Port to show in Cyan (tooltip, etc)
     */
    public String Description;
    /**
     * Direction (Yodiwo.API.Plegma.ioPortDirection) of Port
     */
    public ioPortDirection ioDirection;
    /**
     * type (Yodiwo.API.Plegma.ePortType) of values that each Port sends / receives
     */
    public ePortType Type;
    /**
     * Current (at latest update/sampling/trigger/etc) value of Port as String.
     * Contains a string representation of the port's state, encoded according to the port's Yodiwo.API.Plegma.ePortTypeOn receiving events the Cloud Server will attempt to parse the State based on its Yodiwo.API.Plegma.ePortTypeWhen sending events the Cloud Server will encode the new state into a string, again according to the Port's Yodiwo.API.Plegma.ePortType
     */
    public String State;
    /**
     * Port state sequence number: incremented by the Cloud server at every state update,  so that Node and servers stay in sync
     */
    public int RevNum;
    /**
     * Configuration flags for port
     */
    public ePortConf ConfFlags;

    public Port() {
    }

    public Port(String PortKey, String Name, String Description, ioPortDirection ioDirection, ePortType Type, String State, int RevNum, ePortConf ConfFlags) {
        this.PortKey = PortKey;
        this.Name = Name;
        this.Description = Description;
        this.ioDirection = ioDirection;
        this.Type = Type;
        this.State = State;
        this.RevNum = RevNum;
        this.ConfFlags = ConfFlags;

    }

}
