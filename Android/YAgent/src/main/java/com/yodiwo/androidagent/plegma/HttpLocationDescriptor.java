package com.yodiwo.androidagent.plegma;

/**
 * Created by r00tb00t on 12/17/15.
 */
public class HttpLocationDescriptor
{
    public String Uri;
    public int RestServiceType;

    public HttpLocationDescriptor () {}

    public HttpLocationDescriptor(String uri, int restServiceType) {
        this.Uri = uri;
        this.RestServiceType = restServiceType;
    }
}
