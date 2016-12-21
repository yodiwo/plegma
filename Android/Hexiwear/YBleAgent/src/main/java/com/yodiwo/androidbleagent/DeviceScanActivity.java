package com.yodiwo.androidbleagent;

import android.Manifest;
import android.annotation.TargetApi;
import android.app.Activity;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothGattCharacteristic;
import android.bluetooth.BluetoothGattService;
import android.bluetooth.BluetoothManager;
import android.bluetooth.le.ScanCallback;
import android.bluetooth.le.ScanResult;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.ServiceConnection;
import android.content.pm.PackageManager;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.os.IBinder;
import android.provider.Settings;
import android.support.annotation.NonNull;
import android.util.Log;
import android.widget.Toast;

import java.util.ArrayList;
import java.util.Collections;
import java.util.HashMap;
import java.util.List;

/**
 * Activity for scanning and displaying available Bluetooth LE devices.
 */
public class DeviceScanActivity extends Activity {

    // =============================================================================================
    // Static information
    // =============================================================================================

    private static final String TAG = DeviceScanActivity.class.getSimpleName();

    private BluetoothAdapter mBluetoothAdapter;
    private Handler mHandler;
    private String mDeviceAddress;

    private static final int REQUEST_ENABLE_BT = 1;
    private static final int PERMISSION_REQUEST_COARSE_LOCATION = 2;
    private static final int REQUEST_ENABLE_LOCATION = 3;

    private static BluetoothLeService mBluetoothLeService;
    private static ArrayList<String> uuidCharsList = new ArrayList<>();
    private static HashMap<String, BluetoothGattService> storedGattService = new HashMap<>();
    private static HashMap<String, BluetoothGattCharacteristic> storedGattCharas = new HashMap<>();
    private static HashMap<String, BluetoothGattCharacteristic> storedUserSpecificCharas = new HashMap<>();
    private static ArrayList<String> devicesMacAddressList = new ArrayList<>();

