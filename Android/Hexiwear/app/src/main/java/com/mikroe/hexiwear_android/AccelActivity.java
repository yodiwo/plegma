package com.mikroe.hexiwear_android;

import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.Bundle;

import com.yodiwo.androidbleagent.BluetoothLeService;
import com.yodiwo.androidbleagent.YodiwoService;
import com.yodiwo.androidnode.NodeService;
import com.yodiwo.androidnode.ThingManager;

import java.util.ArrayList;

public class AccelActivity extends Activity {

    private CustomProgress_Vertical progressBarX;
    private CustomProgress_Vertical progressBarY;
    private CustomProgress_Vertical progressBarZ;

    private YodiwoService hexiwearService;

    private final ArrayList<String> uuidArray = new ArrayList<String>();

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.accel_screen);

        uuidArray.add(IntroActivity.UUID_CHAR_ACCEL);

        progressBarX = (CustomProgress_Vertical) findViewById(R.id.accelProgressX);
        progressBarY = (CustomProgress_Vertical) findViewById(R.id.accelProgressY);
        progressBarZ = (CustomProgress_Vertical) findViewById(R.id.accelProgressZ);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////
    @Override

    protected void onResume() {
        super.onResume();
        hexiwearService = new YodiwoService(uuidArray);
        hexiwearService.readCharStart(10);
        registerReceiver(mGattUpdateReceiver, makeGattUpdateIntentFilter());
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////
    @Override

    protected void onPause() {
        super.onPause();
        hexiwearService.readCharStop();
        unregisterReceiver(mGattUpdateReceiver);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////1//////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////
    @Override

    protected void onDestroy() {

        super.onDestroy();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////

    private void displayCharData(String uuid, byte[] data) {
        int tmpLong;
        float tmpFloat;
        String[] portStates = new String[3];

        if (uuid.equals(IntroActivity.UUID_CHAR_ACCEL)) {
            tmpLong = (((int)data[1]) << 8) | (data[0] & 0xff);
            tmpFloat = (float)tmpLong / 100;
            portStates[0] = String.valueOf(tmpFloat);
            progressBarX.setProgressTitle(portStates[0] + "g");
            tmpLong += (progressBarX.getProgressMax() >> 1);
            if(tmpLong > progressBarX.getProgressMax()) {
                tmpLong = progressBarX.getProgressMax();
            }
            progressBarX.setProgressValue(tmpLong);

            tmpLong = (((int)data[3]) << 8) | (data[2] & 0xff);
            tmpFloat = (float)tmpLong / 100;
            portStates[1] = String.valueOf(tmpFloat);
            progressBarY.setProgressTitle(portStates[1] + "g");

            tmpLong += (progressBarY.getProgressMax() >> 1);
            if(tmpLong > progressBarY.getProgressMax()) {
                tmpLong = progressBarY.getProgressMax();
            }
            progressBarY.setProgressValue(tmpLong);

            tmpLong = (((int)data[5]) << 8) | (data[4] & 0xff);
            tmpFloat = (float)tmpLong / 100;
            portStates[2] = String.valueOf(tmpFloat);
            progressBarZ.setProgressTitle(portStates[2] + "g");
            tmpLong += (progressBarZ.getProgressMax() >> 1);
            if(tmpLong > progressBarZ.getProgressMax()) {
                tmpLong = progressBarZ.getProgressMax();
            }
            progressBarZ.setProgressValue(tmpLong);

            // Send batch port event to Yodiwo Cloud
            NodeService.SendPortMsg(this, ThingManager.HexiwearAccel, portStates, 0);
        }

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////

    private static IntentFilter makeGattUpdateIntentFilter() {
        final IntentFilter intentFilter = new IntentFilter();
        intentFilter.addAction(BluetoothLeService.ACTION_GATT_DISCONNECTED);
        intentFilter.addAction(BluetoothLeService.ACTION_DATA_AVAILABLE);
        return intentFilter;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Handles various events fired by the Service.
    // ACTION_GATT_DISCONNECTED: disconnected from a GATT server.
    // ACTION_DATA_AVAILABLE: received data from the device.  This can be a result of read
    //                        or notification operations.

    private final BroadcastReceiver mGattUpdateReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            final String action = intent.getAction();
            if (BluetoothLeService.ACTION_GATT_DISCONNECTED.equals(action)) {
                invalidateOptionsMenu();
                Intent intentAct = new Intent(AccelActivity.this, IntroActivity.class);
                startActivity(intentAct);
            } else if (BluetoothLeService.ACTION_DATA_AVAILABLE.equals(action)) {
                byte[] data = intent.getByteArrayExtra(BluetoothLeService.EXTRA_DATA);
                String uuid = intent.getStringExtra(BluetoothLeService.EXTRA_CHAR);
                displayCharData(uuid, data);
            }
        }
    };
}
