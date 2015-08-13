package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 3/8/2015 10:26:22 &#956;&#956;.
 */

/**
 * Allowed operations in Yodiwo.API.Plegma.PortStateReq messages
 */
public enum ePortStateOperation {
    /**
     * reserved; should not be used
     */
    Invalid,
    /**
     * request array of current state for the specified PortKey(s)
     */
    SpecificKeys,
    /**
     * request array of PortKeys of Ports currently deployed in graphs
     */
    ActivePortKeys,
    /**
     * request array of current states for ports currently deployed in graphs
     */
    ActivePortStates,
    /**
     * request array of current states for all ports of this Node
     */
    AllPortStates,
}
