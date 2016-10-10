package com.yodiwo.androidagent.plegma;

import java.util.ArrayList;

import retrofit.RestAdapter;
import retrofit.client.Response;
import retrofit.http.Body;
import retrofit.http.GET;
import retrofit.http.POST;
import retrofit.http.Path;

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
        ArrayList<String>
        ListApiCalls();

        @POST("/api/1/thingsset")
        String SendNodeThings(@Body ThingsSet things);

        @POST("/api/1/porteventmsg")
        String SendPortEvent(@Body PortEventMsg portEventMsg);

        @POST("/api/1/thingsget")
        ThingsSet SendThingsGet(@Body ThingsGet thingsGet);

        // Pairing API
        @POST("/pairing/1/gettokensreq")
        PairingServerTokensResponse SendPairingGetTokens(@Body PairingNodeGetTokensRequest request);

        @POST("/pairing/1/getkeysreq")
        PairingServerKeysResponse SendPairingGetKeys(@Body PairingNodeGetKeysRequest request);

        //GCM API
        @POST("/api/1/{nkey}/{skey}/gcmconnectionmsg")
        Response SendGcmConnectionMsg(@Path("nkey") String nodeKey, @Path("skey") String secretKey, @Body GcmConnectionMsg request);

        @POST("/api/1/{nkey}/{skey}/gcmdisconnectionmsg")
        Response SendGcmDisconnectionMsg(@Path("nkey") String nodeKey, @Path("skey") String secretKey, @Body GcmDisconnectionMsg request);
    }
}
