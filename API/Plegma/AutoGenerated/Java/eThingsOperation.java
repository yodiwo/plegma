package com.yodiwo.plegma;

import java.util.ArrayList;

/**
 * Created by ApiGenerator Tool (Java) on 11/08/2015 18:56:31.
 */
    /** 
 * Internal operation ID for Yodiwo.API.Plegma.ThingsReq and Yodiwo.API.Plegma.ThingsRsp messages
 */
        public enum eThingsOperation
        {
            /** 
 * invalid opcode
 */
            Invalid,
            /** 
 * referenced things are to be created at receiver. If they already exist, they are updated
 */
            Create,
            /** 
 * referenced things are to be updated at receiver. Previously existing things at endpoint are not touched If a thing referenced in the message does not already exist at receiver, nothing should happen. If existing things are to be updated, then Yodiwo.API.Plegma.eThingsOperation.Create should be used
 */
            Update,
            /** 
 * referenced things are to be updated at receiver if they exist, created if not.
 * Previously existing things at receiver that are not in this message are *deleted*
 */
            Overwrite,
            /** 
 * ask that the receiver deletes referenced (by the ThingKey) thing
 */
            Delete,
            /** 
 * ask that receiver sends back its existing things as a Yodiwo.API.Plegma.ThingsRsp
 */
            Get,
            /** 
 * ask that the receiver scans for new things and send back all results (new and old) as a Yodiwo.API.Plegma.ThingsRsp
 */
            Scan,
        }
