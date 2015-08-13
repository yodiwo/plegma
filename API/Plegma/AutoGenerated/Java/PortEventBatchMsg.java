package com.yodiwo.basicApi;

import java.util.ArrayList;

/**
 * Created by ApiGenerator Tool (Java) on 09/07/2015 10:52:40.
 */
public class PortEventBatchMsg extends APIMsg
{
    public PortEventMsg[] PortEvents;
    
    public PortEventBatchMsg()
    {
        this.Id = eAPIType.PortEventBatchMsg;
    }
    
    public PortEventBatchMsg(PortEventMsg[] PortEvents)
    {  
		this.PortEvents = PortEvents;
		this.Id = eAPIType.PortEventBatchMsg;

    }
    
}
