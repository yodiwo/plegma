package com.yodiwo.androidagent.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:56.
 */


public class ThingsSet extends ApiMsg {

    public static final Integer Update = 1;
    public static final Integer Overwrite = 2;
    public static final Integer Delete = 3;
    public static final Integer Get = 4;
    public static final Integer Scan = 5;

    /**
     * Identifier of this message's operation of type Yodiwo.API.Plegma.eThingsOperationOperation fields must match between Req and Rsp.
     */
    public int Operation;
    /**
     * Indicates if the request was successful and this response contains actual data
     */
    public Boolean Status;
    /**
     * Array of Yodiwo.API.Plegma.Things that contain data related to the selected Operation, if applicable
     */
    public Thing[] Data;

    public int RevNum;

    public ThingsSet() {
    }

    public ThingsSet(int SeqNo, int Operation, Boolean Status, Thing[] Data, int RevNum) {
        this.SeqNo = SeqNo;
        this.Operation = Operation;
        this.Status = Status;
        this.Data = Data;
        this.RevNum = RevNum;
    }

}
