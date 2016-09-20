package com.yodiwo.androidagent.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:35:04.
 */

public class PairingNodeGetTokensRequest {

    public String uuid;

    public String name;

    public String RedirectUri;

    public String image;
    public String description;
    public String pathcss;
    public String PairingCompletionInstructions;
    public byte[] PublicKey;
    public boolean NoUUIDAuthentication;

    public PairingNodeGetTokensRequest() {
    }

    public PairingNodeGetTokensRequest(String uuid, String name, String redirectUrl) {
        this.uuid = uuid;
        this.name = name;
        this.RedirectUri = redirectUrl;
    }

}
