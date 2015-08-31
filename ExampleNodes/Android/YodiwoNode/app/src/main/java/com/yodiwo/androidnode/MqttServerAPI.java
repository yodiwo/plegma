package com.yodiwo.androidnode;

import android.content.Context;
import android.util.Log;

import com.google.gson.Gson;
import com.yodiwo.plegma.MqttAPIMessage;
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


public class MqttServerAPI implements IServerAPI {
    // =============================================================================================
    // Static

    public static final String TAG = MqttServerAPI.class.getSimpleName();


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

    private Context context;
    private SettingsProvider settingsProvider;
    private Gson gson = new Gson();

    public MqttServerAPI(Context context) {
        this.context = context;
        settingsProvider = SettingsProvider.getInstance(context);

        Log.d(TAG, "Starting MQTT server API.");

        InitMqttClient();
    }

    // ---------------------------------------------------------------------------------------------

    private boolean _Send(String topic, Object mqtt_msg) {
        try {
            if (publish(topic, 2, gson.toJson(mqtt_msg)))
                return true;
        } catch (Exception ex) {
            Log.e(TAG, ex.getMessage());
        }
        return false;
    }

    // =============================================================================================
    // Public API


    @Override
    public boolean SendRsp(Object msg, int RespToSeqNo) {
        try {
            String topic = mqttPubTopicPrefix + PlegmaAPI.ApiMsgNames.get(msg.getClass());
            MqttAPIMessage mqttAPIMessage = new MqttAPIMessage(RespToSeqNo, gson.toJson(msg));

            if (_Send(topic, mqttAPIMessage))
                return true;
        } catch (Exception ex) {
            Log.e(TAG, ex.getMessage());
        }
        return false;
    }

    @Override
    public boolean Send(Object msg) {
        try {
            String topic = mqttPubTopicPrefix + PlegmaAPI.ApiMsgNames.get(msg.getClass());
            MqttAPIMessage mqttAPIMessage = new MqttAPIMessage(0, gson.toJson(msg));

            if (_Send(topic, mqttAPIMessage))
                return true;
        } catch (Exception ex) {
            Log.e(TAG, ex.getMessage());
        }
        return false;
    }

    // ---------------------------------------------------------------------------------------------

    private void _startRx() {
        if (!RxStarted) {
            // Init the subscriptions
            if (subscribe(mqttSubTopicPrefix + "#", 2 /* QOS */))
                RxStarted = true;
            mqttClient.registerResources(context);
        }
    }

    @Override
    public void StartRx() {
        RxEnabled = true;
        if (connectionStatus == ConnectionStatus.CONNECTED) {
            _startRx();
        }
    }

    // ---------------------------------------------------------------------------------------------

    @Override
    public void StopRx() {
        RxEnabled = false;
        // Unsubscripe
        if (RxStarted) {
            mqttClient.unregisterResources();
            RxStarted = false;

            // TODO: Stop RX.....
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

        // Create uri for mqtt and set for secure conenction
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
        mqttOpt.setKeepAliveInterval(10);

        // Define a topic prefix for this node
        mqttPubTopicPrefix = "/api/in/" + settingsProvider.getUserKey() + "/" + settingsProvider.getNodeKey() + "/";
        mqttSubTopicPrefix = "/api/out/" + settingsProvider.getNodeKey() + "/";

        Connect();
    }

    // ---------------------------------------------------------------------------------------------
    public void Connect() {
        try {
            connectionStatus = ConnectionStatus.CONNECTING;
            Log.d(TAG, "MQTT new status:" + connectionStatus);

            mqttClient.connect(mqttOpt, null, new MqttActionListener(context, MqttAction.CONNECT, null));
        } catch (MqttSecurityException e) {
            Log.e(this.getClass().getCanonicalName(), "Failed to Connect the mqtt client", e);
        } catch (MqttException e) {
            Log.e(this.getClass().getCanonicalName(), "Failed to Connect the mqtt client", e);
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
            Log.e(this.getClass().getCanonicalName(), "Failed to subscribe to" + topic, e);
        } catch (MqttException e) {
            Log.e(this.getClass().getCanonicalName(), "Failed to subscribe to" + topic, e);
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
            Log.e(this.getClass().getCanonicalName(), "Failed to publish a message", e);
        } catch (MqttException e) {
            Log.e(this.getClass().getCanonicalName(), "Failed to publish a message", e);
        }
        return false;
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
                connectionStatus = ConnectionStatus.DISCONNECTED;

                Log.e(TAG, "We have lost MQTT connection.");

                // if we disconnected kill and the rx path
                RxStarted = false;
                NodeService.ReceiveConnStatus(context, false);
            }
        }

