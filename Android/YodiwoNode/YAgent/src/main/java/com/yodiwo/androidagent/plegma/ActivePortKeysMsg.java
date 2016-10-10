package com.yodiwo.androidagent.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:35:00.
 */

/**
 * Active Port Keys Payload Informs Node of all currently active Ports (i.e. Ports that are connected and active in currently deployed graphs).
 * Should be used by Nodes to suppress events from inactive ports, allowing more efficient use of medium
 * Direction: Cloud -> Node
 */
public class ActivePortKeysMsg extends ApiMsg {
    /**
     * Array of portkeys of currently active Ports
     */
    public String[] ActivePortKeys;

    public ActivePortKeysMsg() {
    }

    public ActivePortKeysMsg(int SeqNo, String[] ActivePortKeys) {
        this.SeqNo = SeqNo;
        this.ActivePortKeys = ActivePortKeys;

    }

}
