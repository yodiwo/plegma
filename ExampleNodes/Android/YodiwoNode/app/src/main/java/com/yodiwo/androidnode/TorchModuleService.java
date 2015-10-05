package com.yodiwo.androidnode;

import android.app.IntentService;
import android.app.Service;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.net.Uri;
import android.os.Bundle;
import android.os.IBinder;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;
import android.hardware.Camera;
import android.hardware.Camera.Parameters;

/**
 * Created by r00tb00t on 10/4/15.
 */
public class TorchModuleService extends IntentService {

    public static final String EXTRA_TORCH_VALUE = "EXTRA_TORCH_VALUE";
    private static final String TAG = "TORCH";

    private static boolean isTorchOn;
    private static Camera camera;
    private static Parameters params;
    private ThingManager thingManager;

    //==============================================================================================

    public TorchModuleService() {
        super("TorchModuleService");
    }

    public TorchModuleService(String name) {
        super(name);
    }
    //==============================================================================================

    // Get camera resource
    private void getCamera() {
        if (camera == null) {
            try {
                camera = Camera.open();
                params = camera.getParameters();
            } catch (RuntimeException e) {
                Log.e("Failed to get camera: ", e.getMessage());
            }
        }
    }

    // Release camera resource
    private void releaseCamera() {
        if (camera != null) {
            camera.release();
            camera = null;
        }
    }

    // Set torch state
    private void setTorch(boolean state) {
        getCamera();
        if (camera == null || params == null) {
            return;
        }

        if (state == true && !isTorchOn) {
            params.setFlashMode(Parameters.FLASH_MODE_TORCH);
            camera.setParameters(params);
            camera.startPreview();

            isTorchOn = true;
        }
        else if(state == false && isTorchOn) {
            params = camera.getParameters();
            params.setFlashMode(Parameters.FLASH_MODE_OFF);
            camera.setParameters(params);
            camera.stopPreview();

            isTorchOn = false;
        }
        releaseCamera();
    }

    //==============================================================================================

    @Override
    protected void onHandleIntent(Intent intent) {

        Bundle bundle = intent.getExtras();
        if(bundle == null)
            return;
        boolean torchValue = bundle.getBoolean(EXTRA_TORCH_VALUE);
        try {
            setTorch(torchValue);
        }
        catch (Exception ex) {
            Log.e(TAG, "Failed to get update data: " + ex.getMessage());
        }
    }
}
