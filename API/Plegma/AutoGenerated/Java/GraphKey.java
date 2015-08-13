package com.yodiwo.plegma;

import java.util.ArrayList;

/**
 * Created by ApiGenerator Tool (Java) on 11/08/2015 18:56:18.
 */
    /** 
 * Globally unique identifier of a Graph
 */
        public class GraphKey 
        {
            
            public GraphDescriptorKey GraphDescriptorKey;
            
            public int GraphId;
            
            public GraphKey()
            {
            }
                
                public GraphKey(GraphDescriptorKey GraphDescriptorKey,int GraphId)
                {
                		this.GraphDescriptorKey = GraphDescriptorKey;
		this.GraphId = GraphId;

                }
                
        }
