package com.yodiwo.basicApi;

import java.util.ArrayList;

/**
 * Created by ApiGenerator Tool (Java) on 09/07/2015 10:52:40.
 */
public class NodeThingsRsp extends APIMsg
{
    public String ThingKey;
    public eNodeThingsOperation Operation;
    public Boolean Status;
    public Thing[] Data;
    
    public NodeThingsRsp()
    {
        this.Id = eAPIType.None;
    }
    
    public NodeThingsRsp(String ThingKey,eNodeThingsOperation Operation,Boolean Status,Thing[] Data)
    {  
		this.ThingKey = ThingKey;
		this.Operation = Operation;
		this.Status = Status;
		this.Data = Data;
		this.Id = eAPIType.None;

    }
    
}