        // -----------------------------------------------------------------------------------------
        @Override
        public void messageArrived(String topic, MqttMessage mqttMessage) throws Exception {

            Log.i(TAG, "MQTT recv topic:" + topic);
            if (mqttMessage != null) {
                // Parse message
                String mqttMsg = new String(mqttMessage.getPayload());
                String apiMsg  = gson.fromJson(mqttMsg, MqttAPIMessage.class).Payload;
                Log.i(TAG, "MQTT qos:" + mqttMessage.getQos() + " payload:" + apiMsg);

                // Remove the preffix
                String msgType = topic.replace(mqttSubTopicPrefix, "");

                // Send the message to node service
                NodeService.RxMsg(context, msgType, apiMsg);
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
            } catch (Exception ex) {
                Log.e(TAG, "MQTT deliveryComplete Error:" + ex.getMessage());
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
                    connect();
                    break;
                case DISCONNECT:
                    disconnect();
                    break;
                case SUBSCRIBE:
                    subscribe();
                    break;
                case PUBLISH:
                    publish();
                    break;
            }
        }

        // -----------------------------------------------------------------------------------------
        @Override
        public void onFailure(IMqttToken iMqttToken, Throwable exception) {
            switch (action) {
                case CONNECT:
                    connect(exception);
                    break;
                case DISCONNECT:
                    disconnect(exception);
                    break;
                case SUBSCRIBE:
                    subscribe(exception);
                    break;
                case PUBLISH:
                    publish(exception);
                    break;
            }
        }

        // -----------------------------------------------------------------------------------------
        // Internal functions for status
        private void connect() {
            connectionStatus = ConnectionStatus.CONNECTED;
            Log.d(TAG, "MQTT new status:" + connectionStatus);

            NodeService.ReceiveConnStatus(context, true);
            // check for pending subscription
            if (RxEnabled) {
                _startRx();
            }
        }

        private void connect(Throwable exception) {
            connectionStatus = ConnectionStatus.ERROR;

            Log.d(TAG, "MQTT new status:" + connectionStatus + " error:" + exception.getMessage());
        }

        // -----------------------------------------------------------------------------------------
        private void disconnect() {
            connectionStatus = ConnectionStatus.DISCONNECTED;
            Log.d(TAG, "MQTT new status:" + connectionStatus);

            // if we disconnected kill and the rx path
            RxStarted = false;
            NodeService.ReceiveConnStatus(context, false);
        }

        private void disconnect(Throwable exception) {
            connectionStatus = ConnectionStatus.DISCONNECTED;
            Log.d(TAG, "MQTT new status:" + connectionStatus);

            // if we disconnected kill and the rx path
            RxStarted = false;
            NodeService.ReceiveConnStatus(context, false);
        }

        // -----------------------------------------------------------------------------------------
        private void subscribe() {
            Log.e(TAG, "Success subscribe to " + additionalArgs + ".");
        }

        private void subscribe(Throwable exception) {
            Log.e(TAG, "Failed to subscribe to " + additionalArgs + ".");

            // If we failed to subscribe rx path set the variable
            if (additionalArgs.startsWith(mqttSubTopicPrefix)) {
                RxStarted = false;
            }
        }

        // -----------------------------------------------------------------------------------------
        private void publish() {
            Log.e(TAG, "Success publish to " + additionalArgs + ".");
        }

        private void publish(Throwable exception) {
            Log.e(TAG, "Failed to publish to " + additionalArgs + ".");
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
