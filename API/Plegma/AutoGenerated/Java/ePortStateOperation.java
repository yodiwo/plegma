package com.yodiwo.plegma;

import java.util.ArrayList;

/**
 * Created by ApiGenerator Tool (Java) on 11/08/2015 18:56:34.
 */
    /** 
 * Allowed operations in Yodiwo.API.Plegma.PortStateReq messages
 */
        public enum ePortStateOperation
        {
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
