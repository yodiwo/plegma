package com.yodiwo.androidnode;

import android.content.Context;
import android.util.Log;

import com.yodiwo.plegma.ApiRestAccess;


public class RestServerAPI extends aServerAPI {

    // =============================================================================================
    // Static

    public static final String TAG = RestServerAPI.class.getSimpleName();

    // Keep local global entry point for any request with Server.
    private static RestServerAPI server = null;

    // get the instance of the settings
    // if we for some reason the instance is not valid create a new one.
    public static RestServerAPI getInstance(Context context) {
        if (server == null) {
            server = new RestServerAPI(context.getApplicationContext());
        }
        return server;
    }

    // =============================================================================================
    // Instance code

    private ApiRestAccess apiRestAccess;


    public RestServerAPI(Context context) {
        this.context = context;
        settingsProvider = SettingsProvider.getInstance(context);

        String apiPath = getAPIPath();
        Log.d(TAG, "API Path: " + apiPath);

        apiRestAccess = new ApiRestAccess(apiPath);    //get from settings
    }

    // ---------------------------------------------------------------------------------------------

    public String getAPIPath() {
        return ((settingsProvider.getServerUseSSL()) ? "https://" : "http://") +
                settingsProvider.getServerAddress() + ":" +
                Integer.toString(settingsProvider.getServerPort());
    }

    // ---------------------------------------------------------------------------------------------

    @Override
    public boolean SendMsg(Object msg) {
        return false;
    }

    @Override
    public boolean SendReq(Object msg) {
        return false;
    }

    @Override
    public boolean SendRsp(Object msg, int syncId) {
        return false;
    }

    @Override
    public void StartRx() {
        RestRxService.StartRx(context);
    }

    // ---------------------------------------------------------------------------------------------

    @Override
    public void StopRx() {
        RestRxService.StopRx(context);
    }

    @Override
    protected void Teardown() {

    }

    // ---------------------------------------------------------------------------------------------

}



