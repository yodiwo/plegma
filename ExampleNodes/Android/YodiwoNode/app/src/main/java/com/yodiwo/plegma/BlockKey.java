package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:42.
 */

/**
 * Globally unique identifier of a Graph's Block
 */
public class BlockKey {

    public GraphKey GraphKey;

    public int BlockId;

    public BlockKey() {
    }

    public BlockKey(GraphKey GraphKey, int BlockId) {
        this.GraphKey = GraphKey;
        this.BlockId = BlockId;

    }

}
