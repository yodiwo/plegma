package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:57.
 */

import java.util.ArrayList;

/**
 * asynchronous Port Event message The main API message to exchange events between Nodes and the Yodiwo Cloud Service
 * Direction: bidirectional (Node->Cloud and Cloud->Node)
 */
public class PortEventMsg extends ApiMsg {
    /**
     * Array of Yodiwo.API.Plegma.PortEvent messages
     */
    public ArrayList<PortEvent> PortEvents;

    public PortEventMsg() {
        SeqNo = 0;
        PortEvents = new ArrayList<>();
    }

    public PortEventMsg(int SeqNo, ArrayList<PortEvent> PortEvents) {
        this.SeqNo = SeqNo;
        this.PortEvents = PortEvents;

    }

}
