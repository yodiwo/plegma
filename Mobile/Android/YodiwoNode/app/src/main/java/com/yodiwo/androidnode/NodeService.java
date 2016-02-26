package com.yodiwo.androidnode;


import android.app.IntentService;
import android.app.Notification;
import android.app.NotificationManager;
import android.content.Context;
import android.content.Intent;
import android.graphics.BitmapFactory;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.net.wifi.WifiInfo;
import android.net.wifi.WifiManager;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.support.v4.content.LocalBroadcastManager;
import android.support.v7.app.NotificationCompat;
import android.text.TextUtils;
import android.util.Log;
import android.widget.Toast;

import com.google.gson.Gson;
import com.yodiwo.plegma.ActivePortKeysMsg;
import com.yodiwo.plegma.BinaryResourceDescriptor;
import com.yodiwo.plegma.HttpLocationDescriptor;
import com.yodiwo.plegma.MqttMsg;
import com.yodiwo.plegma.NodeInfoReq;
import com.yodiwo.plegma.NodeInfoRsp;
import com.yodiwo.plegma.NodeUnpairedReq;
import com.yodiwo.plegma.NodeUnpairedRsp;
import com.yodiwo.plegma.PlegmaAPI;
import com.yodiwo.plegma.Port;
import com.yodiwo.plegma.PortEvent;
import com.yodiwo.plegma.PortEventMsg;
import com.yodiwo.plegma.PortState;
import com.yodiwo.plegma.PortStateReq;
import com.yodiwo.plegma.PortStateRsp;
import com.yodiwo.plegma.Thing;
import com.yodiwo.plegma.ThingKey;
import com.yodiwo.plegma.ThingsGet;
import com.yodiwo.plegma.ThingsSet;
import com.yodiwo.plegma.eBinaryResourceContentType;
import com.yodiwo.plegma.eBinaryResourceLocationType;
import com.yodiwo.plegma.eNodeCapa;
import com.yodiwo.plegma.eNodeType;
import com.yodiwo.plegma.ePortStateOperation;
import com.yodiwo.plegma.ePortType;
import com.yodiwo.plegma.eRestServiceType;

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
import java.util.Collections;
import java.util.HashMap;
import java.util.HashSet;
import java.util.concurrent.atomic.AtomicInteger;
import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantReadWriteLock;

import javax.net.ssl.HttpsURLConnection;

public class NodeService extends IntentService {

    // =============================================================================================
    // Static information's

    public static final String TAG = NodeService.class.getSimpleName();

    public static final String EXTRA_REQUEST_TYPE = "EXTRA_REQUEST_TYPE";
    public static final String EXTRA_STATUS = "EXTRA_STATUS";


    private static final String EXTRA_THING = "EXTRA_THING";
    private static final String EXTRA_THING_NAME = "EXTRA_THING_NAME";
    private static final String EXTRA_PORT_INDEX = "EXTRA_PORT_INDEX";
    private static final String EXTRA_PORT_DATA = "EXTRA_PORT_DATA";
    private static final String EXTRA_PORT_DATA_ARRAY = "EXTRA_PORT_DATA_ARRAY";
    private static final String EXTRA_QOS = "EXTRA_QOS";
    private static final String EXTRA_SERVICE_TYPE = "EXTRA_SERVICE_TYPE";

    public static final int REQUEST_SENDTHINGS = 0;
    public static final int REQUEST_ADDTHING = 1;
    public static final int REQUEST_CLEANTHINGS = 2;
    public static final int REQUEST_PORTMSG = 3;
    public static final int REQUEST_PORTMSG_ARRAY = 4;
    public static final int REQUEST_SERVICE_START = 5;
    public static final int REQUEST_SERVICE_STOP = 6;

    public static final int REQUEST_FILE_UPLOAD = 7;
    public static final int REQUEST_FILE_DOWNLOAD = 8;

