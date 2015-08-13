package com.yodiwo.virtualgateway;


import android.app.IntentService;
import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;

import com.google.gson.Gson;
import com.yodiwo.plegma.Port;
import com.yodiwo.plegma.PortEvent;
import com.yodiwo.plegma.PortEventMsg;
import com.yodiwo.plegma.PortState;
import com.yodiwo.plegma.PortStateRsp;
import com.yodiwo.plegma.Thing;
import com.yodiwo.plegma.ThingKey;
import com.yodiwo.plegma.ThingsMsg;
import com.yodiwo.plegma.ePortStateOperation;
import com.yodiwo.plegma.eThingsOperation;

import java.util.HashMap;

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

    private static final String EXTRA_UPDATED_THINGS_TYPE = "EXTRA_UPDATED_THINGS_TYPE";
    private static final String EXTRA_UPDATED_MSG = "EXTRA_UPDATED_MSG";


    public static final int REQUEST_SENDNODES = 0;
    public static final int REQUEST_ADDTHING = 1;
    public static final int REQUEST_CLEANTHINGS = 2;
    public static final int REQUEST_PORTMSG = 3;
    public static final int REQUEST_PORTMSG_ARRAY = 4;
    public static final int REQUEST_SERVICE_START = 5;
    public static final int REQUEST_SERVICE_STOP = 6;

    public static final int REQUEST_RX_START = 10;
    public static final int REQUEST_RX_STOP = 11;
    public static final int REQUEST_RX_UPDATE = 12;
    public static final int RECEIVE_RX_PORT_EVENT_MSG = 13;
    public static final int RECEIVE_RX_PORT_STATE_RSP = 14;

    public static final String BROADCAST_THING_UPDATE = "NodeService.BROADCAST_THING_UPDATE";

    public static final String EXTRA_UPDATED_THING = "EXTRA_UPDATED_THING";
    public static final String EXTRA_UPDATED_THING_NAME = "EXTRA_UPDATED_THING_NAME";
    public static final String EXTRA_UPDATED_PORT_ID = "EXTRA_UPDATED_PORT_ID";
    public static final String EXTRA_UPDATED_STATE = "EXTRA_UPDATED_STATE";
    // This indicate that the update is event or a starting point
    public static final String EXTRA_UPDATED_ISEVENT = "EXTRA_UPDATED_ISEVENT";


    public static final int APIVersion = 1;
    // =============================================================================================

    public static final String PortValue_Boolean_False = "False";
    public static final String PortValue_Boolean_True = "True";


    // =============================================================================================
    // Service overrides

    private static Boolean thingsRegistered = false;

    private IServerAPI serverAPI = null;
    private SettingsProvider settingsProvider;
    private static HashMap<String, Thing> thingHashMap = new HashMap<String, Thing>();
    private int SendSeqNum = 0;


    private static HashMap<String, Thing> PortKeyToThingsHashMap = new HashMap<>();
    private static HashMap<String, Port> PortKeyToPortHashMap = new HashMap<>();

    private SensorsListener sensorsListener = null;

    public NodeService() {
        super("NodeService");
    }

    public NodeService(String name) {
        super(name);
    }

    @Override
    protected void onHandleIntent(Intent intent) {

        settingsProvider = SettingsProvider.getInstance(getApplicationContext());

        // Init server api and select MQTT of REST transport
        if (serverAPI == null) {
            if (settingsProvider.getServerTransport() == SettingsProvider.ServerAPITransport.REST)
                serverAPI = RestServerAPI.getInstance(getApplicationContext());
            else
                serverAPI = MqttServerAPI.getInstance(getApplicationContext());
        }

        if (sensorsListener == null)
            sensorsListener = SensorsListener.getInstance(getApplicationContext());

        Bundle bundle = intent.getExtras();
        int request_type = bundle.getInt(EXTRA_REQUEST_TYPE);
        switch (request_type) {
            // -------------------------------------
            case REQUEST_SERVICE_START: {
                SensorsListener.SensorType type = (SensorsListener.SensorType) bundle.getSerializable(EXTRA_SERVICE_TYPE);
                sensorsListener.StartService(type);
            }
            break;
            // -------------------------------------
            case REQUEST_SERVICE_STOP: {
                SensorsListener.SensorType type = (SensorsListener.SensorType) bundle.getSerializable(EXTRA_SERVICE_TYPE);
                sensorsListener.StopService(type);
            }
            break;
            // -------------------------------------
            case REQUEST_SENDNODES:
                SendNodes(settingsProvider);
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

                for(Port p : thing.Ports) {
                    if(!PortKeyToThingsHashMap.containsKey(p.PortKey))
                        PortKeyToThingsHashMap.remove(p.PortKey);
                    PortKeyToThingsHashMap.put(p.PortKey, thing);

                    if(!PortKeyToPortHashMap.containsKey(p.PortKey))
                        PortKeyToPortHashMap.remove(p.PortKey);
                    PortKeyToPortHashMap.put(p.PortKey, p);
                }
            }
            break;
            // -------------------------------------
            case REQUEST_PORTMSG: {
                String thingName = bundle.getString(EXTRA_THING_NAME);
                Thing thing = thingHashMap.get(ThingKey.CreateKey(settingsProvider.getNodeKey(), thingName));
                int portIndex = bundle.getInt(EXTRA_PORT_INDEX);
                String data = bundle.getString(EXTRA_PORT_DATA);
                SendPortMsg(thing, portIndex, data);
            }
            break;
            // -------------------------------------
            case REQUEST_PORTMSG_ARRAY: {
                String thingName = bundle.getString(EXTRA_THING_NAME);
                Thing thing = thingHashMap.get(ThingKey.CreateKey(settingsProvider.getNodeKey(), thingName));
                String[] data = new Gson().fromJson(bundle.getString(EXTRA_PORT_DATA_ARRAY), String[].class);
                SendPortMsg(thing, data);
            }
            break;
            // -------------------------------------
            case RECEIVE_RX_PORT_EVENT_MSG: {
                PortEventMsg msg = new Gson().fromJson(bundle.getString(EXTRA_UPDATED_MSG), PortEventMsg.class);
                RxPortEventMsg(msg);
            }
            break;
            // -------------------------------------
            case RECEIVE_RX_PORT_STATE_RSP: {
                PortStateRsp rsp = new Gson().fromJson(bundle.getString(EXTRA_UPDATED_MSG), PortStateRsp.class);
                RxPortStateRsp(rsp);
            }
            break;
            // -------------------------------------
            case REQUEST_RX_START: {
                serverAPI.StartRx();
            }
            break;
            // -------------------------------------
            case REQUEST_RX_STOP: {
                serverAPI.StopRx();
            }
            break;
            // -------------------------------------
            case REQUEST_RX_UPDATE: {
                // TODO: Update code for rxUpdate
                /*
                if (!serverAPI.SendNodeThingsReq(new NodeThingsReq(
                        null,
                        eNodeThingsOperation.Get,
                        null,
                        settingsProvider.getNodeKey(),
                        settingsProvider.getNodeSecretKey(),
                        APIVersion,
                        0))) {
                    // We failed to send the request wait 250MS and try again
                    // most probably we are not connected to server !!!!
                    // TODO: find better way to handle retransmitions
                    new Thread(new Runnable() {
                        public void run() {
                            try {
                                Thread.sleep(250);
                            } catch (Exception ex) {
                            }
                            RequesttUpdatedState(getApplicationContext());
                        }
                    }).start();
                }
                */
            }
            break;
        }
    }


    // =============================================================================================
    // Service execution (background thread)

    private void SendNodes(SettingsProvider settingsProvider) {
        try {
            ThingsMsg msg = new ThingsMsg(eThingsOperation.Overwrite,
                    true,
                    thingHashMap.values().toArray(new Thing[0]),
                    APIVersion,
                    SendSeqNum++,
                    0);
            serverAPI.SendThingsMsg(msg);
        } catch (Exception e) {
            Log.e(TAG, e.getMessage());
        }
    }

    // ---------------------------------------------------------------------------------------------

    private void SendPortMsg(Thing thing, int portIndex, String data) {
        if (data != null) {
            //Send REST API request
            PortEventMsg msg = new PortEventMsg();
            msg.PortEvents = new PortEvent[1];

            // Fill the port event for each data
            msg.PortEvents[0] = new PortEvent(
                    thing.Ports.get(portIndex).PortKey,
                    data,
                    0); // TODO: See if we need to have actual sequence number per port

            //Send REST API request
            try {
                serverAPI.SendPortEvent(msg);
            } catch (Exception e) {
                Log.e(TAG, "Failed to send ports event for:" + thing.ThingKey);
            }
        }
    }

    // ---------------------------------------------------------------------------------------------

    private void SendPortMsg(Thing thing, String[] data) {
        if (data != null && data.length > 0) {
            //Send REST API request
            PortEventMsg msg = new PortEventMsg();
            msg.PortEvents = new PortEvent[data.length];

            // Fill the port event for each data
            for (int i = 0; i < data.length; i++) {
                msg.PortEvents[i] = new PortEvent(
                        thing.Ports.get(i).PortKey,
                        data[i],
                        0); // TODO: See if we need to have actual sequence number per port
            }

            //Send REST API request
            try {
                serverAPI.SendPortEvent(msg);
            } catch (Exception e) {
                Log.e(TAG, "Failed to send ports event for:" + thing.ThingKey);
            }
        }
    }

    // ---------------------------------------------------------------------------------------------

    private void RxPortStateRsp(PortStateRsp rsp) {

        if(rsp.Operation == ePortStateOperation.AllPortStates || rsp.Operation == ePortStateOperation.ActivePortStates)
        {
            for(PortState portState : rsp.PortStates)
            {
                // Get the local thing and check if we have new data
                Port localP = PortKeyToPortHashMap.get(portState.PortKey);
                Thing localT = PortKeyToThingsHashMap.get(portState.PortKey);

                // Update the local state
                localP.State = portState.State;
                // TODO: Save port seqno

                // Send the event for this port
                Intent intent = new Intent(BROADCAST_THING_UPDATE);
                intent.putExtra(EXTRA_UPDATED_THING, localT.ThingKey);
                intent.putExtra(EXTRA_UPDATED_THING_NAME, localT.Name);
                intent.putExtra(EXTRA_UPDATED_PORT_ID, localT.Ports.indexOf(localP));
                intent.putExtra(EXTRA_UPDATED_STATE, localP.State );
                intent.putExtra(EXTRA_UPDATED_ISEVENT, false);

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

    private void RxPortEventMsg(PortEventMsg msg) {
        // Now we need to find any changes and update the UI
        for (PortEvent pmsg : msg.PortEvents) {


            // Get the local thing and check if we have new data
            Port localP = PortKeyToPortHashMap.get(pmsg.PortKey);
            Thing localT = PortKeyToThingsHashMap.get(pmsg.PortKey);

            // Update the local state
            localP.State = pmsg.State;
            // TODO: Save port seqno

            // Send the event for this port
            Intent intent = new Intent(BROADCAST_THING_UPDATE);
            intent.putExtra(EXTRA_UPDATED_THING, localT.ThingKey);
            intent.putExtra(EXTRA_UPDATED_THING_NAME, localT.Name);
            intent.putExtra(EXTRA_UPDATED_PORT_ID, localT.Ports.indexOf(localP));
            intent.putExtra(EXTRA_UPDATED_STATE, localP.State );
            intent.putExtra(EXTRA_UPDATED_ISEVENT, true);

            LocalBroadcastManager
                    .getInstance(getApplicationContext())
                    .sendBroadcast(intent);
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

    public static void SendNode(Context context) {
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_SENDNODES);
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    public static void RegisterNode(Context context, Boolean forceUpdate) {
        // TODO: register nodes only when we have some change, keep a dirty flag

        if (!thingsRegistered || forceUpdate) {
            ThingManager.getInstance(context).RegisterThings();

            Intent intent = new Intent(context, NodeService.class);
            intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_SENDNODES);
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

    public static void StartService(Context context, SensorsListener.SensorType type) {
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_SERVICE_START);
        intent.putExtra(EXTRA_SERVICE_TYPE, type);
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    public static void StopService(Context context, SensorsListener.SensorType type) {
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_SERVICE_STOP);
        intent.putExtra(EXTRA_SERVICE_TYPE, type);
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    public static void RxPortStateRsp(Context context, PortStateRsp rsp) {
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, RECEIVE_RX_PORT_STATE_RSP);
        intent.putExtra(EXTRA_UPDATED_MSG, new Gson().toJson(rsp));
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    public static void RxPortEventMsg(Context context, PortEventMsg msg) {
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, RECEIVE_RX_PORT_EVENT_MSG);
        intent.putExtra(EXTRA_UPDATED_MSG, new Gson().toJson(msg));
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    public static void StartRx(Context context) {
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_RX_START);
        context.startService(intent);
        Log.d(TAG, "DEBUG RX Start");
    }

    public static void StopRx(Context context) {
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_RX_STOP);
        context.startService(intent);
        Log.d(TAG, "DEBUG RX Stop");
    }

    // ---------------------------------------------------------------------------------------------

    public static void RequesttUpdatedState(Context context) {
        Intent intent = new Intent(context, NodeService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_RX_UPDATE);
        context.startService(intent);
        Log.d(TAG, "DEBUG RX Update");
    }

    // =============================================================================================
}
