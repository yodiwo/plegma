package com.yodiwo.plegma;

import java.util.ArrayList;
import java.util.HashMap;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:43.
 */
    /** 
 * Globally unique identifier of a TimelineDescriptor
 */
        public class TimelineDescriptorKey 
        {
            
            public UserKey UserKey;
            
            public String Id;
            
            public TimelineDescriptorKey()
            {
            }
                
                public TimelineDescriptorKey(UserKey UserKey,String Id)
                {
                		this.UserKey = UserKey;
		this.Id = Id;

                }
                
        }
