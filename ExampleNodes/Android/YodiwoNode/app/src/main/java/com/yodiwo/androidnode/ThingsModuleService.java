package com.yodiwo.androidnode;

import android.annotation.TargetApi;
import android.app.IntentService;
import android.app.Service;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.pm.PackageManager;
import android.hardware.camera2.CameraAccessException;
import android.hardware.camera2.CameraCharacteristics;
import android.hardware.camera2.CameraManager;
import android.net.Uri;
import android.os.Build;
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

    CameraManager manager;

    //==============================================================================================

    // Torch
    public static final String EXTRA_TORCH_THING = "EXTRA_TORCH_THING";
    public static final String EXTRA_TORCH_THING_STATE = "EXTRA_TORCH_THING_STATE";
    public static boolean hasTorch;
    private static boolean isTorchOn;

    //ANDROID 6.0+
    private static CameraManager cameraManager = null;
    private static String torchCameraId = null;
    //ANDROID pre-6.0
    private static Camera camera = null;
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
    @TargetApi(Build.VERSION_CODES.LOLLIPOP)
    public static void initTorch(Context context) {
        if (Build.VERSION.SDK_INT < Build.VERSION_CODES.M) {
            hasTorch = context.getPackageManager().hasSystemFeature(PackageManager.FEATURE_CAMERA_FLASH);
        }
        else {
            cameraManager = (CameraManager) context.getSystemService(Context.CAMERA_SERVICE);
            if(cameraManager == null) {
                hasTorch = false;
                return;
            }
            try {
                for (String camId : cameraManager.getCameraIdList()) {
                    if(cameraManager.getCameraCharacteristics(camId).get(CameraCharacteristics.FLASH_INFO_AVAILABLE)) {
                        hasTorch = true;
                        torchCameraId = camId;
                        break;
                    }
                }
            }
            catch (CameraAccessException e) {
                Helpers.logException(TAG, e);
                hasTorch = false;
            }
        }
    }    //==============================================================================================

    // Get camera resource
    public static void resumeTorch(Context context) {
        if (Build.VERSION.SDK_INT < Build.VERSION_CODES.M) {
            if (camera == null) {
                try {
                    camera = Camera.open();
                    params = camera.getParameters();
                } catch (RuntimeException e) {
                    Helpers.logException(TAG, e);
                }
            }
        }
    }

    // Release camera resource
    public static void pauseTorch() {
        if (camera != null) {
            camera.release();
            camera = null;
        }
    }

    // Set torch state
    @TargetApi(Build.VERSION_CODES.LOLLIPOP)
    private void setTorch(boolean state) {

        if (!hasTorch) {
            return;
        }

        if (state == true && !isTorchOn) {

            if (Build.VERSION.SDK_INT < Build.VERSION_CODES.M) {
                params.setFlashMode(Parameters.FLASH_MODE_TORCH);
                camera.setParameters(params);
                camera.startPreview();
            }
            else {
                cameraManager = (CameraManager) getSystemService(Context.CAMERA_SERVICE);
                try {
                    cameraManager.setTorchMode(torchCameraId, true);
                }
                catch (CameraAccessException e) {
                    Helpers.logException(TAG, e);
                    return;
                }
            }

            isTorchOn = true;
        }
        else if(state == false && isTorchOn) {

            if (Build.VERSION.SDK_INT < Build.VERSION_CODES.M) {
                params = camera.getParameters();
                params.setFlashMode(Parameters.FLASH_MODE_OFF);
                camera.setParameters(params);
                camera.stopPreview();
            }
            else {
                cameraManager = (CameraManager) getSystemService(Context.CAMERA_SERVICE);
                try {
                    cameraManager.setTorchMode(torchCameraId, false);
                }
                catch (CameraAccessException e) {
                    Helpers.logException(TAG, e);
                    return;
                }
            }
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
            } catch (Exception e) {
                Helpers.logException(TAG, e);
            }
        }
        // else if { } /* Other things */
    }
}
