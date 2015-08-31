package com.yodiwo.plegma;

import java.util.ArrayList;
import java.util.HashMap;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:42.
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
