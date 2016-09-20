package com.yodiwo.androidagent.core;


import android.app.IntentService;
import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.app.TaskStackBuilder;
import android.content.Context;
import android.content.Intent;
import android.graphics.BitmapFactory;
import android.media.RingtoneManager;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.support.v4.content.LocalBroadcastManager;
import android.support.v7.app.NotificationCompat;
import android.util.Log;
import android.widget.Toast;

import com.google.gson.Gson;
import com.yodiwo.androidagent.R;
import com.yodiwo.androidagent.plegma.ActivePortKeysMsg;
import com.yodiwo.androidagent.plegma.AddFriendReq;
import com.yodiwo.androidagent.plegma.BinaryResourceDescriptor;
import com.yodiwo.androidagent.plegma.BinaryResourceDescriptorKey;
import com.yodiwo.androidagent.plegma.GenericRsp;
import com.yodiwo.androidagent.plegma.GetFriendsReq;
import com.yodiwo.androidagent.plegma.GetFriendsRsp;
import com.yodiwo.androidagent.plegma.GetNodeInfoReq;
import com.yodiwo.androidagent.plegma.GetNodeInfoRsp;
import com.yodiwo.androidagent.plegma.GetThingsReq;
import com.yodiwo.androidagent.plegma.GetThingsRsp;
import com.yodiwo.androidagent.plegma.GetUserInfoReq;
import com.yodiwo.androidagent.plegma.GetUserInfoRsp;
import com.yodiwo.androidagent.plegma.GetUserTokensReq;
import com.yodiwo.androidagent.plegma.GetUserTokensRsp;
import com.yodiwo.androidagent.plegma.GraphActionReq;
import com.yodiwo.androidagent.plegma.GraphActionRsp;
import com.yodiwo.androidagent.plegma.HttpLocationDescriptor;
import com.yodiwo.androidagent.plegma.LiveKeysAddMsg;
import com.yodiwo.androidagent.plegma.LiveKeysDelMsg;
import com.yodiwo.androidagent.plegma.LiveValuesMsg;
import com.yodiwo.androidagent.plegma.MyHackingReq;
import com.yodiwo.androidagent.plegma.MyHackingRsp;
import com.yodiwo.androidagent.plegma.NodeInfoReq;
import com.yodiwo.androidagent.plegma.NodeInfoRsp;
import com.yodiwo.androidagent.plegma.NodeUnpairedReq;
import com.yodiwo.androidagent.plegma.PlegmaAPI;
import com.yodiwo.androidagent.plegma.Port;
import com.yodiwo.androidagent.plegma.PortEvent;
import com.yodiwo.androidagent.plegma.PortEventMsg;
import com.yodiwo.androidagent.plegma.PortKey;
import com.yodiwo.androidagent.plegma.PortState;
import com.yodiwo.androidagent.plegma.PortStateGet;
import com.yodiwo.androidagent.plegma.PortStateSet;
import com.yodiwo.androidagent.plegma.ShareActionReq;
import com.yodiwo.androidagent.plegma.ShareActionRsp;
import com.yodiwo.androidagent.plegma.ShareNotification;
import com.yodiwo.androidagent.plegma.ShareNotifyMsg;
import com.yodiwo.androidagent.plegma.ShareThingsReq;
import com.yodiwo.androidagent.plegma.ShareThingsRsp;
import com.yodiwo.androidagent.plegma.Thing;
import com.yodiwo.androidagent.plegma.ThingKey;
import com.yodiwo.androidagent.plegma.ThingsGet;
import com.yodiwo.androidagent.plegma.ThingsSet;
import com.yodiwo.androidagent.plegma.eBinaryResourceContentType;
import com.yodiwo.androidagent.plegma.eBinaryResourceLocationType;
import com.yodiwo.androidagent.plegma.eMsgFlags;
import com.yodiwo.androidagent.plegma.eNodeCapa;
import com.yodiwo.androidagent.plegma.eNodeType;
import com.yodiwo.androidagent.plegma.ePortStateOperation;
import com.yodiwo.androidagent.plegma.ePortType;
import com.yodiwo.androidagent.plegma.eRestServiceType;

import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.BufferedReader;
import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Collections;
import java.util.HashMap;
import java.util.HashSet;
import java.util.concurrent.atomic.AtomicInteger;
import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantReadWriteLock;

import javax.net.ssl.HttpsURLConnection;


public class NodeService extends IntentService {

    // =============================================================================================
    // Static information
    // =============================================================================================

    public static final String TAG = NodeService.class.getSimpleName();

    /**
     * Extras
     */
    private static final String EXTRA_REQUEST_TYPE = "EXTRA_REQUEST_TYPE";
    private static final String EXTRA_STATUS = "EXTRA_STATUS";
    private static final String EXTRA_THING = "EXTRA_THING";
    private static final String EXTRA_THING_NAME = "EXTRA_THING_NAME";
    private static final String EXTRA_PORT_INDEX = "EXTRA_PORT_INDEX";
    private static final String EXTRA_PORT_DATA = "EXTRA_PORT_DATA";
    private static final String EXTRA_PORT_DATA_ARRAY = "EXTRA_PORT_DATA_ARRAY";
    private static final String EXTRA_QOS = "EXTRA_QOS";
    private static final String EXTRA_SERVICE_TYPE = "EXTRA_SERVICE_TYPE";
    public static final String EXTRA_UPDATED_THING_KEY = "EXTRA_UPDATED_THING_KEY";
    public static final String EXTRA_UPDATED_THING_NAME = "EXTRA_UPDATED_THING_NAME";
    public static final String EXTRA_UPDATED_PORT_ID = "EXTRA_UPDATED_PORT_ID";
    public static final String EXTRA_UPDATED_STATE = "EXTRA_UPDATED_STATE";
    // This indicates whether the update is a new event or a starting point
    public static final String EXTRA_UPDATED_IS_EVENT = "EXTRA_UPDATED_IS_EVENT";
    public static final String EXTRA_UPDATED_ACTIVE_THINGS_KEYS = "EXTRA_UPDATED_ACTIVE_THINGS_KEYS";
    public static final String EXTRA_UPDATED_ACTIVE_PORT_KEYS = "EXTRA_UPDATED_ACTIVE_PORT_KEYS";
    // This indicates whether this is and an internally-generated event, contrast to external
    // PortEvent messages, PortState requests
    public static final String EXTRA_IS_INTERNAL_EVENT = "EXTRA_IS_INTERNAL_EVENT";
    public static final String EXTRA_NODE_UNPAIRING_INFO = "EXTRA_NODE_UNPAIRING_INFO";

    private static final String EXTRA_RX_TOPIC = "EXTRA_REQUEST_TOPIC";
    private static final String EXTRA_RX_MSG_PAYLOAD = "EXTRA_REQUEST_MSG_PAYLOAD";
    private static final String EXTRA_RX_MSG_SYNC_ID = "EXTRA_REQUEST_MSG_SYNCID";
    private static final String EXTRA_RX_MSG_FLAGS = "EXTRA_REQUEST_MSG_FLAGS";

    private static final String EXTRA_FILE_PATH = "EXTRA_FILE_PATH";
    private static final String EXTRA_URI = "EXTRA_URI";
    public static final String EXTRA_FILE_DOWNLOADED = "EXTRA_FILE_DOWNLOADED";

    private static final String EXTRA_NOTIFICATION_ID = "EXTRA_NOTIFICATION_ID";

    private static final String EXTRA_SHARE_ACTION_REQ = "EXTRA_SHARE_ACTION_REQ";
    private static final String EXTRA_GET_USER_TOKENS_REQ = "EXTRA_GET_USER_TOKENS_REQ";
    private static final String EXTRA_GET_USER_INFO_REQ = "EXTRA_GET_USER_INFO_REQ";
    private static final String EXTRA_GET_FRIENDS_REQ = "EXTRA_GET_FRIENDS_REQ";
    private static final String EXTRA_GET_THINGS_REQ = "EXTRA_GET_THINGS_REQ";
    private static final String EXTRA_GRAPH_ACTION_REQ = "EXTRA_GRAPH_ACTION_REQ";
    private static final String EXTRA_GET_NODE_INFO_REQ = "EXTRA_GET_NODE_INFO_REQ";
    private static final String EXTRA_ADD_FRIEND_REQ = "EXTRA_ADD_FRIEND_REQ";
    private static final String EXTRA_MYHACKING_REQ = "EXTRA_MYHACKING_REQ";
    public static final String EXTRA_SHARE_ACTION_RSP = "EXTRA_SHARE_ACTION_RSP";
    public static final String EXTRA_SHARE_NOTIFICATION = "EXTRA_SHARE_NOTIFICATION";
    public static final String EXTRA_GET_USER_TOKENS_RSP = "EXTRA_GET_USER_TOKENS_RSP";
    public static final String EXTRA_GET_USER_INFO_RSP = "EXTRA_GET_USER_INFO_RSP";
    public static final String EXTRA_GET_FRIENDS_RSP = "EXTRA_GET_FRIENDS_RSP";
    public static final String EXTRA_GET_THINGS_RSP = "EXTRA_GET_THINGS_RSP";
    public static final String EXTRA_GET_NODE_INFO_RSP = "EXTRA_GET_NODE_INFO_RSP";
    public static final String EXTRA_GRAPH_ACTION_RSP = "EXTRA_GRAPH_ACTION_RSP";
    public static final String EXTRA_GENERIC_RSP = "EXTRA_GENERIC_RSP";
    public static final String EXTRA_MYHACKING_RSP = "EXTRA_MYHACKING_RSP";
    public static final String EXTRA_AVATAR_ID = "EXTRA_AVATAR_ID";
    public static final String EXTRA_AVATAR_ISUSER_FLAG = "EXTRA_AVATAR_ISUSER_FLAG";
    private static final String EXTRA_LIVE_KEYS_ADD_MSG = "EXTRA_LIVE_KEYS_ADD_MSG";
    private static final String EXTRA_LIVE_KEYS_DEL_MSG = "EXTRA_LIVE_KEYS_DEL_MSG";
    public static final String EXTRA_LIVE_VALUES_MSG = "EXTRA_LIVE_VALUES_MSG";

    /**
     * Requests
     */
    private static final int REQUEST_SENDTHINGS = 0;
    private static final int REQUEST_ADDTHING = 1;
    private static final int REQUEST_CLEANTHINGS = 2;
    private static final int REQUEST_PORTMSG = 3;
    private static final int REQUEST_PORTMSG_ARRAY = 4;
    private static final int REQUEST_SERVICE_START = 5;
    private static final int REQUEST_SERVICE_STOP = 6;
    private static final int REQUEST_FILE_UPLOAD = 7;
    private static final int REQUEST_FILE_DOWNLOAD = 8;
    private static final int REQUEST_DELETETHING = 9;
    private static final int REQUEST_RESUME = 10;
    private static final int REQUEST_PAUSE = 11;
    private static final int REQUEST_RX_UPDATE = 12;
    private static final int REQUEST_STARTUP = 13;
    private static final int REQUEST_TEARDOWN = 14;
    private static final int REQUEST_RX_MSG = 15;
    private static final int REQUEST_FORCE_ACTIVE_PORT_UPDATE = 16;
    private static final int NETWORK_CONN_STATUS = 17;
    private static final int CLOUD_CONN_STATUS = 18;
    private static final int REQUEST_SHAREACTION = 19;
    private static final int REQUEST_GETUSERTOKENS = 20;
    private static final int REQUEST_GETUSERINFO = 21;
    private static final int REQUEST_GETFRIENDS = 22;
    private static final int REQUEST_GETTHINGS = 23;
    private static final int REQUEST_GRAPHACTION = 24;
    private static final int REQUEST_GETNODEINFO = 25;
    private static final int REQUEST_ADDFRIEND = 26;
    private static final int REQUEST_MYHACKING = 27;
    private static final int REQUEST_LIVEKEYSADD = 28;
    private static final int REQUEST_LIVEKEYSDEL = 29;
    private static final int REQUEST_RESET = 30;
    private static final int REQUEST_UPDATETHING = 31;

