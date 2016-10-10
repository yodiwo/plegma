package com.yodiwo.androidagent.gcm;

import android.app.IntentService;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.os.AsyncTask;
import android.os.Bundle;
import android.util.Log;
import android.widget.Toast;

import com.google.android.gms.gcm.GcmPubSub;
import com.google.android.gms.gcm.GoogleCloudMessaging;
import com.google.android.gms.iid.InstanceID;

import java.io.IOException;

import com.yodiwo.androidagent.R;
import com.yodiwo.androidagent.core.GcmServerAPI;
import com.yodiwo.androidagent.core.Helpers;
import com.yodiwo.androidagent.core.PairingService;
import com.yodiwo.androidagent.core.SettingsProvider;
import com.yodiwo.androidagent.plegma.ApiRestAccess;
import com.yodiwo.androidagent.plegma.GcmConnectionMsg;
import com.yodiwo.androidagent.plegma.GcmDisconnectionMsg;

import retrofit.client.Response;

public class GcmRxService extends IntentService {

    private static final String TAG = GcmRxService.class.getSimpleName();
    private static final String[] TOPICS = {"global"};
    private static final String TOPIC_PREFIX = "/topics/";
    private SettingsProvider settingsProvider;
    private Context context;
    private InstanceID instanceID;
    private GcmPubSub pubSub;
    private String registrationToken;
    private GcmServerAPI serverAPI = null;
    private ApiRestAccess apiRestAccess;

    private static final String EXTRA_REQUEST_TYPE = "EXTRA_REQUEST_TYPE";

    private static final int REQUEST_RX_START = 10;
    private static final int REQUEST_RX_STOP = 11;

    public GcmRxService() {
        super(TAG);
    }

    @Override
    protected void onHandleIntent(Intent intent) {

        /** Initially this call goes out to the network to retrieve the token, subsequent calls are local.
         * R.string.gcm_defaultSenderId (the Sender ID) is typically derived from google-services.json.
         * See https://developers.google.com/cloud-messaging/android/start for details on this file.
         */

        context = getApplicationContext();

        if (settingsProvider == null)
            settingsProvider = SettingsProvider.getInstance(context);

        if (serverAPI == null)
            serverAPI = GcmServerAPI.getInstance(context);

        if (apiRestAccess == null)
            apiRestAccess = new ApiRestAccess(PairingService.getPairingWebUrl(settingsProvider));


        Bundle bundle = intent.getExtras();
        int request_type = bundle.getInt(EXTRA_REQUEST_TYPE);
        switch (request_type) {
            // -------------------------------------
            case REQUEST_RX_START: {
                try {
                    instanceID = InstanceID.getInstance(this);
                    // defaultSenderId is the GCM project_number
                    // INSTANCE_ID_SCOPE is just a String (“GCM”)
                    registrationToken = instanceID.getToken(getString(R.string.gcm_defaultSenderId),
                            GoogleCloudMessaging.INSTANCE_ID_SCOPE, null);
                    Helpers.log(Log.INFO, TAG, "GCM Registration Token: " + registrationToken);

                    // TODO: send any registration to my app's servers, if applicable.
                    //  registration token is needed to be stored on the app-server and is used to send a message to the device.
                     sendRegistrationToServer(registrationToken);

                    // TODO: Subscribe to topic channels, if applicable.
                    // subscribeTopics(token);

                    // boolean that indicates whether the generated token has been
                    // sent to your server.
                    GcmServerAPI.sentTokenToServer = true;

                } catch(IOException ex){
                    Helpers.logException(TAG, ex);
                    GcmServerAPI.sentTokenToServer = false;
                }
            }
            break;
            // -------------------------------------
            case REQUEST_RX_STOP: {
                // TODO: choose the appropriate one

                try {
                    // instanceID.deleteToken();
                    instanceID.deleteInstanceID();
                }catch (IOException ex){
                    Helpers.logException(TAG, ex);
                }
                // pubSub.unsubscribe();
            }

            break;
            // -----------------------------------
            default:
                Helpers.log(Log.ERROR, TAG, "GcmRxService received unknown action: " + request_type);
        }
    }

