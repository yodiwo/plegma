package com.yodiwo.androidnode.core;

import android.Manifest;
import android.annotation.SuppressLint;
import android.app.Activity;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.pm.PackageManager;
import android.graphics.BitmapFactory;
import android.graphics.drawable.ColorDrawable;
import android.graphics.drawable.Drawable;
import android.graphics.drawable.GradientDrawable;
import android.graphics.drawable.ShapeDrawable;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.net.Uri;
import android.net.wifi.WifiInfo;
import android.net.wifi.WifiManager;
import android.os.Bundle;
import android.os.Environment;
import android.provider.MediaStore;
import android.support.annotation.NonNull;
import android.support.v4.app.ActivityCompat;
import android.support.v4.app.Fragment;
import android.support.v4.content.LocalBroadcastManager;
import android.text.TextUtils;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.SurfaceHolder;
import android.view.SurfaceView;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.CompoundButton;
import android.widget.ImageButton;
import android.widget.ProgressBar;
import android.widget.SeekBar;
import android.widget.Switch;
import android.widget.TextView;
import android.widget.Toast;

import com.google.gson.Gson;
import com.yodiwo.androidagent.core.Helpers;
import com.yodiwo.androidagent.core.NodeService;
import com.yodiwo.androidagent.core.PairingService;
import com.yodiwo.androidagent.core.SettingsProvider;
import com.yodiwo.androidagent.core.ThingManager;
import com.yodiwo.androidagent.core.aServerAPI;
import com.yodiwo.androidagent.plegma.BinaryResourceDescriptor;
import com.yodiwo.androidagent.plegma.HttpLocationDescriptor;
import com.yodiwo.androidagent.plegma.eBinaryResourceContentType;
import com.yodiwo.androidagent.plegma.eBinaryResourceLocationType;
import com.yodiwo.androidagent.plegma.eRestServiceType;
import com.yodiwo.androidnode.R;

import java.io.File;
import java.util.ArrayList;
import java.util.Set;


/**
 * A placeholder fragment containing a simple view.
 */
public class MainActivityFragment extends Fragment implements SurfaceHolder.Callback {

    // =============================================================================================
    // Variables
    // =============================================================================================

    public static final String TAG = MainActivityFragment.class.getSimpleName();

    private ThingManager thingManager;
    private static View view;
    /**
     * The absence of a connection type.
     */
    private static final int NO_CONNECTION_TYPE = -1;
    /**
     * The last processed network type.
     */
    private static int sLastType = NO_CONNECTION_TYPE;
    private static Boolean isRegistered = false;

    // =============================================================================================
    // Constructor
    // =============================================================================================

    public MainActivityFragment() {
    }

    // =============================================================================================
    // SurfaceHolder Overrides
    // =============================================================================================

    @Override
    public void surfaceCreated(SurfaceHolder holder) {
        Helpers.log(Log.INFO, TAG, "surfaceCreated called");
    }

    @Override
    public void surfaceChanged(SurfaceHolder holder, int format, int width, int height) {
        Helpers.log(Log.INFO, TAG, "surfaceChanged called");
    }

    @Override
    public void surfaceDestroyed(SurfaceHolder holder) {
        Helpers.log(Log.INFO, TAG, "surfaceDestroyed called");
    }

    // =============================================================================================
    // Fragment Overrides
    // =============================================================================================

    @Override
    public void onPause() {
        super.onPause();
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
    }

