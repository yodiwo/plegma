package com.yodiwo.androidnode;

import android.content.Context;
import android.util.Log;
import android.widget.Toast;

import com.google.gson.Gson;
import com.yodiwo.plegma.MqttMsg;
import com.yodiwo.plegma.PlegmaAPI;

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

import java.util.Timer;
import java.util.TimerTask;
import java.util.concurrent.atomic.AtomicInteger;


public class MqttServerAPI extends aServerAPI {
    // =============================================================================================
    // Static

    public static final String TAG = MqttServerAPI.class.getSimpleName();

    private static boolean connectionRetrying = false;

    private static final int RECONNECT_PERIOD = 2 * 1000; //2 sec

    private static AtomicInteger LastSyncId = new AtomicInteger();

    // Keep local global entry point for any request with Server.
    private static MqttServerAPI server = null;

    // get the instance of the settings
    // if we for some reason the instance is not valid create a new one.
    public static MqttServerAPI getInstance(Context context) {
        if (server == null) {
            server = new MqttServerAPI(context.getApplicationContext());
        }
        return server;
    }

    // =============================================================================================
    // Instance code

    private Gson gson = new Gson();

    public MqttServerAPI(Context context) {
        this.context = context;
        this.settingsProvider = SettingsProvider.getInstance(context);

        Log.d(TAG, "Starting MQTT server API.");

        RxActive = false;
        TxActive = false;
        RequestConnectivityUiUpdate();

        InitMqttClient();
    }

    // ---------------------------------------------------------------------------------------------

