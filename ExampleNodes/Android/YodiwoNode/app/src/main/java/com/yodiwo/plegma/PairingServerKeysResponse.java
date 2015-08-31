package com.yodiwo.plegma;

import java.util.ArrayList;
import java.util.HashMap;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:35:06.
 */
    
        public class PairingServerKeysResponse 
        {
            
            public String nodeKey;
            
            public String secretKey;
            
            public PairingServerKeysResponse()
            {
            }
                
                public PairingServerKeysResponse(String nodeKey,String secretKey)
                {
                		this.nodeKey = nodeKey;
		this.secretKey = secretKey;

                }
                
        }
