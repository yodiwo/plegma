package com.yodiwo.plegma;

import retrofit.RestAdapter;
import retrofit.http.Body;
import retrofit.http.POST;

    /*
    private static final String workerIp = "10.1.1.10";
    private static final String workerUrl = "http://" + workerIp + ":3334";
    private static final String apiUrl = workerUrl + "/api/";
    private static final String postMsgPath = apiUrl + "portevent";
    */


public class ApiRestAccess {

    public YodiwoPlegmaApi service;

    public ApiRestAccess(String ApiTarget) {
        RestAdapter restAdapter = new RestAdapter.Builder()
                .setLogLevel(RestAdapter.LogLevel.FULL)
                .setEndpoint(ApiTarget)
                .build();

        service = restAdapter.create(YodiwoPlegmaApi.class);
    }

    public interface YodiwoPlegmaApi {
        /*
                @GET("/api")
                ArrayList<String> ListApiCalls();

                @POST("/api/nodethingsmsg")
                String SendNodeThings(@Body NodeThingsMsg things);

                @POST("/api/portevent")
                String SendPortEvent(@Body PortEventMsg portEventMsg);

                @POST("/api/batchportevent")
                String SendBatchPortEvent(@Body PortEventBatchMsg portEventMsg);

                @POST("/api/nodethingsreq")
                NodeThingsRsp SendNodeThingsReq(@Body NodeThingsReq portEventMsg);
            */

        // Pairing API
        @POST("/pairing/gettokens")
        PairingServerResponseTokens SendPairingGetTokens(@Body PairingNodeGetTokensRequest request);

        @POST("/pairing/getkeys")
        PairingServerResponseKeys SendPairingGetKeys(@Body PairingNodeGetKeysRequest request);

    }
}
