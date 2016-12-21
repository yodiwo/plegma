package com.yodiwo.androidbleagent;


import android.bluetooth.BluetoothGattCharacteristic;
import android.util.Log;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;
import java.util.Timer;
import java.util.TimerTask;

public class YodiwoService {

    // =============================================================================================
    // Static information
    // =============================================================================================

    private static final String TAG = YodiwoService.class.getSimpleName();

    private BluetoothLeService mBluetoothLeService;
    private HashMap<String, BluetoothGattCharacteristic> mGattCharacteristics = new HashMap<>();

    private final ArrayList<BluetoothGattCharacteristic> charas = new ArrayList<BluetoothGattCharacteristic>();

    private int charCnt = 0;
    private Timer myTimer;

    // ---------------------------------------------------------------------------------------------

    public YodiwoService(ArrayList<String> uuidArray) {
        mGattCharacteristics = DeviceScanActivity.getCharacteristicsOfInterest();
        mBluetoothLeService = DeviceScanActivity.getBluetoothLeService();

        if (uuidArray == null || uuidArray.isEmpty()) {
            charas.addAll(mGattCharacteristics.values());
        }
        else{
            for (Map.Entry<String, BluetoothGattCharacteristic> entry : mGattCharacteristics.entrySet()) {
                if (uuidArray.contains(entry.getValue().getUuid().toString())) {
                    charas.add(entry.getValue());
                }
            }
        }
    }

    // =============================================================================================
    // Timer task
    // =============================================================================================

    private class ReadCharTask extends TimerTask {
        public void run() {
            if (charas.size() > 0) {
                readCharacteristic(charas.get(charCnt++));
                if (charCnt == charas.size()) {
                    charCnt = 0;
                }
            }
        }
    }

    // ---------------------------------------------------------------------------------------------

    public void readCharStart(long interval) {
        myTimer = new Timer();
        ReadCharTask readCharTask = new ReadCharTask();
        myTimer.schedule(readCharTask, 200, interval);
    }

    // ---------------------------------------------------------------------------------------------

    public void readCharStop() {
        myTimer.cancel();
    }

    // =============================================================================================
    // Helpers
    // =============================================================================================

    private void readCharacteristic(BluetoothGattCharacteristic characteristic) {
        if (characteristic != null) {
            final int charaProp = characteristic.getProperties();

            if ((charaProp & BluetoothGattCharacteristic.PROPERTY_READ) > 0) {
                while(mBluetoothLeService.readCharacteristic(characteristic) == false) {
                    try {
                        Thread.sleep(50);
                    }
                    catch (InterruptedException e) {
                        Log.e(TAG, "InterruptedException");
                    }
                }
            }
        }
    }
}
