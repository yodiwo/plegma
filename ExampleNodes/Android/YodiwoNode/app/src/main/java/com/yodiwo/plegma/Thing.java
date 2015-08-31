package com.yodiwo.plegma;

import java.util.ArrayList;
import java.util.HashMap;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:47.
 */
    /** 
 * Main representation of a Thing that can interact with the Yodiwo cloud service
 */
        public class Thing 
        {
            /** 
 * Globally unique Key string of this Thing
 */
            public String ThingKey;
            /** 
 * friendly name of this Thing
 */
            public String Name;
            /** 
 * list of vendor provided configuration parameters
 */
            public ArrayList<ConfigParameter> Config = new ArrayList<>();
            /** 
 * list of ports (inputs / outputs) that this Thing implements
 */
            public ArrayList<Port> Ports = new ArrayList<>();
            /** 
 * Specifies the Thing's type Yodiwo.API.Plegma.NodeModelType
 */
            public String Type;
            /** 
 * Specifies the Thing's block type if it's to be specially modeled in the Cyan UI It can be left null if this Thing is to be modeled by the default Cyan UI blocks In this case Output-type Ports are gathered and represented as a Cyan UI Input Thing (thing->cloud events) and Input-type Ports are gathered and represented as a Cyan UI Output Thing (cloud->thing events) Both event directions occur via the Yodiwo.API.Plegma.PortEventMsg and PortEventBatchMsg messages
 */
            public String BlockType;
            /** 
 * Hints for the UI system
 */
            public ThingUIHints UIHints;
            
            public Thing()
            {
            }
                
                public Thing(String ThingKey,String Name,ArrayList<ConfigParameter> Config,ArrayList<Port> Ports,String Type,String BlockType,ThingUIHints UIHints)
                {
                		this.ThingKey = ThingKey;
		this.Name = Name;
		this.Config = Config;
		this.Ports = Ports;
		this.Type = Type;
		this.BlockType = BlockType;
		this.UIHints = UIHints;

                }
                
        }
