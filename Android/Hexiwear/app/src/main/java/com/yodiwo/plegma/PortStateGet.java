package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:58.
 */

/**
 * Port State Request. Will result in a response of type Yodiwo.API.Plegma.PortStateSet
 * Direction: node->cloud
 */
public class PortStateGet extends ApiMsg {
    /**
     * Type of operation requested
     */
    public int Operation;
    /**
     * List of PortKeys that the server should send an update for (in conjuction with Yodiwo.API.Plegma.ePortStateOperation.SpecificKeys).
     * If set to null or an empty array then the server will send an update for all relevant PortKeys
     */
    public String[] PortKeys;

    public PortStateGet() {
    }

    public PortStateGet(int SeqNo, int Operation, String[] PortKeys) {
        this.SeqNo = SeqNo;
        this.Operation = Operation;
        this.PortKeys = PortKeys;

    }
}
