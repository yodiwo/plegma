package com.yodiwo.plegma;

import java.util.ArrayList;

/**
 * Created by ApiGenerator Tool (Java) on 11/08/2015 18:56:16.
 */
    /** 
 * Globally unique identifier of a Yodiwo.API.Plegma.Thing's Yodiwo.API.Plegma.Port
 */
        public class PortKey 
        {
            
            public ThingKey ThingKey;
            
            public String PortUID;
            
            public PortKey()
            {
            }
                
                public PortKey(ThingKey ThingKey,String PortUID)
                {
                		this.ThingKey = ThingKey;
		this.PortUID = PortUID;

                }
                
            @Override public String toString() { return ThingKey.toString() + "-"+ PortUID; }
            public static String CreateKey(String ThingKey,String PortUID) { return ThingKey + "-"+ PortUID; }
        }
