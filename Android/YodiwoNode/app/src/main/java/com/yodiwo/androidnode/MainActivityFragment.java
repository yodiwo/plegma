package com.yodiwo.androidnode;

import android.Manifest;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.pm.PackageManager;
import android.graphics.BitmapFactory;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.net.Uri;
import android.os.Bundle;
import android.os.Environment;
import android.provider.MediaStore;
import android.support.v4.app.ActivityCompat;
import android.support.v4.app.Fragment;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.CompoundButton;
import android.widget.ProgressBar;
import android.widget.SeekBar;
import android.widget.Switch;
import android.widget.TextView;

import com.google.gson.Gson;
import com.google.gson.internal.LinkedTreeMap;
import com.yodiwo.plegma.BinaryResourceDescriptor;
import com.yodiwo.plegma.HttpLocationDescriptor;
import com.yodiwo.plegma.eBinaryResourceContentType;
import com.yodiwo.plegma.eBinaryResourceLocationType;
import com.yodiwo.plegma.eRestServiceType;

import java.io.File;
import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.Set;


/**
 * A placeholder fragment containing a simple view.
 */
public class MainActivityFragment extends Fragment {

    // =============================================================================================
    // Static

    public static final String TAG = MainActivityFragment.class.getSimpleName();

    // =============================================================================================

    private ThingManager thingManager;

    private ProgressBar inputProgressBar;
    private SeekBar outputSeekBar;
    private Switch outputSwitch1;
    private Switch outputSwitch2;
    private Switch outputSwitch3;
    private Switch inputSwitch1;
    private Switch inputSwitch2;
    private Switch inputSwitch3;
    private Button colorButton1;
    private Button colorButton2;
    private Button colorButton3;
    private TextView textView;
    private Button gpsButton;
    private Button cameraButton;
    private Button button1;
    private Button button2;
    private Button button3;

    private TextView outputStr;
    private TextView inputStr;

