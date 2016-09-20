package com.yodiwo.androidagent.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:43.
 */

/**
 * type of values that each Port sends / receives
 */
public class ePortType {
    /**
     * integer values
     */
    public static final int Integer = 1;
    /**
     * single precision floating point values
     */
    public static final int Decimal = 2;
    /**
     * double precision floating point values
     */
    public static final int DecimalHigh = 3;
    /**
     * boolean values (can be true/false, on/off, 1/0, etc)
     */
    public static final int Boolean = 4;
    /**
     * RGB triplet in "R,G,B" format
     */
    public static final int Color = 5;
    /**
     * generic string
     */
    public static final int String = 6;
    /**
     * video
     */
    public static final int VideoDescriptor = 7;
    /**
     * audio
     */
    public static final int AudioDescriptor = 8;
    /**
     * binary resource port
     */
    public static final int BinaryResourceDescriptor = 9;
    /**
     *  i2c thing
     */
    public static final int I2CDescriptor = 10;
    /**
     * json string
      */
    public static final int JsonString = 11;
    /**
     * incident descriptor port
      */
    public static final int IncidentDescriptor = 12;
}
