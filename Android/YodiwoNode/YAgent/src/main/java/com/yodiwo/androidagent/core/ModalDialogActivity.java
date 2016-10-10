package com.yodiwo.androidagent.core;

import android.app.Activity;
import android.bluetooth.BluetoothAdapter;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.graphics.Color;
import android.graphics.Typeface;
import android.hardware.Sensor;
import android.hardware.SensorManager;
import android.hardware.camera2.CameraCharacteristics;
import android.hardware.camera2.CameraManager;
import android.location.LocationManager;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.nfc.NfcAdapter;
import android.os.Build;
import android.os.Bundle;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;

import com.yodiwo.androidagent.R;

/**
 * Created by vaskanas on 05-Feb-16.
 */
public class ModalDialogActivity extends Activity implements View.OnClickListener {

    private static final String TAG = ModalDialogActivity.class.getSimpleName();

    public static boolean isShown = false;
    public static boolean buttonClicked = false;

    private Context context;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.modal_dialog);

        context = getApplicationContext();

        try {
            Helpers.log(Log.INFO, TAG, "onCreate called");

            setModalView();
            Button ok_btn = (Button) findViewById(R.id.modal_dialog_ok_button);
            ok_btn.setOnClickListener(this);
        }
        catch(Exception ex){
            Helpers.logException(TAG, ex);
        }
    }

    @Override
    public void onBackPressed(){
        super.onBackPressed();
    }

    @Override
    public void onStart() {
        super.onStart();
    }

    @Override
    public void onStop() {
        super.onStop();
    }

    @Override
    public void onDestroy() {
        super.onDestroy();
    }


    @Override
    public void onClick(View v) {
        if (v.getId() == R.id.modal_dialog_ok_button) {
            isShown = true;

            // Broadcast the finish of the first pairing
            LocalBroadcastManager
                .getInstance(getApplicationContext())
                .sendBroadcast(new Intent(PairingService.BROADCAST_MODAL_DIALOG_FINISHED));

            this.finish();
        }
    }

    private void setModalView() {

        SensorManager mSensorManager = (SensorManager) getSystemService(SENSOR_SERVICE);

        try {
            // network status
            TextView networkStatus = (TextView) findViewById(R.id.modal_dialog_network_status);
            ConnectivityManager cm = (ConnectivityManager) context.getSystemService(Context.CONNECTIVITY_SERVICE);
            NetworkInfo activeNetInfo = cm.getActiveNetworkInfo();
            if (activeNetInfo != null) {
                if ((activeNetInfo.getTypeName().equalsIgnoreCase("WIFI")) && (activeNetInfo.isConnected())) {
                    networkStatus.setText(R.string.wifi);
                    networkStatus.setTextColor(Color.WHITE);
                } else if ((activeNetInfo.getTypeName().equalsIgnoreCase("MOBILE")) && (activeNetInfo.isConnected())) {
                    networkStatus.setText(R.string.mobile);
                    networkStatus.setTextColor(Color.WHITE);
                } else
                    networkStatus.setText(R.string.disabled);
            } else {
                networkStatus.setText(R.string.not_supported);
                networkStatus.setTypeface(null, Typeface.ITALIC);
            }

            // location status
            TextView locationStatus = (TextView) findViewById(R.id.modal_dialog_location_status);
            LocationManager locationManager = (LocationManager) context.getSystemService(Context.LOCATION_SERVICE);
            if (locationManager != null) {
                boolean isGPSEnabled = locationManager.isProviderEnabled(LocationManager.GPS_PROVIDER);
                boolean isNetworkLocationEnabled = locationManager.isProviderEnabled(LocationManager.NETWORK_PROVIDER);
                if (isGPSEnabled && !isNetworkLocationEnabled){
                    locationStatus.setText(R.string.gps);
                    locationStatus.setTextColor(Color.WHITE);
                }
                else if (!isGPSEnabled && isNetworkLocationEnabled){
                        locationStatus.setText(R.string.Network);
                        locationStatus.setTextColor(Color.WHITE);
                }
                else if (isGPSEnabled && isNetworkLocationEnabled){
                    locationStatus.setText(R.string.gps_network);
                    locationStatus.setTextColor(Color.WHITE);
                }
                else{
                    locationStatus.setText(R.string.disabled);
                }
            } else {
                locationStatus.setText(R.string.not_supported);
                locationStatus.setTypeface(null, Typeface.ITALIC);
            }

            // bluetooth status
            TextView bleStatus = (TextView) findViewById(R.id.modal_dialog_ble_status);
            if (BluetoothAdapter.getDefaultAdapter() == null) {
                bleStatus.setText(R.string.not_supported);
                bleStatus.setTypeface(null, Typeface.ITALIC);
            } else {
                if (!BluetoothAdapter.getDefaultAdapter().isEnabled())
                    bleStatus.setText(R.string.disabled);
                else {
                    bleStatus.setText(R.string.enabled);
                    bleStatus.setTextColor(Color.WHITE);
                }
            }

            // NFC status
            TextView nfcStatus = (TextView) findViewById(R.id.modal_dialog_nfc_status);
            NfcAdapter mNfcAdapter = NfcAdapter.getDefaultAdapter(context);
            if ((mNfcAdapter != null) && (NfcAdapter.getDefaultAdapter(context).isEnabled())) {
                nfcStatus.setText(R.string.enabled);
                nfcStatus.setTextColor(Color.WHITE);
            }
            else if ((mNfcAdapter != null) && (!NfcAdapter.getDefaultAdapter(context).isEnabled())) {
                nfcStatus.setText(R.string.disabled);
            }
            else {
                nfcStatus.setText(R.string.not_supported);
                nfcStatus.setTypeface(null, Typeface.ITALIC);
            }

            // camera status
            boolean hasCamera;
            TextView cameraStatus = (TextView) findViewById(R.id.modal_dialog_camera_status);

            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1) {
                hasCamera = context.getPackageManager().hasSystemFeature(PackageManager.FEATURE_CAMERA_ANY);
            }
            else{
                CameraManager cameraManager = null;
                if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.LOLLIPOP) {
                    cameraManager = (CameraManager) getSystemService(Context.CAMERA_SERVICE);
                }
                hasCamera = cameraManager != null;
            }

            if (hasCamera){
                cameraStatus.setText(R.string.supported);
                cameraStatus.setTextColor(Color.WHITE);
            }
            else{
                cameraStatus.setText(R.string.not_supported);
                cameraStatus.setTypeface(null, Typeface.ITALIC);
            }

            // torch status
            TextView torchStatus = (TextView) findViewById(R.id.modal_dialog_torch_status);
            if (Build.VERSION.SDK_INT < Build.VERSION_CODES.M) {
                if (context.getPackageManager().hasSystemFeature(PackageManager.FEATURE_CAMERA_FLASH)){
                    torchStatus.setText(R.string.supported);
                    torchStatus.setTextColor(Color.WHITE);
                }
                else{
                    torchStatus.setText(R.string.not_supported);
                    torchStatus.setTypeface(null, Typeface.ITALIC);
                }
            }
            else {
                CameraManager cameraManager = (CameraManager) getSystemService(Context.CAMERA_SERVICE);
                boolean found = false;
                try {
                    for (String camId : cameraManager.getCameraIdList()) {
                        if (cameraManager.getCameraCharacteristics(camId).get(CameraCharacteristics.FLASH_INFO_AVAILABLE)) {
                            torchStatus.setText(R.string.supported);
                            torchStatus.setTextColor(Color.WHITE);
                            found = true;
                            break;
                        }
                    }
                }catch (Exception ex){
                    Helpers.logException(TAG, ex) ;
                }
                if(!found) {
                    torchStatus.setText(R.string.not_supported);
                    torchStatus.setTypeface(null, Typeface.ITALIC);
                }
            }

            // Accelerometer status
            TextView accStatus = (TextView) findViewById(R.id.modal_dialog_accelerometer_status);
            if (mSensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER) != null) {
                accStatus.setText(R.string.supported);
                accStatus.setTextColor(Color.WHITE);
            } else {
                accStatus.setText(R.string.not_supported);
                accStatus.setTypeface(null, Typeface.ITALIC);
            }
            // Light sensor status
            TextView lightStatus = (TextView) findViewById(R.id.modal_dialog_brightness_status);
            if (mSensorManager.getDefaultSensor(Sensor.TYPE_LIGHT) != null) {
                lightStatus.setText(R.string.supported);
                lightStatus.setTextColor(Color.WHITE);
            } else {
                lightStatus.setText(R.string.not_supported);
                lightStatus.setTypeface(null, Typeface.ITALIC);
            }

            // gyroscope status
            TextView gyroStatus = (TextView) findViewById(R.id.modal_dialog_gyroscope_status);
            if (mSensorManager.getDefaultSensor(Sensor.TYPE_GYROSCOPE) != null) {
                gyroStatus.setText(R.string.supported);
                gyroStatus.setTextColor(Color.WHITE);
            } else {
                gyroStatus.setText(R.string.not_supported);
                gyroStatus.setTypeface(null, Typeface.ITALIC);
            }

            // rotation status
            TextView rotStatus = (TextView) findViewById(R.id.modal_dialog_rotation_status);
            if (mSensorManager.getDefaultSensor(Sensor.TYPE_ROTATION_VECTOR) != null) {
                rotStatus.setText(R.string.supported);
                rotStatus.setTextColor(Color.WHITE);
            } else {
                rotStatus.setText(R.string.not_supported);
                rotStatus.setTypeface(null, Typeface.ITALIC);
            }

            // proximity status
            TextView proxStatus = (TextView) findViewById(R.id.modal_dialog_proximity_status);
            if (mSensorManager.getDefaultSensor(Sensor.TYPE_PROXIMITY) != null) {
                proxStatus.setText(R.string.supported);
                proxStatus.setTextColor(Color.WHITE);
            } else {
                proxStatus.setText(R.string.not_supported);
                proxStatus.setTypeface(null, Typeface.ITALIC);
            }
        } catch(Exception ex){
            Helpers.logException(TAG, ex);
        }
    }
}

