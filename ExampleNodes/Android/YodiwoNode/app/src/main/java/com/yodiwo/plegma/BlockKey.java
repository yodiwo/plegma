package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 17/8/2015 3:43:36 &#956;&#956;.
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
