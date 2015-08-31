package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 17/8/2015 3:43:39 &#956;&#956;.
 */

public enum ePortConf {
    /**
     * no configuration set
     */
    None,
    /**
     * port should receive all events, not only "dirty" ones (i.e. value not changed but triggered in graph)
     */
    ReceiveAllEvents,
    /**
     * mark the port as a trigger port (this may have an effect on where it's placed on the block model and how events from it are propagated)
     */
    IsTrigger,
}
