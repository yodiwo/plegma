package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 3/8/2015 10:26:20 &#956;&#956;.
 */

/**
 * Internal operation ID for Things{Req/Msg} messages
 */
public enum eThingsOperation {
    /**
     * invalid opcode
     */
    Invalid,
    /**
     * [ThingsMsg only] referenced things are to be created at endpoint
     */
    Create,
    /**
     * [ThingsReq only] ask that the endpoint sends referenced thing(s)
     */
    Get,
    /**
     * [ThingsMsg only] referenced things are to be updated at endpoint. Previously existing things at endpoint are not altered
     */
    Update,
    /**
     * [ThingsMsg only] referenced things are to be updated at endpoint if they exist, created if not.
     * Previously existing things at endpoint that are not in this message are deleted
     */
    Overwrite,
    /**
     * ask that the endpoint deletes referenced thing
     */
    Delete,
    /**
     * ask that the endpoint scans for new things
     */
    Scan,
}
