package com.yodiwo.androidagent.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:42.
 */

/**
 * Globally unique identifier of a Graph's Block
 */
public class BlockKey {

    public com.yodiwo.androidagent.plegma.GraphKey GraphKey;

    public int BlockId;

    public BlockKey() {
    }

    public BlockKey(com.yodiwo.androidagent.plegma.GraphKey GraphKey, int BlockId) {
        this.GraphKey = GraphKey;
        this.BlockId = BlockId;

    }

}
