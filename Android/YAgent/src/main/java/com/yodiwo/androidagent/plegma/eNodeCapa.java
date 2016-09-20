package com.yodiwo.androidagent.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:50.
 */

/**
 * enum of possible node capabilites
 */
public class eNodeCapa {
    /**
     * no capabilities
     */
    public static final int None = 0;
    /**
     * Node supports graph splitting
     */
    public static final int SupportsGraphSplitting = 1;
    /**
     * Node supports scanning for new Things
     * (i.e. Things are not just fixed at initial setup)
     */
    public static final int Scannable = 2;
    /**
     * Node supports the Warlock API
     */
    public static final int IsWarlock = 4;
}
