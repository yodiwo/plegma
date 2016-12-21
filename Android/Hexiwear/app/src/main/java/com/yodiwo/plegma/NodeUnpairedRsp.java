package com.yodiwo.plegma;

/**
 * Created by Nikos on 28/11/2015.
 */

/**
 * Node Unpairing Response
 * Payload is empty, message exists to make sure that node does receive message before being forcefully disconnected
 * Direction: Node->Cloud
 */
public class NodeUnpairedRsp extends ApiMsg {

    public NodeUnpairedRsp() {
    }

    public NodeUnpairedRsp(int SeqNo) {
        this.SeqNo = SeqNo;
    }
}
