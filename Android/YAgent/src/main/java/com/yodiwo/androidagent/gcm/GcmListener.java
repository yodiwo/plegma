
package com.yodiwo.androidagent.gcm;

import android.os.Bundle;
import android.util.Log;

import com.google.android.gms.gcm.GcmListenerService;
import com.google.gson.Gson;

import com.yodiwo.androidagent.core.NodeService;
import com.yodiwo.androidagent.plegma.GcmMsg;

public class GcmListener extends GcmListenerService {

    private static final String TAG = GcmListener.class.getSimpleName();

    private static final String s_GcmDataField = "Data";

    private Gson gson = new Gson();
//    private Context context = getApplicationContext();

    /**
     * Called when message is received.
     *
     * @param from SenderID of the sender.
     * @param data Data bundle containing message data as key/value pairs.
     *             For Set of keys use data.keySet().
     */
    // [START receive_message]
    @Override
    public void onMessageReceived(String from, Bundle data) {
        String message = data.getString(s_GcmDataField);
        Log.d(TAG, "From: " + from);
        Log.d(TAG, "GCM received msg: " + message);

        if (from.startsWith("/topics/")) {
            // message received from some topic.
        }
        else {
            if (message != null) {
                //convert to GcmMsg
                GcmMsg gcmMessage = gson.fromJson(message, GcmMsg.class);

                String msgPayload = gcmMessage.Payload;
                int msgSyncId = gcmMessage.SyncId;
                int msgFlags = gcmMessage.Flags;
                String msgType = gcmMessage.PayloadType;

                Log.i(TAG, "type: " + msgFlags + " SyncID: " + msgSyncId + " payload:" + msgPayload);

                // Send the message to node service
                NodeService.RxMsg(getApplicationContext(), msgType, msgPayload, msgSyncId, msgFlags);
            }
        }
    }

    // ---------------------------------------------------------------------------------------------

    @Override
    public void onDeletedMessages (){
        /** Called when GCM server deletes pending messages due to exceeded storage limits,
         * for example, when the device cannot be reached for an extended period of time.
         * It is recommended to retrieve any missing messages directly from the app server.
         */
        // TODO: how to handle deleted msgs
        Log.d(TAG, "Deleted messages on server");
    }

    // ---------------------------------------------------------------------------------------------

    @Override
    public void onMessageSent (String syncId){
        /**
         * Called when an upstream message has been successfully sent to the GCM connection server.
         */
        Log.d(TAG, "Successfully sent upstream message. syncId= " + syncId);
    }

    // ---------------------------------------------------------------------------------------------

    @Override
    public void onSendError (String syncId, String error){
        /**
         * Called when there was an error sending an upstream message.
         */
        Log.e(TAG, "Error: " + error + " with syncId" + syncId);
    }
}
