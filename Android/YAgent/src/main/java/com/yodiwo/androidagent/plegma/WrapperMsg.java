package com.yodiwo.androidagent.plegma;

/**
 * Created by vaskanas on 03-Mar-16.
 */
public class WrapperMsg {

    /**
     * <summary>
     * Wrapper class mainly for providing synchronization services to sync-less protocols (mqtt, websockets, etc)
     * </summary>
     */

    /**
     * flags: req, rsp, msg
     */
    public int Flags;

    /**
     * for RPC sync only: Id of this message or of previous message that this message is responding to
     */
    public int SyncId;

    /**
     * JSON Serialized payload
     */
    public String Payload;

    /**
     *  Size of packed/serialized payload
     */
    public int PayloadSize;


}