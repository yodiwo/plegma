/*
 * Copyright (C) 2013 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

package com.mikroe.hexiwear_android;

import android.app.Activity;
import android.bluetooth.BluetoothGattCharacteristic;
import android.bluetooth.BluetoothGattService;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.Bundle;
import android.os.Handler;
import android.provider.Settings;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;
import android.widget.TextView;

import com.yodiwo.androidbleagent.BluetoothLeService;
import com.yodiwo.androidbleagent.DeviceScanActivity;
import com.yodiwo.androidnode.NodeService;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.List;

/**
 * Activity for scanning and displaying available Bluetooth LE devices.
 */
public class IntroActivity extends Activity {

    private static final String TAG = "IntroActivity";

    ////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////// VARIABLES ////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////

    private static final String HEXIWEAR_ADDRESS = "00:44:40:0C:00:41";//"00:04:9F:00:00:01";
    private static final String HEXIWEAR_NAME = "HEXIWEAR";
    private final String LIST_NAME = "NAME";
    private final String LIST_UUID = "UUID";

    public static final String BROADCAST_RX_MSG = "IntroActivity.BROADCAST_RX_MSG";

    public static final String UUID_CHAR_AMBIENT_LIGHT = "00002011-0000-1000-8000-00805f9b34fb";
    public static final String UUID_CHAR_TEMPERATURE   = "00002012-0000-1000-8000-00805f9b34fb";
    public static final String UUID_CHAR_HUMIDITY      = "00002013-0000-1000-8000-00805f9b34fb";
    public static final String UUID_CHAR_PRESSURE      = "00002014-0000-1000-8000-00805f9b34fb";
    public static final String UUID_CHAR_HEARTRATE     = "00002021-0000-1000-8000-00805f9b34fb";
    public static final String UUID_CHAR_BATTERY       = "00002a19-0000-1000-8000-00805f9b34fb";
    public static final String UUID_CHAR_ACCEL  = "00002001-0000-1000-8000-00805f9b34fb";
    public static final String UUID_CHAR_GYRO   = "00002002-0000-1000-8000-00805f9b34fb";
    public static final String UUID_CHAR_MAGNET = "00002003-0000-1000-8000-00805f9b34fb";
    private static final String UUID_CHAR_ALERTIN = "00002031-0000-1000-8000-00805f9b34fb";

    private BluetoothGattCharacteristic alertInCharacteristic;
    private TextView mScanTitle;
    private static BluetoothLeService mBluetoothLeService;
    private Handler mHandler;
    private static ArrayList<ArrayList<BluetoothGattCharacteristic>> mGattCharacteristics = new ArrayList<ArrayList<BluetoothGattCharacteristic>>();

