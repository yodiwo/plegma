package com.yodiwo.androidagent.core;

import android.app.Activity;
import android.content.Context;
import android.os.AsyncTask;
import android.os.Bundle;
import android.util.Log;

import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.GoogleApiAvailability;
import com.google.android.gms.gcm.GoogleCloudMessaging;
import com.google.gson.Gson;

import java.io.IOException;
import java.util.concurrent.atomic.AtomicInteger;

import com.yodiwo.androidagent.gcm.GcmRxService;
import com.yodiwo.androidagent.plegma.GcmMsg;
import com.yodiwo.androidagent.plegma.PlegmaAPI;
import com.yodiwo.androidagent.plegma.eMsgFlags;


/**
 * Created by vaskanas on 02-Mar-16.
 */
public class GcmServerAPI extends aServerAPI {

    // =============================================================================================
    // Static
    // =============================================================================================

    protected static String GCM_SERVER_ADDRESS = "@gcm.googleapis.com";

    private static final int PLAY_SERVICES_RESOLUTION_REQUEST = 0;
    public static final String TAG = GcmServerAPI.class.getSimpleName();

    // Keep local global entry point for any request with Server.
    private static GcmServerAPI server = null;

    private AtomicInteger LastSyncId = new AtomicInteger();
    private Gson gson = new Gson();

    private static String userKey;
    private static String nodeKey;
    private static String secretKey;
    public static Boolean sentTokenToServer = false;
    protected static final String SENDER_ID = "";

    // get the instance of the settings
    // if for some reason the instance is not valid create a new one.
    public static GcmServerAPI getInstance(Context context) {
        if (server == null) {
            server = new GcmServerAPI(context.getApplicationContext());
        }
        return server;
    }

    // =============================================================================================
    // Instance code
    // ============================================================================================

    public GcmServerAPI(Context context) {
        this.context = context;
        this.settingsProvider = SettingsProvider.getInstance(context);

        // check if SENDER ID is not empty
        if (SENDER_ID.replace(" ", "").length() == 0) {
            Log.d(TAG, "GCM server API cannot be started: Empty Sender ID.");
            return;
        }

        Log.d(TAG, "Starting GCM server API.");

        RxActive = false;
        TxActive = false;
        RequestConnectivityUiUpdate();

        InitGcmClient();
    }

    // ---------------------------------------------------------------------------------------------

    private void InitGcmClient() {
        if (checkPlayServices())
            Connect();
    }

    // =============================================================================================
    // Public API
    // =============================================================================================

    @Override
    public void Connect() {

        try {
            //check that we are paired and connected
            userKey = settingsProvider.getUserKey();
            nodeKey = settingsProvider.getNodeKey();
            secretKey = settingsProvider.getNodeSecretKey();

            if (userKey == null || nodeKey == null || secretKey == null)
                return;

            // register client with GCM.
            StartRx();

            // connectivity UI update
            if (sentTokenToServer) {
                RxActive = true;
                TxActive = true;
                RequestConnectivityUiUpdate();
            }

            // send notification of (dis)connection to Node Service
            //TODO: this needs some synchronization, we need to wait for some result before sending GcmServerAPI.sentTokenToServer
            //because this is called before the intentservice has a chance to do anything
            NodeService.SetCloudConnStatus(context, GcmServerAPI.sentTokenToServer);
            //so for now just send true
            NodeService.SetCloudConnStatus(context, true);



            Log.d(TAG, "GCM new status: " + sentTokenToServer);

        }catch(Exception ex) {
            Helpers.logException(TAG, ex);
        }
    }

    // ---------------------------------------------------------------------------------------------

