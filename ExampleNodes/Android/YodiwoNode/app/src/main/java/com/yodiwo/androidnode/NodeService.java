package com.yodiwo.androidnode;


import android.app.IntentService;
import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;

import com.google.gson.Gson;
import com.yodiwo.plegma.ActivePortKeysMsg;
import com.yodiwo.plegma.NodeInfoReq;
import com.yodiwo.plegma.NodeInfoRsp;
import com.yodiwo.plegma.PlegmaAPI;
import com.yodiwo.plegma.Port;
import com.yodiwo.plegma.PortEvent;
import com.yodiwo.plegma.PortEventMsg;
import com.yodiwo.plegma.PortState;
import com.yodiwo.plegma.PortStateReq;
import com.yodiwo.plegma.PortStateRsp;
import com.yodiwo.plegma.Thing;
import com.yodiwo.plegma.ThingKey;
import com.yodiwo.plegma.ThingsReq;
import com.yodiwo.plegma.ThingsRsp;
import com.yodiwo.plegma.eNodeCapa;
import com.yodiwo.plegma.eNodeType;
import com.yodiwo.plegma.ePortStateOperation;
import com.yodiwo.plegma.ePortType;

import java.util.HashMap;
import java.util.HashSet;
import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantReadWriteLock;

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
    private static final String EXTRA_SERVICE_TYPE = "EXTRA_SERVICE_TYPE";

    public static final int REQUEST_SENDTHINGS = 0;
    public static final int REQUEST_ADDTHING = 1;
    public static final int REQUEST_CLEANTHINGS = 2;
    public static final int REQUEST_PORTMSG = 3;
    public static final int REQUEST_PORTMSG_ARRAY = 4;
    public static final int REQUEST_SERVICE_START = 5;
    public static final int REQUEST_SERVICE_STOP = 6;

    private static final int REQUEST_RESUME = 10;
    private static final int REQUEST_PAUSE = 11;
    private static final int REQUEST_RX_UPDATE = 12;
    private static final int REQUEST_STARTUP = 13;
    private static final int REQUEST_TEARDOWN = 14;
    private static final int REQUEST_RX_MSG = 15;

    public static final int RECEIVE_CONN_STATUS = 20;

    public static final String BROADCAST_THING_UPDATE = "NodeService.BROADCAST_THING_UPDATE";

    public static final String EXTRA_UPDATED_THING_KEY = "EXTRA_UPDATED_THING_KEY";
    public static final String EXTRA_UPDATED_THING_NAME = "EXTRA_UPDATED_THING_NAME";
    public static final String EXTRA_UPDATED_PORT_ID = "EXTRA_UPDATED_PORT_ID";
    public static final String EXTRA_UPDATED_STATE = "EXTRA_UPDATED_STATE";
    // This indicates whether the update is a new event or a starting point
    public static final String EXTRA_UPDATED_IS_EVENT = "EXTRA_UPDATED_IS_EVENT";

    public static final String EXTRA_RX_TOPIC = "EXTRA_REQUEST_TOPIC";
    public static final String EXTRA_RX_MSG = "EXTRA_REQUEST_MSG";

    // =============================================================================================

    public static final String PortValue_Boolean_False = "False";
    public static final String PortValue_Boolean_True = "True";


    // =============================================================================================
    // Service overrides
    private SettingsProvider settingsProvider;

    private static Boolean thingsRegistered = false;

    private static aServerAPI serverAPI = null;
    private static HashMap<String, Thing> thingHashMap = new HashMap<String, Thing>();
    private static int SendSeqNum = 0;
    private int GetSendSeqNum() {
        SendSeqNum++;
        return SendSeqNum;
    }


    private static HashSet<String> ActivePortKeysHashSet = new HashSet<>();
    private static HashMap<String, Thing> PortKeyToThingsHashMap = new HashMap<>();
    private static HashMap<String, Port> PortKeyToPortHashMap = new HashMap<>();
    private static final ReentrantReadWriteLock ActivePkeyLock = new ReentrantReadWriteLock();

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
        }
        catch (Exception e) {
            Helpers.logException(TAG, e);
            return;
        }

        //Handle STARTUP/TEARDOWN requests differently
        if(request_type == REQUEST_STARTUP) {
            // Init server api and select MQTT or REST transport
            if (serverAPI == null) {
                if (settingsProvider.getServerTransport() == SettingsProvider.ServerAPITransport.REST)
                    serverAPI = RestServerAPI.getInstance(context);
                else
                    serverAPI = MqttServerAPI.getInstance(context);

                // Init RX handlers
                InitRxHandlers();
            }
            return;
        }
        else if(request_type == REQUEST_TEARDOWN) {
            if (serverAPI != null) {
                serverAPI.Teardown();
                serverAPI = null;
            }
            this.stopSelf();
            return;
        }

        String nodeKey = settingsProvider.getNodeKey();

        //from here on we need pairing and a serverAPI to do anything useful
        if(serverAPI == null || nodeKey == null)
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
                        if (!PortKeyToThingsHashMap.containsKey(p.PortKey))
                            PortKeyToThingsHashMap.remove(p.PortKey);
                        PortKeyToThingsHashMap.put(p.PortKey, thing);

                        if (!PortKeyToPortHashMap.containsKey(p.PortKey))
                            PortKeyToPortHashMap.remove(p.PortKey);
                        PortKeyToPortHashMap.put(p.PortKey, p);
                    }
                }
                break;
                // -------------------------------------
                case REQUEST_PORTMSG: {
                    String thingName = bundle.getString(EXTRA_THING_NAME);
                    Thing thing = thingHashMap.get(ThingKey.CreateKey(nodeKey, thingName));
                    if(thing != null) {
                        int portIndex = bundle.getInt(EXTRA_PORT_INDEX);
                        String data = bundle.getString(EXTRA_PORT_DATA);
                        SendPortMsg(thing, portIndex, data);
                    }
                }
                break;
                // -------------------------------------
                case REQUEST_PORTMSG_ARRAY: {
                    String thingName = bundle.getString(EXTRA_THING_NAME);
                    Thing thing = thingHashMap.get(ThingKey.CreateKey(nodeKey, thingName));
                    if(thing != null) {
                        String[] data = new Gson().fromJson(bundle.getString(EXTRA_PORT_DATA_ARRAY), String[].class);
                        SendPortMsg(thing, data);
                    }
                }
                break;
                // -------------------------------------
                case REQUEST_RX_MSG: {
                    String msg = bundle.getString(EXTRA_RX_MSG);
                    String topic = bundle.getString(EXTRA_RX_TOPIC);
                    HandleRxMsg(topic, msg);
                }
                break;
                // -------------------------------------
                case REQUEST_RX_UPDATE: {
                    SendPortStateReq();
                }
                break;
                // -----------------------------------
                case RECEIVE_CONN_STATUS: {
                    Boolean isConnected = bundle.getBoolean(EXTRA_STATUS);
                    if (isConnected) {
                        if (!serverIsConnected) {
                            serverIsConnected = true;

                            NodeService.RegisterNode(this, false);
                        }
                    } else {
                        if (serverIsConnected) {
                            serverIsConnected = false;
                        }
                    }
                    break;
                }
            }
        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
    }


    // =============================================================================================
    // Service execution (background thread)

    private void SendThings() {
        try {
            ThingsReq msg = new ThingsReq(GetSendSeqNum(),
                    ThingsReq.Overwrite,
                    "",
                    thingHashMap.values().toArray(new Thing[0]));
            serverAPI.Send(msg);
        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
    }

    // ---------------------------------------------------------------------------------------------

    private void SendPortStateReq() {
        try {
            PortStateReq msg = new PortStateReq(GetSendSeqNum(), ePortStateOperation.AllPortStates, null);
            serverAPI.Send(msg);
        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
    }

    // ---------------------------------------------------------------------------------------------

    private void SendPortMsg(Thing thing, int portIndex, String data) {
        if (data != null) {
            Lock l = ActivePkeyLock.readLock();
            l.lock();
            Boolean allow = ActivePortKeysHashSet.contains(thing.Ports.get(portIndex).PortKey);
            l.unlock();

            if(!allow)
                return;

            PortEventMsg msg = new PortEventMsg();

            // Fill the port event for each data
            msg.PortEvents.add(
                    new PortEvent(thing.Ports.get(portIndex).PortKey, data, 0)
            );
            //Send API request
            try {
                serverAPI.Send(msg);
            } catch (Exception e) {
                Helpers.logException(TAG, e);
            }
        }
    }

    // ---------------------------------------------------------------------------------------------

    private void SendPortMsg(Thing thing, String[] data) {

        if (data != null && data.length > 0) {
            Lock l = ActivePkeyLock.readLock();
            PortEventMsg msg = new PortEventMsg();

            l.lock();
            // Fill the port event for each data element which is enabled (deployed)
            for (int i = 0; i < data.length; i++) {
                if (ActivePortKeysHashSet.contains(thing.Ports.get(i).PortKey)) {
                    msg.PortEvents.add(
                        new PortEvent(thing.Ports.get(i).PortKey, data[i],0)
                    );
                }
            }
            l.unlock();

            if(msg.PortEvents.isEmpty())
                return;

            try {
                serverAPI.Send(msg);
            } catch (Exception e) {
                Helpers.logException(TAG, e);
            }
        }
    }

    // ---------------------------------------------------------------------------------------------

    private void RxPortStateRsp(PortStateRsp rsp) {

        if (rsp.Operation == ePortStateOperation.AllPortStates || rsp.Operation == ePortStateOperation.ActivePortStates) {
            for (PortState portState : rsp.PortStates) {
                // Get the local thing and check if we have new data
                Port localP = PortKeyToPortHashMap.get(portState.PortKey);
                Thing localT = PortKeyToThingsHashMap.get(portState.PortKey);

                // Update the local state
                localP.State = portState.State;
                // TODO: Save port seqno

                if (localP.Type != ePortType.String && localP.State == "") {
                    Helpers.log(Log.ERROR, TAG, "Empty state passed in!");
                    return;
                }

                // Send the event for this port
                Intent intent = new Intent(BROADCAST_THING_UPDATE);
                intent.putExtra(EXTRA_UPDATED_THING_KEY, localT.ThingKey);
                intent.putExtra(EXTRA_UPDATED_THING_NAME, localT.Name);
                intent.putExtra(EXTRA_UPDATED_PORT_ID, localT.Ports.indexOf(localP));
                intent.putExtra(EXTRA_UPDATED_STATE, localP.State);
                intent.putExtra(EXTRA_UPDATED_IS_EVENT, false);

                LocalBroadcastManager
                        .getInstance(getApplicationContext())
                        .sendBroadcast(intent);
            }
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

    // ---------------------------------------------------------------------------------------------

    private void RxPortEventMsg(PortEventMsg msg) {
        // Now we need to find any changes and update the UI
        for (PortEvent pmsg : msg.PortEvents) {


            // Get the local thing and check if we have new data
            Port localP = PortKeyToPortHashMap.get(pmsg.PortKey);
            Thing localT = PortKeyToThingsHashMap.get(pmsg.PortKey);

            if (localP.Type != ePortType.String && localP.State == "") {
                Helpers.log(Log.ERROR, TAG, "Empty state passed in!");
                return;
            }

            // Update the local state
            localP.State = pmsg.State;
            // TODO: Save port seqno

            // Send the event for this port
            Intent intent = new Intent(BROADCAST_THING_UPDATE);
            intent.putExtra(EXTRA_UPDATED_THING_KEY, localT.ThingKey);
            intent.putExtra(EXTRA_UPDATED_THING_NAME, localT.Name);
            intent.putExtra(EXTRA_UPDATED_PORT_ID, localT.Ports.indexOf(localP));
            intent.putExtra(EXTRA_UPDATED_STATE, localP.State);
            intent.putExtra(EXTRA_UPDATED_IS_EVENT, true);

            LocalBroadcastManager
                    .getInstance(getApplicationContext())
                    .sendBroadcast(intent);
        }
    }

    // =============================================================================================
    // RX Handling

    private static HashMap<String, RxHandler> rxHandlers = null;
    private static HashMap<String, Class<?>> rxHandlersClass = null;

    interface RxHandler {
        void Handle(String topic, String json, Object msg);
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
                        public void Handle(String topic, String json, Object msg) {
                            NodeInfoReq req = (NodeInfoReq)msg;

                            NodeInfoRsp rsp = new NodeInfoRsp(GetSendSeqNum(),
                                    settingsProvider.getDeviceName(),
                                    eNodeType.EndpointSingle,
                                    eNodeCapa.None,
                                    null);
                            if (serverAPI != null)
                                serverAPI.SendRsp(rsp, req.SeqNo);
                        }
                    });

            // ---------------------> PortEventMsg
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(PortEventMsg.class), PortEventMsg.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(PortEventMsg.class),
                    new RxHandler() {
                        @Override
                        public void Handle(String topic, String json, Object msg) {
                            PortEventMsg portEventMsg = (PortEventMsg)msg;
                            RxPortEventMsg(portEventMsg);
                        }
                    });

            // ---------------------> PortStateRsp
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(PortStateRsp.class), PortStateRsp.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(PortStateRsp.class),
                    new RxHandler() {
                        @Override
                        public void Handle(String topic, String json, Object msg) {
                            PortStateRsp portStateRsp = (PortStateRsp)msg;
                            RxPortStateRsp(portStateRsp);
                        }
                    });

            // ---------------------> ThingsReq
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(ThingsReq.class), ThingsReq.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(ThingsReq.class),
                    new RxHandler() {
                        @Override
                        public void Handle(String topic, String json, Object msg) {
                            ThingsReq req = (ThingsReq)msg;

                            // Send the internal things of the node.
                            if(req.Operation == ThingsReq.Get) {
                                ThingsRsp rsp = new ThingsRsp(
                                        GetSendSeqNum(),
                                        req.Operation,
                                        true,
                                        thingHashMap.values().toArray(new Thing[0])
                                        );

                                if (serverAPI != null)
                                    serverAPI.SendRsp(rsp, req.SeqNo);
                            }
                        }
                    });


            // ---------------------> ActivePortKeysMsg
            rxHandlersClass.put(PlegmaAPI.ApiMsgNames.get(ActivePortKeysMsg.class), ActivePortKeysMsg.class);
            rxHandlers.put(PlegmaAPI.ApiMsgNames.get(ActivePortKeysMsg.class),
                    new RxHandler() {
                        @Override
                        public void Handle(String topic, String json, Object obj) {
                            ActivePortKeysMsg msg = (ActivePortKeysMsg)obj;
                            Lock l = ActivePkeyLock.writeLock();

                            l.lock();
                            ActivePortKeysHashSet.clear();
                            for (String pkey: msg.ActivePortKeys) {
                                ActivePortKeysHashSet.add(pkey);
                            }
                            l.unlock();
                        }
                    });
        }
    }

    private void HandleRxMsg(String topic, String msg) {

        RxHandler handler = rxHandlers.get(topic);
        if (handler != null) {
            try {
                Object obj = new Gson().fromJson(msg, rxHandlersClass.get(topic));
                handler.Handle(topic, msg, obj);
            } catch (Exception e) {
                Helpers.logException(TAG, e);
            }
        }
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
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_PORTMSG_ARRAY);
        intent.putExtra(EXTRA_THING_NAME, thingName);
        intent.putExtra(EXTRA_PORT_DATA_ARRAY, new Gson().toJson(data));
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    public static void SendPortMsg(Context context, String thingName, int portIndex, String data) {
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_PORTMSG);
        intent.putExtra(EXTRA_THING_NAME, thingName);
        intent.putExtra(EXTRA_PORT_INDEX, portIndex);
        intent.putExtra(EXTRA_PORT_DATA, data);
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    public static void RxMsg(Context context, String topic, String json) {
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_RX_MSG);
        intent.putExtra(EXTRA_RX_TOPIC, topic);
        intent.putExtra(EXTRA_RX_MSG, json);
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

    public static void ReceiveConnStatus(Context context, boolean connected) {
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, RECEIVE_CONN_STATUS);
        intent.putExtra(EXTRA_STATUS, connected);
        context.startService(intent);
    }


    // =============================================================================================
}