    ////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////// ACTIVITY OVERRIDES ////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        if (getIntent().getBooleanExtra("EXIT", false)) {
            android.os.Process.killProcess(android.os.Process.myPid());
            System.exit(0);
            finish();
            return;
        }

        mHandler = new Handler();
        mScanTitle = (TextView) findViewById(R.id.scanTitle);

        //checkNotificationEnabled();

        // register receivers
        registerReceivers();

        // start notification service
        startService(new Intent(IntroActivity.this, NotificationService.class));

        // launch main scan activity
        DeviceScanActivity.setDevicesOfInterest(new String[] {HEXIWEAR_ADDRESS});
        startActivity(new Intent(this, DeviceScanActivity.class));
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
    }

    @Override
    protected void onResume() {
        super.onResume();
    }

    @Override
    protected void onPause() {
        super.onPause();
    }

    @Override
    public void onBackPressed() {
        super.onBackPressed();
        Intent intent = new Intent(getApplicationContext(), IntroActivity.class);
        intent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
        intent.putExtra("EXIT", true);
        startActivity(intent);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////// BROADCAST RECEIVERS ////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////

    /**
     * onNotice
     */
    private BroadcastReceiver onNotice = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            // String pack = intent.getStringExtra("package");
            String title = intent.getStringExtra("title");
            String text = intent.getStringExtra("text");

            mBluetoothLeService = DeviceScanActivity.getBluetoothLeService();

            if (alertInCharacteristic != null && mBluetoothLeService != null) {
                int charaProp = alertInCharacteristic.getProperties();
                if ((charaProp & BluetoothGattCharacteristic.PROPERTY_WRITE) > 0) {
                    byte[] bytes = Arrays.copyOf(text.getBytes(), 20);
                    alertInCharacteristic.setValue(bytes);

                    while (mBluetoothLeService.writeNoResponseCharacteristic(alertInCharacteristic) == false) {
                        try {
                            Thread.sleep(50);
                        } catch (InterruptedException e) {
                            Log.e(TAG, "InterruptedException");
                        }
                    }
                }
            }
        }
    };

    /**
     * mGattUpdateReceiver
     */
    // Handles various events fired by the Service.
    // ACTION_GATT_CONNECTED: connected to a GATT server.
    // ACTION_GATT_DISCONNECTED: disconnected from a GATT server.
    // ACTION_GATT_SERVICES_DISCOVERED: discovered GATT services.
    // ACTION_DATA_AVAILABLE: received data from the device.  This can be a result of read
    //                        or notification operations.
    private final BroadcastReceiver mGattUpdateReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            final String action = intent.getAction();

            try {
                switch (action) {
                    case BluetoothLeService.ACTION_GATT_CONNECTED:
                        if (mScanTitle != null)
                            mScanTitle.setText("connected");
                        invalidateOptionsMenu();
                        break;
                    case BluetoothLeService.ACTION_GATT_DISCONNECTED:
                        invalidateOptionsMenu();
                        break;
                    case BluetoothLeService.ACTION_GATT_SERVICES_DISCOVERED:
                        // Show all the supported services and characteristics on the user interface.
                        displayGattServices(BluetoothLeService.getSupportedGattServices());
                        // start main screen activity
                        launchMainScreen();
                        break;
                    case BluetoothLeService.ACTION_DATA_AVAILABLE:
                        break;
                    case BluetoothLeService.ACTION_WRITE_RESPONSE_OK:
                    case BluetoothLeService.ACTION_WRITE_RESPONSE_ERROR:
                        break;
                }
            } catch (Exception ex){
                Log.e(TAG, ex.getMessage());
            }
        }
    };

    ////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////// HELPERS ////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////

    // Demonstrates how to iterate through the supported GATT Services/Characteristics.
    // In this sample, we populate the data structure that is bound to the ExpandableListView
    // on the UI.
    private void displayGattServices(List<BluetoothGattService> gattServices) {
        if (gattServices == null) return;
        String uuid = null;
        String unknownServiceString = getResources().getString(R.string.unknown_service);
        String unknownCharaString = getResources().getString(R.string.unknown_characteristic);
        ArrayList<HashMap<String, String>> gattServiceData = new ArrayList<HashMap<String, String>>();
        ArrayList<ArrayList<HashMap<String, String>>> gattCharacteristicData = new ArrayList<ArrayList<HashMap<String, String>>>();
        mGattCharacteristics.clear();
        mGattCharacteristics = new ArrayList<ArrayList<BluetoothGattCharacteristic>>();

        // Loops through available GATT Services.
        try {
            for (BluetoothGattService gattService : gattServices) {
                HashMap<String, String> currentServiceData = new HashMap<String, String>();
                uuid = gattService.getUuid().toString();
                currentServiceData.put(LIST_UUID, uuid);
                gattServiceData.add(currentServiceData);

                ArrayList<HashMap<String, String>> gattCharacteristicGroupData = new ArrayList<HashMap<String, String>>();
                List<BluetoothGattCharacteristic> gattCharacteristics = gattService.getCharacteristics();
                ArrayList<BluetoothGattCharacteristic> charas = new ArrayList<BluetoothGattCharacteristic>();

                // Loops through available Characteristics.
                for (BluetoothGattCharacteristic gattCharacteristic : gattCharacteristics) {

                    charas.add(gattCharacteristic);
                    HashMap<String, String> currentCharaData = new HashMap<String, String>();
                    uuid = gattCharacteristic.getUuid().toString();
                    if (uuid.equals(UUID_CHAR_ALERTIN)) {
                        alertInCharacteristic = gattCharacteristic;
                        byte[] value = {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
                        alertInCharacteristic.setValue(value);
                    }
                    currentCharaData.put(LIST_UUID, uuid);
                    gattCharacteristicGroupData.add(currentCharaData);
                }
                mGattCharacteristics.add(charas);
                gattCharacteristicData.add(gattCharacteristicGroupData);
            }
        } catch (Exception ex){
            Log.e(TAG, ex.getMessage());
        }
    }

    private void launchMainScreen(){
        mHandler.postDelayed(new Runnable() {
            @Override
            public void run() {
                Intent intent = new Intent(IntroActivity.this, MainScreenActivity.class);
                startActivity(intent);
            }
        }, 1500);
    }

    private boolean checkNotificationEnabled() {
        try{
            if(Settings.Secure.getString(this.getContentResolver(),
                    "enabled_notification_listeners").contains(getApplicationContext().getPackageName()))
            {
                return true;
            } else {
                //service is not enabled try to enabled by calling...
                Intent intent=new Intent("android.settings.ACTION_NOTIFICATION_LISTENER_SETTINGS");
                intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                startActivity(intent);
                return false;
            }

        }catch(Exception e) {
            e.printStackTrace();
        }
        return false;
    }

    private void registerReceivers(){
        LocalBroadcastManager
                .getInstance(this)
                .registerReceiver(onNotice, new IntentFilter(IntroActivity.BROADCAST_RX_MSG));
        registerReceiver(mGattUpdateReceiver, makeGattUpdateIntentFilter());
    }

    private IntentFilter makeGattUpdateIntentFilter() {
        final IntentFilter intentFilter = new IntentFilter();
        intentFilter.addAction(BluetoothLeService.ACTION_GATT_CONNECTED);
        intentFilter.addAction(BluetoothLeService.ACTION_GATT_DISCONNECTED);
        intentFilter.addAction(BluetoothLeService.ACTION_GATT_SERVICES_DISCOVERED);
        intentFilter.addAction(BluetoothLeService.ACTION_DATA_AVAILABLE);
        intentFilter.addAction(BluetoothLeService.ACTION_WRITE_RESPONSE_OK);
        intentFilter.addAction(BluetoothLeService.ACTION_WRITE_RESPONSE_ERROR);
        return intentFilter;
    }
}