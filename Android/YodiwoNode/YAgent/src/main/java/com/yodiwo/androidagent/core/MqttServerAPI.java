package com.yodiwo.androidagent.core;

import android.content.Context;
import android.net.ConnectivityManager;
import android.util.Log;
import android.widget.Toast;

import com.google.gson.Gson;
import com.yodiwo.androidagent.plegma.MqttMsg;
import com.yodiwo.androidagent.plegma.PlegmaAPI;
import com.yodiwo.androidagent.plegma.eMsgFlags;

import org.eclipse.paho.android.service.MqttAndroidClient;
import org.eclipse.paho.android.service.MqttTraceHandler;
import org.eclipse.paho.client.mqttv3.IMqttActionListener;
import org.eclipse.paho.client.mqttv3.IMqttDeliveryToken;
import org.eclipse.paho.client.mqttv3.IMqttToken;
import org.eclipse.paho.client.mqttv3.MqttCallback;
import org.eclipse.paho.client.mqttv3.MqttConnectOptions;
import org.eclipse.paho.client.mqttv3.MqttException;
import org.eclipse.paho.client.mqttv3.MqttMessage;
import org.eclipse.paho.client.mqttv3.MqttSecurityException;

import java.util.HashSet;
import java.util.Timer;
import java.util.TimerTask;
import java.util.concurrent.atomic.AtomicInteger;


class MqttServerAPI extends aServerAPI {
    // =============================================================================================
    // Static

    public static final String TAG = MqttServerAPI.class.getSimpleName();

    private static final int RECONNECT_PERIOD = 5 * 1000; //2 sec

    private boolean connectionRetrying = false;

    private AtomicInteger LastSyncId = new AtomicInteger();

    // Keep local global entry point for any request with Server.
    private static MqttServerAPI server = null;

    private static HashSet<String> subscribedTopics = new HashSet<>();

    // get the instance of the settings
    // if for some reason the instance is not valid create a new one.
    public static MqttServerAPI getInstance(Context context) {
        if (server == null) {
            server = new MqttServerAPI(context.getApplicationContext());
        }
        return server;
    }

    // =============================================================================================
    // Instance code

    private Gson gson = new Gson();

    private MqttServerAPI(Context context) {
        this.context = context;
        this.settingsProvider = SettingsProvider.getInstance(context);

        Helpers.log(Log.DEBUG, TAG, "Starting MQTT server API.");

        RxActive = false;
        TxActive = false;
        RequestConnectivityUiUpdate();

        InitMqttClient();
    }

    // ---------------------------------------------------------------------------------------------

    private boolean _Send(String topic, Object mqtt_msg) {
        return _Send(topic, mqtt_msg, 2);
    }

