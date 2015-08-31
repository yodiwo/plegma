package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 17/8/2015 3:43:49 &#956;&#956;.
 */

/**
 * Internal operation ID for Yodiwo.API.Plegma.ThingsReq and Yodiwo.API.Plegma.ThingsRsp messages
 */
public enum eThingsOperation {
    /**
     * invalid opcode
     */
    Invalid,
    /**
     * referenced things are to be updated at receiver. If they don't already exist, they are created
     */
    Update,
    /**
     * referenced things are to be updated at receiver if they exist, created if not.  Previously existing things at receiver that are not in this message are *deleted*
     */
    Overwrite,
    /**
     * ask that the receiver deletes referenced (by the ThingKey) thing
     */
    Delete,
    /**
     * ask that receiver sends back its existing things as a Yodiwo.API.Plegma.ThingsRsp
     */
    Get,
    /**
     * ask that the receiver scans for new things and send back all results (new and old) as a Yodiwo.API.Plegma.ThingsRsp
     */
    Scan,
}
