package com.yodiwo.plegma;

import java.util.ArrayList;

/**
 * Created by ApiGenerator Tool (Java) on 11/08/2015 18:56:18.
 */
    /** 
 * Globally unique identifier of a GraphDescriptor
 */
        public class GraphDescriptorKey 
        {
            
            public UserKey UserKey;
            
            public String Id;
            
            public int Revision;
            
            public GraphDescriptorKey()
            {
            }
                
                public GraphDescriptorKey(UserKey UserKey,String Id,int Revision)
                {
                		this.UserKey = UserKey;
		this.Id = Id;
		this.Revision = Revision;

                }
                
        }
