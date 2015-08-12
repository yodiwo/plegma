package com.yodiwo.basicApi;

import java.util.ArrayList;

/**
 * Created by ApiGenerator Tool (Java) on 09/07/2015 10:52:39.
 */
public class NodeThingsReq extends APIMsg
{
    public String ThingKey;
    public eNodeThingsOperation Operation;
    public Thing Data;
    
    public NodeThingsReq()
    {
        this.Id = eAPIType.None;
    }
    
    public NodeThingsReq(String ThingKey,eNodeThingsOperation Operation,Thing Data)
    {  
		this.ThingKey = ThingKey;
		this.Operation = Operation;
		this.Data = Data;
		this.Id = eAPIType.None;

    }
    
}
