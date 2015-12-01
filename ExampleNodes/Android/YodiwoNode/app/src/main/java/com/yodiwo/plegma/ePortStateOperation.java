package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:58.
 */

/**
 * Allowed operations in Yodiwo.API.Plegma.PortStateReq messages
 */
public class ePortStateOperation {
    /**
     * request array of current state for the specified PortKey(s)
     */
    public static final int SpecificKeys  = 1;
    /**
     * request array of current states for ports currently deployed in graphs
     */
    public static final int ActivePortStates = 2;
    /**
     * request array of current states for all ports of this Node
     */
    public static final int AllPortStates = 3;
}
