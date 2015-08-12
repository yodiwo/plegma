package com.yodiwo.plegma;

import java.util.ArrayList;

/**
 * Created by ApiGenerator Tool (Java) on 11/08/2015 18:56:19.
 */
    /** 
 * type of values that each Port sends / receives
 */
        public enum ePortType
        {
            /** 
 * undefined, should not be used!
 */
            Undefined,
            /** 
 * integer values
 */
            Scalar,
            /** 
 * single precision floating point values
 */
            Decimal,
            /** 
 * double precision floating point values
 */
            DecimalHigh,
            /** 
 * boolean values (can be true/false, on/off, 1/0, etc)
 */
            Boolean,
            /** 
 * RGB triplet in "R,G,B" format
 */
            Color,
            /** 
 * generic string
 */
            String,
        }