    /**
     * Broadcasts
     */
    public static final String BROADCAST_THING_UPDATE = "NodeService.BROADCAST_THING_UPDATE";
    public static final String BROADCAST_ACTIVE_PORT_UPDATE = "NodeService.BROADCAST_ACTIVE_PORT_UPDATE";
    public static final String BROADCAST_SEND_UPDATES = "NodeService.BROADCAST_SEND_UPDATES";
    public static final String BROADCAST_NODE_UNPAIRING = "NodeService.BROADCAST_NODE_UNPAIRING";
    public static final String BROADCAST_CONFIGURATION_THING_UPDATE = "NodeService.BROADCAST_CONFIGURATION_THING_UPDATE";
    public static final String BROADCAST_SHARE_ACTION_RSP = "NodeService.BROADCAST_SHARE_ACTION_RSP";
    public static final String BROADCAST_SHARE_NOTIFICATION = "NodeService.BROADCAST_SHARE_NOTIFICATION";
    public static final String BROADCAST_GET_USER_TOKENS_RSP = "NodeService.BROADCAST_GET_USER_TOKENS_RSP";
    public static final String BROADCAST_GET_USER_INFO_RSP = "NodeService.BROADCAST_GET_USER_INFO_RSP";
    public static final String BROADCAST_NODE_INFO_REQ = "NodeService.BROADCAST_NODE_INFO_REQ";
    public static final String BROADCAST_GET_FRIENDS_RSP = "NodeService.BROADCAST_GET_FRIENDS_RSP";
    public static final String BROADCAST_GET_THINGS_RSP = "NodeService.BROADCAST_GET_THINGS_RSP";
    public static final String BROADCAST_GRAPH_ACTION_RSP = "NodeService.BROADCAST_GRAPH_ACTION_RSP";
    public static final String BROADCAST_GET_NODE_INFO_RSP = "NodeService.BROADCAST_GET_NODE_INFO_RSP";
    public static final String BROADCAST_GENERIC_RSP = "NodeService.BROADCAST_GENERIC_RSP";
    public static final String BROADCAST_MYHACKING_RSP = "NodeService.BROADCAST_MYHACKING_RSP";
    public static final String BROADCAST_FILE_DOWNLOADED = "NodeService.BROADCAST_FILE_DOWNLOADED";
    public static final String BROADCAST_LIVE_VALUES_MSG = "NodeService.LIVE_VALUES_MSG";

    // =============================================================================================

    public static final String PortValue_Boolean_False = "False";
    public static final String PortValue_Boolean_True = "True";
    private static final ReentrantReadWriteLock ActivePkeyLock = new ReentrantReadWriteLock();

    private Boolean isMqttSelected = false;
    private Boolean isGcmSelected = false;

    private static SettingsProvider settingsProvider;

    private static boolean iHazTheInternets = false;
    private static HashMap<String, Thing> preDefinedThings = new HashMap<>();
    private static Boolean isReset = false;
    private static AtomicInteger SendSeqNum = new AtomicInteger();

    private static boolean serverIsConnected = false;
    private static MqttServerAPI mqttServerAPI = null;
    private static GcmServerAPI gcmServerAPI = null;
    private static HashMap<String, Thing> thingHashMap = new HashMap<>();
    private static HashSet<String> ActivePortKeysHashSet = new HashSet<>();
    private static HashMap<String, Thing> PortKeyToThingsHashMap = new HashMap<>();
    private static HashMap<String, Port> PortKeyToPortHashMap = new HashMap<>();

    /**
     * Notification related
     */
    private static int numThingUpdateMsg = 0;
    private static int numIpConnectivityMsg = 0;
    private static int numBleThingMsg = 0;
    private static int numActivePortUpdateMsg = 0;

    public static final int THING_UPDATE_ID = 1;
    public static final int IP_CONNECTIVITY_ID = 2;
    public static final int BLE_THING_ID = 3;
    public static final int ACTIVE_PORT_UPDATE_ID = 4;

    // =============================================================================================

    private static int GetSendSeqNum() {
        return SendSeqNum.incrementAndGet();
    }

    class UploadResp {
        public boolean Success;
        public String Guid;
    }

    // =============================================================================================
    // Service Overrides
    // =============================================================================================

    public NodeService() {
        super("NodeService");
    }

    public NodeService(String name) {
        super(name);
    }