    public MainActivityFragment() {
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {

        final Context context = this.getActivity().getApplicationContext();

        View view = inflater.inflate(R.layout.fragment_main, container, false);

        SettingsProvider settingsProvider = SettingsProvider.getInstance(context);
        thingManager = ThingManager.getInstance(context);

        // Link UI elements
        inputProgressBar = (ProgressBar) view.findViewById(R.id.input_progressBar);
        outputSeekBar = (SeekBar) view.findViewById(R.id.seekBar);
        outputSwitch1 = (Switch) view.findViewById(R.id.output_switch1);
        outputSwitch2 = (Switch) view.findViewById(R.id.output_switch2);
        outputSwitch3 = (Switch) view.findViewById(R.id.output_switch3);
        inputSwitch1 = (Switch) view.findViewById(R.id.input_switch1);
        inputSwitch2 = (Switch) view.findViewById(R.id.input_switch2);
        inputSwitch3 = (Switch) view.findViewById(R.id.input_switch3);
        colorButton1 = (Button) view.findViewById(R.id.input_button_color1);
        colorButton2 = (Button) view.findViewById(R.id.input_button_color2);
        colorButton3 = (Button) view.findViewById(R.id.input_button_color3);
        textView = (TextView) view.findViewById(R.id.textView3);
        gpsButton = (Button) view.findViewById(R.id.button_GPS);
        cameraButton = (Button) view.findViewById(R.id.camera_button);
        button1 = (Button) view.findViewById(R.id.button1);
        button2 = (Button) view.findViewById(R.id.button2);
        button3 = (Button) view.findViewById(R.id.button3);

        // Link button1 to code
        button1.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View view, MotionEvent motionEvent) {
                int action = motionEvent.getAction();
                if (action == MotionEvent.ACTION_DOWN)
                    NodeService.SendPortMsg(context, ThingManager.Buttons, ThingManager.ButtonPort0, NodeService.PortValue_Boolean_True);
                else if (action == MotionEvent.ACTION_UP)
                    NodeService.SendPortMsg(context, ThingManager.Buttons, ThingManager.ButtonPort0, NodeService.PortValue_Boolean_False);
                return false;
            }
        });

        // Link button2 to code
        button2.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View view, MotionEvent motionEvent) {
                int action = motionEvent.getAction();
                if (action == MotionEvent.ACTION_DOWN)
                    NodeService.SendPortMsg(context, ThingManager.Buttons, ThingManager.ButtonPort1, NodeService.PortValue_Boolean_True);
                else if (action == MotionEvent.ACTION_UP)
                    NodeService.SendPortMsg(context, ThingManager.Buttons, ThingManager.ButtonPort1, NodeService.PortValue_Boolean_False);
                return false;
            }
        });

        // Link button3 to code
        button3.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View view, MotionEvent motionEvent) {
                int action = motionEvent.getAction();
                if (action == MotionEvent.ACTION_DOWN)
                    NodeService.SendPortMsg(context, ThingManager.Buttons, ThingManager.ButtonPort2, NodeService.PortValue_Boolean_True);
                else if (action == MotionEvent.ACTION_UP)
                    NodeService.SendPortMsg(context, ThingManager.Buttons, ThingManager.ButtonPort2, NodeService.PortValue_Boolean_False);
                return false;
            }
        });

        // Link the slider
        outputSeekBar.setOnSeekBarChangeListener(new SeekBar.OnSeekBarChangeListener() {
            @Override
            public void onProgressChanged(SeekBar seekBar, int progress, boolean fromUser) {
                // Send normalize value
                String value = Float.toString(progress / (float) seekBar.getMax());
                NodeService.SendPortMsg(context, ThingManager.Slider1, ThingManager.SliderPort, value);
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
                NodeService.SendPortMsg(context, ThingManager.Switches, ThingManager.SwitchPort0,
                        (isChecked) ? NodeService.PortValue_Boolean_True : NodeService.PortValue_Boolean_False);
            }
        });

        outputSwitch2.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
                // do something, the isChecked will be true if the switch is in the On position
                NodeService.SendPortMsg(context, ThingManager.Switches, ThingManager.SwitchPort1,
                        (isChecked) ? NodeService.PortValue_Boolean_True : NodeService.PortValue_Boolean_False);
            }
        });

        outputSwitch3.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
                // do something, the isChecked will be true if the switch is in the On position
                NodeService.SendPortMsg(context, ThingManager.Switches, ThingManager.SwitchPort2,
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
                MainActivity main = (MainActivity) getActivity();
                main.SendNewLocation(null);
            }
        });

        LocalBroadcastManager.getInstance(context).registerReceiver(mMessageReceiverMainActivityService,
                new IntentFilter(NodeService.BROADCAST_THING_UPDATE));

        LocalBroadcastManager.getInstance(context).registerReceiver(mMessageReceiverMainActivityService,
                new IntentFilter(NodeService.BROADCAST_ACTIVE_PORT_UPDATE));

        // UI elements to signify connectivity
        outputStr = (TextView) view.findViewById(R.id.textView);
        inputStr = (TextView) view.findViewById(R.id.textView2);

        // Start out greyed out (no connection)
        outputStr.setAlpha(0.3f);
        inputStr.setAlpha(0.3f);

        LocalBroadcastManager.getInstance(context).registerReceiver(mMessageReceiverMainActivityService,
                new IntentFilter(aServerAPI.CONNECTIVITY_UI_UPDATE));

        // WiFi-related
        context.registerReceiver(mMessageReceiverMainActivityService,
                new IntentFilter(ConnectivityManager.CONNECTIVITY_ACTION));

        // Bluetooth-related
        context.registerReceiver(mMessageReceiverMainActivityService,
                new IntentFilter(BluetoothDevice.ACTION_FOUND));
        context.registerReceiver(mMessageReceiverMainActivityService,
                new IntentFilter(BluetoothAdapter.ACTION_STATE_CHANGED));
        context.registerReceiver(mMessageReceiverMainActivityService,
                new IntentFilter(BluetoothAdapter.ACTION_DISCOVERY_STARTED));
        context.registerReceiver(mMessageReceiverMainActivityService,
                new IntentFilter(BluetoothAdapter.ACTION_DISCOVERY_FINISHED));
        context.registerReceiver(mMessageReceiverMainActivityService,
                new IntentFilter(BluetoothAdapter.ACTION_CONNECTION_STATE_CHANGED));

        return view;
    }

    // =============================================================================================
    // Create file class for camera capture

    private void dispatchTakePictureIntent() {
        Intent takePictureIntent = new Intent(MediaStore.ACTION_IMAGE_CAPTURE_SECURE);
        MainActivity main = (MainActivity) getActivity();
        // Ensure that there's a camera activity to handle the intent
        if (takePictureIntent.resolveActivity(main.getPackageManager()) != null) {
            // Create the File where the photo should go
            File photoFile = null;
            try {
                photoFile = createImageFile();

                // save the file path for result
                main.mCurrentPhotoPath = photoFile.getAbsolutePath();
            } catch (IOException ex) {
                // Error occurred while creating the File
                Helpers.logException(TAG, ex);
            }
            // Continue only if the File was successfully created
            if (photoFile != null) {
                takePictureIntent.putExtra(MediaStore.EXTRA_OUTPUT, Uri.fromFile(photoFile));
                main.startActivityForResult(takePictureIntent, MainActivity.REQUEST_CAMERA);
            }
        }
    }

    private File createImageFile() throws IOException {

        // Verify storage permissions
        int REQUEST_EXTERNAL_STORAGE = 1;
        String[] PERMISSIONS_STORAGE = {
                Manifest.permission.READ_EXTERNAL_STORAGE,
                Manifest.permission.WRITE_EXTERNAL_STORAGE
         };
        int permission = ActivityCompat.checkSelfPermission(getActivity(), Manifest.permission.WRITE_EXTERNAL_STORAGE);
        if (permission != PackageManager.PERMISSION_GRANTED) {
                // We don't have permission so prompt the user
                ActivityCompat.requestPermissions(
                        getActivity(),
                        PERMISSIONS_STORAGE,
                        REQUEST_EXTERNAL_STORAGE
                );
        }

        // Create an image file name
        String timeStamp = new SimpleDateFormat("ddMMyyyy_HH:mm:ss").format(new Date());
        String imageFileName = "JPEG_" + timeStamp + "_";
        String path = Environment.getExternalStorageDirectory()
                + File.separator + Environment.DIRECTORY_DCIM
                + File.separator + "Camera";

        return File.createTempFile(
                imageFileName,  /* prefix */
                ".jpg",         /* suffix */
                new File(path)      /* directory */
        );
    }

    // =============================================================================================
    // Events from background services

    private BroadcastReceiver mMessageReceiverMainActivityService = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            String action = intent.getAction();
            Log.i(TAG, "Broadcast received: " + action);

            try {
                //----------------------------- Things Update --------------------------------------
                switch (action) {
                    case NodeService.BROADCAST_THING_UPDATE: {
                        Bundle b = intent.getExtras();
                        int portID = b.getInt(NodeService.EXTRA_UPDATED_PORT_ID, -1);
                        String thingKey = b.getString(NodeService.EXTRA_UPDATED_THING_KEY);
                        String thingName = b.getString(NodeService.EXTRA_UPDATED_THING_NAME);
                        String portState = b.getString(NodeService.EXTRA_UPDATED_STATE);
                        Boolean isEvent = b.getBoolean(NodeService.EXTRA_UPDATED_IS_EVENT);
                        Boolean isInternalEvent = b.getBoolean(NodeService.EXTRA_IS_INTERNAL_EVENT, false);

                        Log.i(TAG, "Update from Thing:" + thingName);

                        //input (from server) ProgressBar (PortStateRsp or PortEventMsg)
                        if (thingKey.equals(thingManager.GetThingKey(ThingManager.InputProgressBar))) {
                            float progress = Float.parseFloat(portState);
                            if (progress >= 0 && progress <= 1)
                                inputProgressBar.setProgress((int) (inputProgressBar.getMax() * progress));
                        }
                        //output (towards server) SeekBar (only when updating stating via PortStateRsp)
                        else if (thingKey.equals(thingManager.GetThingKey(ThingManager.Slider1))) {
                            float progress = Float.parseFloat(portState);
                            if (progress >= 0 && progress <= 1)
                                outputSeekBar.setProgress((int) (outputSeekBar.getMax() * progress));
                        }
                        //input (from server) switches (PortStateRsp or PortEventMsg)
                        else if (thingKey.equals(thingManager.GetThingKey(ThingManager.InputSwitches))) {
                            if (portID == ThingManager.InputSwitchPort0)
                                inputSwitch1.setChecked(Boolean.parseBoolean(portState));
                            else if (portID == ThingManager.InputSwitchPort1)
                                inputSwitch2.setChecked(Boolean.parseBoolean(portState));
                            else if (portID == ThingManager.InputSwitchPort2)
                                inputSwitch3.setChecked(Boolean.parseBoolean(portState));
                        }
                        //output (towards server) switches (only when updating stating via PortStateRsp)
                        else if (thingKey.equals(thingManager.GetThingKey(ThingManager.Switches))) {
                            if (portID == ThingManager.SwitchPort0)
                                outputSwitch1.setChecked(Boolean.parseBoolean(portState));
                            else if (portID == ThingManager.SwitchPort1)
                                outputSwitch2.setChecked(Boolean.parseBoolean(portState));
                            else if (portID == ThingManager.SwitchPort2)
                                outputSwitch3.setChecked(Boolean.parseBoolean(portState));
                        }
                        // color Buttons
                        else if (thingKey.equals(thingManager.GetThingKey(ThingManager.InputColors))) {
                            double val = Double.parseDouble(portState);
                            int color = 0xff000000 | (int) ((double) 0xffffff * val);

                            if (portID == ThingManager.InputColorPort0) {
                                colorButton1.setBackgroundColor(color);
                            } else if (portID == ThingManager.InputColorPort1) {
                                colorButton2.setBackgroundColor(color);
                            } else if (portID == ThingManager.InputColorPort2) {
                                colorButton3.setBackgroundColor(color);
                            }
                        }
                        // Photo viewer
                        else if (thingKey.equals(thingManager.GetThingKey(ThingManager.PhotoViewer))) {
                            if (!isInternalEvent) {
                                if (isEvent && !portState.isEmpty()) {
                                    try {
                                        // Deserialize BinaryResourceDescriptor
                                        BinaryResourceDescriptor descriptor = new Gson().fromJson(portState, BinaryResourceDescriptor.class);
                                        if (descriptor == null || descriptor.ContentType != eBinaryResourceContentType.Image) {
                                            return;
                                        }

                                        if (descriptor.LocationType == eBinaryResourceLocationType.Http) {
                                            HttpLocationDescriptor locDescriptor = new HttpLocationDescriptor();
                                            locDescriptor.Uri = ((LinkedTreeMap<String, String>) descriptor.LocationDescriptor).get("Uri");
                                            locDescriptor.RestServiceType = (((LinkedTreeMap<String, Double>) descriptor.LocationDescriptor).get("RestServiceType")).intValue();

                                            if (locDescriptor.RestServiceType == eRestServiceType.Undefined) { // TODO: Add yodiwo-offered HTTP service type
                                                NodeService.DownloadFile(context,
                                                        locDescriptor.Uri,
                                                        thingName,
                                                        ThingManager.PhotoViewerPort0);
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
                                    byte[] imageByteArray = downloadedFileBytes;
                                    main.onUserImageHandling(BitmapFactory.decodeByteArray(imageByteArray, 0, imageByteArray.length));
                                }
                            }
                        }
                        // text box
                        else if (thingKey.equals(thingManager.GetThingKey(ThingManager.InputTextBox))) {
                            if (portID != ThingManager.InputTextBoxPort) {
                                Helpers.log(Log.ERROR, TAG, "TextMessage for unknown port");
                                return;
                            }
                            textView.setText(portState);
                        }
                        // Android intent
                        else if (thingKey.equals(thingManager.GetThingKey(ThingManager.InputAndroidIntent))) {
                            if (isEvent) {
                                Intent i = new Intent(Intent.ACTION_VIEW, Uri.parse(portState));
                                startActivity(i);
                            }
                        }
                        // torch
                        else if (thingKey.equals(thingManager.GetThingKey(ThingManager.Torch))) {
                            Intent i = new Intent(context, ThingsModuleService.class);
                            i.putExtra(ThingsModuleService.EXTRA_INTENT_FOR_THING, ThingsModuleService.EXTRA_TORCH_THING);
                            i.putExtra(ThingsModuleService.EXTRA_TORCH_THING_STATE, Boolean.parseBoolean(portState));
                            context.startService(i);
                        }
                        // Android camera trigger
                        else if (thingKey.equals(thingManager.GetThingKey(ThingManager.Camera))) {
                            if (Boolean.parseBoolean(portState) && isEvent) {
                                if (portID == ThingManager.CameraTriggerPort) {
                                    if (Boolean.parseBoolean(portState) && isEvent) {
                                        dispatchTakePictureIntent();
                                    }
                                }
                            }
                        }
                        // GPS trigger
                        else if (thingKey.equals(thingManager.GetThingKey(ThingManager.GPS))) {
                            if (Boolean.parseBoolean(portState) && isEvent) {
                                if (portID == ThingManager.GPSTriggerPort) {
                                    MainActivity main = (MainActivity) getActivity();
                                    main.SendNewLocation(null);
                                }
                            }
                        }
                        // Bluetooth
                        else if (thingKey.equals(thingManager.GetThingKey(ThingManager.Bluetooth))) {
                            if (Boolean.parseBoolean(portState) && isEvent) {
                                if (portID == ThingManager.BluetoothTriggerSingleShotDiscoveryPort) {
                                    BluetoothAdapter mBtAdapter = BluetoothAdapter.getDefaultAdapter();
                                    // check if device is already discovering
                                    if (mBtAdapter.isDiscovering()) {
                                        mBtAdapter.cancelDiscovery();
                                    }
                                    Boolean res = mBtAdapter.startDiscovery();
                                    if (!res) {
                                        Log.e(TAG, "Failed to start device discovery");
                                    }
                                } else if (portID == ThingManager.BluetoothRequestPairedDevicesPort) {
                                    Set<BluetoothDevice> pairedDevices = BluetoothAdapter.getDefaultAdapter().getBondedDevices();

                                    if (pairedDevices.size() > 0) {
                                        ArrayList<String> pairedDevicesList = new ArrayList<String>();
                                        for (BluetoothDevice device : pairedDevices) {
                                            String toSendPairedDevice = device.getName() + "," + device.getAddress();

                                            // Notify NodeService
                                            NodeService.SendPortMsg(context.getApplicationContext(),
                                                    ThingManager.Bluetooth,
                                                    ThingManager.BluetoothPairedDevicesPort,
                                                    toSendPairedDevice);
                                        }
                                        // TODO: Send all paired devices as single delimited string ?
                                        // String[] toSendPairedDevices = new String[pairedDevicesList.size()];
                                        // toSendPairedDevices  = pairedDevicesList.toArray(toSendPairedDevices);
                                    }
                                }
                            }
                        }
                        break;
                    }
                    //------------------------------- UI -----------------------------------------------
                    case aServerAPI.CONNECTIVITY_UI_UPDATE: {
                        Bundle b = intent.getExtras();
                        Boolean rxActive = b.getBoolean(aServerAPI.EXTRA_UPDATED_RX_STATE);
                        Boolean txActive = b.getBoolean(aServerAPI.EXTRA_UPDATED_TX_STATE);

                        outputStr.setAlpha(txActive ? 1.0f : 0.3f);
                        inputStr.setAlpha(rxActive ? 1.0f : 0.3f);
                        break;
                    }
                    //------------------------ IP Connectivity -----------------------------------------
                    case ConnectivityManager.CONNECTIVITY_ACTION: {
                        int networkType = intent.getIntExtra(ConnectivityManager.EXTRA_NETWORK_TYPE, -1);

                        ConnectivityManager cm = (ConnectivityManager) context.getSystemService(Context.CONNECTIVITY_SERVICE);
                        NetworkInfo activeNetInfo = cm.getActiveNetworkInfo();

                        if (activeNetInfo == null || !activeNetInfo.isConnected()) {
                            /* if null: airplane mode, or mobile data & wifi off, or out of signal */

                            //update global connectivity so that we don't constantly try to connect to cloud
                            NodeService.SetNetworkConnStatus(context, false);

                            //no internets, no message to send
                            break;
                        }

                        //update global connectivity: we do have the internets; rejoice
                        NodeService.SetNetworkConnStatus(context, true);

                        NodeService.SendWifiUpdate(context);
                        break;
                    }
                    //----------------------------- Bluetooth ------------------------------------------
                    case BluetoothAdapter.ACTION_STATE_CHANGED: {
                        int state = intent.getIntExtra(BluetoothAdapter.EXTRA_STATE, -1);

                        // Notify NodeService
                        NodeService.SendPortMsg(context.getApplicationContext(),
                                ThingManager.Bluetooth,
                                ThingManager.BluetoothPowerStatusPort,
                                ThingManager.bluetoothPowerStateCodesToNames.get(state));
                        break;
                    }
                    case BluetoothAdapter.ACTION_CONNECTION_STATE_CHANGED: {
                        // STATE_DISCONNECTED, STATE_CONNECTING, STATE_CONNECTED, STATE_DISCONNECTING --> EXTRA_CONNECTION_STATE
                        int state = intent.getIntExtra(BluetoothAdapter.EXTRA_CONNECTION_STATE, -1);

                        // Notify NodeService
                        NodeService.SendPortMsg(context.getApplicationContext(),
                                ThingManager.Bluetooth,
                                ThingManager.BluetoothConnectionStatusPort,
                                ThingManager.bluetoothConnectionStateCodesToNames.get(state));
                        break;
                    }
                    case BluetoothAdapter.ACTION_DISCOVERY_STARTED:
                        Log.d(TAG, "Device discovery started");
                        break;
                    case BluetoothDevice.ACTION_FOUND:
                        // Get the BluetoothDevice object from the Intent
                        BluetoothDevice device = intent.getParcelableExtra(BluetoothDevice.EXTRA_DEVICE);

                        // Notify NodeService
                        // TODO: Discuss - whether we should send more info
                        NodeService.SendPortMsg(context.getApplicationContext(),
                                ThingManager.Bluetooth,
                                ThingManager.BluetoothDiscoveredDevicesPort,
                                device.getName() + " (" + device.getAddress() + ")");
                        break;
                    case BluetoothAdapter.ACTION_DISCOVERY_FINISHED:
                        // This has to be called to ensure discovery is disabled
                        BluetoothAdapter.getDefaultAdapter().cancelDiscovery();
                        break;

                    //------------------------ Active Port Update-----------------------------------------
                    case NodeService.BROADCAST_ACTIVE_PORT_UPDATE: {
                        Bundle b = intent.getExtras();

                        String[] thingKeys = b.getStringArray(NodeService.EXTRA_UPDATED_ACTIVE_THINGS_KEYS);
                        if (thingKeys == null)
                            break;

                        String[] portKeys = b.getStringArray(NodeService.EXTRA_UPDATED_ACTIVE_PORT_KEYS);
                        if (portKeys == null)
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
                            if (portKey.equals(thingManager.GetPortKey(ThingManager.Switches, ThingManager.SwitchPort0))) {
                                outputSwitch1.setEnabled(true);
                            } else if (portKey.equals(thingManager.GetPortKey(ThingManager.Switches, ThingManager.SwitchPort1))) {
                                outputSwitch2.setEnabled(true);
                            } else if (portKey.equals(thingManager.GetPortKey(ThingManager.Switches, ThingManager.SwitchPort2))) {
                                outputSwitch3.setEnabled(true);
                            } else if (portKey.equals(thingManager.GetPortKey(ThingManager.Buttons, ThingManager.ButtonPort0))) {
                                button1.setEnabled(true);
                            } else if (portKey.equals(thingManager.GetPortKey(ThingManager.Buttons, ThingManager.ButtonPort1))) {
                                button2.setEnabled(true);
                            } else if (portKey.equals(thingManager.GetPortKey(ThingManager.Buttons, ThingManager.ButtonPort2))) {
                                button3.setEnabled(true);
                            } else if (portKey.equals(thingManager.GetPortKey(ThingManager.Slider1, ThingManager.SliderPort))) {
                                outputSeekBar.setEnabled(true);
                            } else if (portKey.equals(thingManager.GetPortKey(ThingManager.Camera, ThingManager.CameraPort))) {
                                cameraButton.setEnabled(true);
                            }
                        }

                        for (String thingKey : thingKeys) {
                            if (thingKey.equals(thingManager.GetThingKey(ThingManager.GPS))) {
                                gpsButton.setEnabled(true);
                            }
                        }
                        break;
                    }
                }
                //----------------------------------------------------------------------------------
            } catch (Exception e) {
                Helpers.logException(TAG, e);
            }
        }
    };
}