    @Override
    public boolean SendMsg(Object msg) {
        try {
            int syncId = 0; //async msgs do not need SyncId set
            String payloadType = PlegmaAPI.ApiMsgNames.get(msg.getClass());
            GcmMsg gcmMsg = new GcmMsg(gson.toJson(msg), syncId, eMsgFlags.Message, payloadType);
            if (_Send(gcmMsg, syncId))
                return true;
        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
        return false;
    }

    @Override
    public boolean SendReq(Object msg) {
        try {
            String payloadType = PlegmaAPI.ApiMsgNames.get(msg.getClass());
            int syncId = LastSyncId.incrementAndGet();
            GcmMsg gcmMsg = new GcmMsg(gson.toJson(msg), syncId, eMsgFlags.Request, payloadType);
            if (_Send(gcmMsg, syncId))
                return true;

        } catch(Exception ex){
            Helpers.logException(TAG, ex);
        }
        return false;
    }

    // ---------------------------------------------------------------------------------------------

    @Override
    public boolean SendRsp(Object msg, int syncId) {
        try {
            GcmMsg gcmMsg;

            if (msg != null) {
                String payloadType = PlegmaAPI.ApiMsgNames.get(msg.getClass());
                gcmMsg = new GcmMsg(gson.toJson(msg), syncId, eMsgFlags.Response,  payloadType);
            }
            else {
                //send a dummy response (to unblock potentially blocked server threads)
                String payloadType = PlegmaAPI.s_UnknownRsp;
                gcmMsg = new GcmMsg("", syncId, eMsgFlags.Response, payloadType);
            }

            if (_Send(gcmMsg, syncId))
                return true;
        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
        return false;
    }

    // ---------------------------------------------------------------------------------------------

    @Override
    protected void Teardown() {
        StopRx();
        server = null;
    }

    // ---------------------------------------------------------------------------------------------

    @Override
    public void StartRx() {
        GcmRxService.StartRx(context);
    }

    // ---------------------------------------------------------------------------------------------

    @Override
    public void StopRx() {

        //update UI
        RxActive = false;
        TxActive = false;
        RequestConnectivityUiUpdate();

        //send notification of disconnection to Node Service
        NodeService.SetCloudConnStatus(context, false);

        // unregister client
        GcmRxService.StopRx(context);
    }

    // =============================================================================================
    // Helpers
    // =============================================================================================
    private boolean checkPlayServices() {

        /**
         * Check the device to make sure it has the Google Play Services APK. If
         * it doesn't, display a dialog that allows users to download the APK from
         * the Google Play Store or enable it in the device's system settings.
         */

        GoogleApiAvailability apiAvailability = GoogleApiAvailability.getInstance();
        int resultCode = apiAvailability.isGooglePlayServicesAvailable(context);
        if (resultCode != ConnectionResult.SUCCESS) {
            if (apiAvailability.isUserResolvableError(resultCode)) {
                apiAvailability.getErrorDialog((Activity) context, resultCode, PLAY_SERVICES_RESOLUTION_REQUEST)
                        .show();
            } else {
                Log.i(TAG, "The device does not support GCM protocol.");
            }
            return false;
        }
        return true;
    }

    // ---------------------------------------------------------------------------------------------

    private boolean _Send(Object gcm_msg, int syncId) {
        try {
            if (publish(gson.toJson(gcm_msg), syncId))
                return true;
        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
        return false;
    }

    // ---------------------------------------------------------------------------------------------

    private boolean publish(String message, final int syncId) {
        final GoogleCloudMessaging gcm = GoogleCloudMessaging.getInstance(context);
        final Boolean[] msgSent = {false};

        if (sentTokenToServer) {
            final Bundle data = new Bundle();
            data.putString("message", message);

            new AsyncTask<Void, Void, String>() {
                @Override
                protected String doInBackground(Void... params) {
                    try {
                        gcm.send(SENDER_ID + GCM_SERVER_ADDRESS, Integer.toString(syncId), data);
                        Log.d(TAG, "Successfully sent upstream message");
                        return "";
                    } catch (IOException ex) {
                        Log.e(TAG, "Error sending upstream message", ex);
                        Helpers.logException(TAG, ex);
                    }
                    return null;
                }

                @Override
                protected void onPostExecute(String result) {
                    msgSent[0] = result != null;
                }
            }.execute(null, null, null);
        }
        return msgSent[0];
    }

}