    private boolean _Send(String topic, Object mqtt_msg) {
        try {
            if (publish(topic, 2, gson.toJson(mqtt_msg)))
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
            MqttMsg mqttMsg = new MqttMsg(gson.toJson(msg), syncId, MqttMsg.Request);

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
            String topic = mqttPubTopicPrefix + PlegmaAPI.ApiMsgNames.get(msg.getClass());
            MqttMsg mqttMsg = new MqttMsg(gson.toJson(msg), syncId, MqttMsg.Response);

            if (_Send(topic, mqttMsg))
                return true;
        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
        return false;
    }

    @Override
    public boolean SendMsg(Object msg) {
        try {
            String topic = mqttPubTopicPrefix + PlegmaAPI.ApiMsgNames.get(msg.getClass());
            MqttMsg mqttMsg = new MqttMsg(gson.toJson(msg), 0, MqttMsg.Message); //async msgs do not need SyncId set

            if (_Send(topic, mqttMsg))
                return true;
        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
        return false;
    }

    // ---------------------------------------------------------------------------------------------

    private void _startRx() {
        if (!RxStarted) {
            // Init the subscriptions
            if (subscribe(mqttSubTopicPrefix + "#", 2 /* QOS */)) {
                RxEnabled = true;
                RequestConnectivityUiUpdate();

                RxStarted = true;
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
                mqttClient.unsubscribe(mqttSubTopicPrefix + "#");
                mqttClient.unregisterResources();
            } catch (MqttException e) {
                Helpers.logException(TAG, e);
            }
        }
    }

    // =============================================================================================
    // MQTT logic

    MqttAndroidClient mqttClient;
    MqttConnectOptions mqttOpt;
    ConnectionStatus connectionStatus = ConnectionStatus.NONE;
    boolean RxStarted = false;
    boolean RxEnabled = false;
    String mqttPubTopicPrefix;
    String mqttSubTopicPrefix;

    private void InitMqttClient() {

        // Create uri for mqtt and set for secure connection
        String uri = null;
        if (settingsProvider.getMqttUseSSL()) {
            uri = "ssl://";
        } else {
            uri = "tcp://";
        }
        uri += settingsProvider.getMqttAddress() + ":" + settingsProvider.getMqttPort();

        Log.d(TAG, "MQTT uri:" + uri);

        // Client ID is the node key
        mqttClient = new MqttAndroidClient(context, uri, settingsProvider.getNodeKey());
        mqttClient.setCallback(new MqttCallbackHandler(context));
        mqttClient.setTraceCallback(new MqttTraceCallback());
        mqttClient.setTraceEnabled(true);

        mqttOpt = new MqttConnectOptions();

        //If a previous session still exists, and cleanSession=true, then the previous session
        //information at the client and server is cleared. If cleanSession=false the previous session is resumed.
        mqttOpt.setCleanSession(false);

        // Use node key and secret key for authentication
        mqttOpt.setUserName(settingsProvider.getNodeKey());
        mqttOpt.setPassword(settingsProvider.getNodeSecretKey().toCharArray());

        mqttOpt.setConnectionTimeout(1000);
        mqttOpt.setKeepAliveInterval(60);

        // Define a topic prefix for this node
        mqttPubTopicPrefix = "/api/in/1/" + settingsProvider.getUserKey() + "/" + settingsProvider.getNodeKey() + "/";
        mqttSubTopicPrefix = "/api/out/1/" + settingsProvider.getNodeKey() + "/";

        Connect();
    }

    // ---------------------------------------------------------------------------------------------
    public void Connect() {
        try {
            connectionStatus = ConnectionStatus.CONNECTING;
            Log.d(TAG, "MQTT new status:" + connectionStatus);

            if (mqttClient != null)
                mqttClient.connect(mqttOpt, null, new MqttActionListener(context, MqttAction.CONNECT, null));
        } catch (MqttSecurityException e) {
            Helpers.logException(TAG, e);
        } catch (MqttException e) {
            Helpers.logException(TAG, e);
        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
    }

    // ---------------------------------------------------------------------------------------------
    @Override
    public void Teardown() {
//        mqttClient.close();
//        mqttClient = null;
        try {
            mqttClient.disconnect();
            //mqttClient.close();
            mqttClient = null;
            server = null;
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
        } catch (MqttSecurityException e) {
            Helpers.logException(TAG, e);
        } catch (MqttException e) {
            Helpers.logException(TAG, e);
        }
        return false;
    }

    private boolean publish(String topic, int qos, String message) {
        try {
            if(connectionStatus == ConnectionStatus.CONNECTED) {
                mqttClient.publish(topic,
                        message.getBytes(),
                        qos,
                        false, // retainer
                        null,
                        new MqttActionListener(context, MqttAction.PUBLISH, topic));
                return true;
            }
        } catch (MqttSecurityException e) {
            Helpers.logException(TAG, e);
        } catch (MqttException e) {
            Helpers.logException(TAG, e);
        }
        return false;
    }

    // =============================================================================================
    // MQTT Reconnection mechanism
    //
    private class ReconnectTask extends TimerTask {

        @Override
        public void run() {
            Log.i(TAG, "Trying to reconnect to MQTT");
            //Toast.makeText(context, "MQTT connection retry", Toast.LENGTH_SHORT).show();
            Connect();
            connectionRetrying = false;
        }
    }

    private void InitiateReconnectTry() {

        if (settingsProvider.getNodeKey() == null || settingsProvider.getNodeSecretKey() == null) {
            return;
        }
        if(!connectionRetrying) {
            connectionRetrying = true;
            Timer timer = new Timer();
            timer.schedule(new ReconnectTask(), RECONNECT_PERIOD);
        }
    }


    // =============================================================================================
    // MQTT Callback handler

    private class MqttCallbackHandler implements MqttCallback {
        private Context context;

        public MqttCallbackHandler(Context context) {
            this.context = context;
        }

        // -----------------------------------------------------------------------------------------
        @Override
        public void connectionLost(Throwable throwable) {
            if (throwable != null) {
                Log.e(TAG, "We have lost MQTT connection.");

                connectionStatus = ConnectionStatus.DISCONNECTED;

                //update UI
                RxActive = false;
                TxActive = false;
                RequestConnectivityUiUpdate();

                //send notification of disconnection to Node Service
                RxStarted = false;
                NodeService.ReceiveConnStatus(context, false);

                //Initiate reconnection procedure
                InitiateReconnectTry();
            }
        }

        // -----------------------------------------------------------------------------------------
        @Override
        public void messageArrived(String topic, MqttMessage mqttMessage) throws Exception {

            Log.i(TAG, "MQTT recv topic:" + topic);

            // Parse message
            if (mqttMessage != null) {
                //get string message
                String mqttMsg_string = new String(mqttMessage.getPayload());
                //convert to MqttMsg
                MqttMsg mqttMsg = gson.fromJson(mqttMsg_string, MqttMsg.class);

                String msgPayload = mqttMsg.Payload;
                int msgSyncId = mqttMsg.SyncId;
                int msgFlags = mqttMsg.Flags;

                Log.i(TAG, "MQTT qos:" + mqttMessage.getQos() + "type: " + msgFlags + " SyncID: " + msgSyncId + " payload:" + msgPayload);

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
                        Log.i(TAG, "MQTT send complete:" + topics[0]);
                    else
                        Log.i(TAG, "MQTT send complete:" + iMqttDeliveryToken.getMessage());
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
    enum ConnectionStatus {
        CONNECTING,
        CONNECTED,
        DISCONNECTING,
        DISCONNECTED,
        ERROR,
        NONE
    }

    enum MqttAction {
        CONNECT,
        DISCONNECT,
        SUBSCRIBE,
        PUBLISH
    }

    // =============================================================================================
    // MQTT action listener

    class MqttActionListener implements IMqttActionListener {
        MqttAction action;
        Context context;
        String additionalArgs;

        public MqttActionListener(Context context, MqttAction action, String additionalArgs) {
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
            connectionStatus = ConnectionStatus.CONNECTED;
            Log.d(TAG, "MQTT new status:" + connectionStatus);

            TxActive = true;
            RequestConnectivityUiUpdate();

            // check for pending subscription
            _startRx();

            NodeService.ReceiveConnStatus(context, true);
        }

        private void OnConnectFailed(Throwable exception) {
            connectionStatus = ConnectionStatus.ERROR;

            Toast.makeText(context, "MQTT connection failed", Toast.LENGTH_SHORT).show();

            Log.d(TAG, "MQTT new status:" + connectionStatus + " error:" + exception.getMessage());
            InitiateReconnectTry();
        }

        // -----------------------------------------------------------------------------------------
        private void _onDisconnected() {
            connectionStatus = ConnectionStatus.DISCONNECTED;

            Toast.makeText(context, "MQTT disconnected", Toast.LENGTH_SHORT).show();

            TxActive = false;
            RxActive = false;
            RequestConnectivityUiUpdate();

            RxStarted = false;
            NodeService.ReceiveConnStatus(context, false);
        }
        // -----------------------------------------------------------------------------------------
        private void OnDisconnected() {
            Log.d(TAG, "Successful MQTT disconnection by request");
            _onDisconnected();
        }

        private void OnDisconnected(Throwable exception) {
            Log.d(TAG, "Disconnection request failed with exception: " + exception.getMessage());

            //we tried to disconnect and failed? what does this even mean?
            _onDisconnected();
        }

        // -----------------------------------------------------------------------------------------
        private void OnSubscribed() {
            Log.i(TAG, "Successful subscribe to " + additionalArgs + ".");

            RxActive = true;
            RequestConnectivityUiUpdate();
            NodeService.RequestUpdatedState(context);
        }

        private void OnSubscribed(Throwable exception) {
            Log.i(TAG, "Failed to subscribe to " + additionalArgs + ".");

            // If we failed to subscribe rx path set the variable
            if (additionalArgs.startsWith(mqttSubTopicPrefix)) {
                RxStarted = false;
            }
        }

        // -----------------------------------------------------------------------------------------
        private void OnPublish() {
            Log.i(TAG, "Successful publish to " + additionalArgs + ".");
        }

        private void OnPublish(Throwable exception) {
            Log.e(TAG, "Failed to publish to " + additionalArgs + ".");

            //TxActive = false; //check...
        }
        // -----------------------------------------------------------------------------------------
    }

    // =============================================================================================
    // MQTT trace callback

    private class MqttTraceCallback implements MqttTraceHandler {

        public void traceDebug(java.lang.String arg0, java.lang.String arg1) {
            Log.i(arg0, arg1);
        }

        public void traceError(java.lang.String arg0, java.lang.String arg1) {
            Log.e(arg0, arg1);
        }

        public void traceException(java.lang.String arg0, java.lang.String arg1,
                                   java.lang.Exception arg2) {
            Log.e(arg0, arg1, arg2);
        }
    }
}
