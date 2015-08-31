package com.yodiwo.virtualgateway;

// The Interface to interacts with Yodiwo server
public interface IServerAPI {

    boolean Send(Object meg);

    void StartRx();

    void StopRx();

}
