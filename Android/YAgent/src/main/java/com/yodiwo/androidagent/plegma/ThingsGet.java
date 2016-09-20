package com.yodiwo.androidagent.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:55.
 */


public class ThingsGet extends ApiMsg {

    public static final Integer Update = 1;
    public static final Integer Overwrite = 2;
    public static final Integer Delete = 3;
    public static final Integer Get = 4;
    public static final Integer Scan = 5;

    /**
     * Identifier of the operation requested; see Yodiwo.API.Plegma.eThingsOperation
     */
    public int Operation;
    /**
     * Yodiwo.API.Plegma.ThingsReq.ThingKey of the Yodiwo.API.Plegma.Thing that this request refers to. If left null (invalid ThingKey)
     * then this operation refers to all of the Node's Things
     */
    public String ThingKey;

    public int RevNum;

    public ThingsGet() {
    }

    public ThingsGet(int SeqNo, int Operation, String ThingKey, int RevNum) {
        this.SeqNo = SeqNo;
        this.Operation = Operation;
        this.ThingKey = ThingKey;
        this.RevNum = RevNum;
    }
}