    @Override
    protected void onHandleIntent(Intent intent) {

        Context context = getApplicationContext();
        settingsProvider = SettingsProvider.getInstance(context);

        // check transport protocol
        if (settingsProvider.getTransportProtocol().equals("1")) {
            isMqttSelected = true;
            //Log.d(TAG, "Mqtt selected");
        }
        else if (settingsProvider.getTransportProtocol().equals("2")){
            isGcmSelected = true;
            //Log.d(TAG, "Gcm selected");
        }
        else{
            isMqttSelected = true;
            //Log.d(TAG, "Mqtt selected by default");
        }

        int request_type;
        Bundle bundle = intent.getExtras();

        try {
            request_type = bundle.getInt(EXTRA_REQUEST_TYPE);
        } catch (Exception e) {
            Helpers.logException(TAG, e);
            return;
        }

        //Handle STARTUP/TEARDOWN/CONNECTIVITY requests differently
        if (request_type == REQUEST_STARTUP) {
            //get current state of network connection
            SetInitialNeworkStatus(context);

            // Init server api and select MQTT or GCM or REST transport
            if (isMqttSelected && mqttServerAPI == null) {
                mqttServerAPI = MqttServerAPI.getInstance(context);

                // Init RX handlers
                InitRxHandlers();
            }
            else if (isGcmSelected && gcmServerAPI == null) {
                gcmServerAPI = GcmServerAPI.getInstance(context);

                // Init RX handlers
                InitRxHandlers();
            }
            return;
        } else if (request_type == REQUEST_TEARDOWN) {
            if (isMqttSelected && mqttServerAPI != null) {
                try {
                    mqttServerAPI.Teardown();
                } catch (Exception e) {
                    Helpers.logException(TAG, e);
                }
                mqttServerAPI = null;
            }
            else if (isGcmSelected && gcmServerAPI != null) {
                try {
                    gcmServerAPI.Teardown();
                } catch (Exception e) {
                    Helpers.logException(TAG, e);
                }
                gcmServerAPI = null;
            }
            this.stopSelf();
            return;
        } else if (request_type == NETWORK_CONN_STATUS) {
            boolean netConnected = bundle.getBoolean(EXTRA_STATUS);
            if (netConnected) {
                if (!iHazTheInternets) {
                    iHazTheInternets = true;

                    if (isMqttSelected && mqttServerAPI != null) {
                        mqttServerAPI.Connect();
                    }

                    if (isGcmSelected && gcmServerAPI != null) {
                        gcmServerAPI.Connect();
                    }
                }
            } else {
                if (iHazTheInternets) {
                    iHazTheInternets = false;
                }
            }
            return;
        } else if (request_type == CLOUD_CONN_STATUS) {
            Boolean cloudConnected = bundle.getBoolean(EXTRA_STATUS);
            if (cloudConnected) {
                if (!serverIsConnected) {
                    serverIsConnected = true;

                    // register Things
                    RegisterThingAction(context, preDefinedThings,RegisterOperation.ADD);
                }
            } else {
                if (serverIsConnected) {
                    serverIsConnected = false;
                }
            }
            return;
        }

        String nodeKey = settingsProvider.getNodeKey();

        //from here on we need pairing, a (mqtt/ gcm)ServerAPI and an active connection to do anything useful (except from the reset case)
        if (!isReset) {
            if ((isMqttSelected && mqttServerAPI == null) ||
                    (isGcmSelected && gcmServerAPI == null) ||
                    nodeKey == null ||
                    !serverIsConnected )
                return;
        }

        try {
            switch (request_type) {
                // -------------------------------------
                case REQUEST_SENDTHINGS:
                    SendThings();
                    break;
                // -------------------------------------
                case REQUEST_CLEANTHINGS: {
                    thingHashMap.clear();
                    PortKeyToThingsHashMap.clear();
                }
                break;
                // -------------------------------------
                case REQUEST_RESET: {
                    thingHashMap.clear();
                    PortKeyToThingsHashMap.clear();
                    PortKeyToPortHashMap.clear();
                    preDefinedThings.clear();
                    Teardown(context);
                }
                break;
                // -------------------------------------
                case REQUEST_ADDTHING: {
                    Thing thing = new Gson().fromJson(bundle.getString(EXTRA_THING), Thing.class);
                    thingHashMap.put(thing.ThingKey, thing);

                    for (Port p : thing.Ports) {
                        if (!PortKeyToThingsHashMap.containsKey(p.PortKey)) {
                            PortKeyToThingsHashMap.remove(p.PortKey);
                        }
                        PortKeyToThingsHashMap.put(p.PortKey, thing);

                        if (!PortKeyToPortHashMap.containsKey(p.PortKey)) {
                            PortKeyToPortHashMap.remove(p.PortKey);
                        }
                        PortKeyToPortHashMap.put(p.PortKey, p);
                    }
                }
                break;
                // -------------------------------------
                case REQUEST_UPDATETHING: {
                    Thing thing = new Gson().fromJson(bundle.getString(EXTRA_THING), Thing.class);
                    thingHashMap.put(thing.ThingKey, thing);

                    for (Port p : thing.Ports) {
                        if (!PortKeyToThingsHashMap.containsKey(p.PortKey)) {
                            PortKeyToThingsHashMap.remove(p.PortKey);
                        }
                        PortKeyToThingsHashMap.put(p.PortKey, thing);

                        if (!PortKeyToPortHashMap.containsKey(p.PortKey)) {
                            PortKeyToPortHashMap.remove(p.PortKey);
                        }
                        PortKeyToPortHashMap.put(p.PortKey, p);
                    }

                    UpdateThing(thing);
                }
                break;
                // -------------------------------------
                case REQUEST_DELETETHING: {
                    Thing thing = new Gson()
                            .fromJson(bundle.getString(EXTRA_THING),
                                    Thing.class);
                    thingHashMap.remove(thing.ThingKey);

                    for (Port p : thing.Ports) {
                        if (!PortKeyToThingsHashMap.containsKey(p.PortKey))
                            PortKeyToThingsHashMap.remove(p.PortKey);

                        if (!PortKeyToPortHashMap.containsKey(p.PortKey))
                            PortKeyToPortHashMap.remove(p.PortKey);
                    }

                    DeleteThing(thing);
                }
                break;
                // -------------------------------------
                case REQUEST_PORTMSG: {
                    String thingName = bundle.getString(EXTRA_THING_NAME);
                    Thing thing = thingHashMap.get(ThingKey.CreateKey(nodeKey, thingName));
                    if (thing != null) {
                        String portIndex = bundle.getString(EXTRA_PORT_INDEX);
                        int qos = bundle.getInt(EXTRA_QOS);
                        String data = bundle.getString(EXTRA_PORT_DATA);
                        SendPortMsg(thing, portIndex, data, qos);
                    }
                }
                break;
                // -------------------------------------
                case REQUEST_PORTMSG_ARRAY: {
                    String thingName = bundle.getString(EXTRA_THING_NAME);
                    Thing thing = thingHashMap.get(ThingKey.CreateKey(nodeKey, thingName));
                    if (thing != null) {
                        String[] data = new Gson().fromJson(bundle.getString(EXTRA_PORT_DATA_ARRAY), String[].class);
                        int qos = bundle.getInt(EXTRA_QOS);
                        SendPortMsg(thing, data, qos);
                    }
                }
                break;
                // -------------------------------------
                case REQUEST_MYHACKING: {
                    MyHackingReq msg = new Gson().fromJson(bundle.getString(EXTRA_MYHACKING_REQ), MyHackingReq.class);
                    if (msg != null) {
                        int qos = bundle.getInt(EXTRA_QOS);
                        SendMyHackingReq(msg, qos);
                    }
                }
                break;
                // -------------------------------------
                case REQUEST_SHAREACTION: {
                    ShareActionReq[] req = new Gson().fromJson(bundle.getString(EXTRA_SHARE_ACTION_REQ), ShareActionReq[].class);
                    int qos = bundle.getInt(EXTRA_QOS);
                    SendShareActionReq(req, qos);
                }
                break;
                // -------------------------------------
                case REQUEST_GETUSERTOKENS: {
                    GetUserTokensReq req = new Gson().fromJson(bundle.getString(EXTRA_GET_USER_TOKENS_REQ), GetUserTokensReq.class);
                    int qos = bundle.getInt(EXTRA_QOS);
                    SendGetUserTokensReq(req, qos);
                }
                break;
                // -------------------------------------
                case REQUEST_ADDFRIEND: {
                    AddFriendReq req = new Gson().fromJson(bundle.getString(EXTRA_ADD_FRIEND_REQ), AddFriendReq.class);
                    int qos = bundle.getInt(EXTRA_QOS);
                    SendAddFriendReq(req, qos);
                }
                break;
                // -------------------------------------
                case REQUEST_GETUSERINFO: {
                    GetUserInfoReq req = new Gson().fromJson(bundle.getString(EXTRA_GET_USER_INFO_REQ), GetUserInfoReq.class);
                    int qos = bundle.getInt(EXTRA_QOS);
                    SendGetUserInfoReq(req, qos);
                }
                break;
                // -------------------------------------
                case REQUEST_GETFRIENDS: {
                    GetFriendsReq req = new Gson().fromJson(bundle.getString(EXTRA_GET_FRIENDS_REQ), GetFriendsReq.class);
                    int qos = bundle.getInt(EXTRA_QOS);
                    SendGetFriendsReq(req, qos);
                }
                break;
                // -------------------------------------
                case REQUEST_GETTHINGS: {
                    GetThingsReq req = new Gson().fromJson(bundle.getString(EXTRA_GET_THINGS_REQ), GetThingsReq.class);
                    int qos = bundle.getInt(EXTRA_QOS);
                    SendGetThingsReq(req, qos);
                }
                break;
                // -------------------------------------
                case REQUEST_GRAPHACTION: {
                    GraphActionReq req = new Gson().fromJson(bundle.getString(EXTRA_GRAPH_ACTION_REQ), GraphActionReq.class);
                    int qos = bundle.getInt(EXTRA_QOS);
                    SendGraphActionReq(req, qos);
                }
                break;
                // -------------------------------------
                case REQUEST_GETNODEINFO: {
                    GetNodeInfoReq req = new Gson().fromJson(bundle.getString(EXTRA_GET_NODE_INFO_REQ), GetNodeInfoReq.class);
                    int qos = bundle.getInt(EXTRA_QOS);
                    SendGetNodeInfoReq(req, qos);
                }
                break;
                // -------------------------------------
                case REQUEST_LIVEKEYSADD: {
                    LiveKeysAddMsg msg = new Gson().fromJson(bundle.getString(EXTRA_LIVE_KEYS_ADD_MSG), LiveKeysAddMsg.class);
                    if (msg != null) {
                        int qos = bundle.getInt(EXTRA_QOS);
                        SendLiveKeysAddMsg(msg, qos);
                    }
                }
                break;
                // -------------------------------------
                case REQUEST_LIVEKEYSDEL: {
                    LiveKeysDelMsg msg = new Gson().fromJson(bundle.getString(EXTRA_LIVE_KEYS_DEL_MSG), LiveKeysDelMsg.class);
                    if (msg != null) {
                        int qos = bundle.getInt(EXTRA_QOS);
                        SendLiveKeysDelMsg(msg, qos);
                    }
                }
                break;
                // -------------------------------------
                case REQUEST_RX_MSG: {
                    String msgPayload = bundle.getString(EXTRA_RX_MSG_PAYLOAD);
                    int msgSyncId = bundle.getInt(EXTRA_RX_MSG_SYNC_ID);
                    int msgFlags = bundle.getInt(EXTRA_RX_MSG_FLAGS);
                    String topic = bundle.getString(EXTRA_RX_TOPIC);
                    HandleRxMsg(topic, msgPayload, msgSyncId, msgFlags);
                }
                break;
                // -------------------------------------
                case REQUEST_RX_UPDATE: {
                    SendPortStateReq();
                }
                break;
                // -----------------------------------
                case REQUEST_FILE_UPLOAD: {
                    String thingName = bundle.getString(EXTRA_THING_NAME);
                    Thing thing = thingHashMap.get(ThingKey.CreateKey(nodeKey, thingName));
                    if (thing != null) {
                        String portIndex = bundle.getString(EXTRA_PORT_INDEX);
                        String filePath = bundle.getString(EXTRA_FILE_PATH);

                        try {
                            // Upload file and construct ΒinaryResourceDescriptor
                            String urlAddress = String.format("%s/upload", settingsProvider.getYodiUpAddress());
                            String response = UploadFile(filePath, urlAddress);
                            UploadResp rsp = new Gson().fromJson(response, UploadResp.class);

                            if (!rsp.Success) {
                                Helpers.log(Log.WARN, TAG, "Failed to upload file: " + filePath);
                                break;
                            }
                            String uri = String.format("%s/get/%s", settingsProvider.getYodiUpAddress(), rsp.Guid);
                            HttpLocationDescriptor locDesc = new HttpLocationDescriptor(
                                    uri,
                                    eRestServiceType.Undefined
                            );
                            BinaryResourceDescriptor descriptor = new BinaryResourceDescriptor(
                                    new BinaryResourceDescriptorKey(),
                                    filePath,
                                    filePath,
                                    0,
                                    eBinaryResourceContentType.Image,
                                    eBinaryResourceLocationType.Http,
                                    null,
                                    new Gson().toJson(locDesc)
                            );

                            // Send PortEventMsg to cloud
                            String payload = new Gson().toJson(descriptor);
                            SendPortMsg(thing, portIndex, payload, 2);
                        } catch (Exception ex) {
                            Helpers.logException(TAG, ex);
                        }
                    }
                    break;
                }
                // -----------------------------------
                case REQUEST_FILE_DOWNLOAD: {
                    byte[] fileByteArray = null;
                    String uri = bundle.getString(EXTRA_URI);
                    try {
                        // Download file
                        if (uri != null && !uri.isEmpty())
                            fileByteArray = DownloadFile(uri);
                    } catch (Exception ex) {
                        Helpers.logException(TAG, ex);
                        break;
                    }

                    String thingName = bundle.getString(EXTRA_THING_NAME);
                    if (thingName != null){
                        Thing thing = thingHashMap.get(ThingKey.CreateKey(nodeKey, thingName));
                        if (thing != null) {
                            String portIndex = bundle.getString(EXTRA_PORT_INDEX);

                            try {
                                // Broadcast successful download
                                Intent imageDownloadedIntent = new Intent(BROADCAST_THING_UPDATE);
                                imageDownloadedIntent.putExtra(EXTRA_UPDATED_THING_KEY, thing.ThingKey);
                                imageDownloadedIntent.putExtra(EXTRA_UPDATED_THING_NAME, thingName);
                                imageDownloadedIntent.putExtra(EXTRA_UPDATED_PORT_ID, portIndex);
                                imageDownloadedIntent.putExtra(EXTRA_UPDATED_IS_EVENT, bundle.getBoolean(NodeService.EXTRA_UPDATED_IS_EVENT));
                                imageDownloadedIntent.putExtra(EXTRA_IS_INTERNAL_EVENT, true);
                                imageDownloadedIntent.putExtra(EXTRA_FILE_DOWNLOADED, fileByteArray);
                                LocalBroadcastManager
                                        .getInstance(context)
                                        .sendBroadcast(imageDownloadedIntent);
                            }catch (Exception ex){
                                Helpers.logException(TAG, ex);
                            }
                        }
                    }
                    else{
                        String avatarId = bundle.getString(EXTRA_AVATAR_ID);
                        Boolean isUser = bundle.getBoolean(EXTRA_AVATAR_ISUSER_FLAG);
                        Intent imageDownloadedIntent = new Intent(BROADCAST_FILE_DOWNLOADED);
                        imageDownloadedIntent.putExtra(EXTRA_AVATAR_ID, avatarId);
                        imageDownloadedIntent.putExtra(EXTRA_AVATAR_ISUSER_FLAG, isUser);
                        imageDownloadedIntent.putExtra(EXTRA_FILE_DOWNLOADED, fileByteArray);
                        LocalBroadcastManager
                                .getInstance(context)
                                .sendBroadcast(imageDownloadedIntent);
                    }

                    break;
                }
                // -----------------------------------
                case REQUEST_FORCE_ACTIVE_PORT_UPDATE: {
                    // Send active port update
                    SendActivePortUpdate();
                    break;
                }
            }
        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
    }

    // =============================================================================================
    // Node --> Cloud messaging
    // =============================================================================================

    private void SendThings() {

        // update revNum
        int localThingsRevNum = Integer.valueOf(SettingsProvider.getInstance(getApplicationContext()).getNodeThingsRevNum()) + 1;
        SettingsProvider.getInstance(getApplicationContext()).setNodeThingsRevNum(String.valueOf(localThingsRevNum));

        try {
            Collection<Thing> values = thingHashMap.values();
            ThingsSet msg = new ThingsSet(GetSendSeqNum(),
                    ThingsSet.Overwrite,
                    true,
                    values.toArray(new Thing[values.size()]),
                    localThingsRevNum);
            if (isMqttSelected) {
                if (mqttServerAPI.SendReq(msg))
                    Log.d(TAG, "Mqtt msg sent: revNum = " + localThingsRevNum + ", #Things = " + thingHashMap.size());
            }
            else if (isGcmSelected) {
                if (gcmServerAPI.SendReq(msg))
                    Log.d(TAG, "GCM msg sent: revNum = " + localThingsRevNum + ", #Things = " + thingHashMap.size());
            }
        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
    }

    // ---------------------------------------------------------------------------------------------

    private void UpdateThing(Thing thing) {

        // update revNum
        int localThingsRevNum = Integer.valueOf(SettingsProvider.getInstance(getApplicationContext()).getNodeThingsRevNum()) + 1;
        SettingsProvider.getInstance(getApplicationContext()).setNodeThingsRevNum(String.valueOf(localThingsRevNum));

        try {
            ThingsSet msg = new ThingsSet(GetSendSeqNum(),
                    ThingsSet.Update,
                    true,
                    new Thing[]{thing},
                    localThingsRevNum);
            if (isMqttSelected) {
                if (mqttServerAPI.SendReq(msg))
                    Log.d(TAG, "Mqtt msg sent: revNum = " + localThingsRevNum);
            }
            else if (isGcmSelected) {
                if (gcmServerAPI.SendReq(msg))
                    Log.d(TAG, "GCM msg sent: revNum = " + localThingsRevNum);
            }
        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
    }

    // ---------------------------------------------------------------------------------------------

    private void DeleteThing(Thing thing) {

        // update revNum
        int localThingsRevNum = Integer.valueOf(SettingsProvider.getInstance(getApplicationContext()).getNodeThingsRevNum()) + 1;
        SettingsProvider.getInstance(getApplicationContext()).setNodeThingsRevNum(String.valueOf(localThingsRevNum));

        try {
            ThingsSet msg = new ThingsSet(GetSendSeqNum(),
                    ThingsSet.Delete,
                    true,
                    new Thing[]{thing},
                    localThingsRevNum);
            if (isMqttSelected) {
                if (mqttServerAPI.SendReq(msg))
                    Log.d(TAG, "Mqtt msg sent: revNum = " + localThingsRevNum);
            }
            else if (isGcmSelected) {
                if (gcmServerAPI.SendReq(msg))
                    Log.d(TAG, "GCM msg sent: revNum = " + localThingsRevNum);
            }
        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
    }
    // ---------------------------------------------------------------------------------------------

    private void SendPortStateReq() {
        try {
            PortStateGet msg = new PortStateGet(GetSendSeqNum(), ePortStateOperation.AllPortStates, null);
            if (isMqttSelected)
                mqttServerAPI.SendReq(msg);
            else if (isGcmSelected)
                gcmServerAPI.SendReq(msg);
        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
    }

    // ---------------------------------------------------------------------------------------------

    private void SendPortMsg(Thing thing, String portIndex, String data, int qos) {
        if (data != null) {
            Lock l = ActivePkeyLock.readLock();
            l.lock();
            Boolean allow = ActivePortKeysHashSet.contains(PortKey.CreateKey(thing.ThingKey, portIndex));
            l.unlock();

            if (!allow)
                return;

            PortEventMsg msg = new PortEventMsg();

            // Fill the port event for each data
            msg.PortEvents.add(
                    new PortEvent(PortKey.CreateKey(thing.ThingKey, portIndex), data, 0)
            );
            //Send API request
            try {
                boolean rc = false;
                if (isMqttSelected)
                    rc = mqttServerAPI.SendMsg(msg, qos);
                else if (isGcmSelected)
                    rc = gcmServerAPI.SendMsg(msg);

                if (rc)
                    Log.i(TAG, "Sent portEventMsg for thing " + thing.Name + " with state " + data);
                else
                    Log.e(TAG, "Could not send portEventMsg for thing " + thing.Name + " with state " + data);

            } catch (Exception e) {
                Helpers.logException(TAG, e);
            }
        }
    }

    // ---------------------------------------------------------------------------------------------

    private void SendPortMsg(Thing thing, String[] data, int qos) {

        if (data != null && data.length > 0) {
            Lock l = ActivePkeyLock.readLock();
            PortEventMsg msg = new PortEventMsg();

            l.lock();
            // Fill the port event for each data element which is enabled (deployed)
            for (int i = 0; i < data.length; i++) {
                if (ActivePortKeysHashSet.contains(thing.Ports.get(i).PortKey)) {
                    msg.PortEvents.add(
                            new PortEvent(thing.Ports.get(i).PortKey, data[i], 0)
                    );
                }
            }
            l.unlock();

            if (msg.PortEvents.isEmpty())
                return;

            try {

                boolean rc = false;
                if (isMqttSelected)
                    rc = mqttServerAPI.SendMsg(msg, qos);
                else if (isGcmSelected)
                    rc = gcmServerAPI.SendMsg(msg);

                if (rc)
                    Log.i(TAG, "Sent portEvent array for thing " + thing.Name);
                else
                    Log.e(TAG, "Could not send portEvent array for thing " + thing.Name);
            } catch (Exception e) {
                Helpers.logException(TAG, e);
            }
        }
    }

    // ---------------------------------------------------------------------------------------------

    private void SendMyHackingReq(MyHackingReq msg, int qos) {
        //Send API request
        try {
            boolean rc = false;
            if (isMqttSelected)
                rc = mqttServerAPI.SendReq(msg);
            else if (isGcmSelected)
                rc = gcmServerAPI.SendReq(msg);

            if (rc)
                Log.i(TAG, "Sent MyHackingReq");
            else
                Log.e(TAG, "Could not send Sent MyHackingReq");

        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
    }

    // ---------------------------------------------------------------------------------------------

    private void SendShareActionReq(ShareActionReq[] req, int qos) {

        // TODO: How to handle SeqNo?
        ShareThingsReq msg =  new ShareThingsReq(0, req);

        //Send API request
        try {
            boolean rc = false;
            if (isMqttSelected)
                rc = mqttServerAPI.SendReq(msg);
            else if (isGcmSelected)
                rc = gcmServerAPI.SendReq(msg);

            if (rc)
                Log.i(TAG, "Sent ShareThingsRequest");
            else
                Log.e(TAG, "Could not send ShareThingsRequest");

        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
    }

    // ---------------------------------------------------------------------------------------------

    private void SendGetUserTokensReq(GetUserTokensReq msg, int qos) {

        //Send API request
        try {
            boolean rc = false;
            if (isMqttSelected)
                rc = mqttServerAPI.SendReq(msg);
            else if (isGcmSelected)
                rc = gcmServerAPI.SendReq(msg);

            if (rc)
                Log.i(TAG, "Sent GetUserTokensReq");
            else
                Log.e(TAG, "Could not send GetUserTokensReq");

        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
    }

    // ---------------------------------------------------------------------------------------------

    private void SendAddFriendReq(AddFriendReq msg, int qos) {

        //Send API request
        try {
            boolean rc = false;
            if (isMqttSelected)
                rc = mqttServerAPI.SendReq(msg);
            else if (isGcmSelected)
                rc = gcmServerAPI.SendReq(msg);

            if (rc)
                Log.i(TAG, "Sent AddFriendReq");
            else
                Log.e(TAG, "Could not send AddFriendReq");

        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
    }

    // ---------------------------------------------------------------------------------------------

    private void SendGetUserInfoReq(GetUserInfoReq msg, int qos) {

        //Send API request
        try {
            boolean rc = false;
            if (isMqttSelected)
                rc = mqttServerAPI.SendReq(msg);
            else if (isGcmSelected)
                rc = gcmServerAPI.SendReq(msg);

            if (rc)
                Log.i(TAG, "Sent GetUserInfoReq");
            else
                Log.e(TAG, "Could not send GetUserInfoReq");

        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
    }

    // ---------------------------------------------------------------------------------------------

    private void SendGetFriendsReq(GetFriendsReq msg, int qos) {

        //Send API request
        try {
            boolean rc = false;
            if (isMqttSelected)
                rc = mqttServerAPI.SendReq(msg);
            else if (isGcmSelected)
                rc = gcmServerAPI.SendMsg(msg);

            if (rc)
                Log.i(TAG, "Sent GetFriendsReq");
            else
                Log.e(TAG, "Could not send GetFriendsReq");

        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
    }

    // ---------------------------------------------------------------------------------------------

    private void SendGetThingsReq(GetThingsReq msg, int qos) {

        //Send API request
        try {
            boolean rc = false;
            if (isMqttSelected)
                rc = mqttServerAPI.SendReq(msg);
            else if (isGcmSelected)
                rc = gcmServerAPI.SendReq(msg);

            if (rc)
                Log.i(TAG, "Sent GetThingsReq");
            else
                Log.e(TAG, "Could not send GetThingsReq");

        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
    }

    // ---------------------------------------------------------------------------------------------

    private void SendGraphActionReq(GraphActionReq msg, int qos) {

        //Send API request
        try {
            boolean rc = false;
            if (isMqttSelected)
                rc = mqttServerAPI.SendReq(msg);
            else if (isGcmSelected)
                rc = gcmServerAPI.SendReq(msg);

            if (rc)
                Log.i(TAG, "Sent GraphActionReq");
            else
                Log.e(TAG, "Could not send GraphActionReq");

        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
    }

    // ---------------------------------------------------------------------------------------------

    private void SendGetNodeInfoReq(GetNodeInfoReq msg, int qos) {

        //Send API request
        try {
            boolean rc = false;
            if (isMqttSelected)
                rc = mqttServerAPI.SendReq(msg);
            else if (isGcmSelected)
                rc = gcmServerAPI.SendReq(msg);

            if (rc)
                Log.i(TAG, "Sent GetNodeInfoReq");
            else
                Log.e(TAG, "Could not send GetNodeInfoReq");

        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
    }

    // ---------------------------------------------------------------------------------------------

    private void SendActivePortUpdate() {
        Lock l = ActivePkeyLock.readLock();

        // Create a list with all active things
        HashSet<String> Things = new HashSet<>();
        HashSet<String> Ports = new HashSet<>();

        l.lock();
        for (String portKey : ActivePortKeysHashSet) {
            // Get the local thing and check if we have new data
            Port localP = PortKeyToPortHashMap.get(portKey);
            Thing localT = PortKeyToThingsHashMap.get(portKey);

            if (localP != null && localT != null) {
                Things.add(localT.ThingKey);
                Ports.add(localP.PortKey);
            }
        }
        l.unlock();

        // Send the event for active ports
        Intent intent = new Intent(BROADCAST_ACTIVE_PORT_UPDATE);
        intent.putExtra(EXTRA_UPDATED_ACTIVE_THINGS_KEYS, Things.toArray(new String[Things.size()]));
        intent.putExtra(EXTRA_UPDATED_ACTIVE_PORT_KEYS, Ports.toArray(new String[Things.size()]));

        LocalBroadcastManager
                .getInstance(getApplicationContext())
                .sendBroadcast(intent);
    }

    // ---------------------------------------------------------------------------------------------

    private void SendLiveKeysAddMsg(LiveKeysAddMsg msg, int qos) {

        //Send API request
        try {
            boolean rc = false;
            if (isMqttSelected)
                rc = mqttServerAPI.SendMsg(msg);
            else if (isGcmSelected)
                rc = gcmServerAPI.SendMsg(msg);

            if (rc)
                Log.i(TAG, "Sent LiveKeysAddMsg");
            else
                Log.e(TAG, "Could not send LiveKeysAddMsg");

        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
    }

    // ---------------------------------------------------------------------------------------------

    private void SendLiveKeysDelMsg(LiveKeysDelMsg msg, int qos) {

        //Send API request
        try {
            boolean rc = false;
            if (isMqttSelected)
                rc = mqttServerAPI.SendMsg(msg);
            else if (isGcmSelected)
                rc = gcmServerAPI.SendMsg(msg);

            if (rc)
                Log.i(TAG, "Sent LiveKeysDelMsg");
            else
                Log.e(TAG, "Could not send LiveKeysDelMsg");

        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
    }

    // =============================================================================================
    // Initialise RX Handling
    // =============================================================================================

    private static HashMap<String, RxHandler> rxHandlers = null;
    private static HashMap<String, Class<?>> rxHandlersClass = null;

    interface RxHandler {
        void Handle(Object msg, int syncId, int flags);
    }

    private void InitRxHandlers() {
        if (rxHandlers == null) {
            rxHandlers = new HashMap<>();
            rxHandlersClass = new HashMap<>();

            // ---------------------> NodeInfoReq
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(NodeInfoReq.class), NodeInfoReq.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(NodeInfoReq.class),
                    new RxHandler() {
                        @Override
                        public void Handle(Object msg, int syncId, int flags) {
                            NodeInfoReq req = (NodeInfoReq) msg;

                            SettingsProvider.getInstance(getApplicationContext()).setNodeThingsRevNum("0");
                            int localThingsRevNum = Integer.valueOf(SettingsProvider.getInstance(getApplicationContext()).getNodeThingsRevNum());

                            NodeInfoRsp rsp = new NodeInfoRsp(GetSendSeqNum(),
                                    settingsProvider.getDeviceName(),
                                    eNodeType.Android,
                                    eNodeCapa.IsWarlock,
                                    null,
                                    localThingsRevNum);

                            if (isMqttSelected && mqttServerAPI != null) {
                                mqttServerAPI.SendRsp(rsp, syncId);

                                if(req.ThingsRevNum > localThingsRevNum) {
                                    ThingsGet thingsget = new ThingsGet(GetSendSeqNum(), ThingsGet.Get, null, localThingsRevNum);
                                    mqttServerAPI.SendReq(thingsget);
                                }
                            }
                            else if (isGcmSelected && gcmServerAPI != null) {
                                gcmServerAPI.SendRsp(rsp, syncId);

                                if(req.ThingsRevNum > localThingsRevNum) {
                                    ThingsGet thingsget = new ThingsGet(GetSendSeqNum(), ThingsGet.Get, null, localThingsRevNum);
                                    gcmServerAPI.SendReq(thingsget);
                                }
                            }

                            // inform main app that nodeInfoReq just arrived
                            LocalBroadcastManager
                                    .getInstance(getApplicationContext())
                                    .sendBroadcast(new Intent(NodeService.BROADCAST_NODE_INFO_REQ));
                        }
                    });

            // ---------------------> PortEventMsg
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(PortEventMsg.class), PortEventMsg.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(PortEventMsg.class),
                    new RxHandler() {
                        @Override
                        public void Handle(Object msg, int syncId, int flags) {
                            PortEventMsg portEventMsg = (PortEventMsg) msg;
                            RxPortEventMsg(portEventMsg);
                        }
                    });

            // ---------------------> PortStateGet
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(PortStateGet.class), PortStateGet.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(PortStateGet.class),
                    new RxHandler() {
                        @Override
                        public void Handle(Object msg, int syncId, int flags) {
                            PortStateGet req = (PortStateGet) msg;
                            PortStateSet rsp = null;

                            if (req.Operation == ePortStateOperation.ActivePortStates) {

                                HashSet<PortState> portStates = new HashSet<>();
                                for (String pkey : ActivePortKeysHashSet) {
                                    portStates.add(new PortState(pkey,
                                            PortKeyToPortHashMap.get(pkey).State,
                                            0, // TODO: what should we do with RevNum ?
                                            true));
                                }

                                rsp = new PortStateSet(GetSendSeqNum(), req.Operation, portStates.toArray(new PortState[portStates.size()]));
                            }
                            else if (req.Operation == ePortStateOperation.AllPortStates) {

                                HashSet<PortState> portStates = new HashSet<>();
                                for (Port port : PortKeyToPortHashMap.values()) {
                                    portStates.add(new PortState(port.PortKey,
                                            port.State,
                                            0, // TODO: what should we do with RevNum ?
                                            ActivePortKeysHashSet.contains(port.PortKey)));
                                }

                                rsp = new PortStateSet(GetSendSeqNum(), req.Operation, portStates.toArray(new PortState[portStates.size()]));
                            }
                            else if (req.Operation == ePortStateOperation.SpecificKeys) {

                                HashSet<PortState> portStates = new HashSet<>();
                                for (String pkey : req.PortKeys) {

                                    if (!PortKeyToPortHashMap.containsKey(pkey)) {
                                        continue;
                                    }

                                    portStates.add(new PortState(pkey,
                                            PortKeyToPortHashMap.get(pkey).State,
                                            0, // TODO: what should we do with RevNum ?
                                            ActivePortKeysHashSet.contains(pkey)));
                                }

                                rsp = new PortStateSet(GetSendSeqNum(), req.Operation, portStates.toArray(new PortState[portStates.size()]));
                            }

                            if (isMqttSelected && mqttServerAPI != null) {
                                mqttServerAPI.SendRsp(rsp, syncId);
                            }
                            else if (isGcmSelected && gcmServerAPI != null) {
                                gcmServerAPI.SendRsp(rsp, syncId);
                            }
                        }
                    });

            // ---------------------> PortStateSet
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(PortStateSet.class), PortStateSet.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(PortStateSet.class),
                    new RxHandler() {
                        @Override
                        public void Handle(Object msg, int syncId, int flags) {
                            PortStateSet portStateRsp = (PortStateSet) msg;
                            RxPortStateRsp(portStateRsp);

                            // send generic response to cloud
                            if (flags == eMsgFlags.Request) {
                                GenericRsp grsp = new GenericRsp(portStateRsp.SeqNo, true, 0, "");
                                if (isMqttSelected && mqttServerAPI != null)
                                    mqttServerAPI.SendRsp(grsp, syncId);
                                else if (isGcmSelected && gcmServerAPI != null)
                                    gcmServerAPI.SendRsp(grsp, syncId);
                            }
                        }
                    });

            // ---------------------> ThingsGet
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(ThingsGet.class), ThingsGet.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(ThingsGet.class),
                    new RxHandler() {
                        @Override
                        public void Handle(Object msg, int syncId, int flags) {
                            ThingsGet req = (ThingsGet) msg;
                            ThingsSet rsp;

                            // update revNum
                            int localThingsRevNum = Integer.valueOf(SettingsProvider.getInstance(getApplicationContext()).getNodeThingsRevNum()) + 1;
                            SettingsProvider.getInstance(getApplicationContext()).setNodeThingsRevNum(String.valueOf(localThingsRevNum));

                            // Send the internal things of the node.
                            if (req.Operation == ThingsGet.Get) {
                                Collection<Thing> values = thingHashMap.values();
                                rsp = new ThingsSet(GetSendSeqNum(),
                                        ThingsSet.Overwrite,
                                        true,
                                        values.toArray(new Thing[values.size()]),
                                        localThingsRevNum);
                            }
                            // TODO: Implement based on upcoming API additions for Thing delete/disable
                            else if (req.Operation == ThingsGet.Delete) {
                                rsp = new ThingsSet(GetSendSeqNum(),
                                        ThingsGet.Delete,
                                        false,
                                        null,
                                        localThingsRevNum);
                            }
                            // TODO: Implement all other operations
                            else {
                                rsp = new ThingsSet(GetSendSeqNum(),
                                        req.Operation,
                                        false,
                                        null,
                                        localThingsRevNum);
                            }

                            if (isMqttSelected && mqttServerAPI != null)
                                mqttServerAPI.SendRsp(rsp, syncId);
                            else if (isGcmSelected && gcmServerAPI != null)
                                gcmServerAPI.SendRsp(rsp, syncId);
                        }
                    });

            // ---------------------> ThingsSet
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(ThingsSet.class), ThingsSet.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(ThingsSet.class),
                    new RxHandler() {
                        @Override
                        public void Handle(Object msg, int syncId, int flags) {
                            ThingsSet thingsset = (ThingsSet) msg;
                            GenericRsp rsp = new GenericRsp();

                            // Update local ThingRevNum
                            SettingsProvider.getInstance(getApplicationContext()).setNodeThingsRevNum(String.valueOf(thingsset.RevNum));

                            if (thingsset.Operation == ThingsSet.Update) {
                                // TODO: how to handle thing configuration updates
                                /*
                                for (Thing thing: thingsset.Data){
                                    AddThing(getApplicationContext(), thing);

                                    // Send the event for configuration update
                                    Intent intent = new Intent(BROADCAST_CONFIGURATION_THING_UPDATE);
                                    intent.putExtra(EXTRA_UPDATED_THING_NAME, thing.Name);
                                    LocalBroadcastManager
                                            .getInstance(getApplicationContext())
                                            .sendBroadcast(intent);
                                    PostToast(getApplicationContext(), "Thing configuration changed", Toast.LENGTH_SHORT);
                                }
                                */
                                rsp.IsSuccess = true;
                            }
                            else if (thingsset.Operation == ThingsSet.Overwrite) {
                                //TODO: Update Hashmaps
                                rsp.IsSuccess = false;
                            }

                            if (isMqttSelected && mqttServerAPI != null)
                                mqttServerAPI.SendRsp(rsp, syncId);
                            else if (isGcmSelected && gcmServerAPI != null)
                                gcmServerAPI.SendRsp(rsp, syncId);
                        }
                    });

            // ---------------------> NodeUnpairedReq
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(NodeUnpairedReq.class), NodeUnpairedReq.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(NodeUnpairedReq.class),
                    new RxHandler() {
                        @Override
                        public void Handle(Object obj, int syncId, int flags) {
                            Unpair(getApplicationContext(), (NodeUnpairedReq) obj, false);
                            GenericRsp rsp = new GenericRsp(0, true, 0, "");
                            if (isMqttSelected && mqttServerAPI != null)
                                mqttServerAPI.SendRsp(rsp, syncId);
                            else if (isGcmSelected && gcmServerAPI != null)
                                gcmServerAPI.SendRsp(rsp, syncId);
                        }
                    });

            // ---------------------> ActivePortKeysMsg
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(ActivePortKeysMsg.class), ActivePortKeysMsg.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(ActivePortKeysMsg.class),
                    new RxHandler() {
                        @Override
                        public void Handle(Object obj, int syncId, int flags) {
                            ActivePortKeysMsg msg = (ActivePortKeysMsg) obj;
                            Lock l = ActivePkeyLock.writeLock();

                            l.lock();
                            ActivePortKeysHashSet.clear();
                            Collections.addAll(ActivePortKeysHashSet, msg.ActivePortKeys);
                            l.unlock();

                            //send updates now that active ports changed
                            SendUpdates(getApplicationContext());

                            // Send active port update
                            SendActivePortUpdate();
                        }
                    });

            // ---------------------> ShareThingsRsp
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(ShareThingsRsp.class), ShareThingsRsp.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(ShareThingsRsp.class),
                    new RxHandler() {
                        @Override
                        public void Handle(Object msg, int syncId, int flags) {
                            ShareThingsRsp sharingMsg = (ShareThingsRsp) msg;
                            RxSharingMsg(sharingMsg);
                        }
                    });

            // ---------------------> ShareThingsReq
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(ShareThingsReq.class), ShareThingsReq.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(ShareThingsReq.class),
                    new RxHandler() {
                        @Override
                        public void Handle(Object msg, int syncId, int flags) {
                            // TODO: handle ShareThingsReq from cloud (e.g. inform node that a thing has been unshared from cloud-side)
                        }
                    });

            // ---------------------> GetUserTokensRsp
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(GetUserTokensRsp.class), GetUserTokensRsp.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(GetUserTokensRsp.class),
                    new RxHandler() {
                        @Override
                        public void Handle(Object msg, int syncId, int flags) {
                            GetUserTokensRsp userTokensRsp = (GetUserTokensRsp) msg;
                            RxGetUserTokensRsp(userTokensRsp);
                        }
                    });

            // ---------------------> ShareThingsNotification
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(ShareNotifyMsg.class), ShareNotifyMsg.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(ShareNotifyMsg.class),
                    new RxHandler() {
                        @Override
                        public void Handle(Object msg, int syncId, int flags) {
                            ShareNotifyMsg notifyMsg = (ShareNotifyMsg) msg;
                            RxShareNotifyMsg(notifyMsg);
                        }
                    });

            // ---------------------> GetUserInfoReq
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(GetUserInfoReq.class), GetUserInfoReq.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(GetUserInfoReq.class),
                    new RxHandler() {
                        @Override
                        public void Handle(Object msg, int syncId, int flags) {
                            // TODO: handle this
                        }
                    });

            // ---------------------> GetUserInfoRsp
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(GetUserInfoRsp.class), GetUserInfoRsp.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(GetUserInfoRsp.class),
                    new RxHandler() {
                        @Override
                        public void Handle(Object msg, int syncId, int flags) {
                            GetUserInfoRsp userinfoRsp = (GetUserInfoRsp) msg;
                            RxGetUserInfoRsp(userinfoRsp);
                        }
                    });

            // ---------------------> GetFriendsRsp
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(GetFriendsRsp.class), GetFriendsRsp.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(GetFriendsRsp.class),
                    new RxHandler() {
                        @Override
                        public void Handle(Object msg, int syncId, int flags) {
                            GetFriendsRsp friendsRsp = (GetFriendsRsp) msg;
                            RxGetFriendsRsp(friendsRsp);
                        }
                    });

            // ---------------------> GetFriendsReq
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(GetFriendsReq.class), GetFriendsReq.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(GetFriendsReq.class),
                    new RxHandler() {
                        @Override
                        public void Handle(Object msg, int syncId, int flags) {
                            GetFriendsReq friendsReq = (GetFriendsReq) msg;
                            // TODO: handle this
                        }
                    });

            // ---------------------> GetThingsRsp
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(GetThingsRsp.class), GetThingsRsp.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(GetThingsRsp.class),
                    new RxHandler() {
                        @Override
                        public void Handle(Object msg, int syncId, int flags) {
                            GetThingsRsp thingsRsp = (GetThingsRsp) msg;
                            RxGetThingsRsp(thingsRsp);
                        }
                    });

            // ---------------------> GetThingsReq
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(GetThingsReq.class), GetThingsReq.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(GetThingsReq.class),
                    new RxHandler() {
                        @Override
                        public void Handle(Object msg, int syncId, int flags) {
                            GetThingsReq thingsReq = (GetThingsReq) msg;
                            // TODO: handle this
                        }
                    });

