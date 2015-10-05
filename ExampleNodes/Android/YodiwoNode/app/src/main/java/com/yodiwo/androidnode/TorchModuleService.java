package com.yodiwo.androidnode;

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
public class TorchModuleService extends Service {

    public static final String TAG = TorchModuleService.class.getSimpleName();

    private static boolean isTorchOn;
    private static Camera camera;
    private static Parameters params;
    private ThingManager thingManager;

    //==============================================================================================

    public TorchModuleService () {
        super();
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
    public IBinder onBind(Intent intent) {
        //
        Log.i(TAG, "onBind: Shouldn't have been called as this is supposed to be a started service");
        return null;
    }

    @Override
    public void onCreate() {
        thingManager = ThingManager.getInstance(this.getApplicationContext());

        LocalBroadcastManager.getInstance(this).registerReceiver(mBroadcastReceiverTorchModuleService,
                new IntentFilter(NodeService.BROADCAST_THING_UPDATE));
    }

    //==============================================================================================

    private BroadcastReceiver mBroadcastReceiverTorchModuleService = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            String action = intent.getAction();
            Log.i(TAG, "Broadcast received: " + action);

            try {
                if (action.equals(NodeService.BROADCAST_THING_UPDATE)) {
                    Bundle b = intent.getExtras();
                    int portID = b.getInt(NodeService.EXTRA_UPDATED_PORT_ID, -1);
                    String thingKey = b.getString(NodeService.EXTRA_UPDATED_THING_KEY);
                    String thingName = b.getString(NodeService.EXTRA_UPDATED_THING_NAME);
                    String portState = b.getString(NodeService.EXTRA_UPDATED_STATE);
                    Boolean isEvent = b.getBoolean(NodeService.EXTRA_UPDATED_IS_EVENT);


                    // Check if message is for Torch thing
                    if (thingKey.equals(thingManager.GetThingKey(ThingManager.Torch))) {
                        // Log
                        Log.i(TAG, "Broadcast BROADCAST_THING_UPDATE received");

                        // Set actual torch state
                        boolean state = Boolean.parseBoolean(portState);
                        getCamera();
                        setTorch(state);
                        releaseCamera();
                    }
                }
            } catch (Exception ex) {
                Log.e(TAG, "Failed to get update data: " + ex.getMessage());
            }
        }
    };
}
