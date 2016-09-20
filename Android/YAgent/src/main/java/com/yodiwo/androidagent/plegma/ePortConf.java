package com.yodiwo.androidagent.plegma;

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
    public static final int PropagateAllEvents = 1;
    /**
     * mark the port as a trigger port (this may have an effect on where it's placed on the block model and how events from it are propagated)
     */
    public static final int IsTrigger = 2;

    /**
     * Enable this flag to force raw values for the port. Normalization will be applied if <see cref="Decimal_Range"/> or <see cref="Integer_Range"/> semantics is used</summary>
     */
    public static final int DoNotNormalize = 3;

    /**
     * The opposite of <see cref="PropagateAllEvents"/>. If set port will only propagate "dirty" events, where the value actually changed and was not just triggered</summary>
     */
    public static final int SupressIdenticalEvents = 4;
}
