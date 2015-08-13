package com.yodiwo.plegma;

import java.util.ArrayList;

/**
 * Created by ApiGenerator Tool (Java) on 11/08/2015 18:56:16.
 */
    /** 
 * Globally unique identifier of a Yodiwo.API.Plegma.Thing
 */
        public class ThingKey 
        {
            
            public NodeKey NodeKey;
            
            public String ThingUID;
            
            public ThingKey()
            {
            }
                
                public ThingKey(NodeKey NodeKey,String ThingUID)
                {
                		this.NodeKey = NodeKey;
		this.ThingUID = ThingUID;

                }
                
            @Override public String toString() { return NodeKey.toString() + "-"+ ThingUID; }
            public static String CreateKey(String NodeKey,String ThingUID) { return NodeKey + "-"+ ThingUID; }
        }
