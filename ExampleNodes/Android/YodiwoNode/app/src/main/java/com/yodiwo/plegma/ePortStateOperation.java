package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 17/8/2015 3:43:51 &#956;&#956;.
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
     * request array of current states for ports currently deployed in graphs
     */
    ActivePortStates,
    /**
     * request array of current states for all ports of this Node
     */
    AllPortStates,
}