    @Override
    public void onResume() {
        super.onResume();
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {

        // inflate view
        view = null;
        try {
            view = inflater.inflate(R.layout.fragment_main, container, false);
        } catch (Exception ex) {
            Helpers.logException(TAG, ex);
            return view;
        }

        // generateFetcher context
        final Context context = this.getActivity().getApplicationContext();

        // generateFetcher thingManager instance
        thingManager = ThingManager.getInstance(context);

        // Link UI elements
        LinkElementsUI(this.getActivity());

        // register receivers
        registerReceivers(context);

        // UI elements to signify connectivity
        TextView outputStr = (TextView) view.findViewById(R.id.textView);
        TextView inputStr = (TextView) view.findViewById(R.id.textView2);
        outputStr.setAlpha(0.3f);
        inputStr.setAlpha(0.3f);

        view.invalidate();
        return view;
    }

    // =============================================================================================
    // (un)Register receivers
    // =============================================================================================
    private void registerReceivers(Context context){
        if (!isRegistered) {

            LocalBroadcastManager.getInstance(context).registerReceiver(mMsgRxMainActivityService,
                    new IntentFilter(NodeService.BROADCAST_THING_UPDATE));
            LocalBroadcastManager.getInstance(context).registerReceiver(mMsgRxMainActivityService,
                    new IntentFilter(NodeService.BROADCAST_ACTIVE_PORT_UPDATE));
            LocalBroadcastManager.getInstance(context).registerReceiver(mMsgRxMainActivityService,
                    new IntentFilter(NodeService.BROADCAST_CONFIGURATION_THING_UPDATE));
            LocalBroadcastManager.getInstance(context).registerReceiver(mMsgRxMainActivityService,
                    new IntentFilter(NodeService.BROADCAST_SEND_UPDATES));
            LocalBroadcastManager.getInstance(context).registerReceiver(mMsgRxMainActivityService,
                    new IntentFilter(NodeService.BROADCAST_NODE_UNPAIRING));
            LocalBroadcastManager.getInstance(context).registerReceiver(mMsgRxMainActivityService,
                    new IntentFilter(aServerAPI.CONNECTIVITY_UI_UPDATE));

            // WiFi-related
            context.registerReceiver(mMsgRxMainActivityService,
                    new IntentFilter(ConnectivityManager.CONNECTIVITY_ACTION));

            // Bluetooth-related
            context.registerReceiver(mMsgRxMainActivityService,
                    new IntentFilter(BluetoothDevice.ACTION_FOUND));
            context.registerReceiver(mMsgRxMainActivityService,
                    new IntentFilter(BluetoothAdapter.ACTION_STATE_CHANGED));
            context.registerReceiver(mMsgRxMainActivityService,
                    new IntentFilter(BluetoothAdapter.ACTION_DISCOVERY_STARTED));
            context.registerReceiver(mMsgRxMainActivityService,
                    new IntentFilter(BluetoothAdapter.ACTION_DISCOVERY_FINISHED));
            context.registerReceiver(mMsgRxMainActivityService,
                    new IntentFilter(BluetoothAdapter.ACTION_CONNECTION_STATE_CHANGED));

            isRegistered = true;
        }
    }

    private void unregisterReceivers(Context context){
        if (isRegistered && mMsgRxMainActivityService != null) {
            context.unregisterReceiver(mMsgRxMainActivityService);
            isRegistered = false;
        }
    }
    // =============================================================================================
    // Link UI elements
    // =============================================================================================

    private void LinkElementsUI(final Activity activity) {
        SeekBar outputSeekBar = (SeekBar) view.findViewById(R.id.seekBar);
        Switch outputSwitch1 = (Switch) view.findViewById(R.id.output_switch1);
        Switch outputSwitch2 = (Switch) view.findViewById(R.id.output_switch2);
        Switch outputSwitch3 = (Switch) view.findViewById(R.id.output_switch3);
        Button gpsButton = (Button) view.findViewById(R.id.button_GPS);
        Button cameraButton = (Button) view.findViewById(R.id.camera_button);
        Button button1 = (Button) view.findViewById(R.id.button1);
        Button button2 = (Button) view.findViewById(R.id.button2);
        Button button3 = (Button) view.findViewById(R.id.button3);

        final Context context = activity.getApplicationContext();

        // Link button1 to code
        button1.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View view, MotionEvent motionEvent) {
                int action = motionEvent.getAction();
                if (action == MotionEvent.ACTION_DOWN)
                    NodeService.SendPortMsg(context, MainActivity.Buttons, MainActivity.ButtonPort0, NodeService.PortValue_Boolean_True);
                else if (action == MotionEvent.ACTION_UP)
                    NodeService.SendPortMsg(context, MainActivity.Buttons, MainActivity.ButtonPort0, NodeService.PortValue_Boolean_False);
                return false;
            }
        });

        // Link button2 to code
        button2.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View view, MotionEvent motionEvent) {
                int action = motionEvent.getAction();
                if (action == MotionEvent.ACTION_DOWN)
                    NodeService.SendPortMsg(context, MainActivity.Buttons, MainActivity.ButtonPort1, NodeService.PortValue_Boolean_True);
                else if (action == MotionEvent.ACTION_UP)
                    NodeService.SendPortMsg(context, MainActivity.Buttons, MainActivity.ButtonPort1, NodeService.PortValue_Boolean_False);
                return false;
            }
        });

        // Link button3 to code
        button3.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View view, MotionEvent motionEvent) {
                int action = motionEvent.getAction();
                if (action == MotionEvent.ACTION_DOWN)
                    NodeService.SendPortMsg(context, MainActivity.Buttons, MainActivity.ButtonPort2, NodeService.PortValue_Boolean_True);
                else if (action == MotionEvent.ACTION_UP)
                    NodeService.SendPortMsg(context, MainActivity.Buttons, MainActivity.ButtonPort2, NodeService.PortValue_Boolean_False);
                return false;
            }
        });

        // Link the slider
        outputSeekBar.setOnSeekBarChangeListener(new SeekBar.OnSeekBarChangeListener() {
            @Override
            public void onProgressChanged(SeekBar seekBar, int progress, boolean fromUser) {
                // Send normalize value
                String value = Float.toString(progress / (float) seekBar.getMax());
                NodeService.SendPortMsg(context, MainActivity.Slider, MainActivity.SliderPort, value);
            }

            @Override
            public void onStartTrackingTouch(SeekBar seekBar) {
            }

            @Override
            public void onStopTrackingTouch(SeekBar seekBar) {
            }
        });

        // Link the switches
        outputSwitch1.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
                // do something, the isChecked will be true if the switch is in the On position
                NodeService.SendPortMsg(context, MainActivity.Switches, MainActivity.SwitchPort0,
                        (isChecked) ? NodeService.PortValue_Boolean_True : NodeService.PortValue_Boolean_False);
            }
        });

        outputSwitch2.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
                // do something, the isChecked will be true if the switch is in the On position
                NodeService.SendPortMsg(context, MainActivity.Switches, MainActivity.SwitchPort1,
                        (isChecked) ? NodeService.PortValue_Boolean_True : NodeService.PortValue_Boolean_False);
            }
        });

        outputSwitch3.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
                // do something, the isChecked will be true if the switch is in the On position
                NodeService.SendPortMsg(context, MainActivity.Switches, MainActivity.SwitchPort2,
                        (isChecked) ? NodeService.PortValue_Boolean_True : NodeService.PortValue_Boolean_False);
            }
        });

        // Link the Camera Button
        cameraButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                dispatchTakePictureIntent();
            }
        });

        // GPS button
        gpsButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                LocationHandler.getInstance(getActivity()).SendNewLocation(null);
            }
        });
    }

    // =============================================================================================
    // Picture capture
    // =============================================================================================

    int REQUEST_PERMISSION_CAMERA = 10;

    @Override
    public void onRequestPermissionsResult(int requestCode, @NonNull String[] permissions, @NonNull int[] grantResults) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);
        if (requestCode == REQUEST_PERMISSION_CAMERA) {
            if (grantResults[0] == PackageManager.PERMISSION_GRANTED) {
                // Now user should be able to use camera
            }
            else {
                // Your app will not have this permission. Turn off all functions
                // that require this permission or it will force close like your
                // original question
            }
        }
    }

    private void takeNoPreviewPicture(Context context) {

        Helpers.log(Log.INFO, TAG, "takeNoPreviewPicture called");

        try {
            // Verify camera permissions
            int permission = PackageManager.PERMISSION_DENIED;
            if (getActivity() != null)
                permission = ActivityCompat.checkSelfPermission(getActivity(), Manifest.permission.CAMERA);

            if (permission != PackageManager.PERMISSION_GRANTED) {
                // We don't have permission so prompt the user
                ActivityCompat.requestPermissions(
                        getActivity(),
                        new String[]{Manifest.permission.CAMERA},
                        REQUEST_PERMISSION_CAMERA
                );
            }

            // resume camera resources
            ThingsModuleService.resumeCamera(context);
            ThingsModuleService.setTorch(context, true, false);

            // generateFetcher surfaceview
            SurfaceView mSurfaceView = (SurfaceView) view.findViewById(R.id.camera_preview);
            SurfaceHolder mSurfaceHolder = mSurfaceView.getHolder();

            // start background preview
            mSurfaceHolder.addCallback(this);
            mSurfaceHolder.setType(SurfaceHolder.SURFACE_TYPE_PUSH_BUFFERS);

            ThingsModuleService.camera.setPreviewDisplay(mSurfaceHolder);
            ThingsModuleService.camera.startPreview();
            ThingsModuleService.camera.takePicture(null, null, new PhotoHandler(getActivity()));
        } catch (Exception eg) {
            Helpers.logException(TAG, eg);
        }
    }

    // ---------------------------------------------------------------------------------------------
    private void dispatchTakePictureIntent() {

        // Verify camera permissions
        int permission = ActivityCompat.checkSelfPermission(getActivity(), Manifest.permission.CAMERA);
        if (permission != PackageManager.PERMISSION_GRANTED) {
            // We don't have permission so prompt the user
            ActivityCompat.requestPermissions(
                    getActivity(),
                    new String[]{Manifest.permission.CAMERA},
                    REQUEST_PERMISSION_CAMERA
            );
        }

        Intent takePictureIntent = new Intent(MediaStore.ACTION_IMAGE_CAPTURE);
        // release camera resources
        ThingsModuleService.releaseCamera();

        try {
            MainActivity main = (MainActivity) getActivity();
            if (main != null) {
                // Ensure that there's a camera activity to handle the intent
                if (takePictureIntent.resolveActivity(main.getPackageManager()) != null) {

                    // Create the File where the photo should go
                    File photoFile = PhotoHandler.createFile(main, "Picture_", ".jpg", Environment.DIRECTORY_PICTURES);

                    if (photoFile != null) {
                        // save the file path for result
                        MainActivity.mCurrentPhotoPath = photoFile.getAbsolutePath();

                        // Continue only if the File was successfully created
                        takePictureIntent.putExtra(MediaStore.EXTRA_OUTPUT, Uri.fromFile(photoFile));
                        main.startActivityForResult(takePictureIntent, MainActivity.REQUEST_CAMERA);
                    }
                }
            }
        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }
    }

    // =============================================================================================
    // Send updates
    // =============================================================================================

    public static void SendWifiUpdate(Context context) {
        ConnectivityManager cm = (ConnectivityManager) context.getSystemService(Context.CONNECTIVITY_SERVICE);
        NetworkInfo wifiNetInfo = cm.getActiveNetworkInfo();

        NetworkInfo.State state = wifiNetInfo.getState(); // Coarse-grained state

        String toSendState = state.toString(); // Port0
        String toSendSSID = "";
        String toSendRSSI = "";

        if (wifiNetInfo.isConnected()) {
            WifiManager wifiManager = (WifiManager) context.getSystemService(Context.WIFI_SERVICE);
            WifiInfo wifiInfo = wifiManager.getConnectionInfo();

            if (wifiInfo != null && !TextUtils.isEmpty(wifiInfo.getSSID())) {
                toSendSSID = wifiInfo.getSSID(); // Port1
                toSendRSSI = Integer.toString(wifiInfo.getRssi()); // Port2
            }
        }

        // Notify NodeService
        NodeService.SendPortMsg(context.getApplicationContext(),
                MainActivity.WiFiStatus,
                new String[]{toSendState, toSendSSID, toSendRSSI});
    }

    // =============================================================================================
    // Events from background services
    // =============================================================================================

    public BroadcastReceiver mMsgRxMainActivityService = new BroadcastReceiver() {
        @SuppressLint("LogConditional")
        @Override
        public void onReceive(Context context, Intent intent) {
            String action = intent.getAction();
            Helpers.log(Log.INFO, TAG, "Broadcast received: " + action);

            SettingsProvider settingsProvider = SettingsProvider.getInstance(context);

            ProgressBar inputProgressBar = (ProgressBar) view.findViewById(R.id.input_progressBar);
            Switch inputSwitch1 = (Switch) view.findViewById(R.id.input_switch1);
            Switch inputSwitch2 = (Switch) view.findViewById(R.id.input_switch2);
            Switch inputSwitch3 = (Switch) view.findViewById(R.id.input_switch3);
            ImageButton colorButton1 = (ImageButton) view.findViewById(R.id.input_button_color1);
            ImageButton colorButton2 = (ImageButton) view.findViewById(R.id.input_button_color2);
            ImageButton colorButton3 = (ImageButton) view.findViewById(R.id.input_button_color3);
            TextView textView = (TextView) view.findViewById(R.id.textView3);
            SeekBar outputSeekBar = (SeekBar) view.findViewById(R.id.seekBar);
            Switch outputSwitch1 = (Switch) view.findViewById(R.id.output_switch1);
            Switch outputSwitch2 = (Switch) view.findViewById(R.id.output_switch2);
            Switch outputSwitch3 = (Switch) view.findViewById(R.id.output_switch3);
            Button gpsButton = (Button) view.findViewById(R.id.button_GPS);
            Button cameraButton = (Button) view.findViewById(R.id.camera_button);
            Button button1 = (Button) view.findViewById(R.id.button1);
            Button button2 = (Button) view.findViewById(R.id.button2);
            Button button3 = (Button) view.findViewById(R.id.button3);

            try {
                switch (action) {
                    //----------------------------- Things Update ----------------------------------
                    case NodeService.BROADCAST_THING_UPDATE: {
                        Bundle b = intent.getExtras();
                        String portID = b.getString(NodeService.EXTRA_UPDATED_PORT_ID);
                        String thingKey = b.getString(NodeService.EXTRA_UPDATED_THING_KEY);
                        String thingName = b.getString(NodeService.EXTRA_UPDATED_THING_NAME);
                        String portState = b.getString(NodeService.EXTRA_UPDATED_STATE);
                        Boolean isEvent = b.getBoolean(NodeService.EXTRA_UPDATED_IS_EVENT);
                        Boolean isInternalEvent = b.getBoolean(NodeService.EXTRA_IS_INTERNAL_EVENT, false);

                        Helpers.log(Log.INFO, TAG, "Update from Thing:" + thingName);

                        if (thingKey == null || portID == null)
                            break;

                        //input (from server) ProgressBar (PortStateRsp or PortEventMsg)
                        if (thingKey.equals(thingManager.GetThingKey(MainActivity.ProgressBar))) {
                            float progress = Float.parseFloat(portState);
                            if (progress >= 0 && progress <= 1) {
                                inputProgressBar.setProgress((int) (inputProgressBar.getMax() * progress));

                                // send notification (if in backgroundMode)
                                if (MainActivity.backgroundMode)
                                    NodeService.displayNotification(context, MainActivity.class,
                                            NodeService.THING_UPDATE_ID,
                                            thingName + " updated",
                                            "Port value: " + portState);
                            }
                        }
                        //output (towards server) SeekBar (only when updating stating via PortStateRsp)
                        else if (thingKey.equals(thingManager.GetThingKey(MainActivity.Slider))) {
                            float progress = Float.parseFloat(portState);
                            if (progress >= 0 && progress <= 1)
                                outputSeekBar.setProgress((int) (outputSeekBar.getMax() * progress));
                        }
                        //input (from server) switches (PortStateRsp or PortEventMsg)
                        else if (thingKey.equals(thingManager.GetThingKey(MainActivity.InputSwitches))) {
                            switch (portID) {
                                case MainActivity.InputSwitchPort0:
                                    inputSwitch1.setChecked(Boolean.parseBoolean(portState));
                                    break;
                                case MainActivity.InputSwitchPort1:
                                    inputSwitch2.setChecked(Boolean.parseBoolean(portState));
                                    break;
                                case MainActivity.InputSwitchPort2:
                                    inputSwitch3.setChecked(Boolean.parseBoolean(portState));
                                    break;
                            }

                            // send notification (if in backgroundMode)
                            if (MainActivity.backgroundMode)
                                NodeService.displayNotification(context, MainActivity.class,
                                        NodeService.THING_UPDATE_ID,
                                        thingName + " updated",
                                        "Port " + portID + " state: " + (Boolean.parseBoolean(portState) ? "ON" : "OFF"));
                        }
                        //output (towards server) switches (only when updating stating via PortStateRsp)
                        else if (thingKey.equals(thingManager.GetThingKey(MainActivity.Switches))) {
                            switch (portID) {
                                case MainActivity.SwitchPort0:
                                    outputSwitch1.setChecked(Boolean.parseBoolean(portState));
                                    break;
                                case MainActivity.SwitchPort1:
                                    outputSwitch2.setChecked(Boolean.parseBoolean(portState));
                                    break;
                                case MainActivity.SwitchPort2:
                                    outputSwitch3.setChecked(Boolean.parseBoolean(portState));
                                    break;
                            }
                        }
                        // leds
                        else if (thingKey.equals(thingManager.GetThingKey(MainActivity.Leds))) {
                            double val = Double.parseDouble(portState);
                            int color = 0xff000000 | (int) ((double) 0xffffff * val);
                            switch (portID) {
                                case MainActivity.LedsPort0:
                                    updateShapeBackground(colorButton1, color);
                                    break;
                                case MainActivity.LedsPort1:
                                    updateShapeBackground(colorButton2, color);
                                    break;
                                case MainActivity.LedsPort2:
                                    updateShapeBackground(colorButton3, color);
                                    break;
                            }

                            // send notification (if in backgroundMode)
                            if (MainActivity.backgroundMode)
                                NodeService.displayNotification(context, MainActivity.class,
                                        NodeService.THING_UPDATE_ID,
                                        thingName + " updated",
                                        "Port " + portID + " value: " + portState);
                        }
                        // Photo viewer
                        else if (thingKey.equals(thingManager.GetThingKey(MainActivity.PhotoViewer))) {
                            if (!isInternalEvent) {
                                if (isEvent && !(portState != null && portState.isEmpty())) {
                                    try {
                                        // Deserialize BinaryResourceDescriptor
                                        BinaryResourceDescriptor descriptor = new Gson().fromJson(portState, BinaryResourceDescriptor.class);
                                        if (descriptor == null || descriptor.ContentType != eBinaryResourceContentType.Image) {
                                            return;
                                        }

                                        if (descriptor.LocationType == eBinaryResourceLocationType.Http) {
                                            HttpLocationDescriptor locDescriptor =
                                                    new Gson().fromJson(descriptor.LocationDescriptorJson, HttpLocationDescriptor.class);
                                            if (locDescriptor.RestServiceType == eRestServiceType.Undefined) { // TODO: Add yodiwo-offered HTTP service type
                                                NodeService.DownloadFile(context,
                                                        locDescriptor.Uri,
                                                        thingName,
                                                        MainActivity.PhotoViewerPort);
                                            }
                                        }
                                    } catch (Exception e) {
                                        Helpers.logException(TAG, e);
                                    }
                                }
                            } else {
                                byte[] downloadedFileBytes = b.getByteArray(NodeService.EXTRA_FILE_DOWNLOADED);

                                if (downloadedFileBytes != null) {
                                    MainActivity main = (MainActivity) getActivity();
                                    if (main != null)
                                        main.onUserImageHandling(main, BitmapFactory.decodeByteArray(downloadedFileBytes, 0, downloadedFileBytes.length));
                                }
                            }
                        }
                        // text box
                        else if (thingKey.equals(thingManager.GetThingKey(MainActivity.InputTextBox))) {
                            if (!portID.equals(MainActivity.InputTextBoxPort)) {
                                Helpers.log(Log.ERROR, TAG, "TextMessage for unknown port");
                                return;
                            }
                            textView.setText(portState);

                            // send notification (if in backgroundMode)
                            if (MainActivity.backgroundMode)
                                NodeService.displayNotification(context, MainActivity.class,
                                        NodeService.THING_UPDATE_ID,
                                        thingName + " updated",
                                        "Port value: " + portState);

                        }
                        // Android intent
                        else if (thingKey.equals(thingManager.GetThingKey(MainActivity.AndroidIntent))) {
                            if (isEvent) {
                                if (portState == null) break;
                                try {
                                    Intent i = new Intent(Intent.ACTION_VIEW, Uri.parse(portState));
                                    if (i.resolveActivity(context.getPackageManager()) != null) {
                                        startActivity(i);
                                    }
                                }
                                catch (Exception ex) {
                                    Toast.makeText(context, "Unable to show image on browser", Toast.LENGTH_SHORT).show();
                                    Helpers.log(Log.INFO, TAG, "Failed to send intent");
                                }

                                // send notification (if in backgroundMode)
                                if (MainActivity.backgroundMode)
                                    NodeService.displayNotification(context, MainActivity.class,
                                            NodeService.THING_UPDATE_ID,
                                            thingName + " activated",
                                            "");
                            }
                        }
                        // torch
                        else if (thingKey.equals(thingManager.GetThingKey(MainActivity.Torch))) {
                            sendTorchIntent(context, Boolean.parseBoolean(portState));

                            // send notification (if in backgroundMode)
                            if (MainActivity.backgroundMode)
                                NodeService.displayNotification(context, MainActivity.class,
                                        NodeService.THING_UPDATE_ID,
                                        thingName + " updated",
                                        "Port state: " + (Boolean.parseBoolean(portState) ? "ON" : "OFF"));
                        }
                        // Android camera trigger
                        else if (thingKey.equals(thingManager.GetThingKey(MainActivity.Camera))) {
                            if (Boolean.parseBoolean(portState) && isEvent) {
                                if (portID.equals(MainActivity.CameraTriggerPort)) {
                                    //dispatchTakePictureIntent();
                                    takeNoPreviewPicture(context);

                                    // send notification (if in backgroundMode)
                                    if (MainActivity.backgroundMode)
                                        if (Boolean.parseBoolean(portState))
                                            NodeService.displayNotification(context, MainActivity.class,
                                                    NodeService.THING_UPDATE_ID,
                                                    thingName + " activated",
                                                    "");
                                }
                            }
                        }
                        // GPS trigger
                        else if (thingKey.equals(thingManager.GetThingKey(MainActivity.GPS))) {
                            if (Boolean.parseBoolean(portState) && isEvent) {
                                if (portID.equals(MainActivity.GPSTriggerPort)) {
                                    LocationHandler.getInstance(getActivity()).SendNewLocation(null);

                                        // send notification (if in backgroundMode)
                                        if (MainActivity.backgroundMode)
                                            NodeService.displayNotification(context, MainActivity.class,
                                                    NodeService.THING_UPDATE_ID,
                                                    thingName + " sent new location",
                                                    "");
                                }
                            }
                        }
                        // Bluetooth
                        else if (thingKey.equals(thingManager.GetThingKey(MainActivity.Bluetooth))) {
                            if (Boolean.parseBoolean(portState) && isEvent) {
                                if (portID.equals(MainActivity.BluetoothTriggerSingleShotDiscoveryPort)) {
                                    BluetoothAdapter mBtAdapter = BluetoothAdapter.getDefaultAdapter();
                                    // check if device is already discovering
                                    if (mBtAdapter.isDiscovering()) {
                                        mBtAdapter.cancelDiscovery();
                                    }
                                    Boolean res = mBtAdapter.startDiscovery();
                                    if (!res) {
                                        Helpers.log(Log.ERROR, TAG, "Failed to start device discovery");
                                    }
                                } else if (portID.equals(MainActivity.BluetoothRequestPairedDevicesPort)) {
                                    Set<BluetoothDevice> pairedDevices = BluetoothAdapter.getDefaultAdapter().getBondedDevices();

                                    if (pairedDevices.size() > 0) {
                                        ArrayList<String> pairedDevicesList = new ArrayList<>();
                                        for (BluetoothDevice device : pairedDevices) {
                                            String toSendPairedDevice = device.getName() + "," + device.getAddress();

                                            // Notify NodeService
                                            NodeService.SendPortMsg(context.getApplicationContext(),
                                                    MainActivity.Bluetooth,
                                                    MainActivity.BluetoothPairedDevicesPort,
                                                    toSendPairedDevice);
                                        }
                                        // TODO: Send all paired devices as single delimited string ?
                                        // String[] toSendPairedDevices = new String[pairedDevicesList.size()];
                                        // toSendPairedDevices  = pairedDevicesList.toArray(toSendPairedDevices);
                                    }
                                }
                            }
                        }

                        // refresh UI
                        if (view != null)
                            view.invalidate();

                        break;
                    }
                    //------------------------------- UI -----------------------------------------------
                    case aServerAPI.CONNECTIVITY_UI_UPDATE: {
                        Bundle b = intent.getExtras();
                        Boolean rxActive = b.getBoolean(aServerAPI.EXTRA_UPDATED_RX_STATE);
                        Boolean txActive = b.getBoolean(aServerAPI.EXTRA_UPDATED_TX_STATE);

                        // grey out text
                        TextView outputStr = (TextView) view.findViewById(R.id.textView);
                        TextView inputStr = (TextView) view.findViewById(R.id.textView2);
                        outputStr.setAlpha(txActive ? 1.0f : 0.3f);
                        inputStr.setAlpha(rxActive ? 1.0f : 0.3f);
                        outputStr.invalidate();
                        inputStr.invalidate();

                        // connectivity progressbar
                        final ProgressBar connectivityProgress = (ProgressBar) view.findViewById(R.id.connectivity_progress);
                        if ((!rxActive || !txActive) && !(PairingService.getPairingStatus() == PairingService.PairingStatus.UNPAIRED))
                            connectivityProgress.setVisibility(View.VISIBLE);
                        else
                            connectivityProgress.setVisibility(View.GONE);

                        // refresh UI
                        if (view != null)
                            view.invalidate();

                        break;
                    }
                    //------------------------ IP Connectivity -----------------------------------------
                    case ConnectivityManager.CONNECTIVITY_ACTION: {
                        if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.JELLY_BEAN_MR1) {
                            int networkType = intent.getIntExtra(ConnectivityManager.EXTRA_NETWORK_TYPE, -1);
                        }

                        ConnectivityManager cm = (ConnectivityManager) context.getSystemService(Context.CONNECTIVITY_SERVICE);
                        NetworkInfo activeNetInfo = cm.getActiveNetworkInfo();

                        int currentType = activeNetInfo != null
                                ? activeNetInfo.getType() : NO_CONNECTION_TYPE;

                        // Avoid handling multiple broadcasts for the same connection type
                        if (sLastType != currentType) {
                            Helpers.log(Log.DEBUG, TAG, "Current network type differs from the previous one");
                            if (currentType == NO_CONNECTION_TYPE || !activeNetInfo.isConnected()) {
                            /* if null: airplane mode, or mobile data & wifi off, or out of signal */

                                //refresh global connectivity so that we don't constantly try to connect to cloud
                                NodeService.SetNetworkConnStatus(context, false);
                            } else {
                                //refresh global connectivity: we do have the internets; rejoice
                                NodeService.SetNetworkConnStatus(context, true);

                                // send updates
                                SendWifiUpdate(context);
                            }

                            sLastType = currentType;
                        } else
                            Helpers.log(Log.DEBUG, TAG, "Current network type does not differ from the previous one");

                        break;
                    }
                    //----------------------------- Bluetooth ------------------------------------------
                    case BluetoothAdapter.ACTION_STATE_CHANGED: {
                        int state = intent.getIntExtra(BluetoothAdapter.EXTRA_STATE, -1);

                        // Notify NodeService
                        NodeService.SendPortMsg(context.getApplicationContext(),
                                MainActivity.Bluetooth,
                                MainActivity.BluetoothPowerStatusPort,
                                MainActivity.bluetoothPowerStateCodesToNames.get(state));

                        // send notification
                        if (MainActivity.backgroundMode)
                            NodeService.displayNotification(context, MainActivity.class,
                                    NodeService.BLE_THING_ID,
                                    "Bluetooth updated",
                                    "Power status: " + MainActivity.bluetoothPowerStateCodesToNames.get(state));

                        break;
                    }
                    case BluetoothAdapter.ACTION_CONNECTION_STATE_CHANGED: {
                        // STATE_DISCONNECTED, STATE_CONNECTING, STATE_CONNECTED, STATE_DISCONNECTING --> EXTRA_CONNECTION_STATE
                        int state = intent.getIntExtra(BluetoothAdapter.EXTRA_CONNECTION_STATE, -1);

                        // Notify NodeService
                        NodeService.SendPortMsg(context.getApplicationContext(),
                                MainActivity.Bluetooth,
                                MainActivity.BluetoothConnectionStatusPort,
                                MainActivity.bluetoothConnectionStateCodesToNames.get(state));

                        // send notification
                        if (MainActivity.backgroundMode)
                            NodeService.displayNotification(context, MainActivity.class,
                                    NodeService.BLE_THING_ID,
                                    "Bluetooth updated",
                                    "Connection status: " + MainActivity.bluetoothConnectionStateCodesToNames.get(state));

                        break;
                    }
                    case BluetoothAdapter.ACTION_DISCOVERY_STARTED:
                        Helpers.log(Log.DEBUG, TAG, "Device discovery started");
                        Toast.makeText(context, "BT discovery started", Toast.LENGTH_SHORT).show();

                        // send notification
                        if (MainActivity.backgroundMode)
                            NodeService.displayNotification(context, MainActivity.class,
                                    NodeService.BLE_THING_ID,
                                    "Bluetooth updated",
                                    "Device discovery started ");

                        break;
                    case BluetoothDevice.ACTION_FOUND:
                        // Get the BluetoothDevice object from the Intent
                        BluetoothDevice device = intent.getParcelableExtra(BluetoothDevice.EXTRA_DEVICE);

                        // Notify NodeService
                        NodeService.SendPortMsg(context.getApplicationContext(),
                                MainActivity.Bluetooth,
                                MainActivity.BluetoothDiscoveredDevicesPort,
                                device.getName() + " (" + device.getAddress() + ")");

                        // send notification
                        if (MainActivity.backgroundMode)
                            NodeService.displayNotification(context, MainActivity.class,
                                    NodeService.BLE_THING_ID,
                                    "Bluetooth devices discovered",
                                    device.getName() + " (" + device.getAddress() + ")");

                        break;
                    case BluetoothAdapter.ACTION_DISCOVERY_FINISHED:
                        // This has to be called to ensure discovery is disabled
                        BluetoothAdapter.getDefaultAdapter().cancelDiscovery();
                        Toast.makeText(context, "BT discovery finished", Toast.LENGTH_SHORT).show();

                        // send notification
                        if (MainActivity.backgroundMode)
                            NodeService.displayNotification(context, MainActivity.class,
                                    NodeService.BLE_THING_ID,
                                    "Bluetooth updated", "Device discovery finished");

                        break;

                    //------------------------ Configuration Thing Update-----------------------------------------
                    case NodeService.BROADCAST_CONFIGURATION_THING_UPDATE: {
                        Bundle b = intent.getExtras();
                        String thingName = b.getString(NodeService.EXTRA_UPDATED_THING_NAME);

                        if (thingName == null)
                            break;

                        if (thingName.equals(MainActivity.GPS)) {
                            LocationHandler.getInstance(getActivity()).setLocationListener();
                        }

                        // TODO: handle this for wearable

                        break;
                    }
                    //------------------------ Send Updates -----------------------------------------
                    case NodeService.BROADCAST_SEND_UPDATES: {
                        SendWifiUpdate(context);
                        break;
                    }

                    //------------------------ UI unpairing info-----------------------------------------
                    case NodeService.BROADCAST_NODE_UNPAIRING: {
                        Bundle b = intent.getExtras();
                        MainActivity.isUnpairedByUser = b.getBoolean(NodeService.EXTRA_NODE_UNPAIRING_INFO);
                        break;
                    }

                    //------------------------ Active Port Update -----------------------------------------
                    case NodeService.BROADCAST_ACTIVE_PORT_UPDATE: {
                        boolean isActiveGps = false;
                        Bundle b = intent.getExtras();
                        String[] thingKeys = b.getStringArray(NodeService.EXTRA_UPDATED_ACTIVE_THINGS_KEYS);
                        String[] portKeys = b.getStringArray(NodeService.EXTRA_UPDATED_ACTIVE_PORT_KEYS);

                        // something went wrong
                        if (thingKeys == null || portKeys == null)
                            break;

                        // Disable all outputs
                        outputSwitch1.setEnabled(false);
                        outputSwitch2.setEnabled(false);
                        outputSwitch3.setEnabled(false);
                        outputSeekBar.setEnabled(false);
                        button1.setEnabled(false);
                        button2.setEnabled(false);
                        button3.setEnabled(false);
                        cameraButton.setEnabled(false);
                        gpsButton.setEnabled(false);

                        // enable only the deployed ports
                        for (String portKey : portKeys) {
                            if (portKey.equals(thingManager.GetPortKey(MainActivity.Switches, MainActivity.SwitchPort0))) {
                                outputSwitch1.setEnabled(true);
                            } else if (portKey.equals(thingManager.GetPortKey(MainActivity.Switches, MainActivity.SwitchPort1))) {
                                outputSwitch2.setEnabled(true);
                            } else if (portKey.equals(thingManager.GetPortKey(MainActivity.Switches, MainActivity.SwitchPort2))) {
                                outputSwitch3.setEnabled(true);
                            } else if (portKey.equals(thingManager.GetPortKey(MainActivity.Buttons, MainActivity.ButtonPort0))) {
                                button1.setEnabled(true);
                            } else if (portKey.equals(thingManager.GetPortKey(MainActivity.Buttons, MainActivity.ButtonPort1))) {
                                button2.setEnabled(true);
                            } else if (portKey.equals(thingManager.GetPortKey(MainActivity.Buttons, MainActivity.ButtonPort2))) {
                                button3.setEnabled(true);
                            } else if (portKey.equals(thingManager.GetPortKey(MainActivity.Slider, MainActivity.SliderPort))) {
                                outputSeekBar.setEnabled(true);
                            } else if (portKey.equals(thingManager.GetPortKey(MainActivity.Camera, MainActivity.CameraPort))) {
                                cameraButton.setEnabled(true);
                            }
                        }

                        // handle GPS Thing UI & listener
                        for (String thingKey : thingKeys) {
                            if (thingKey.equals(thingManager.GetThingKey(MainActivity.GPS))) {
                                gpsButton.setEnabled(true);
                                isActiveGps = true;
                                LocationHandler.getInstance(getActivity()).setLocationListener();
                            }
                        }

                        if (LocationHandler.isSetLocationListener && !isActiveGps) {
                            LocationHandler.getInstance(getActivity()).unsetLocationListener();
                        }

                        // refresh UI
                        if (view != null)
                            view.invalidate();

                        break;
                    }
                }
            } catch (Exception e) {
                Helpers.logException(TAG, e);
            }
        }
    };

    // =============================================================================================
    // Helpers
    // =============================================================================================

    private void updateShapeBackground(ImageButton button, int color) {
        Drawable background = button.getBackground();

        if (background instanceof ShapeDrawable) {
            // cast to 'ShapeDrawable'
            ShapeDrawable shapeDrawable = (ShapeDrawable)background;
            shapeDrawable.getPaint().setColor(color);
            button.setBackground(shapeDrawable);
        } else if (background instanceof GradientDrawable) {
            // cast to 'GradientDrawable'
            GradientDrawable gradientDrawable = (GradientDrawable)background;
            gradientDrawable.setColor(color);
            button.setBackground(gradientDrawable);
        } else if (background instanceof ColorDrawable) {
            // alpha value may need to be set again after this call
            ColorDrawable colorDrawable = (ColorDrawable)background;
            colorDrawable.setColor(color);
            button.setBackground(colorDrawable);
        }
        button.invalidate();
    }

    private void sendTorchIntent(Context context, boolean state) {
        Intent i = new Intent(context, ThingsModuleService.class);
        i.putExtra(ThingsModuleService.EXTRA_INTENT_FOR_THING, ThingsModuleService.EXTRA_TORCH_THING);
        i.putExtra(ThingsModuleService.EXTRA_TORCH_THING_STATE, state);
        context.startService(i);
    }
}
