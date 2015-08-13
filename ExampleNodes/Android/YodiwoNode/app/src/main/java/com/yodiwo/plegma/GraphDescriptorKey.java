package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 3/8/2015 10:26:10 &#956;&#956;.
 */

/**
 * Globally unique identifier of a GraphDescriptor
 */
public class GraphDescriptorKey {

    public UserKey UserKey;

    public String Id;

    public int Revision;

    public GraphDescriptorKey() {
    }

    public GraphDescriptorKey(UserKey UserKey, String Id, int Revision) {
        this.UserKey = UserKey;
        this.Id = Id;
        this.Revision = Revision;

    }

}