    private static final int REQUEST_RESUME = 10;
    private static final int REQUEST_PAUSE = 11;
    private static final int REQUEST_RX_UPDATE = 12;
    private static final int REQUEST_STARTUP = 13;
    private static final int REQUEST_TEARDOWN = 14;
    private static final int REQUEST_RX_MSG = 15;

    private static final int REQUEST_FORCE_ACTIVE_PORT_UPDATE = 17;

    public static final int NETWORK_CONN_STATUS = 20;
    public static final int CLOUD_CONN_STATUS = 21;

    public static final String BROADCAST_THING_UPDATE = "NodeService.BROADCAST_THING_UPDATE";
    public static final String BROADCAST_ACTIVE_PORT_UPDATE = "NodeService.BROADCAST_ACTIVE_PORT_UPDATE";

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

    public static final String EXTRA_RX_TOPIC = "EXTRA_REQUEST_TOPIC";
    public static final String EXTRA_RX_MSG_PAYLOAD = "EXTRA_REQUEST_MSG_PAYLOAD";
    public static final String EXTRA_RX_MSG_SYNC_ID = "EXTRA_REQUEST_MSG_SYNCID";
    public static final String EXTRA_RX_MSG_FLAGS = "EXTRA_REQUEST_MSG_FLAGS";

    public static final String EXTRA_FILE_PATH = "EXTRA_FILE_PATH";
    public static final String EXTRA_URI = "EXTRA_URI";

    public static final String EXTRA_FILE_DOWNLOADED = "EXTRA_FILE_DOWNLOADED";


    // =============================================================================================

    public static final String PortValue_Boolean_False = "False";
    public static final String PortValue_Boolean_True = "True";


    // =============================================================================================
    // Service overrides
    private SettingsProvider settingsProvider;

    private static Boolean thingsRegistered = false;

    private static MqttServerAPI serverAPI = null;
    private static HashMap<String, Thing> thingHashMap = new HashMap<String, Thing>();
    private static AtomicInteger SendSeqNum = new AtomicInteger();

    private int GetSendSeqNum() {
        return SendSeqNum.incrementAndGet();
    }


    private static HashSet<String> ActivePortKeysHashSet = new HashSet<>();
    private static HashMap<String, Thing> PortKeyToThingsHashMap = new HashMap<>();
    private static HashMap<String, Port> PortKeyToPortHashMap = new HashMap<>();
    private static final ReentrantReadWriteLock ActivePkeyLock = new ReentrantReadWriteLock();

    private static boolean iHazTheInternets = false;
    private static boolean serverIsConnected = false;

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

