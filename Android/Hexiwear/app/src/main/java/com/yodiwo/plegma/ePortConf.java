package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:45.
 */

public class ePortConf {
    /**
     * no configuration set
     */
    public static final int None = 0;
    /**
     * port should receive all events, not only "dirty" ones (i.e. value not changed but triggered in graph)
     */
    public static final int ReceiveAllEvents = 1;
    /**
     * mark the port as a trigger port (this may have an effect on where it's placed on the block model and how events from it are propagated)
     */
    public static final int IsTrigger = 2;
}
