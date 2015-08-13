package com.yodiwo.basicApi;

import java.util.ArrayList;

/**
 * Created by ApiGenerator Tool (Java) on 09/07/2015 10:52:39.
 */
public class IdRsp extends APIMsg
{
    public NodeKey NodeKey;
    public eNodeType Type;
    public String Name;
    public NodeThingType[] ThingTypes;
    
    public IdRsp()
    {
        this.Id = eAPIType.None;
    }
    
    public IdRsp(NodeKey NodeKey,eNodeType Type,String Name,NodeThingType[] ThingTypes)
    {  
		this.NodeKey = NodeKey;
		this.Type = Type;
		this.Name = Name;
		this.ThingTypes = ThingTypes;
		this.Id = eAPIType.None;

    }
    
}
