package com.yodiwo.basicApi;

import java.util.ArrayList;

import retrofit.RestAdapter;
import retrofit.http.Body;
import retrofit.http.GET;
import retrofit.http.POST;

    /*
    private static final String workerIp = "10.1.1.10";
    private static final String workerUrl = "http://" + workerIp + ":3334";
    private static final String apiUrl = workerUrl + "/api/";
    private static final String postMsgPath = apiUrl + "portevent";
    */


public class ApiRestAccess {

    public PlegmaApi service;

    public ApiRestAccess(String ApiTarget) {
        RestAdapter restAdapter = new RestAdapter.Builder()
                .setLogLevel(RestAdapter.LogLevel.FULL)
                .setEndpoint(ApiTarget)
                .build();

        service = restAdapter.create(PlegmaApi.class);
    }


    public interface PlegmaApi {
        @GET("/api")
        ArrayList<String> ListApiCalls();

        @POST("/api/thingsrsp")
        String SendNodeThings(@Body ThingsRsp things);

        @POST("/api/porteventmsg")
        String SendPortEvent(@Body PortEventMsg portEventMsg);

        @POST("/api/thingsreq")
        NodeThingsRsp SendNodeThingsReq(@Body ThingsReq thingsReq);

        // Pairing API
        @POST("/pairing/gettokens")
        PairingServerResponseTokens SendPairingGetTokens(@Body PairingNodeGetTokensRequest request);

        @POST("/pairing/getkeys")
        PairingServerResponseKeys SendPairingGetKeys(@Body PairingNodeGetKeysRequest request);
    }
}
