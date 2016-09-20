package com.yodiwo.androidagent.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:41.
 */

/**
 * Globally unique identifier of a GraphDescriptor
 */
public class GraphDescriptorKey {

    public com.yodiwo.androidagent.plegma.UserKey UserKey;

    public String Id;

    public int Revision;

    public GraphDescriptorKey() {
    }

    public GraphDescriptorKey(com.yodiwo.androidagent.plegma.UserKey UserKey, String Id, int Revision) {
        this.UserKey = UserKey;
        this.Id = Id;
        this.Revision = Revision;

    }

}
