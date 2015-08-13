package com.yodiwo.virtualgateway;


import com.yodiwo.plegma.PortEventMsg;
import com.yodiwo.plegma.ThingsMsg;
import com.yodiwo.plegma.ThingsReq;

// The Interface to interacts with Yodiwo server
public interface IServerAPI {

    boolean SendThingsMsg(ThingsMsg meg);

    boolean SendThingsReq(ThingsReq req);

    boolean SendPortEvent(PortEventMsg msg);

    void StartRx();

    void StopRx();

}
