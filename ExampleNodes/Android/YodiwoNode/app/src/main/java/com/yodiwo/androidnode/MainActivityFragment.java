package com.yodiwo.androidnode;

import android.bluetooth.BluetoothAdapter;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.net.NetworkInfo;
import android.net.Uri;
import android.net.ConnectivityManager;
import android.net.wifi.WifiInfo;
import android.net.wifi.WifiManager;
import android.bluetooth.BluetoothDevice;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;
import android.text.TextUtils;
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

import java.util.ArrayList;
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

    private TextView outputStr;
    private TextView inputStr;

    public MainActivityFragment() { }

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

        // Link button1 to code
        Button button1 = (Button) view.findViewById(R.id.button1);
        button1.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View view, MotionEvent motionEvent) {
                int action = motionEvent.getAction();
                if(action == MotionEvent.ACTION_DOWN)
                    NodeService.SendPortMsg(context, ThingManager.Buttons, ThingManager.ButtonPort0, NodeService.PortValue_Boolean_True);
                else if(action == MotionEvent.ACTION_UP)
                    NodeService.SendPortMsg(context, ThingManager.Buttons, ThingManager.ButtonPort0, NodeService.PortValue_Boolean_False);
                return false;
            }
        });

        // Link button2 to code
        Button button2 = (Button) view.findViewById(R.id.button2);
        button2.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View view, MotionEvent motionEvent) {
                int action = motionEvent.getAction();
                if(action == MotionEvent.ACTION_DOWN)
                    NodeService.SendPortMsg(context, ThingManager.Buttons, ThingManager.ButtonPort1, NodeService.PortValue_Boolean_True);
                else if(action == MotionEvent.ACTION_UP)
                    NodeService.SendPortMsg(context, ThingManager.Buttons, ThingManager.ButtonPort1, NodeService.PortValue_Boolean_False);
                return false;
            }
        });

        // Link button3 to code
        Button button3 = (Button) view.findViewById(R.id.button3);
        button3.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View view, MotionEvent motionEvent) {
                int action = motionEvent.getAction();
                if(action == MotionEvent.ACTION_DOWN)
                    NodeService.SendPortMsg(context, ThingManager.Buttons, ThingManager.ButtonPort2, NodeService.PortValue_Boolean_True);
                else if(action == MotionEvent.ACTION_UP)
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
                NodeService.SendPortMsg(context, ThingManager.Switches, ThingManager.ButtonPort0,
                        (isChecked) ? NodeService.PortValue_Boolean_True : NodeService.PortValue_Boolean_False);
            }
        });

        outputSwitch2.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
                // do something, the isChecked will be true if the switch is in the On position
                NodeService.SendPortMsg(context, ThingManager.Switches, ThingManager.ButtonPort1,
                        (isChecked) ? NodeService.PortValue_Boolean_True : NodeService.PortValue_Boolean_False);
            }
        });

        outputSwitch3.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
                // do something, the isChecked will be true if the switch is in the On position
                NodeService.SendPortMsg(context, ThingManager.Switches, ThingManager.ButtonPort2,
                        (isChecked) ? NodeService.PortValue_Boolean_True : NodeService.PortValue_Boolean_False);
            }
        });

        // GPS button
        Button gpsButton = (Button) view.findViewById(R.id.button_GPS);

        gpsButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                MainActivity main = (MainActivity) getActivity();
                main.SendNewLocation(null);
            }
        });

        LocalBroadcastManager.getInstance(context).registerReceiver(mMessageReceiverMainActivityService,
                new IntentFilter(NodeService.BROADCAST_THING_UPDATE));

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
    // Events from background services

    private BroadcastReceiver mMessageReceiverMainActivityService = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            String action = intent.getAction();
            Log.i(TAG, "Broadcast received: " + action);

            try {
                //----------------------------- Things Update --------------------------------------
                if (action.equals(NodeService.BROADCAST_THING_UPDATE)) {
                    Bundle b = intent.getExtras();
                    int portID = b.getInt(NodeService.EXTRA_UPDATED_PORT_ID, -1);
                    String thingKey = b.getString(NodeService.EXTRA_UPDATED_THING_KEY);
                    String thingName = b.getString(NodeService.EXTRA_UPDATED_THING_NAME);
                    String portState = b.getString(NodeService.EXTRA_UPDATED_STATE);
                    Boolean isEvent = b.getBoolean(NodeService.EXTRA_UPDATED_IS_EVENT);
                    Log.i(TAG, "Update from Thing:" + thingName);

                    //input (from server) ProgressBar (PortStateRsp or PortEventMsg)
                    if (thingKey.equals(thingManager.GetThingKey(ThingManager.InputProgressBar))) {
                        float progress = Float.parseFloat(portState);
                        if(progress >= 0 && progress <= 1)
                            inputProgressBar.setProgress((int) (inputProgressBar.getMax() * progress));
                    }
                    //output (towards server) SeekBar (only when updating stating via PortStateRsp)
                    else if (thingKey.equals(thingManager.GetThingKey(ThingManager.Slider1))) {
                        float progress = Float.parseFloat(portState);
                        if(progress >= 0 && progress <= 1)
                            outputSeekBar.setProgress((int) (outputSeekBar.getMax() * progress));
                    }
                    //input (from server) switches (PortStateRsp or PortEventMsg)
                    else if (thingKey.equals(thingManager.GetThingKey(ThingManager.InputSwitches))) {
                        if(portID == 0)
                            inputSwitch1.setChecked(Boolean.parseBoolean(portState));
                        else if(portID==1)
                            inputSwitch2.setChecked(Boolean.parseBoolean(portState));
                        else if(portID==2)
                            inputSwitch3.setChecked(Boolean.parseBoolean(portState));
                    }
                    //output (towards server) switches (only when updating stating via PortStateRsp)
                    else if (thingKey.equals(thingManager.GetThingKey(ThingManager.Switches))) {
                        if(portID == 0)
                            outputSwitch1.setChecked(Boolean.parseBoolean(portState));
                        else if(portID==1)
                            outputSwitch2.setChecked(Boolean.parseBoolean(portState));
                        else if(portID==2)
                            outputSwitch3.setChecked(Boolean.parseBoolean(portState));
                    }
                    else if (thingKey.equals(thingManager.GetThingKey(ThingManager.InputColors))) {
                        double val = Double.parseDouble(portState);
                        int color = 0xff000000 | (int)((double)0xffffff * val);

                        if(portID == 0) {
                            colorButton1.setBackgroundColor(color);
                        } else if(portID == 1) {
                            colorButton2.setBackgroundColor(color);
                        } else if(portID == 2) {
                            colorButton3.setBackgroundColor(color);
                        }
                    }
                    else if (thingKey.equals(thingManager.GetThingKey(ThingManager.InputAndroidIntent))) {
                        if(isEvent) {
                            Intent i = new Intent(android.content.Intent.ACTION_VIEW,
                                    Uri.parse(portState));
                            startActivity(i);
                        }
                    }
                    else if (thingKey.equals(thingManager.GetThingKey(ThingManager.Torch))) {
                        Intent i = new Intent(context, ThingsModuleService.class);

                        i.putExtra(ThingsModuleService.EXTRA_INTENT_FOR_THING, ThingsModuleService.EXTRA_TORCH_THING);
                        i.putExtra(ThingsModuleService.EXTRA_TORCH_THING_STATE, Boolean.parseBoolean(portState));
                        context.startService(i);
                    }
                    else if (thingKey.equals(thingManager.GetThingKey(ThingManager.BluetoothControl))) {
                        if (portID == ThingManager.BluetoothTriggerSingleShotDiscoveryPort) {
                            Boolean res = BluetoothAdapter.getDefaultAdapter().startDiscovery();
                            if (res == false) {
                                // Report
                                Log.e(TAG, "Failed to start device discovery");
                            }
                        }
                        else if (portID == ThingManager.BluetoothRequestPairedDevicesPort) {
                            Set<BluetoothDevice> pairedDevices = BluetoothAdapter.getDefaultAdapter().getBondedDevices();

                            if (pairedDevices.size() > 0) {
                                ArrayList<String> pairedDevicesList = new ArrayList<String>();
                                for (BluetoothDevice device : pairedDevices) {
                                    String toSendPairedDevice = device.getName() + "," + device.getAddress();

                                    // Notify NodeService
                                    NodeService.SendPortMsg(context.getApplicationContext(),
                                            ThingManager.BluetoothStatus,
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
                //------------------------------- UI -----------------------------------------------
                else if (action.equals(aServerAPI.CONNECTIVITY_UI_UPDATE)) {
                    Bundle b = intent.getExtras();
                    Boolean rxActive = b.getBoolean(aServerAPI.EXTRA_UPDATED_RX_STATE);
                    Boolean txActive = b.getBoolean(aServerAPI.EXTRA_UPDATED_TX_STATE);

                    outputStr.setAlpha(txActive ? 1.0f : 0.3f);
                    inputStr.setAlpha(rxActive ? 1.0f : 0.3f);
                }
                //----------------------------- WiFi -----------------------------------------------
                else if (action.equals(ConnectivityManager.CONNECTIVITY_ACTION)) {
                    int networkType = intent.getIntExtra(ConnectivityManager.EXTRA_NETWORK_TYPE, -1);

                    if (networkType == ConnectivityManager.TYPE_WIFI) {
                        String toSendState = "";
                        String toSendSSID = "";
                        String toSendRSSI = "";

                        ConnectivityManager connManager = (ConnectivityManager) context.getSystemService(Context.CONNECTIVITY_SERVICE);
                        NetworkInfo networkInfo = connManager.getNetworkInfo(ConnectivityManager.TYPE_WIFI);
                        NetworkInfo.State state = networkInfo.getState(); // Coarse-grained state

                        toSendState = state.toString(); // Port0

                        if (networkInfo.isConnected()) {
                            WifiManager wifiManager = (WifiManager)context.getSystemService(Context.WIFI_SERVICE);
                            WifiInfo wifiInfo = wifiManager.getConnectionInfo();

                            if (wifiInfo != null && !TextUtils.isEmpty(wifiInfo.getSSID())) {
                                toSendSSID = wifiInfo.getSSID(); // Port1
                                toSendRSSI = Integer.toString(wifiInfo.getRssi()); // Port2
                            }
                        }

                        // Notify NodeService
                        NodeService.SendPortMsg(context.getApplicationContext(),
                                ThingManager.WiFiStatus,
                                new String[] { toSendState, toSendSSID, toSendRSSI });
                    }
                }
                //----------------------------- Bluetooth ------------------------------------------
                else if (action.equals(BluetoothAdapter.ACTION_STATE_CHANGED)) {
                    int state = intent.getIntExtra(BluetoothAdapter.EXTRA_STATE, -1);

                    // Notify NodeService
                    NodeService.SendPortMsg(context.getApplicationContext(),
                            ThingManager.BluetoothStatus,
                            ThingManager.BluetoothPowerStatusPort,
                            ThingManager.bluetoothPowerStateCodesToNames.get(state));
                }
                else if (action.equals(BluetoothAdapter.ACTION_CONNECTION_STATE_CHANGED)) {
                    // STATE_DISCONNECTED, STATE_CONNECTING, STATE_CONNECTED, STATE_DISCONNECTING --> EXTRA_CONNECTION_STATE
                    int state = intent.getIntExtra(BluetoothAdapter.EXTRA_CONNECTION_STATE, -1);

                    // Notify NodeService
                    NodeService.SendPortMsg(context.getApplicationContext(),
                            ThingManager.BluetoothStatus,
                            ThingManager.BluetoothConnectionStatusPort,
                            ThingManager.bluetoothConnectionStateCodesToNames.get(state));
                }
                else if (action.equals(BluetoothAdapter.ACTION_DISCOVERY_STARTED)) {
                    Log.d(TAG, "Device discovery started");
                }
                else if (action.equals(BluetoothDevice.ACTION_FOUND)) {
                    // Get the BluetoothDevice object from the Intent
                    BluetoothDevice device = intent.getParcelableExtra(BluetoothDevice.EXTRA_DEVICE);

                    // Notify NodeService
                    // TODO: Discuss - whether we should send more info
                    NodeService.SendPortMsg(context.getApplicationContext(),
                            ThingManager.BluetoothStatus,
                            ThingManager.BluetoothDiscoveredDevicesPort,
                            device.getName() + " (" + device.getAddress() + ")");
                }
                else if (BluetoothAdapter.ACTION_DISCOVERY_FINISHED.equals(action)) {
                    // This has to be called to ensure discovery is disabled
                    BluetoothAdapter.getDefaultAdapter().cancelDiscovery();
                }
                //----------------------------------------------------------------------------------
            } catch (Exception e) {
                Helpers.logException(TAG, e);
            }
        }
    };
}