    // ---------------------------------------------------------------------------------------------

     /**
     * Persist registration to third-party servers.
     * Register a GCM registration token with the app server
     * @param token Registration token to be registered
     * @return true if request succeeds
     * @throws IOException
     */
     private void sendRegistrationToServer(String token) throws IOException {
         Helpers.log(Log.DEBUG, TAG, "Sending Registration Id / GCM connection notification to the Yodiwo server");
         String nodeKey = settingsProvider.getNodeKey();
         String secretKey = settingsProvider.getNodeSecretKey();
         GcmConnectionMsg conMsg = new GcmConnectionMsg(nodeKey, token);
         Response rsp = null;
         try
         {
             rsp = apiRestAccess.service.SendGcmConnectionMsg(nodeKey, secretKey, conMsg);
         }
         catch (Exception ex)
         {
            Helpers.logException(TAG, ex);
         }

         //TODO: better
         if (rsp == null || rsp.getStatus() != 200)
         {
             Helpers.log(Log.DEBUG, TAG, "sending of GcmConnectionMsg failed");
         }
     }

    private void sendDisconnectionToServer(String token) throws IOException {
        Helpers.log(Log.DEBUG, TAG, "Sending GCM disconnection notification to the Yodiwo server");
        String nodeKey = settingsProvider.getNodeKey();
        String secretKey = settingsProvider.getNodeSecretKey();
        GcmDisconnectionMsg disconMsg = new GcmDisconnectionMsg(token);
        Response rsp = apiRestAccess.service.SendGcmDisconnectionMsg(nodeKey, secretKey, disconMsg);
        if (rsp == null || rsp.getStatus() != 200)
        {
            Helpers.log(Log.DEBUG, TAG, "sending of GcmDisconnectionMsg failed");
        }
    }

    // ---------------------------------------------------------------------------------------------

    /**
     * Subscribe to any GCM topics of interest, as defined by the TOPICS constant.
     *
     * @param token GCM token
     * @throws IOException if unable to reach the GCM PubSub service
     */
    // [START subscribe_topics]
    private void subscribeTopics(String token) throws IOException {
        pubSub = GcmPubSub.getInstance(this);
        for (String topic : TOPICS) {
            if (topic.equals("") || !topic.startsWith(TOPIC_PREFIX) ||
                    topic.length() <= TOPIC_PREFIX.length()) {
                Toast.makeText(context, "Make sure topic is in format \\\"/topics/topicName\\\"\"", Toast.LENGTH_SHORT).show();
                return;
            }

            new SubscribeToTopicTask().execute(topic);
        }
    }
    // [END subscribe_topics]

    /**
     * Subscribe the client to the passed topic.
     */
    private class SubscribeToTopicTask extends AsyncTask<String, Void, Boolean> {

        private String topic;

        @Override
        protected Boolean doInBackground(String... params) {
            if (params.length > 0) {
                topic = params[0];
                try {
                    pubSub.subscribe(registrationToken, topic, null);
                    return true;
                } catch (IOException e) {
                    Helpers.logException(TAG, e);
                }
            }
            return false;
        }

        @Override
        protected void onPostExecute(Boolean succeed) {
            if (succeed) {
                Helpers.log(Log.DEBUG, TAG, "Subscribed to topic: " + topic);
            } else {
                Helpers.log(Log.DEBUG, TAG, "Subscription to topic failed: " + topic);
            }
        }
    }

    // ---------------------------------------------------------------------------------------------

    public static void StartRx(Context context) {
        Intent intent = new Intent(context, GcmRxService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_RX_START);
        ComponentName srv = context.startService(intent);
        Helpers.log(Log.DEBUG, TAG, "DEBUG RX Start");
    }

    // ---------------------------------------------------------------------------------------------

    public static void StopRx(Context context) {
        Intent intent = new Intent(context, GcmRxService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_RX_STOP);
        context.startService(intent);
        Helpers.log(Log.DEBUG, TAG, "DEBUG RX Stop");
    }

    // ---------------------------------------------------------------------------------------------
}
