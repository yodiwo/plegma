package com.yodiwo.plegma;

import java.util.ArrayList;
import java.util.HashMap;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:58.
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
