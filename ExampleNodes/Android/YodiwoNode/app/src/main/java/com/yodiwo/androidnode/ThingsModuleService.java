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
public class ThingsModuleService extends IntentService {

    private static final String TAG = "ThingsModuleService";
    public static final String EXTRA_INTENT_FOR_THING = "EXTRA_INTENT_FOR_THING";

    //==============================================================================================

    // Torch
    public static final String EXTRA_TORCH_THING = "EXTRA_TORCH_THING";
    public static final String EXTRA_TORCH_THING_STATE = "EXTRA_TORCH_THING_STATE";
    public static boolean hasTorch;
    private static boolean isTorchOn;
    private static Camera camera;
    private static Parameters params;

    //==============================================================================================

    public ThingsModuleService() {
        super("ThingsModuleService");
    }

    public ThingsModuleService(String name) {
        super(name);
    }

    //==============================================================================================

    // Get camera resource
    public static void getCamera() {
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
    public static void releaseCamera() {
        if (camera != null) {
            camera.release();
            camera = null;
        }
    }

    // Set torch state
    private void setTorch(boolean state) {
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
    }

    //==============================================================================================

    @Override
    protected void onHandleIntent(Intent intent) {

        Bundle bundle = intent.getExtras();
        if(bundle == null) {
            return;
        }

        String forThing = bundle.getString(EXTRA_INTENT_FOR_THING);
        if (forThing == null) {
            Log.e(TAG, "Received unknown intent type");
            return;
        }

        if(forThing.compareTo(EXTRA_TORCH_THING) == 0) {
            boolean state = bundle.getBoolean(EXTRA_TORCH_THING_STATE);

            boolean torchState = bundle.getBoolean(EXTRA_TORCH_THING_STATE);
            try {
                setTorch(torchState);
            } catch (Exception ex) {
                Log.e(TAG, "Failed to set torch state: " + ex.getMessage());
            }
        }
        // else if { } /* Other things */
    }
}