    private boolean _Send(String topic, Object mqtt_msg, int qos) {
        try {
            if (publish(topic, qos, gson.toJson(mqtt_msg)))
                return true;
        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
        return false;
    }

    // =============================================================================================
    // Public API

    @Override
    public boolean SendReq(Object msg) {
        try {
            String topic = mqttPubTopicPrefix + PlegmaAPI.ApiMsgNames.get(msg.getClass());
            int syncId = LastSyncId.incrementAndGet();
            MqttMsg mqttMsg = new MqttMsg(gson.toJson(msg), syncId, eMsgFlags.Request);

            if (_Send(topic, mqttMsg))
                return true;
        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
        return false;
    }

    @Override
    public boolean SendRsp(Object msg, int syncId) {
        try {
            String topic;
            MqttMsg mqttMsg;

            if (msg != null) {
                //create wrapper for response and find topic string from msg class
                topic = mqttPubTopicPrefix + PlegmaAPI.ApiMsgNames.get(msg.getClass());
                mqttMsg = new MqttMsg(gson.toJson(msg), syncId, eMsgFlags.Response);
            } else {
                //send a dummy response (to unblock potentially blocked server threads)
                topic = mqttPubTopicPrefix + PlegmaAPI.s_UnknownRsp;
                mqttMsg = new MqttMsg("", syncId, eMsgFlags.Response);
            }
            if (_Send(topic, mqttMsg))
                return true;
        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
        return false;
    }

    @Override
    public boolean SendMsg(Object msg) {
        return SendMsg(msg, 2);
    }

    boolean SendMsg(Object msg, int qos) {
        try {
            String prefix = qos == 0 ? mqttQosZeroPubTopicPrefix : mqttPubTopicPrefix;
            String topic = prefix + PlegmaAPI.ApiMsgNames.get(msg.getClass());
            MqttMsg mqttMsg = new MqttMsg(gson.toJson(msg), 0, eMsgFlags.Message); //async msgs do not need SyncId set

            if (_Send(topic, mqttMsg, qos))
                return true;
        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
        return false;
    }
    // ---------------------------------------------------------------------------------------------

    private void _startRx() {
        if (!RxStarted) {
            if (!subscribedTopics.contains(mqttSubTopicPrefix + "#") ) {
                // Init the subscriptions
                if (subscribe(mqttSubTopicPrefix + "#", 2 /* QOS */)) {
                    Helpers.log(Log.VERBOSE, TAG, "requested subscription to " + mqttSubTopicPrefix + "#");
                }
            }
            mqttClient.registerResources(context);
        }
    }

    @Override
    public void StartRx() {
        if (connectionStatus == ConnectionStatus.CONNECTED) {
            _startRx();
        }
    }

    // ---------------------------------------------------------------------------------------------

    @Override
    public void StopRx() {
        RxEnabled = false;
        RequestConnectivityUiUpdate();

        // Unsubscribe
        if (RxStarted) {
            RxStarted = false;

            try {
                subscribedTopics.remove(mqttSubTopicPrefix + "#");
                mqttClient.unsubscribe(mqttSubTopicPrefix + "#");
                mqttClient.unregisterResources();
            } catch (MqttException e) {
                Helpers.logException(TAG, e);
            }
        }
    }

    // =============================================================================================
    // MQTT logic

    private MqttAndroidClient mqttClient;
    private MqttConnectOptions mqttOpt;
    private ConnectionStatus connectionStatus = ConnectionStatus.NONE;
    private boolean RxStarted = false;
    private boolean RxEnabled = false;
    private String mqttPubTopicPrefix;
    private String mqttQosZeroPubTopicPrefix;
    private String mqttSubTopicPrefix;

    private void InitMqttClient() {

        // Create uri for mqtt and set for secure connection
        String uri;
        if (settingsProvider.getMqttUseSSL()) {
            uri = "ssl://" + settingsProvider.getMqttAddress() + ":8883";
        } else {
            uri = "tcp://" + settingsProvider.getMqttAddress() + ":1883";
        }

        Helpers.log(Log.DEBUG, TAG, "MQTT uri:" + uri);

        // Client ID is the node key
        mqttClient = new MqttAndroidClient(context, uri, settingsProvider.getNodeKey());
        mqttClient.setCallback(new MqttCallbackHandler(context));
        mqttClient.setTraceCallback(new MqttTraceCallback());
        mqttClient.setTraceEnabled(true);

        mqttOpt = new MqttConnectOptions();

        //If a previous session still exists, and cleanSession=true, then the previous session
        //information at the client and server is cleared. If cleanSession=false the previous session is resumed.
        mqttOpt.setCleanSession(true);

        //mqttOpt.setConnectionTimeout(1000);
        //mqttOpt.setKeepAliveInterval(0);

        Connect();
    }

    // ---------------------------------------------------------------------------------------------
    @Override
    public void Connect() {
        try {
            //check that we are paired
            String ukey = settingsProvider.getUserKey();
            String nkey = settingsProvider.getNodeKey();
            String skey = settingsProvider.getNodeSecretKey();

            if (ukey == null || nkey == null || skey == null) {
                return;
            }

            //check that we aren't already connected or trying to connect
            if (mqttClient == null
                    || mqttClient.isConnected()
                    || connectionStatus == ConnectionStatus.CONNECTING
                    || connectionStatus == ConnectionStatus.CONNECTED) {
                return;
            }
            connectionStatus = ConnectionStatus.CONNECTING;
            Helpers.log(Log.DEBUG, TAG, "MQTT new status:" + connectionStatus);

            subscribedTopics.clear();

            // Use node key and secret key for authentication
            mqttOpt.setUserName(nkey);
            mqttOpt.setPassword(skey.toCharArray());

            // Define a topic prefix for this node
            mqttPubTopicPrefix = "/api/in/1/" + ukey + "/" + nkey + "/";
            mqttQosZeroPubTopicPrefix = "/api_qos0/in/1/" + ukey + "/" + nkey + "/";
            mqttSubTopicPrefix = "/api/out/1/" + nkey + "/";

            //attempt MQTT connection
            mqttClient.connect(mqttOpt, null, new MqttActionListener(context, MqttAction.CONNECT, null));

        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
    }

    // ---------------------------------------------------------------------------------------------
    @Override
    public void Teardown() {
        try {
            if (reconnectTimer != null){
                reconnectTimer.cancel();
                reconnectTimer.purge();
                reconnectTimer = null;
                Helpers.log(Log.INFO, TAG, "Reconnect timer cancelled");
            }

            if (mqttClient != null) {
                //mqttClient.close();
                mqttClient.disconnect(null, new MqttActionListener(context, MqttAction.DISCONNECT, null));
                mqttClient = null;
                server = null;
            }
        } catch (MqttException e) {
            Helpers.logException(TAG, e);
        }
    }
    // ---------------------------------------------------------------------------------------------

    private boolean subscribe(String topic, int qos) {
        try {
            mqttClient.subscribe(topic,
                    qos,
                    null,
                    new MqttActionListener(context, MqttAction.SUBSCRIBE, topic));
            return true;
        } catch (MqttException e) {
            Helpers.logException(TAG, e);
        }
        return false;
    }

    private boolean publish(String topic, int qos, String message) {
        try {
            if (connectionStatus == ConnectionStatus.CONNECTED) {
                mqttClient.publish(topic,
                        message.getBytes(),
                        qos,
                        false, // retainer
                        null,
                        new MqttActionListener(context, MqttAction.PUBLISH, topic));
                return true;
            }
        } catch (MqttException e) {
            Helpers.logException(TAG, e);
        }
        return false;
    }

    // =============================================================================================
    // MQTT Reconnection mechanism
    //
    private static Timer reconnectTimer;

    private class ReconnectTask extends TimerTask {

        @Override
        public void run() {
            Helpers.log(Log.INFO, TAG, "Trying to reconnect to MQTT");
            InitMqttClient();
            connectionRetrying = false;
        }
    }

    private void InitiateReconnectTry() {

        if (settingsProvider.getNodeKey() == null || settingsProvider.getNodeSecretKey() == null) {
            return;
        }

        if (PairingService.PairingStatus.UNPAIRED == PairingService.getPairingStatus()){
            return;
        }

        // return if no network connection
        ConnectivityManager cm = (ConnectivityManager) context.getSystemService(Context.CONNECTIVITY_SERVICE);
        if (cm != null && (cm.getActiveNetworkInfo() == null || !cm.getActiveNetworkInfo().isConnected())) {
            Toast.makeText(context, "No network connection", Toast.LENGTH_SHORT).show();
            return;
        }
        if (!connectionRetrying) {
            connectionRetrying = true;

            Toast.makeText(context, "Trying to connect to Yodiwo cloud", Toast.LENGTH_SHORT).show();

            Helpers.log(Log.INFO, TAG, "MQTT connection retry in " + RECONNECT_PERIOD + " msec");

            try {
                // if timerTask is cancelled create a new one
                if (reconnectTimer == null)
                    reconnectTimer = new Timer();

                reconnectTimer.schedule(new ReconnectTask(), RECONNECT_PERIOD);
            } catch(Exception ex){
                Helpers.logException(TAG, ex);
            }
        }
    }


    // =============================================================================================
    // MQTT Callback handler

    private class MqttCallbackHandler implements MqttCallback {
        private Context context;

        MqttCallbackHandler(Context context) {
            this.context = context;
        }

        // -----------------------------------------------------------------------------------------
        @Override
        public void connectionLost(Throwable throwable) {
            if (throwable != null) {
                if (connectionStatus != ConnectionStatus.DISCONNECTED) {
                    Helpers.log(Log.ERROR, TAG, "We have lost MQTT connection:  " + throwable.getMessage());
                    connectionStatus = ConnectionStatus.DISCONNECTED;

                    //update UI
                    RxActive = false;
                    TxActive = false;
                    RequestConnectivityUiUpdate();

                    //send notification of disconnection to Node Service
                    StopRx();
                    NodeService.SetCloudConnStatus(context, false);

                    //Initiate reconnection procedure
                    InitiateReconnectTry();
                }
            }
        }

        // -----------------------------------------------------------------------------------------
        @Override
        public void messageArrived(String topic, MqttMessage mqttMessage) throws Exception {

            Helpers.log(Log.INFO, TAG, "MQTT recv topic:" + topic);

            // Parse message
            if (mqttMessage != null) {
                //get string message
                String mqttMsg_string = new String(mqttMessage.getPayload());
                //convert to MqttMsg
                MqttMsg mqttMsg = gson.fromJson(mqttMsg_string, MqttMsg.class);

                String msgPayload = mqttMsg.Payload;
                int msgSyncId = mqttMsg.SyncId;
                int msgFlags = mqttMsg.Flags;

                Helpers.log(Log.INFO, TAG, "MQTT qos:" + mqttMessage.getQos() + "type: " + msgFlags + " SyncID: " + msgSyncId + " payload:" + msgPayload);

                // Remove the topic prefix to get the message type
                String msgType = topic.replace(mqttSubTopicPrefix, "");

                // Send the message to node service
                NodeService.RxMsg(context, msgType, msgPayload, msgSyncId, msgFlags);
            }
        }

        // -----------------------------------------------------------------------------------------
        @Override
        public void deliveryComplete(IMqttDeliveryToken iMqttDeliveryToken) {
            try {
                if (iMqttDeliveryToken != null) {
                    String[] topics = iMqttDeliveryToken.getTopics();
                    if (topics != null)
                        Helpers.log(Log.INFO, TAG, "MQTT send complete:" + topics[0]);
                    else
                        Helpers.log(Log.INFO, TAG, "MQTT send complete:" + iMqttDeliveryToken.getMessage());
                }
            } catch (Exception e) {
                Helpers.logException(TAG, e);
            }
        }
        // -----------------------------------------------------------------------------------------
    }

    // =============================================================================================
    // MQTT enums

    /**
     * Connections status for  a connection
     */
    private enum ConnectionStatus {
        CONNECTING,
        CONNECTED,
        DISCONNECTING,
        DISCONNECTED,
        ERROR,
        NONE
    }

    private enum MqttAction {
        CONNECT,
        DISCONNECT,
        SUBSCRIBE,
        PUBLISH
    }

    // =============================================================================================
    // MQTT action listener

    private class MqttActionListener implements IMqttActionListener {
        MqttAction action;
        Context context;
        String additionalArgs;

        MqttActionListener(Context context, MqttAction action, String additionalArgs) {
            this.action = action;
            this.context = context;
            this.additionalArgs = additionalArgs;
        }

        // -----------------------------------------------------------------------------------------
        @Override
        public void onSuccess(IMqttToken iMqttToken) {
            switch (action) {
                case CONNECT:
                    OnConnected();
                    break;
                case DISCONNECT:
                    OnDisconnected();
                    break;
                case SUBSCRIBE:
                    OnSubscribed();
                    break;
                case PUBLISH:
                    OnPublish();
                    break;
            }
        }

        // -----------------------------------------------------------------------------------------
        @Override
        public void onFailure(IMqttToken iMqttToken, Throwable exception) {
            switch (action) {
                case CONNECT:
                    OnConnectFailed(exception);
                    break;
                case DISCONNECT:
                    OnDisconnected(exception);
                    break;
                case SUBSCRIBE:
                    OnSubscribed(exception);
                    break;
                case PUBLISH:
                    OnPublish(exception);
                    break;
            }
        }

        // -----------------------------------------------------------------------------------------
        // Internal functions for status
        private void OnConnected() {
            if (connectionStatus != ConnectionStatus.CONNECTED) {

                connectionStatus = ConnectionStatus.CONNECTED;
                Helpers.log(Log.DEBUG, TAG, "MQTT new status:" + connectionStatus);

                RxActive = true;
                TxActive = true;
                RequestConnectivityUiUpdate();

                // check for pending subscription
                StartRx();

                NodeService.SetCloudConnStatus(context, true);
            }
        }

        private void OnConnectFailed(Throwable exception) {
            if (connectionStatus != ConnectionStatus.ERROR) {
                connectionStatus = ConnectionStatus.ERROR;

                //update UI
                RxActive = false;
                TxActive = false;
                RequestConnectivityUiUpdate();

                Toast.makeText(context, "Connection to Yodiwo cloud failed", Toast.LENGTH_SHORT).show();

                Helpers.log(Log.DEBUG, TAG, "MQTT new status:" + connectionStatus + " error:" + exception.getMessage());
                InitiateReconnectTry();
            }
        }

        // -----------------------------------------------------------------------------------------
        private void _onDisconnected() {
            connectionStatus = ConnectionStatus.DISCONNECTED;

            //Toast.makeText(context, "MQTT disconnected", Toast.LENGTH_SHORT).show();

            TxActive = false;
            RxActive = false;
            RequestConnectivityUiUpdate();

            RxStarted = false;
            NodeService.SetCloudConnStatus(context, false);
        }

        // -----------------------------------------------------------------------------------------
        private void OnDisconnected() {
            Helpers.log(Log.DEBUG, TAG, "Successful MQTT disconnection by request");
            _onDisconnected();
        }

        private void OnDisconnected(Throwable exception) {
            Helpers.log(Log.DEBUG, TAG, "Disconnection request failed with exception: " + exception.getMessage());

            //we tried to disconnect and failed? what does this even mean?
            _onDisconnected();
        }

        // -----------------------------------------------------------------------------------------
        private void OnSubscribed() {
            Helpers.log(Log.INFO, TAG, "Successful subscribe to " + additionalArgs + ".");

            RxStarted = true;
            subscribedTopics.add(additionalArgs);

            TxActive = true;
            RxActive = true;
            RequestConnectivityUiUpdate();
        }

        private void OnSubscribed(Throwable exception) {
            Helpers.log(Log.INFO, TAG, "Failed to subscribe to " + additionalArgs + ".");

            // If we failed to subscribe rx path set the variable
            if (additionalArgs.startsWith(mqttSubTopicPrefix)) {
                RxStarted = false;
            }
        }

        // -----------------------------------------------------------------------------------------
        private void OnPublish() {
            Helpers.log(Log.INFO, TAG, "Successful publish to " + additionalArgs + ".");
        }

        private void OnPublish(Throwable exception) {
            Helpers.log(Log.ERROR, TAG, "Failed to publish to " + additionalArgs + ".");

            //TxActive = false; //check...
        }
        // -----------------------------------------------------------------------------------------
    }

    // =============================================================================================
    // MQTT trace callback

    private class MqttTraceCallback implements MqttTraceHandler {

        public void traceDebug(String arg0, String arg1) {
            Log.i(arg0, arg1);
        }

        public void traceError(String arg0, String arg1) {
            Log.e(arg0, arg1);
        }

        public void traceException(String arg0, String arg1,
                                   Exception arg2) {
            Log.e(arg0, arg1, arg2);
        }
    }
}