            // Init server api and select MQTT or REST transport
            if (serverAPI == null) {
                serverAPI = MqttServerAPI.getInstance(context);

                // Init RX handlers
                InitRxHandlers();
            }
            return;
        } else if (request_type == REQUEST_TEARDOWN) {
            if (serverAPI != null) {
                try {
                    serverAPI.Teardown();
                } catch (Exception e) {
                    Helpers.logException(TAG, e);
                }
                serverAPI = null;
            }
            this.stopSelf();
            return;
        } else if (request_type == NETWORK_CONN_STATUS) {
            boolean netConnected = bundle.getBoolean(EXTRA_STATUS);
            if (netConnected) {
                if (!iHazTheInternets) {
                    iHazTheInternets = true;

                    if (serverAPI != null) {
                        serverAPI.Connect();
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

                    //register things (will be updated)
                    NodeService.RegisterNode(context, false);
                }
            } else {
                if (serverIsConnected) {
                    serverIsConnected = false;
                }
            }
            return;
        }

        String nodeKey = settingsProvider.getNodeKey();

        //from here on we need pairing, a serverAPI and an active connection to do anything useful
        if (serverAPI == null || nodeKey == null || !serverIsConnected)
            return;

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
                case REQUEST_PORTMSG: {
                    String thingName = bundle.getString(EXTRA_THING_NAME);
                    Thing thing = thingHashMap.get(ThingKey.CreateKey(nodeKey, thingName));
                    if (thing != null) {
                        int portIndex = bundle.getInt(EXTRA_PORT_INDEX);
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
                        int portIndex = bundle.getInt(EXTRA_PORT_INDEX);
                        String filePath = bundle.getString(EXTRA_FILE_PATH);

                        try {
                            // Upload file and construct ΒinaryResourceDescriptor
                            String urlAddress = String.format("%s/upload", settingsProvider.getYodiUpAddress());
                            String response = UploadFile(filePath, urlAddress);
                            UploadRsp rsp = new Gson().fromJson(response, UploadRsp.class);

                            if (!rsp.Success) {
                                Helpers.log(Log.WARN, TAG, "Failed to upload file: " + filePath);
                                break;
                            }
                            String uri = String.format("%s/get/%s", settingsProvider.getYodiUpAddress(), rsp.Guid);
                            HttpLocationDescriptor locDesc =
                                    new HttpLocationDescriptor(uri, eRestServiceType.Undefined);
                            BinaryResourceDescriptor descriptor =
                                    new BinaryResourceDescriptor(filePath, filePath, 0, eBinaryResourceContentType.Image, eBinaryResourceLocationType.Http, null, locDesc);

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
                    String thingName = bundle.getString(EXTRA_THING_NAME);
                    Thing thing = thingHashMap.get(ThingKey.CreateKey(nodeKey, thingName));
                    if (thing != null) {
                        int portIndex = bundle.getInt(EXTRA_PORT_INDEX);
                        String uri = bundle.getString(EXTRA_URI);

                        try {
                            // Download file
                            byte[] fileByteArray = DownloadFile(uri);

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

                        } catch (Exception ex) {
                            Helpers.logException(TAG, ex);
                        }
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

    // ---------------------------------------------------------------------------------------------

    class UploadRsp {
        public boolean Success;
        public String Guid;
    }

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
                    .setSmallIcon(R.drawable.icon)
                    .setLargeIcon(BitmapFactory.decodeResource(getResources(), R.drawable.icon))
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
            urlConnection.setReadTimeout(10000);
            urlConnection.setConnectTimeout(15000);
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

            // TODO: Delete image ??
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

    // ---------------------------------------------------------------------------------------------

    private static void SetInitialNeworkStatus(Context context) {
        ConnectivityManager cm = (ConnectivityManager) context.getSystemService(Context.CONNECTIVITY_SERVICE);
        NetworkInfo activeNetInfo = cm.getActiveNetworkInfo();

        iHazTheInternets = (activeNetInfo != null) && (activeNetInfo.isConnected());
    }


    // =============================================================================================
    // Service execution (background thread)

    private void SendThings() {
        try {
            ThingsSet msg = new ThingsSet(GetSendSeqNum(),
                    ThingsSet.Overwrite,
                    true,
                    thingHashMap.values().toArray(new Thing[0]),
                    0);
            serverAPI.SendReq(msg);
        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
    }

    // ---------------------------------------------------------------------------------------------

    private void SendPortStateReq() {
        try {
            PortStateReq msg = new PortStateReq(GetSendSeqNum(), ePortStateOperation.AllPortStates, null);
            serverAPI.SendReq(msg);
        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
    }

    // ---------------------------------------------------------------------------------------------

    private void SendPortMsg(Thing thing, int portIndex, String data, int qos) {
        if (data != null) {
            Lock l = ActivePkeyLock.readLock();
            l.lock();
            Boolean allow = ActivePortKeysHashSet.contains(thing.Ports.get(portIndex).PortKey);
            l.unlock();

            if (!allow)
                return;

            PortEventMsg msg = new PortEventMsg();

            // Fill the port event for each data
            msg.PortEvents.add(
                    new PortEvent(thing.Ports.get(portIndex).PortKey, data, 0)
            );
            //Send API request
            try {
                boolean rc = serverAPI.SendMsg(msg, qos);
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
                boolean rc = serverAPI.SendMsg(msg, qos);
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

    private void HandlePortMsg(String portKey, String state, int revNum, boolean isDeployed, boolean isEvent) {
        // Get the local thing and check if we have new data
        Port localP = PortKeyToPortHashMap.get(portKey);
        Thing localT = PortKeyToThingsHashMap.get(portKey);

        if (localP == null || localT == null) {
            Helpers.log(Log.ERROR, TAG, "event for non existent port " + portKey);
            return;
        }
        //TODO: Save port revNum?

        if (localP.Type != ePortType.String && (state == "" || state == null)) {
            Helpers.log(Log.ERROR, TAG, "Empty state passed in!");
            return;
        }
        // Send the event for this port
        Intent intent = new Intent(BROADCAST_THING_UPDATE);
        intent.putExtra(EXTRA_UPDATED_THING_KEY, localT.ThingKey);
        intent.putExtra(EXTRA_UPDATED_THING_NAME, localT.Name);
        intent.putExtra(EXTRA_UPDATED_PORT_ID, localT.Ports.indexOf(localP));
        intent.putExtra(EXTRA_UPDATED_STATE, state);
        intent.putExtra(EXTRA_UPDATED_IS_EVENT, isEvent);

        LocalBroadcastManager
                .getInstance(getApplicationContext())
                .sendBroadcast(intent);
    }

    // ---------------------------------------------------------------------------------------------

    private void RxPortStateRsp(PortStateRsp rsp) {

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

    private String PortKeyGetThingKey(String portKey) {
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

    // =============================================================================================
    // RX Handling

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

                            int localThingsRevNum = Integer.valueOf(SettingsProvider.getInstance(getApplicationContext()).getNodeThingsRevNum());

                            NodeInfoRsp rsp = new NodeInfoRsp(GetSendSeqNum(),
                                    settingsProvider.getDeviceName(),
                                    eNodeType.EndpointSingle,
                                    eNodeCapa.None,
                                    null,
                                    localThingsRevNum);

                            if (serverAPI != null) {
                                serverAPI.SendRsp(rsp, syncId);

                                if(req.ThingsRevNum > localThingsRevNum) {
                                    ThingsGet thingsget = new ThingsGet(GetSendSeqNum(), ThingsGet.Get, null, localThingsRevNum);
                                    serverAPI.SendReq(thingsget);
                                }
                            }
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

            // ---------------------> PortStateReq
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(PortStateReq.class), PortStateReq.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(PortStateReq.class),
                    new RxHandler() {
                        @Override
                        public void Handle(Object msg, int syncId, int flags) {
                            PortStateReq req = (PortStateReq) msg;
                            PortStateRsp rsp = null;

                            if (req.Operation == ePortStateOperation.ActivePortStates) {

                                HashSet<PortState> portStates = new HashSet<>();
                                for (String pkey : ActivePortKeysHashSet) {
                                    portStates.add(new PortState(pkey,
                                            PortKeyToPortHashMap.get(pkey).State,
                                            0, // TODO: what should we do with RevNum ?
                                            true));
                                }

                                rsp = new PortStateRsp(GetSendSeqNum(), req.Operation, portStates.toArray(new PortState[portStates.size()]));
                            } else if (req.Operation == ePortStateOperation.AllPortStates) {

                                HashSet<PortState> portStates = new HashSet<>();
                                for (Port port : PortKeyToPortHashMap.values()) {
                                    portStates.add(new PortState(port.PortKey,
                                            port.State,
                                            0, // TODO: what should we do with RevNum ?
                                            ActivePortKeysHashSet.contains(port.PortKey)));
                                }

                                rsp = new PortStateRsp(GetSendSeqNum(), req.Operation, portStates.toArray(new PortState[portStates.size()]));
                            } else if (req.Operation == ePortStateOperation.SpecificKeys) {

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

                                rsp = new PortStateRsp(GetSendSeqNum(), req.Operation, portStates.toArray(new PortState[portStates.size()]));
                            }

                            if (serverAPI != null) {
                                serverAPI.SendRsp(rsp, syncId);
                            }
                        }
                    });

            // ---------------------> PortStateRsp
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(PortStateRsp.class), PortStateRsp.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(PortStateRsp.class),
                    new RxHandler() {
                        @Override
                        public void Handle(Object msg, int syncId, int flags) {
                            PortStateRsp portStateRsp = (PortStateRsp) msg;
                            RxPortStateRsp(portStateRsp);
                        }
                    });

            // ---------------------> ThingsGet
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(ThingsGet.class), ThingsGet.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(ThingsGet.class),
                    new RxHandler() {
                        @Override
                        public void Handle(Object msg, int syncId, int flags) {
                            ThingsGet req = (ThingsGet) msg;
                            ThingsSet rsp = null;

                            // Send the internal things of the node.
                            if (req.Operation == ThingsGet.Get) {

                                rsp = new ThingsSet(GetSendSeqNum(),
                                        ThingsSet.Update,
                                        true,
                                        thingHashMap.values().toArray(new Thing[0]),
                                        Integer.valueOf(SettingsProvider.getInstance(getApplicationContext()).getNodeThingsRevNum())
                                );
                            }
                            // TODO: Implement based on upcoming API additions for Thing delete/disable
                            else if (req.Operation == ThingsGet.Delete) {

                                rsp = new ThingsSet(GetSendSeqNum(),
                                        req.Operation,
                                        false,
                                        null,
                                        0);
                            }
                            // TODO: Implement all other operations
                            else {

                                rsp = new ThingsSet(GetSendSeqNum(),
                                        req.Operation,
                                        false,
                                        null,
                                        0);
                            }

                            if (serverAPI != null)
                                serverAPI.SendRsp(rsp, syncId);
                        }
                    });

            // ---------------------> ThingsSet
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(ThingsSet.class), ThingsSet.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(ThingsSet.class),
                    new RxHandler() {
                        @Override
                        public void Handle(Object msg, int syncId, int flags) {
                            ThingsSet thingsset = (ThingsSet) msg;

                            // Update local ThingRevNum
                            SettingsProvider.getInstance(getApplicationContext()).setNodeThingsRevNum(String.valueOf(thingsset.RevNum));
                            if (thingsset.Operation == ThingsSet.Update) {
                                //TODO:: Update Hashmaps
                            }
                            else if (thingsset.Operation == ThingsSet.Overwrite) {
                                //TODO: Update Hashmaps
                            }
                        }
                    });

            // ---------------------> NodeUnpairedReq
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(NodeUnpairedReq.class), NodeUnpairedReq.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(NodeUnpairedReq.class),
                    new RxHandler() {
                        @Override
                        public void Handle(Object obj, int syncId, int flags) {
                            Unpair(getApplicationContext(), (NodeUnpairedReq) obj);
                            if (serverAPI != null)
                                serverAPI.SendRsp(new NodeUnpairedRsp(), syncId);
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
                            NodeService.SendUpdates(getApplicationContext());

                            // Send active port update
                            SendActivePortUpdate();
                        }
                    });
        }
    }

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
            if ((flags & MqttMsg.Request) != 0) {
                //if it's a REQ we still need to offer a RSP
                if (serverAPI != null) {
                    serverAPI.SendRsp(null, syncId);
                }
            }
        }
    }

    // =============================================================================================
    private void Unpair(Context context, NodeUnpairedReq msg) {
        String reason = msg.ReasonCode == NodeUnpairedReq.UserRequested ? "user request" :
                msg.ReasonCode == NodeUnpairedReq.InvalidOperation ? "invalid app operation" :
                        msg.ReasonCode == NodeUnpairedReq.TooManyAuthFailures ? "too many failed logins" : "unknown";
        PostToast(context, "Unpaired from Cloud Services because of " + reason, "short");
        PairingService.UnPair(context);
    }

    // =============================================================================================
    // Periodic Updating


    // =============================================================================================
    // Public Functions

    public static void CleanThings(Context context) {
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_CLEANTHINGS);
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------


    public static void RegisterNode(Context context, Boolean forceUpdate) {
        // TODO: register nodes only when we have some change, keep a dirty flag

        if (!thingsRegistered || forceUpdate) {
            ThingManager.getInstance(context).RegisterThings();

            Intent intent = new Intent(context, NodeService.class);
            intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_SENDTHINGS);
            context.startService(intent);
        }
    }

    // ---------------------------------------------------------------------------------------------

    public static void AddThing(Context context, Thing thing) {
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_ADDTHING);
        intent.putExtra(EXTRA_THING, new Gson().toJson(thing));
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

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

    public static void SendPortMsg(Context context, String thingName, int portIndex, String data) {
        SendPortMsg(context, thingName, portIndex, data, 2);
    }

    public static void SendPortMsg(Context context, String thingName, int portIndex, String data, int qos) {
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_PORTMSG);
        intent.putExtra(EXTRA_THING_NAME, thingName);
        intent.putExtra(EXTRA_PORT_INDEX, portIndex);
        intent.putExtra(EXTRA_PORT_DATA, data);
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

    public static void Pause(Context context) {
        /*
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_PAUSE);
        context.startService(intent);
        Helpers.log(Log.DEBUG, TAG, "DEBUG Node Service Paused");
        */
    }

    public static void Startup(Context context) {
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_STARTUP);
        context.startService(intent);
        Helpers.log(Log.DEBUG, TAG, "Node Service Startup requested");
    }

    public static void Teardown(Context context) {
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_TEARDOWN);
        context.startService(intent);
        Helpers.log(Log.DEBUG, TAG, "Node Service Teardown requested");
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
        SendWifiUpdate(context);
    }
    // ---------------------------------------------------------------------------------------------

    public static void SendWifiUpdate(Context context) {
        ConnectivityManager cm = (ConnectivityManager) context.getSystemService(Context.CONNECTIVITY_SERVICE);
        NetworkInfo wifiNetInfo = cm.getNetworkInfo(ConnectivityManager.TYPE_WIFI);

        NetworkInfo.State state = wifiNetInfo.getState(); // Coarse-grained state

        String toSendState = state.toString(); // Port0
        String toSendSSID = "";
        String toSendRSSI = "";

        if (wifiNetInfo.isConnected()) {
            WifiManager wifiManager = (WifiManager) context.getSystemService(Context.WIFI_SERVICE);
            WifiInfo wifiInfo = wifiManager.getConnectionInfo();

            if (wifiInfo != null && !TextUtils.isEmpty(wifiInfo.getSSID())) {
                toSendSSID = wifiInfo.getSSID(); // Port1
                toSendRSSI = Integer.toString(wifiInfo.getRssi()); // Port2
            }
        }

        // Notify NodeService
        NodeService.SendPortMsg(context.getApplicationContext(),
                ThingManager.WiFiStatus,
                new String[]{toSendState, toSendSSID, toSendRSSI});
    }

    // ---------------------------------------------------------------------------------------------

    public static void UploadFile(Context context,
                                  String filePath,
                                  String thingName,
                                  int portIndex) {
        Helpers.log(Log.INFO, TAG, "Upload file:" + filePath + " for: " + thingName + ":" + Integer.toString(portIndex));

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
    // =============================================================================================

    public static void DownloadFile(Context context, String uri, String thingName, int portIndex) {
        Helpers.log(Log.INFO, TAG, "Download file: " + uri + " for: " + thingName + ":" + Integer.toString(portIndex));

        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_FILE_DOWNLOAD);
        intent.putExtra(EXTRA_URI, uri);
        intent.putExtra(EXTRA_THING_NAME, thingName);
        intent.putExtra(EXTRA_PORT_INDEX, portIndex);
        context.startService(intent);
    }

    // =============================================================================================

    public static  void PostToast(final Context context, final String text, final String toastLength){
        new Handler(Looper.getMainLooper()).post(new Runnable() {
            @Override
            public void run() {
                if (toastLength.equalsIgnoreCase("long"))
                    Toast.makeText(context, text, Toast.LENGTH_LONG).show();
                else
                    Toast.makeText(context, text, Toast.LENGTH_SHORT).show();
            }
        });
    }
}
