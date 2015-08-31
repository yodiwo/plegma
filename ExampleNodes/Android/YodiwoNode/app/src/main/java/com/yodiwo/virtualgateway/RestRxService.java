package com.yodiwo.virtualgateway;


import android.app.IntentService;
import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;

import com.yodiwo.plegma.PlegmaAPI;
import com.yodiwo.plegma.ThingsReq;
import com.yodiwo.plegma.eThingsOperation;

public class RestRxService  extends IntentService {

    // =============================================================================================
    // Static information's

    public static final String TAG = RestRxService.class.getSimpleName();

    public static final String EXTRA_REQUEST_TYPE = "EXTRA_REQUEST_TYPE";

    public static final int REQUEST_RX_POLLING = 10;
    public static final int REQUEST_RX_START = 11;
    public static final int REQUEST_RX_STOP = 12;

    // =============================================================================================


    private static Boolean rxThreadIsRunning = false;
    private SettingsProvider settingsProvider;
    private long rxTimeStamp = 0;

    private RestServerAPI serverAPI = null;

    public RestRxService() {
        super("RestRxService");
    }

    public RestRxService(String name) {
        super(name);
    }

    @Override
    protected void onHandleIntent(Intent intent) {

        if (settingsProvider == null)
            settingsProvider = SettingsProvider.getInstance(getApplicationContext());

        if (serverAPI == null)
            serverAPI = RestServerAPI.getInstance(getApplicationContext());

        Bundle bundle = intent.getExtras();
        int request_type = bundle.getInt(EXTRA_REQUEST_TYPE);
        switch (request_type) {
            // -------------------------------------
            case REQUEST_RX_POLLING: {
                if (rxThreadIsRunning)
                    RecvUpdateNodes(settingsProvider);
            }
            break;
            // -------------------------------------
            case REQUEST_RX_START: {
                rxThreadIsRunning = true;
                RecvUpdateNodes(settingsProvider);
            }
            break;
            // -------------------------------------
            case REQUEST_RX_STOP: {
                rxThreadIsRunning = false;
            }
            break;
        }
    }

// ---------------------------------------------------------------------------------------------

    private void RecvUpdateNodes(SettingsProvider settingsProvider) {

        Log.d(TAG, "DEBUG RX POLLING");

        try {
            // Get the thing status
            ThingsReq req = new ThingsReq(
                    PlegmaAPI.APIVersion,
                    0,
                    0,
                    eThingsOperation.Get,
                    "",
                    null);

            serverAPI.Send(req);

            Log.d(TAG, "Loop time: " + (System.currentTimeMillis() - rxTimeStamp));
            rxTimeStamp = System.currentTimeMillis();

            // Log.d(TAG, "...");
        } catch (Exception ex) {
            Log.d(TAG, "Failed to get update data.");
        }

        // Send a new update request
        new Thread(new Runnable() {
            public void run() {
                try {
                    Thread.sleep(250);
                } catch (Exception ex) {
                }

                Context context = getApplicationContext();
                Intent intent = new Intent(context, RestRxService.class);
                intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_RX_POLLING);
                context.startService(intent);
            }
        }).start();
    }

    // ---------------------------------------------------------------------------------------------

    public static void StartRx(Context context) {
        Intent intent = new Intent(context, RestRxService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_RX_START);
        context.startService(intent);
        Log.d(TAG, "DEBUG RX Start");
    }

    public static void StopRx(Context context) {
        Intent intent = new Intent(context, RestRxService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_RX_STOP);
        context.startService(intent);
        Log.d(TAG, "DEBUG RX Stop");
    }

    // ---------------------------------------------------------------------------------------------
}
