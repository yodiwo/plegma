package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:56.
 */

/**
 * Node Things Response Response to a Yodiwo.API.Plegma.ThingsReq request
 * a ThingsRsp message should have:  - Yodiwo.API.Plegma.ThingsRsp.Operation set to ThingReq's operation
 * - ApiMsg.ResponseToSeqNo set to ThingReq's Yodiwo.API.Plegma.ApiMsg.SeqNo- Yodiwo.API.Plegma.ThingsRsp.Status set to True if ThingsReq was successfully handled and this Payload has valid data, False otherwise
 * - if Yodiwo.API.Plegma.ThingsRsp.Status is True, Yodiwo.API.Plegma.ThingsRsp.Data set to correspond to requested Req's operation, set to Null otherwise. Yodiwo.API.Plegma.ThingsRsp.Data is allowed to be null if originally requested operation does not expect back data, only status
 * Direction: bidirectional (Node->Cloud and Cloud->Node)
 */
public class ThingsRsp extends ApiMsg {

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

    public ThingsRsp() {
    }

    public ThingsRsp(int SeqNo, int Operation, Boolean Status, Thing[] Data) {
        this.SeqNo = SeqNo;
        this.Operation = Operation;
        this.Status = Status;
        this.Data = Data;

    }

}