            // ---------------------> GetNodeInfoReq
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(GetNodeInfoReq.class), GetNodeInfoReq.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(GetNodeInfoReq.class),
                    new RxHandler() {
                        @Override
                        public void Handle(Object msg, int syncId, int flags) {
                            GetNodeInfoReq nodeinfoReq = (GetNodeInfoReq) msg;
                            // TODO: handle this
                        }
                    });

            // ---------------------> GetNodeInfoRsp
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(GetNodeInfoRsp.class), GetNodeInfoRsp.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(GetNodeInfoRsp.class),
                    new RxHandler() {
                        @Override
                        public void Handle(Object msg, int syncId, int flags) {
                            GetNodeInfoRsp nodeinfoRsp = (GetNodeInfoRsp) msg;
                            RxGetNodeInfoRsp(nodeinfoRsp);
                        }
                    });

            // ---------------------> GraphActionReq
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(GraphActionReq.class), GraphActionReq.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(GraphActionReq.class),
                    new RxHandler() {
                        @Override
                        public void Handle(Object msg, int syncId, int flags) {
                            GraphActionReq graphActionReq = (GraphActionReq) msg;
                            // TODO: handle this
                        }
                    });

            // ---------------------> GraphActionRsp
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(GraphActionRsp.class), GraphActionRsp.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(GraphActionRsp.class),
                    new RxHandler() {
                        @Override
                        public void Handle(Object msg, int syncId, int flags) {
                            GraphActionRsp graphActionRsp = (GraphActionRsp) msg;
                            RxGraphActionRsp(graphActionRsp);
                        }
                    });

            // ---------------------> MyHackingRsp
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(MyHackingRsp.class), MyHackingRsp.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(MyHackingRsp.class),
                    new RxHandler() {
                        @Override
                        public void Handle(Object msg, int syncId, int flags) {
                            MyHackingRsp myHackingRsp = (MyHackingRsp) msg;
                            RxMyHackingRsp(myHackingRsp);
                        }
                    });

            // ---------------------> GenericRsp
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(GenericRsp.class), GenericRsp.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(GenericRsp.class),
                    new RxHandler() {
                        @Override
                        public void Handle(Object msg, int syncId, int flags) {
                            GenericRsp genericRsp = (GenericRsp) msg;
                            RxGenericRsp(genericRsp);
                        }
                    });

            // ---------------------> LiveValuesMsg
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(LiveValuesMsg.class), LiveValuesMsg.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(LiveValuesMsg.class),
                    new RxHandler() {
                        @Override
                        public void Handle(Object msg, int syncId, int flags) {
                            LiveValuesMsg liveUpdateValue = (LiveValuesMsg) msg;
                            RxLiveUpdateValue(liveUpdateValue);
                        }
                    });
        }
    }

    // =============================================================================================
    // Global Rx-Msg Handler (Cloud --> Node messaging)
    // =============================================================================================

    private void HandleRxMsg(String topic, String payload, int syncId, int flags) {

        RxHandler handler = rxHandlers.get(topic);
        if (handler != null) {
            try {
                Object obj = new Gson().fromJson(payload, rxHandlersClass.get(topic));
                handler.Handle(obj, syncId, flags);
            } catch (Exception e) {
                Helpers.logException(TAG, e);
            }
        } else {
            //unknown message
            if ((flags & eMsgFlags.Request) != 0) {
                //if it's a REQ we still need to offer a RSP
                if (isMqttSelected && mqttServerAPI != null) {
                    mqttServerAPI.SendRsp(null, syncId);
                }
                else if (isGcmSelected && gcmServerAPI != null) {
                    gcmServerAPI.SendRsp(null, syncId);
                }
            }
        }
    }

    // =============================================================================================
    // Rx msg sub-handlers
    // =============================================================================================

    private void RxPortStateRsp(PortStateSet rsp) {

        if (rsp == null || rsp.PortStates == null)
            return;

        for (PortState portState : rsp.PortStates) {
            HandlePortMsg(portState.PortKey, portState.State, portState.RevNum, portState.IsDeployed, false);
        }
    }

    // ---------------------------------------------------------------------------------------------

    private void RxPortEventMsg(PortEventMsg msg) {
        // Now we need to find any changes and update the UI
        for (PortEvent pmsg : msg.PortEvents) {
            HandlePortMsg(pmsg.PortKey, pmsg.State, pmsg.RevNum, true, true);
        }
    }

    // ---------------------------------------------------------------------------------------------

    private void RxSharingMsg(ShareThingsRsp msg) {
        // Now we need to handle msg and update the UI
        if (msg == null || !msg.Handled){
            Helpers.log(Log.INFO, TAG, "Cloud unable to handle com.yodiwo.nebit.sharing things action");
            return;
        }

        if (msg.ShareActionRsps == null || msg.ShareActionRsps.length == 0)
            return;

        for (ShareActionRsp rsp : msg.ShareActionRsps) {
            // send Broadcast msg
            Intent intent = new Intent(BROADCAST_SHARE_ACTION_RSP);
            intent.putExtra(EXTRA_SHARE_ACTION_RSP, new Gson().toJson(rsp));
            LocalBroadcastManager
                    .getInstance(getApplicationContext())
                    .sendBroadcast(intent);

        }
    }

    // ---------------------------------------------------------------------------------------------

    private void RxGetUserTokensRsp(GetUserTokensRsp msg) {
        // Now we need to handle msg and update the UI
        if (msg.Tokens == null || msg.Tokens.size() == 0 ){
            Helpers.log(Log.INFO, TAG, "No users tokens exist");
            return;
        }

        Intent intent = new Intent(BROADCAST_GET_USER_TOKENS_RSP);
        intent.putExtra(EXTRA_GET_USER_TOKENS_RSP, new Gson().toJson(msg));
        LocalBroadcastManager
                .getInstance(getApplicationContext())
                .sendBroadcast(intent);
    }

    // ---------------------------------------------------------------------------------------------

    private void RxGetUserTokensReq(GetUserTokensReq msg) {
        // TODO: handle such kind of msg
    }

    // ---------------------------------------------------------------------------------------------

    private void RxShareNotifyMsg(ShareNotifyMsg msg) {
        // Now we need to handle msg and update the UI
        for (ShareNotification notif : msg.Notifications) {
            // send Broadcast msg
            Intent intent = new Intent(BROADCAST_SHARE_NOTIFICATION);
            intent.putExtra(EXTRA_SHARE_NOTIFICATION, new Gson().toJson(notif));
            LocalBroadcastManager
                    .getInstance(getApplicationContext())
                    .sendBroadcast(intent);
        }
    }

    // ---------------------------------------------------------------------------------------------

    private void RxGetUserInfoRsp(GetUserInfoRsp msg) {
        // Now we need to handle msg and update the UI
        Intent intent = new Intent(BROADCAST_GET_USER_INFO_RSP);
        intent.putExtra(EXTRA_GET_USER_INFO_RSP, new Gson().toJson(msg));
        LocalBroadcastManager
                .getInstance(getApplicationContext())
                .sendBroadcast(intent);
    }

    // ---------------------------------------------------------------------------------------------

    private void RxGetFriendsRsp(GetFriendsRsp msg) {
        // Now we need to handle msg and update the UI
        Intent intent = new Intent(BROADCAST_GET_FRIENDS_RSP);
        intent.putExtra(EXTRA_GET_FRIENDS_RSP, new Gson().toJson(msg));
        LocalBroadcastManager
                .getInstance(getApplicationContext())
                .sendBroadcast(intent);
    }

    // ---------------------------------------------------------------------------------------------

    private void RxGetThingsRsp(GetThingsRsp msg) {
        // Now we need to handle msg and update the UI
        Intent intent = new Intent(BROADCAST_GET_THINGS_RSP);
        intent.putExtra(EXTRA_GET_THINGS_RSP, new Gson().toJson(msg));
        LocalBroadcastManager
                .getInstance(getApplicationContext())
                .sendBroadcast(intent);
    }

    // ---------------------------------------------------------------------------------------------

    private void RxGraphActionRsp(GraphActionRsp msg) {
        // Now we need to handle msg and update the UI
        Intent intent = new Intent(BROADCAST_GRAPH_ACTION_RSP);
        intent.putExtra(EXTRA_GRAPH_ACTION_RSP, new Gson().toJson(msg));
        LocalBroadcastManager
                .getInstance(getApplicationContext())
                .sendBroadcast(intent);
    }

    // ---------------------------------------------------------------------------------------------

    private void RxLiveUpdateValue(LiveValuesMsg msg) {
        // Now we need to find any changes and update the UI
        Intent intent = new Intent(BROADCAST_LIVE_VALUES_MSG);
        intent.putExtra(EXTRA_LIVE_VALUES_MSG, new Gson().toJson(msg));
        LocalBroadcastManager
                .getInstance(getApplicationContext())
                .sendBroadcast(intent);
    }

    // ---------------------------------------------------------------------------------------------

    private void RxMyHackingRsp(MyHackingRsp msg) {
        // Now we need to handle msg and update the UI
        Intent intent = new Intent(BROADCAST_MYHACKING_RSP);
        intent.putExtra(EXTRA_MYHACKING_RSP, new Gson().toJson(msg));
        LocalBroadcastManager
                .getInstance(getApplicationContext())
                .sendBroadcast(intent);
    }

    // ---------------------------------------------------------------------------------------------

    private void RxGenericRsp(GenericRsp msg) {
        // Now we need to handle msg and update the UI
        Intent intent = new Intent(BROADCAST_GENERIC_RSP);
        intent.putExtra(EXTRA_GENERIC_RSP, new Gson().toJson(msg));
        LocalBroadcastManager
                .getInstance(getApplicationContext())
                .sendBroadcast(intent);
    }

    // ---------------------------------------------------------------------------------------------

    private void RxGetNodeInfoRsp(GetNodeInfoRsp msg) {
        // Now we need to handle msg and update the UI
        Intent intent = new Intent(BROADCAST_GET_NODE_INFO_RSP);
        intent.putExtra(EXTRA_GET_NODE_INFO_RSP, new Gson().toJson(msg));
        LocalBroadcastManager
                .getInstance(getApplicationContext())
                .sendBroadcast(intent);
    }

    // =============================================================================================
    // Unpairing
    // =============================================================================================

    private void Unpair(Context context, NodeUnpairedReq msg, boolean isUnpairedByUser) {
        String reason = msg.ReasonCode == NodeUnpairedReq.UserRequested ? "user request" :
                msg.ReasonCode == NodeUnpairedReq.InvalidOperation ? "invalid app operation" :
                        msg.ReasonCode == NodeUnpairedReq.TooManyAuthFailures ? "too many failed logins" : "unknown";
        PostToast(context, "Unpaired from Cloud Services because of " + reason, Toast.LENGTH_SHORT);
        PairingService.UnPair(context, isUnpairedByUser);
    }

    // =============================================================================================
    // Actions on nodeThings
    // =============================================================================================

    private static void AddThings(Context context, HashMap<String, Thing> things, Boolean forceUpdate) {

        if (forceUpdate) {
            // clean hashmaps
            CleanThings(context);

            // register things
            RegisterThingAction(context, things, RegisterOperation.ADD);

            // send updated things to cloud
            SendThings(context);
        }
    }

    // ---------------------------------------------------------------------------------------------

    private static void SendThings(Context context){
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_SENDTHINGS);
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    private static void CleanThings(Context context) {
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_CLEANTHINGS);
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    private static void AddThing(Context context, Thing thing) {
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_ADDTHING);
        intent.putExtra(EXTRA_THING, new Gson().toJson(thing));
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    private static void UpdateThing(Context context, Thing thing) {
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_UPDATETHING);
        intent.putExtra(EXTRA_THING, new Gson().toJson(thing));
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    private static void DeleteThing(Context context, Thing thing) {
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_DELETETHING);
        intent.putExtra(EXTRA_THING, new Gson().toJson(thing));
        context.startService(intent);
    }

    // =============================================================================================
    // Public Functions
    // =============================================================================================

    public static void SendPortMsg(Context context, String thingName, String[] data) {
        SendPortMsg(context, thingName, data, 2);
    }

    public static void SendPortMsg(Context context, String thingName, String[] data, int qos) {
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_PORTMSG_ARRAY);
        intent.putExtra(EXTRA_THING_NAME, thingName);
        intent.putExtra(EXTRA_PORT_DATA_ARRAY, new Gson().toJson(data));
        intent.putExtra(EXTRA_QOS, qos);
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    public static void SendPortMsg(Context context, String thingName, String portIndex, String data) {
        SendPortMsg(context, thingName, portIndex, data, 2);
    }

    public static void SendPortMsg(Context context, String thingName, String portIndex, String data, int qos) {
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_PORTMSG);
        intent.putExtra(EXTRA_THING_NAME, thingName);
        intent.putExtra(EXTRA_PORT_INDEX, portIndex);
        intent.putExtra(EXTRA_PORT_DATA, data);
        intent.putExtra(EXTRA_QOS, qos);
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    public static void SendMyHackingReq(Context context, MyHackingReq msg){
        SendMyHackingReq(context, msg, 2);
    }

    public static void SendMyHackingReq(Context context, MyHackingReq msg, int qos){
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_MYHACKING);
        intent.putExtra(EXTRA_MYHACKING_REQ, new Gson().toJson(msg));
        intent.putExtra(EXTRA_QOS, qos);
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    public static void SendShareActionReq(Context context, ShareActionReq[] req){
        SendShareActionReq(context, req, 2);
    }

    public static void SendShareActionReq(Context context, ShareActionReq[] req, int qos){
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_SHAREACTION);
        intent.putExtra(EXTRA_SHARE_ACTION_REQ, new Gson().toJson(req));
        intent.putExtra(EXTRA_QOS, qos);
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    public static void SendGetUserInfoReq(Context context, GetUserInfoReq req){
        SendGetUserInfoReq(context, req, 2);
    }

    public static void SendGetUserInfoReq(Context context, GetUserInfoReq req, int qos){
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_GETUSERINFO);
        intent.putExtra(EXTRA_GET_USER_INFO_REQ, new Gson().toJson(req));
        intent.putExtra(EXTRA_QOS, qos);
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    public static void SendGetNodeInfoReq(Context context, GetNodeInfoReq req){
        SendGetNodeInfoReq(context, req, 2);
    }

    public static void SendGetNodeInfoReq(Context context, GetNodeInfoReq req, int qos){
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_GETNODEINFO);
        intent.putExtra(EXTRA_GET_NODE_INFO_REQ, new Gson().toJson(req));
        intent.putExtra(EXTRA_QOS, qos);
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    public static void SendGraphActionReq(Context context, GraphActionReq req){
        SendGraphActionReq(context, req, 2);
    }

    public static void SendGraphActionReq(Context context, GraphActionReq req, int qos){
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_GRAPHACTION);
        intent.putExtra(EXTRA_GRAPH_ACTION_REQ, new Gson().toJson(req));
        intent.putExtra(EXTRA_QOS, qos);
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    public static void SendGetFriendsReq(Context context, GetFriendsReq req){
        SendGetFriendsReq(context, req, 2);
    }

    public static void SendGetFriendsReq(Context context, GetFriendsReq req, int qos){
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_GETFRIENDS);
        intent.putExtra(EXTRA_GET_FRIENDS_REQ, new Gson().toJson(req));
        intent.putExtra(EXTRA_QOS, qos);
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    public static void SendGetThingsReq(Context context, GetThingsReq req){
        SendGetThingsReq(context, req, 2);
    }

    public static void SendGetThingsReq(Context context, GetThingsReq req, int qos){
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_GETTHINGS);
        intent.putExtra(EXTRA_GET_THINGS_REQ, new Gson().toJson(req));
        intent.putExtra(EXTRA_QOS, qos);
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    public static void SendGetUserTokensReq(Context context, GetUserTokensReq req){
        SendGetUserTokensReq(context, req, 2);
    }

    public static void SendGetUserTokensReq(Context context, GetUserTokensReq req, int qos){
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_GETUSERTOKENS);
        intent.putExtra(EXTRA_GET_USER_TOKENS_REQ, new Gson().toJson(req));
        intent.putExtra(EXTRA_QOS, qos);
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    public static void SendLiveKeysAddMsg(Context context, LiveKeysAddMsg msg){
        SendLiveKeysAddMsg(context, msg, 2);
    }

    public static void SendLiveKeysAddMsg(Context context, LiveKeysAddMsg msg, int qos){
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_LIVEKEYSADD);
        intent.putExtra(EXTRA_LIVE_KEYS_ADD_MSG, new Gson().toJson(msg));
        intent.putExtra(EXTRA_QOS, qos);
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    public static void SendLiveKeysDelMsg(Context context, LiveKeysDelMsg msg){
        SendLiveKeysDelMsg(context, msg, 2);
    }

    public static void SendLiveKeysDelMsg(Context context, LiveKeysDelMsg msg, int qos){
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_LIVEKEYSDEL);
        intent.putExtra(EXTRA_LIVE_KEYS_DEL_MSG, new Gson().toJson(msg));
        intent.putExtra(EXTRA_QOS, qos);
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    public static void SendAddFriendReq(Context context, AddFriendReq req){
        SendAddFriendReq(context, req, 2);
    }

    public static void SendAddFriendReq(Context context, AddFriendReq req, int qos){
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_ADDFRIEND);
        intent.putExtra(EXTRA_ADD_FRIEND_REQ, new Gson().toJson(req));
        intent.putExtra(EXTRA_QOS, qos);
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    public static void RxMsg(Context context, String topic, String json, int syncId, int flags) {
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_RX_MSG);
        intent.putExtra(EXTRA_RX_TOPIC, topic);
        intent.putExtra(EXTRA_RX_MSG_PAYLOAD, json);
        intent.putExtra(EXTRA_RX_MSG_SYNC_ID, syncId);
        intent.putExtra(EXTRA_RX_MSG_FLAGS, flags);
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    public static void Resume(Context context) {
        /*
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_RESUME);
        context.startService(intent);
        Helpers.log(Log.DEBUG, TAG, "DEBUG Node Service Resumed");
        */
    }

    // ---------------------------------------------------------------------------------------------

    public static void Pause(Context context) {
        /*
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_PAUSE);
        context.startService(intent);
        Helpers.log(Log.DEBUG, TAG, "DEBUG Node Service Paused");
        */
    }

    // ---------------------------------------------------------------------------------------------

    public static void Startup(Context context) {
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_STARTUP);
        context.startService(intent);
        Helpers.log(Log.DEBUG, TAG, "Node Service Startup requested");
    }

    // ---------------------------------------------------------------------------------------------

    public static void Teardown(Context context) {
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_TEARDOWN);
        context.startService(intent);
        Helpers.log(Log.DEBUG, TAG, "Node Service Teardown requested");
    }

    // ---------------------------------------------------------------------------------------------

    public static void Reset(Context context) {
        isReset = true;
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_RESET);
        context.startService(intent);
        Helpers.log(Log.DEBUG, TAG, "Node Service reset requested");
    }

    // ---------------------------------------------------------------------------------------------

    public static void RequestUpdatedState(Context context) {
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_RX_UPDATE);
        context.startService(intent);
        Helpers.log(Log.DEBUG, TAG, "RX Update");
    }

    // ---------------------------------------------------------------------------------------------

    public static void SetCloudConnStatus(Context context, boolean connected) {
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, CLOUD_CONN_STATUS);
        intent.putExtra(EXTRA_STATUS, connected);
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    public static void SetNetworkConnStatus(Context context, boolean connected) {
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, NETWORK_CONN_STATUS);
        intent.putExtra(EXTRA_STATUS, connected);
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    public static boolean IsNetworkConnected() {
        return iHazTheInternets;
    }

    // ---------------------------------------------------------------------------------------------

    public static void SendUpdates(Context context) {
        Helpers.log(Log.INFO, TAG, "Sending updates ");

        Intent intent = new Intent(BROADCAST_SEND_UPDATES);
        LocalBroadcastManager
                .getInstance(context)
                .sendBroadcast(intent);
    }

    // ---------------------------------------------------------------------------------------------

    public static void UploadFile(Context context,
                                  String filePath,
                                  String thingName,
                                  String portIndex) {
        Helpers.log(Log.INFO, TAG, "Upload file:" + filePath + " for: " + thingName + ":" + portIndex);

        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_FILE_UPLOAD);
        intent.putExtra(EXTRA_FILE_PATH, filePath);
        intent.putExtra(EXTRA_THING_NAME, thingName);
        intent.putExtra(EXTRA_PORT_INDEX, portIndex);
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    public static void ForceActiveKeyUpdate(Context context) {
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_FORCE_ACTIVE_PORT_UPDATE);
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    public static void DownloadFile(Context context, String uri, String thingName, String portIndex) {
        Helpers.log(Log.INFO, TAG, "Download file: " + uri + " for: " + thingName + ":" + portIndex);

        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_FILE_DOWNLOAD);
        intent.putExtra(EXTRA_URI, uri);
        intent.putExtra(EXTRA_THING_NAME, thingName);
        intent.putExtra(EXTRA_PORT_INDEX, portIndex);
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    public static void DownloadFile(Context context, String uri, String avatarId, Boolean isUser) {
        Helpers.log(Log.INFO, TAG, "Download file: " + uri + " for: " + avatarId);

        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_FILE_DOWNLOAD);
        intent.putExtra(EXTRA_AVATAR_ID, avatarId);
        intent.putExtra(EXTRA_AVATAR_ISUSER_FLAG, isUser);
        intent.putExtra(EXTRA_URI, uri);
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    public static void PostToast(final Context context, final String text, final int toastLength){
        new Handler(Looper.getMainLooper()).post(new Runnable() {
            @Override
            public void run() {
                Toast.makeText(context, text, toastLength).show();
            }
        });
    }

    // ---------------------------------------------------------------------------------------------

    public static void ImportThings(Context context, ArrayList<Thing> things, Boolean forceUpdate){
        for (Thing thing : things){
            preDefinedThings.put(thing.ThingKey, thing);
        }

        AddThings(context, preDefinedThings, forceUpdate);
    }

    // ---------------------------------------------------------------------------------------------

    public static Boolean getResetStatus(){
        return isReset;
    }

    // ---------------------------------------------------------------------------------------------

    public enum RegisterOperation {
        ADD,
        DELETE,
        UPDATE,
        HIDE,
        UNHIDE,
        NONE
    }

    public static void RegisterThingAction(Context context, HashMap<String, Thing> thingsList, RegisterOperation oper) {
        for (String thingkey : thingsList.keySet()) {
            if (oper == RegisterOperation.ADD) {
                AddThing(context, thingsList.get(thingkey));
            }
            else if (oper == RegisterOperation.DELETE) {
                DeleteThing(context, thingsList.get(thingkey));
            }
            else if (oper == RegisterOperation.HIDE) {
                // TODO: hide Things
            }
            else if (oper == RegisterOperation.UNHIDE){
                // TODO: unhide Things
            }
        }
    }

    public static void RegisterThingAction(Context context, Thing[] thingsList, RegisterOperation oper) {
        for (Thing thing : thingsList) {
            if (oper == RegisterOperation.ADD) {
                AddThing(context, thing);
            }
            else if (oper == RegisterOperation.DELETE) {
                DeleteThing(context, thing);
            }
            else if (oper == RegisterOperation.UPDATE) {
                UpdateThing(context, thing);
            }
            else if (oper == RegisterOperation.HIDE) {
                // TODO: hide Things
            }
            else if (oper == RegisterOperation.UNHIDE){
                // TODO: unhide Things
            }
        }
    }

    // ---------------------------------------------------------------------------------------------

    public static Thing getThing(String thingkey){
        return thingHashMap.get(thingkey);
    }

    // =============================================================================================
    // Network status
    // =============================================================================================

    private static void SetInitialNeworkStatus(Context context) {
        ConnectivityManager cm = (ConnectivityManager) context.getSystemService(Context.CONNECTIVITY_SERVICE);
        NetworkInfo activeNetInfo = cm.getActiveNetworkInfo();

        iHazTheInternets = (activeNetInfo != null) && (activeNetInfo.isConnected());
    }

    // =============================================================================================
    // File upload/download
    // =============================================================================================

    private String UploadFile(String filePath, String urlAddress) {

        Helpers.log(Log.DEBUG, TAG, "File to upload: " + filePath);
        Helpers.log(Log.DEBUG, TAG, "url: " + urlAddress);

        String response = "";

        HttpURLConnection urlConnection = null;
        try {
            int notificationId = 1;
            // Init notification for progress
            NotificationManager mNotifyManager =
                    (NotificationManager) getSystemService(Context.NOTIFICATION_SERVICE);
            NotificationCompat.Builder mBuilder = new NotificationCompat.Builder(this);
            mBuilder.setContentTitle("Picture Upload")
                    .setContentText("Upload in progress")
                    .setSmallIcon(R.drawable.launcher_icon_yodiwo)
                    .setLargeIcon(BitmapFactory.decodeResource(getResources(), R.drawable.launcher_icon_yodiwo))
                    .setAutoCancel(true)
                    .setPriority(Notification.PRIORITY_MAX);


            mBuilder.setProgress(100, 0, false);
            mNotifyManager.notify(notificationId, mBuilder.build());

            URL url = new URL(urlAddress);
            urlConnection = (HttpURLConnection) url.openConnection();

            urlConnection.setDoInput(true);
            urlConnection.setDoOutput(true);
            urlConnection.setChunkedStreamingMode(0);
            urlConnection.setRequestMethod("POST");
            urlConnection.setReadTimeout(30000);
            urlConnection.setConnectTimeout(45000);
            urlConnection.setRequestProperty("Content-Type", "image/jpeg");

            OutputStream out = new BufferedOutputStream(urlConnection.getOutputStream());

            // Read the image
            File inFile = new File(filePath);
            InputStream in = new FileInputStream(inFile);

            // Transfer bytes from in to out
            byte[] buf = new byte[128*1024]; // 128KB windows size
            int len;

            int sendedLen = 0;
            int maxLen = (int) inFile.length();
            while ((len = in.read(buf)) > 0) {
                out.write(buf, 0, len);
                out.flush();

                sendedLen += len;
                mBuilder.setProgress(maxLen, sendedLen, false)
                        .setContentText(String.format("Upload in progress (%d/%d)", sendedLen, maxLen));
                mNotifyManager.notify(notificationId, mBuilder.build());
            }
            out.close();

            int responseCode = urlConnection.getResponseCode();
            if (responseCode == HttpsURLConnection.HTTP_OK) {
                String line;
                BufferedReader br = new BufferedReader(new InputStreamReader(urlConnection.getInputStream()));
                while ((line = br.readLine()) != null) {
                    response += line;
                }
            }

            Helpers.log(Log.INFO, TAG, "File Uploaded: " + response);


            // Close notification when we finished update
            mNotifyManager.cancel(notificationId);

        } catch (Exception ex) {
            Helpers.logException(TAG, ex);
        } finally {
            if (urlConnection != null)
                urlConnection.disconnect();
        }

        return response;
    }

    // ---------------------------------------------------------------------------------------------

    private byte[] DownloadFile(String uri) {

        Helpers.log(Log.DEBUG, TAG, "File to download: " + uri);

        ByteArrayOutputStream output = null;
        HttpURLConnection connection = null;
        try {
            URL url = new URL(uri);

            connection = (HttpURLConnection) url.openConnection();

            connection.setReadTimeout(10000);
            connection.setConnectTimeout(15000);
            connection.setRequestMethod("GET");
            connection.setDoInput(true);
            connection.connect(); // Starts the query
            InputStream input = new BufferedInputStream(connection.getInputStream());

            byte[] buffer = new byte[2048];
            int n;

            output = new ByteArrayOutputStream();
            while ((n = input.read(buffer)) != -1) {
                output.write(buffer, 0, n);
            }
            output.close();

            int responseCode = connection.getResponseCode();
            if (responseCode != HttpsURLConnection.HTTP_OK) {
                return null;
            }

            Helpers.log(Log.INFO, TAG, "File downloaded.");
        } catch (Exception ex) {
            Helpers.logException(TAG, ex);
        } finally {
            if (connection != null)
                connection.disconnect();
        }

        return (output == null) ? null : output.toByteArray();
    }

    // =============================================================================================
    // Handle port Msg
    // =============================================================================================

    private void HandlePortMsg(String portKey, String state, int revNum, boolean isDeployed, boolean isEvent) {
        // Get the local thing and check if we have new data
        Port localP = PortKeyToPortHashMap.get(portKey);
        Thing localT = PortKeyToThingsHashMap.get(portKey);

        if (localP == null || localT == null) {
            Helpers.log(Log.ERROR, TAG, "event for non existent port " + portKey);
            return;
        }
        //TODO: Save port revNum?

        if (localP.Type != ePortType.String && (state.equals(""))) {
            Helpers.log(Log.ERROR, TAG, "Empty state passed in!");
            return;
        }
        // Send the event for this port
        Intent intent = new Intent(BROADCAST_THING_UPDATE);
        intent.putExtra(EXTRA_UPDATED_THING_KEY, localT.ThingKey);
        intent.putExtra(EXTRA_UPDATED_THING_NAME, localT.Name);
        intent.putExtra(EXTRA_UPDATED_PORT_ID, String.valueOf(localT.Ports.indexOf(localP)));
        intent.putExtra(EXTRA_UPDATED_STATE, state);
        intent.putExtra(EXTRA_UPDATED_IS_EVENT, isEvent);

        LocalBroadcastManager
                .getInstance(getApplicationContext())
                .sendBroadcast(intent);
    }

    // =============================================================================================
    // Notification display
    // =============================================================================================

    private void displayNotification(Context context, Class activity, int NotificationId, String title, String msg[]) {

        NotificationCompat.Builder mBuilder;
        settingsProvider = SettingsProvider.getInstance(context);

        try {
            if (!settingsProvider.getNotificationEnable()) {
                return;
            }
            else {
                mBuilder = (NotificationCompat.Builder) new NotificationCompat.Builder(context)
                        .setSmallIcon(com.yodiwo.androidagent.R.drawable.launcher_icon_yodiwo)
                        .setContentTitle(title)
                        .setAutoCancel(true);

                if (settingsProvider.getNotificationSoundMode().equals("1")){
                }
                else if (settingsProvider.getNotificationSoundMode().equals("2")){
                    mBuilder.setSound(RingtoneManager.getDefaultUri(RingtoneManager.TYPE_NOTIFICATION));
                }
                else if (settingsProvider.getNotificationSoundMode().equals("3")){
                    mBuilder.setDefaults(Notification.DEFAULT_VIBRATE);
                }
                else if (settingsProvider.getNotificationSoundMode().equals("4")){
                    mBuilder.setDefaults(Notification.DEFAULT_SOUND | Notification.DEFAULT_VIBRATE);
                }
            }

            /* Add Big View Specific Configuration */
            NotificationCompat.InboxStyle inboxStyle = new NotificationCompat.InboxStyle();

            // Sets a title for the Inbox style big view
            inboxStyle.setBigContentTitle(title);

            // Moves events into the big view
            for (String aMsg : msg) {
                inboxStyle.addLine(aMsg);
            }
            mBuilder.setStyle(inboxStyle);

            // Creates an explicit intent for an Activity in your app
            Intent resultIntent = new Intent(context, activity);
            resultIntent.putExtra(EXTRA_NOTIFICATION_ID, NotificationId);

            // Sets the Activity to start in a new, empty task
            resultIntent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK
                    | Intent.FLAG_ACTIVITY_CLEAR_TASK);

            // The stack builder object will contain an artificial back stack for the
            // started Activity.
            // This ensures that navigating backward from the Activity leads out of
            // your application to the Home screen.
            TaskStackBuilder stackBuilder = TaskStackBuilder.create(context);

            // Adds the back stack for the Intent (but not the Intent itself)
            stackBuilder.addParentStack(activity);

            // Adds the Intent that starts the Activity to the top of the stack
            stackBuilder.addNextIntent(resultIntent);
            PendingIntent resultPendingIntent =
                    stackBuilder.getPendingIntent(0, PendingIntent.FLAG_UPDATE_CURRENT);
            mBuilder.setContentIntent(resultPendingIntent);
            NotificationManager mNotificationManager =
                    (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);

            // numbering allows you to update the notification later on.
            if (NotificationId == ACTIVE_PORT_UPDATE_ID) {
                mNotificationManager.notify(++numActivePortUpdateMsg, mBuilder.build());
            } else {
                Log.w(TAG, "Not valid notification ID");
            }
        }catch (Exception ex){
            Helpers.logException(TAG, ex);
        }
    }

    // ---------------------------------------------------------------------------------------------

    public static void displayNotification(Context context, Class activity, int NotificationId, String title, String msg) {

        NotificationCompat.Builder mBuilder;
        settingsProvider = SettingsProvider.getInstance(context);

        try {
            if (!settingsProvider.getNotificationEnable()) {
                return;
            }
            else {
                mBuilder = (NotificationCompat.Builder) new NotificationCompat.Builder(context)
                        .setSmallIcon(com.yodiwo.androidagent.R.drawable.launcher_icon_yodiwo)
                        .setContentTitle(title)
                        .setContentText(msg)
                        .setAutoCancel(true);

                if (settingsProvider.getNotificationSoundMode().equals("1")){
                }
                else if (settingsProvider.getNotificationSoundMode().equals("2")){
                    mBuilder.setSound(RingtoneManager.getDefaultUri(RingtoneManager.TYPE_NOTIFICATION));
                }
                else if (settingsProvider.getNotificationSoundMode().equals("3")){
                    mBuilder.setDefaults(Notification.DEFAULT_VIBRATE);
                }
                else if (settingsProvider.getNotificationSoundMode().equals("4")){
                    mBuilder.setDefaults(Notification.DEFAULT_SOUND | Notification.DEFAULT_VIBRATE);
                }
            }

            // Creates an explicit intent for an Activity in your app
            Intent resultIntent = new Intent(context, activity);
            resultIntent.putExtra(EXTRA_NOTIFICATION_ID, NotificationId);

            // Sets the Activity to start in a new, empty task
            resultIntent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK
                    | Intent.FLAG_ACTIVITY_CLEAR_TASK);

            // The stack builder object will contain an artificial back stack for the
            // started Activity.
            // This ensures that navigating backward from the Activity leads out of
            // your application to the Home screen.
            TaskStackBuilder stackBuilder = TaskStackBuilder.create(context);

            // Adds the back stack for the Intent (but not the Intent itself)
            stackBuilder.addParentStack(activity);

            // Adds the Intent that starts the Activity to the top of the stack
            stackBuilder.addNextIntent(resultIntent);
            PendingIntent resultPendingIntent =
                    stackBuilder.getPendingIntent(0, PendingIntent.FLAG_UPDATE_CURRENT);
            mBuilder.setContentIntent(resultPendingIntent);
            NotificationManager mNotificationManager =
                    (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);

            // numbering allows you to update the notification later on.
            if (NotificationId == THING_UPDATE_ID) {
                mNotificationManager.notify(++numThingUpdateMsg, mBuilder.build());
            } else if (NotificationId == IP_CONNECTIVITY_ID) {
                mNotificationManager.notify(++numIpConnectivityMsg, mBuilder.build());
            } else if (NotificationId == BLE_THING_ID) {
                mNotificationManager.notify(++numBleThingMsg, mBuilder.build());
            } else {
                Log.w(TAG, "Not valid notification ID");
            }
        }catch (Exception ex) {
            Helpers.logException(TAG, ex);
        }
    }

    // =============================================================================================
    // Converters
    // =============================================================================================

    public static String PortKeyGetThingKey(String portKey) {
        if (portKey != null) {
            String[] array = portKey.split("\\-");
            if (array.length > 1) {
                String result = "";
                // The last one is the portID and we need to remove it to get ThingKey
                for (int i = 0; i < array.length - 1; i++) {
                    result += array[i];
                    if (i != array.length - 2) // Add "-" in all internal elements
                        result += "-";
                }
                return result;
            }
        }
        return null;
    }

    // ---------------------------------------------------------------------------------------------

    public static String ThingKeyGetThingUID(String thingkey) {
        if (thingkey != null) {
            String[] array = thingkey.split("\\-");
            if (array.length > 1) {
                return array[2];
            }
        }
        return null;
    }

    // ---------------------------------------------------------------------------------------------

    public static String PortKeyGetPortUID(String portkey) {
        if (portkey != null) {
            String[] array = portkey.split("\\-");
            if (array.length > 1) {
                return array[3];
            }
        }
        return null;
    }

    // ---------------------------------------------------------------------------------------------

    public static String NodeKeyGetNodeID(String nodekey) {
        if (nodekey != null) {
            String[] array = nodekey.split("\\-");
            if (array.length > 1) {
                return array[1];
            }
        }
        return null;
    }
}
