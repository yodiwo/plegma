package com.yodiwo.plegma;

import java.util.ArrayList;
import java.util.HashMap;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:51.
 */
    /** 
 * Describes restrictions and gives information of a configuration parameter.
 */
        public class ConfigDescription 
        {
            /** 
 * The default value (can be null)
 */
            public String DefaultValue;
            /** 
 * Human readable description (can be null)
 */
            public String Description;
            /** 
 * Human readable label (can be null or empty)
 */
            public String Label;
            /** 
 * Name of the configuration parameter (must neither be null nor empty)
 */
            public String Name;
            /** 
 * Specifies whether the parameter is required
 */
            public Boolean Required;
            /** 
 * The data type of the parameter (can be null)
 */
            public String Type;
            /** 
 * Minimum value
 */
            public double Minimum;
            /** 
 * Maximum value 
 */
            public double Maximum;
            /** 
 * Change step size
 */
            public double Stepsize;
            /** 
 * Specifies whether the parameter is read only
 */
            public Boolean ReadOnly;
            
            public ConfigDescription()
            {
            }
                
                public ConfigDescription(String DefaultValue,String Description,String Label,String Name,Boolean Required,String Type,double Minimum,double Maximum,double Stepsize,Boolean ReadOnly)
                {
                		this.DefaultValue = DefaultValue;
		this.Description = Description;
		this.Label = Label;
		this.Name = Name;
		this.Required = Required;
		this.Type = Type;
		this.Minimum = Minimum;
		this.Maximum = Maximum;
		this.Stepsize = Stepsize;
		this.ReadOnly = ReadOnly;

                }
                
        }
