package com.yodiwo.androidnode.core;

import android.annotation.TargetApi;
import android.app.IntentService;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.hardware.Camera;
import android.hardware.Camera.Parameters;
import android.hardware.camera2.CameraManager;
import android.os.Build;
import android.os.Bundle;
import android.util.Log;

import com.yodiwo.androidagent.core.Helpers;

import java.util.List;

/**
 * Created by r00tb00t on 10/4/15.
 */
public class ThingsModuleService extends IntentService {

    private static final String TAG = ThingsModuleService.class.getSimpleName();

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
    public static Camera camera = null;
    private static Parameters params;

    //==============================================================================================

    public ThingsModuleService() {
        super("ThingsModuleService");
    }

    public ThingsModuleService(String name) {
        super(name);
    }

    //==============================================================================================
    // Camera/Torch resource
    //==============================================================================================

    // initialise torch
    @TargetApi(Build.VERSION_CODES.LOLLIPOP)
    public static void initTorch(Context context) {
        hasTorch = false;

        //if (Build.VERSION.SDK_INT < Build.VERSION_CODES.M) {
            hasTorch = context.getPackageManager().hasSystemFeature(PackageManager.FEATURE_CAMERA_FLASH);
//        } else {
//            cameraManager = (CameraManager) context.getSystemService(Context.CAMERA_SERVICE);
//            if (cameraManager == null) {
//                return;
//            }
//            try {
//                for (String camId : cameraManager.getCameraIdList()) {
//                    if (cameraManager.getCameraCharacteristics(camId).get(CameraCharacteristics.FLASH_INFO_AVAILABLE)) {
//                        hasTorch = true;
//                        torchCameraId = camId;
//                        break;
//                    }
//                }
//            } catch (CameraAccessException e) {
//                Helpers.logException(TAG, e);
//            }
//        }
    }


    // Get camera resource
    public static void resumeCamera(Context context) {
        //if (Build.VERSION.SDK_INT < Build.VERSION_CODES.M) {
            if (camera == null) {
                try {
                    camera = Camera.open();
                    params = camera.getParameters();
                } catch (RuntimeException e) {
                    Helpers.logException(TAG, e);
                }
            }
        //}
    }

    // Release camera resource
    public static void releaseCamera() {
        if (camera != null) {
            camera.release();
            camera = null;
        }
    }

    // Set torch state
    @TargetApi(Build.VERSION_CODES.LOLLIPOP)
    public static void setTorch(Context context, boolean state, boolean allowRelease) {

        if (!hasTorch) {
            return;
        }

        //if (Build.VERSION.SDK_INT < Build.VERSION_CODES.M) {

            try {

                // get camera resources
                ThingsModuleService.resumeCamera(context);

                if (state) {
                    params.setFlashMode(Parameters.FLASH_MODE_TORCH);
                    camera.setParameters(params);
                    camera.startPreview();
                } else {
                    params = camera.getParameters();
                    params.setFlashMode(Parameters.FLASH_MODE_OFF);
                    camera.setParameters(params);
                    camera.stopPreview();
                    // release camera resources
                    if (allowRelease)
                        ThingsModuleService.releaseCamera();
                }
            } catch (Exception e) {
                Helpers.logException(TAG, e);
            }


//        } else {    //ANDROID M (6.0) and onwards
//
//            try {
//                cameraManager = (CameraManager) context.getSystemService(Context.CAMERA_SERVICE);
//                if (cameraManager != null)
//                    cameraManager.setTorchMode(torchCameraId, state);
//                else
//                    Helpers.log(Log.ERROR, TAG, "Could not get camera manager");
//            } catch (CameraAccessException e) {
//                Helpers.logException(TAG, e);
//            }
//        }
    }

    public static void setResolution(Context context){
        // get camera resources
        ThingsModuleService.resumeCamera(context);

        // set resolution
        //if (Build.VERSION.SDK_INT < Build.VERSION_CODES.M) {
            try {
                params = camera.getParameters();
                List<Camera.Size> pictureSizes = params.getSupportedPictureSizes();
                Camera.Size pictureSize = pictureSizes.get(pictureSizes.size() - 1); // .... select the lowest one
                params.setPictureSize(pictureSize.width, pictureSize.height);

                List<Camera.Size> previewSizes = params.getSupportedPreviewSizes();
                Camera.Size previewSize = previewSizes.get(previewSizes.size() - 1); // .... select the lowest one
                params.setPictureSize(previewSize.width, previewSize.height);

                camera.setParameters(params);
            } catch (Exception ex){
                Helpers.logException(TAG, ex);
            }
//        } else {
//            cameraManager = (CameraManager) context.getSystemService(Context.CAMERA_SERVICE);
//            // TODO: set resolution
//        }
    }

    public static List<Camera.Size> getResolutions(Context context){
        List<Camera.Size> pictureSizes = null;

        // get camera resources
        ThingsModuleService.resumeCamera(context);

        // set resolution
        //if (Build.VERSION.SDK_INT < Build.VERSION_CODES.M) {
            try {
                params = camera.getParameters();
                pictureSizes = params.getSupportedPictureSizes();
            } catch (Exception ex){
                Helpers.logException(TAG, ex);
            }
//        } else {
//            cameraManager = (CameraManager) context.getSystemService(Context.CAMERA_SERVICE);
//            // TODO: get resolution
//        }
        return pictureSizes;
    }

    //==============================================================================================
    // Intent handling
    //==============================================================================================

    @Override
    protected void onHandleIntent(Intent intent) {

        Bundle bundle = intent.getExtras();
        if (bundle == null) {
            return;
        }

        String forThing = bundle.getString(EXTRA_INTENT_FOR_THING);
        if (forThing == null) {
            Helpers.log(Log.ERROR, TAG, "Received unknown intent type");
            return;
        }

        // handling for specific Things
        if (forThing.compareTo(EXTRA_TORCH_THING) == 0) {

            boolean torchState = bundle.getBoolean(EXTRA_TORCH_THING_STATE);
            try {
                setTorch(getApplicationContext(), torchState, true);
            } catch (Exception e) {
                Helpers.logException(TAG, e);
            }
        }
        // else if { } /* Other things */
    }

    // =============================================================================================
}
