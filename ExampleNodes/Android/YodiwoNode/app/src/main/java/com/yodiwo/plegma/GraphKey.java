package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 17/8/2015 3:43:36 &#956;&#956;.
 */

/**
 * Globally unique identifier of a Graph
 */
public class GraphKey {

    public GraphDescriptorKey GraphDescriptorKey;

    public int GraphId;

    public GraphKey() {
    }

    public GraphKey(GraphDescriptorKey GraphDescriptorKey, int GraphId) {
        this.GraphDescriptorKey = GraphDescriptorKey;
        this.GraphId = GraphId;

    }

}