    // =============================================================================================
    // Activity overrides
    // =============================================================================================

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.device_scan);

        // needed only for version > 6.0
        // TODO: move to main app?
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
            requestPermissions(new String[]{Manifest.permission.ACCESS_COARSE_LOCATION}, PERMISSION_REQUEST_COARSE_LOCATION);
        } else{
            InitDeviceScanActivity();
        }
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        storedGattService.clear();
        storedUserSpecificCharas.clear();
        scanLeDevice(false);
        mBluetoothLeService = null;
        unbindService(mServiceConnection);
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
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        // User chose not to enable Bluetooth.
        if (requestCode == REQUEST_ENABLE_BT && resultCode == Activity.RESULT_CANCELED) {
            finish();
            return;
        }
        if (requestCode == REQUEST_ENABLE_LOCATION && resultCode == Activity.RESULT_OK) {
            InitDeviceScanActivity();
        }
        super.onActivityResult(requestCode, resultCode, data);
    }

    @Override
    public void onRequestPermissionsResult(int requestCode,
                                           @NonNull String permissions[], @NonNull int[] grantResults) {
           switch (requestCode) {
            case PERMISSION_REQUEST_COARSE_LOCATION: {
                if (grantResults[0] == PackageManager.PERMISSION_GRANTED) {
                    InitDeviceScanActivity();
                } else {
                    Intent enableLocationIntent = new Intent(Settings.ACTION_LOCATION_SOURCE_SETTINGS);
                    startActivityForResult(enableLocationIntent, REQUEST_ENABLE_LOCATION);
                }
            }
        }
    }

    // =============================================================================================
    // Init activity
    // =============================================================================================

    private void InitDeviceScanActivity() {
        mHandler = new Handler();

        // Use this check to determine whether BLE is supported on the device.  Then you can
        // selectively disable BLE-related features.
        checkBleSupport();

        // Initializes a Bluetooth adapter.  For API level 18 and above, get a reference to
        // BluetoothAdapter through BluetoothManager.
        checkBthSupport();

        // Ensures Bluetooth is enabled on the device.  If Bluetooth is not currently enabled,
        // fire an intent to display a dialog asking the user to grant permission to enable it.
        enableBth();

        // start scanning + binding
        startUp();
    }

    // =============================================================================================
    // Ble device scanning
    // =============================================================================================

    private void scanLeDevice(boolean enable) {
        try {
            if (enable) {
                startScanning();
            } else {
                stopScanning();
            }
        } catch (Exception ex) {
            Log.e(TAG, "scanLeDevice issue, reason = " + ex);
        }
    }
    // ---------------------------------------------------------------------------------------------

    private void scanLeDevice(boolean enable, int scanPeriod) {
        try {
            if (enable) {
                // Stops scanning after a pre-defined scan period.
                mHandler.postDelayed(new Runnable() {
                    @Override
                    public void run() {
                        stopScanning();
                    }
                }, scanPeriod);

                startScanning();
            } else {
                stopScanning();
            }
        } catch (Exception ex) {
            Log.e(TAG, "scanLeDevice issue, reason = " + ex);
        }
    }
    // ---------------------------------------------------------------------------------------------

    private void stopScanning() {
        Log.d(TAG, "Stop scanning");

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP) {
            mBluetoothAdapter.getBluetoothLeScanner().stopScan(new ScanCallback() {
                @Override
                public void onScanResult(int callbackType, ScanResult result) {
                    Log.d(TAG, "Stop scanning successfully");
                }

                @Override
                public void onScanFailed(int errorCode) {
                    Log.e(TAG, "Error: Ble stop scanning code: " + errorCode);
                }

                @Override
                public void onBatchScanResults(List<ScanResult> results) {
                }
            });
        }
        else
            mBluetoothAdapter.stopLeScan(new BluetoothAdapter.LeScanCallback() {
                @Override
                public void onLeScan(BluetoothDevice device, int rssi, byte[] scanRecord) {
                    Log.d(TAG, "Stop scanning successfully");
                }
            });
    }

    // ---------------------------------------------------------------------------------------------

    private void startScanning(){
        Log.d(TAG, "Start scanning");

        try {
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP) {
                // scan for devices
                mBluetoothAdapter.getBluetoothLeScanner().startScan(new ScanCallback() {
                    @TargetApi(Build.VERSION_CODES.LOLLIPOP)
                    @Override
                    public void onScanResult(int callbackType, ScanResult result) {
                        BluetoothDevice device = result.getDevice();
                        Log.d(TAG, "Successful discovery: deviceUuid = " + device + ", deviceName = " + device.getName());

                        if (!devicesMacAddressList.isEmpty()) {
                            if (devicesMacAddressList.contains(device.getAddress()) && device.getName().equals("HEXIWEAR")) {
                                mDeviceAddress = device.getAddress();
                                if (mBluetoothLeService != null)
                                    mBluetoothLeService.connect(mDeviceAddress);
                            }
                        } else {
                            mDeviceAddress = device.getAddress();
                            if (mBluetoothLeService != null)
                                mBluetoothLeService.connect(mDeviceAddress);
                        }
                    }

                    @Override
                    public void onScanFailed(int errorCode) {
                        Log.e(TAG, "Error: Scan failed code: " + errorCode);
                    }

                    @TargetApi(Build.VERSION_CODES.LOLLIPOP)
                    @Override
                    public void onBatchScanResults(List<ScanResult> results) {
                        Log.d(TAG, "Start scanning successfully");
                        for (ScanResult sr : results) {
                            Log.i(TAG, "ScanResult - Results: deviceName =  " + sr.toString());
                        }
                    }
                });
            }
            else
                mBluetoothAdapter.startLeScan(new BluetoothAdapter.LeScanCallback() {
                    @Override
                    public void onLeScan(BluetoothDevice device, int rssi, byte[] scanRecord) {
                        Log.d(TAG, "Successful discovery: deviceUuid = " + device + ", deviceName = " + device.getName());
                        if (!devicesMacAddressList.isEmpty()) {
                            if (devicesMacAddressList.contains(device.getAddress())) {
                                Log.d(TAG, "Connect to: " + device.getAddress());
                                mDeviceAddress = device.getAddress();
                                if (mBluetoothLeService != null)
                                    mBluetoothLeService.connect(mDeviceAddress);
                            }
                        } else {
                            Log.w(TAG, "Device list is empty.");
                            mDeviceAddress = device.getAddress();
                            if (mBluetoothLeService != null)
                                mBluetoothLeService.connect(mDeviceAddress);
                        }
                    }
                });
        }catch (Exception ex){
            Log.e(TAG, ex.getMessage());
        }
    }

    // =============================================================================================
    // Ble Service lifecycle management.
    // =============================================================================================

    private final ServiceConnection mServiceConnection = new ServiceConnection() {

        @Override
        public void onServiceConnected(ComponentName componentName, IBinder service) {
            Log.d(TAG, "Ble service connected");

            mBluetoothLeService = ((BluetoothLeService.LocalBinder) service).getService();
            if (!mBluetoothLeService.initialize()) {
                finish();
            }
            // Automatically connects to the device upon successful start-up initialization.
            mBluetoothLeService.connect(mDeviceAddress);
        }

        @Override
        public void onServiceDisconnected(ComponentName componentName) {
            mBluetoothLeService = null;
        }
    };

    // =============================================================================================
    // Helpers
    // =============================================================================================

    private void startUp(){
        try {
            // start scanning for LE devices
            scanLeDevice(true);
            // bind activity to BLE service
            Intent gattServiceIntent = new Intent(DeviceScanActivity.this, BluetoothLeService.class);
            bindService(gattServiceIntent, mServiceConnection, BIND_AUTO_CREATE);
        }catch(Exception ex){
            Log.e(TAG, ex.getMessage());
        }
    }

    private void enableBth() {
        if (!mBluetoothAdapter.isEnabled()) {
            if (!mBluetoothAdapter.isEnabled()) {
                Intent enableBtIntent = new Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE);
                startActivityForResult(enableBtIntent, REQUEST_ENABLE_BT);
            }
        }
    }

    private void checkBthSupport() {
        final BluetoothManager bluetoothManager = (BluetoothManager) getSystemService(Context.BLUETOOTH_SERVICE);
        mBluetoothAdapter = bluetoothManager.getAdapter();

        // Checks if Bluetooth is supported on the device.
        if (mBluetoothAdapter == null) {
            Toast.makeText(this, "Error: Bluetooth not supported", Toast.LENGTH_SHORT).show();
            finish();
        }
    }

    private void checkBleSupport() {
        if (!getPackageManager().hasSystemFeature(PackageManager.FEATURE_BLUETOOTH_LE)) {
            Toast.makeText(this, R.string.ble_not_supported, Toast.LENGTH_SHORT).show();
            finish();
        }
    }

    public static void setDevicesOfInterest(String[] deviceList){
        devicesMacAddressList.clear();
        Log.w(TAG, "Device list set");
        if (deviceList != null && deviceList.length > 0)
            Collections.addAll(devicesMacAddressList, deviceList);
    }

    // ---------------------------------------------------------------------------------------------

    public static void storeGattData(List<BluetoothGattService> gattServices) {
        Log.d(TAG, "Trying to keep gatt data");

        if (gattServices == null) {
            Log.w(TAG, "Gatt services not obtained");
            return;
        }

        storedGattService.clear();
        storedGattCharas.clear();
        storedUserSpecificCharas.clear();

        try {
            // Loops through available GATT Services
            for (BluetoothGattService gattService : gattServices) {
                storedGattService.put(gattService.getUuid().toString(), gattService);

                for (BluetoothGattCharacteristic gattCharacteristic : gattService.getCharacteristics()) {
                    String charasUUID = gattCharacteristic.getUuid().toString();
                    if (uuidCharsList.contains(charasUUID)) {
                        storedUserSpecificCharas.put(charasUUID, gattCharacteristic);
                    }
                    storedGattCharas.put(charasUUID, gattCharacteristic);
                }
            }
            Log.d(TAG, "Successful gatt data storage");
        } catch (Exception ex){
            Log.e(TAG, "Unsuccessful gatt data storage, reason = " + ex.getMessage());
        }
    }

    // ---------------------------------------------------------------------------------------------

    public static void setUuidCharacteristicsOfInterest(ArrayList<String> uuidList){
        uuidCharsList = uuidList;
    }

    // ---------------------------------------------------------------------------------------------

    public static HashMap<String, BluetoothGattCharacteristic> getCharacteristicsOfInterest(){
        Log.d(TAG, "Get gatt characteristics of interest");

        return storedUserSpecificCharas.isEmpty() ?
                storedGattCharas : storedUserSpecificCharas;
    }

    // ---------------------------------------------------------------------------------------------

    public static BluetoothLeService getBluetoothLeService() {
        Log.d(TAG, "Get BLE service");
        return mBluetoothLeService;
    }

}