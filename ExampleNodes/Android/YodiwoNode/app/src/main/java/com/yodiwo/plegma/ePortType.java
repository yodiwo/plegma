package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 3/8/2015 10:26:11 &#956;&#956;.
 */

/**
 * type of values that each Port sends / receives
 */
public enum ePortType {
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
