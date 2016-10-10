package com.yodiwo.androidnode.core;

import android.annotation.TargetApi;
import android.app.Activity;
import android.app.AlertDialog;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.pm.PackageManager;
import android.graphics.Bitmap;
import android.hardware.camera2.CameraManager;
import android.os.Build;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.v4.content.LocalBroadcastManager;
import android.support.v7.app.AppCompatActivity;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.ViewFlipper;

import com.yodiwo.androidagent.core.Helpers;
import com.yodiwo.androidagent.core.NodeService;
import com.yodiwo.androidagent.core.PairingActivity;
import com.yodiwo.androidagent.core.SettingsActivity;
import com.yodiwo.androidagent.core.SettingsProvider;
import com.yodiwo.androidagent.plegma.ConfigParameter;
import com.yodiwo.androidagent.plegma.Port;
import com.yodiwo.androidagent.plegma.PortKey;
import com.yodiwo.androidagent.plegma.Thing;
import com.yodiwo.androidagent.plegma.ThingKey;
import com.yodiwo.androidagent.plegma.ThingUIHints;
import com.yodiwo.androidagent.plegma.ePortConf;
import com.yodiwo.androidagent.plegma.ePortType;
import com.yodiwo.androidagent.plegma.ioPortDirection;
import com.yodiwo.androidnode.R;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;
import java.util.Timer;
import java.util.TimerTask;

public class MainActivity extends AppCompatActivity {

    // =============================================================================================
    // Variables
    // =============================================================================================

    private final static String TAG = MainActivity.class.getSimpleName();

    private NfcHandler nfcHandler = null;
    private SettingsProvider settingsProvider = null;

    public static boolean isUnpairedByUser;

    public static final int REQUEST_ENABLE_BT = 666;
    public static final int REQUEST_CAMERA = 1234;
    public static boolean backgroundMode = false;

    private static boolean ActivityInitialized;
    private static boolean hasCamera;
    private static final int PERMISSION_REQUEST_COARSE_LOCATION = 1;
    protected static String mCurrentPhotoPath = null;

    /**
     * NebiT termination broadcast
     */
    public static final String BROADCAST_NEBIT_TEARDOWN = "MainActivity.BROADCAST_NEBIT_TEARDOWN";
    public static final String EXTRA_TEARDOWN_INFO = "EXTRA_TEARDOWN_INFO";

    private HashMap<String, Boolean> teardownProcess = new HashMap<>();

    // =============================================================================================
    // Activity Overrides
    // =============================================================================================

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        // Sets the default uncaught exception handler. This handler is invoked
        // in case any Thread dies due to an unhandled exception.
        if (SettingsActivity.isRelease)
            Thread.setDefaultUncaughtExceptionHandler(new UncaughtExceptionHandler(getApplicationContext()));

        // initialize main activity
        backgroundMode = false;
        ActivityInitialized = false;

        if (settingsProvider == null)
            settingsProvider = SettingsProvider.getInstance(this);

        if (settingsProvider.getNodeKey() != null && settingsProvider.getNodeSecretKey() != null) {
            ActivityInitialized = true;
            isUnpairedByUser = false;
            InitMainActivity();
        }
    }

    @Override
    protected void onStart() {
        super.onStart();
    }

    @Override
    public void onResume() {
        super.onResume();

        // Resume NFC
        if (nfcHandler != null)
            nfcHandler.setupForegroundNFCDispatch(this);

        backgroundMode = false;

        try {
            if (settingsProvider.getNodeKey() == null && settingsProvider.getNodeSecretKey() == null){
                if (!isUnpairedByUser) {
                    AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    builder.setTitle("Pair now?")
                            .setMessage("Application is not paired to Yodiwo Services. Pair now?")
                            .setIcon(android.R.drawable.ic_dialog_alert)
                            .setPositiveButton("Yes", new DialogInterface.OnClickListener() {
                                @Override
                                public void onClick(DialogInterface dialog, int which) {
                                    Intent intent = new Intent(MainActivity.this, PairingActivity.class);
                                    startActivity(intent);
                                }
                            })
                            .setNegativeButton("No", null)
                            .setCancelable(true)
                            .show();
                }
            }
            else {
                if (!ActivityInitialized || NodeService.getResetStatus()) {
                    ActivityInitialized = true;
                    InitMainActivity();
                }

                // Tell NodeService to handle Resuming itself
                NodeService.Resume(this);

                // start sensor listener
                SensorsListener sl = SensorsListener.getInstance(this);
                if (sl != null) {
                    sl.Start();
                }
            }
        } catch(Exception ex){
            Helpers.logException(TAG, ex);
        }
    }

    @Override
    public void onPause() {
        super.onPause();

        // Tell NodeService to handle Pausing itself
        NodeService.Pause(this);

        // release torch-camera resources
        if (ThingsModuleService.hasTorch || hasCamera) {
            ThingsModuleService.releaseCamera();
        }

        // Call this before onPause, otherwise an IllegalArgumentException is thrown as well.
        if (nfcHandler != null)
            nfcHandler.stopForegroundNFCDispatch(this);
    }

    @Override
    public void onDestroy(){
        if (SettingsProvider.getInstance(getApplicationContext()).getLogDataEnable())
            UncaughtExceptionHandler.saveLogcatToFile("NeBiT");
        super.onDestroy();
    }

    @Override
    protected void onStop() {
        super.onStop();
    }

    @Override
    protected void onUserLeaveHint()
    {
        backgroundMode = true;
        super.onUserLeaveHint();
    }
    @Override
    protected void onActivityResult(final int requestCode, final int resultCode, final Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        Helpers.log(Log.INFO, TAG, "Get activity results for:" + Integer.toString(requestCode) + " with code: " + Integer.toString(resultCode));

        if (requestCode == REQUEST_ENABLE_BT) {
            if (resultCode != RESULT_OK) {
                //Toast.makeText(this, "Bluetooth Thing is disabled", Toast.LENGTH_SHORT).show();
            }
        } else if (requestCode == REQUEST_CAMERA) {
            if (resultCode == RESULT_OK) {
                // Send the request
                NodeService.UploadFile(getApplicationContext(),
                        mCurrentPhotoPath,
                        Camera,
                        CameraPort);
            }
        }
    }

    @Override
    protected void onNewIntent(Intent intent) {
        nfcHandler.handleNFCIntent(intent, this);
    }

    @Override
    public void onRequestPermissionsResult(int requestCode,
                                           @NonNull String permissions[], @NonNull int[] grantResults) {
        switch (requestCode) {
            case PERMISSION_REQUEST_COARSE_LOCATION: {
                if (grantResults[0] == PackageManager.PERMISSION_GRANTED) {
                    Helpers.log(Log.DEBUG, TAG, "coarse location permission granted");
                } else {
                    final AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    builder.setTitle("Functionality limited");
                    builder.setMessage("Since location access has not been granted, this app will not be able to discover beacons when in the background.");
                    builder.setPositiveButton(android.R.string.ok, null);
                    if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1) {
                        builder.setOnDismissListener(new DialogInterface.OnDismissListener() {
                            @Override
                            public void onDismiss(DialogInterface dialog) {
                            }
                        });
                    }
                    builder.show();
                }
            }
        }
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_main, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        try {
            int id = item.getItemId();

            Intent intent = null;

            // Select the opened activity
            switch (id) {
                // Start settings activity
                case R.id.action_settings:
                    intent = new Intent(this, SettingsActivity.class);
                    break;
                // Start Pairing activity
                case R.id.action_repairing:
                    intent = new Intent(this, PairingActivity.class);
                    break;
            }

            if (intent != null) {
                startActivity(intent);
                return true;
            }
        }catch (Exception ex){
            Helpers.logException(TAG, ex);
            return super.onOptionsItemSelected(item);
        }

        return super.onOptionsItemSelected(item);
    }

    // =============================================================================================
    // Initialise MainActivity
    // =============================================================================================

    @TargetApi(Build.VERSION_CODES.JELLY_BEAN_MR1)
    private void InitMainActivity() {

        // start wifi scanning for Wisper
        //WisperHandler.startUp(this);

        // generateFetcher nfc handler
        nfcHandler = new NfcHandler(this);

        //start Node Service
        NodeService.Startup(this);

        // check camera availability
        if (Build.VERSION.SDK_INT < Build.VERSION_CODES.M)
            hasCamera = this.getPackageManager().hasSystemFeature(PackageManager.FEATURE_CAMERA_ANY);
        else{
            CameraManager cameraManager = (CameraManager) getSystemService(Context.CAMERA_SERVICE);
            hasCamera = cameraManager != null;
        }

        // Initialize torch
        ThingsModuleService.initTorch(this);
        if (!ThingsModuleService.hasTorch) {
            // do nothing
        } else {
            // Start service
            Intent intent = new Intent(this, ThingsModuleService.class);
            startService(intent);
        }

        // set predefined Things
        NodeService.ImportThings(getApplicationContext(), setThings(), false);
    }

    // =============================================================================================
    // TEARDOWN
    // =============================================================================================

    private int backPressCounter = 0;

    private class BackPressCounterTimeout extends TimerTask {
        @Override
        public void run() {
            backPressCounter = 0;
        }
    }

    @Override
    public void onBackPressed() {
        if (backPressCounter == 0) {
            //Toast.makeText(this, "Press back once more to disconnect and exit", Toast.LENGTH_SHORT).show();
            backPressCounter++;
            (new Timer()).schedule(new BackPressCounterTimeout(), 2 * 1000);  //2 sec
        }
        else {
            super.onBackPressed();
            stopEverything();
        }
        Helpers.log(Log.DEBUG, TAG, "Back button tapped");
        isUnpairedByUser = false;
    }

    private void exitApp() {
        AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(this, R.style.MyDialogStyle);
        alertDialogBuilder.setTitle("Exit")
                .setMessage("Are you sure you want to exit?")
                .setPositiveButton("YES", new DialogInterface.OnClickListener() {
                    public void onClick(final DialogInterface dialog, int id) {
                        dialog.cancel();
                        Helpers.log(Log.DEBUG, TAG, "Exit");
                        stopEverything();
                    }
                })
                .setNegativeButton("NO", new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int id) {
                        dialog.cancel();
                        Helpers.log(Log.DEBUG, TAG, "Cancel exit");
                        //Toast.makeText(getApplicationContext(), "Image deleted...", Toast.LENGTH_SHORT).show();
                    }
                })
                .create().show();
    }

    private void stopEverything() {

        // register receivers
        LocalBroadcastManager.getInstance(getApplicationContext()).registerReceiver(teardownService,
                new IntentFilter(MainActivity.BROADCAST_NEBIT_TEARDOWN));
        LocalBroadcastManager.getInstance(getApplicationContext()).registerReceiver(teardownService,
                new IntentFilter(NodeService.BROADCAST_NODE_SERVICE_TEARDOWN));

        // stop sensor services
        teardownProcess.put(SensorsListener.TAG, false);
        SensorsListener sl = SensorsListener.getInstance(this);
        if (sl != null) {
            sl.Stop();
        }

        // unregister location listener
        teardownProcess.put(LocationHandler.TAG, false);
        LocationHandler.getInstance(this).unsetLocationListener();

        // stop NodeService
        teardownProcess.put(NodeService.TAG, false);
        NodeService.Teardown(this);
    }

    private void terminate(){
        try{
            unregisterReceiver(teardownService);
        } catch (Exception ex){
            Helpers.logException(TAG, ex);
        } finally {
            getApplication().onTerminate();
            finish();
        }
    }

    public BroadcastReceiver teardownService = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            String action = intent.getAction();

            try{
                switch (action){
                    case BROADCAST_NEBIT_TEARDOWN:{
                        String info = intent.getExtras().getString(EXTRA_TEARDOWN_INFO);
                        if (teardownProcess.containsKey(info))
                            teardownProcess.put(info, true);
                        break;
                    }
                    case NodeService.BROADCAST_NODE_SERVICE_TEARDOWN:{
                        String info = NodeService.TAG;
                        if (teardownProcess.containsKey(info))
                            teardownProcess.put(info, true);
                        break;
                    }
                }

                // terminate app if everything is terminated
                boolean appTermination = true;
                for (Boolean isDone : teardownProcess.values()){
                    if (!isDone) {
                        appTermination = false;
                        break;
                    }
                }
                if (appTermination)
                    terminate();
            }catch (Exception ex){
                Helpers.logException(TAG, ex);
            }
        }
    };

    // =============================================================================================
    // Bluetooth
    // =============================================================================================

    public static final Map<Integer, String> bluetoothPowerStateCodesToNames = fillBluetoothPowerStateCodesToNames();

    private static Map<Integer, String> fillBluetoothPowerStateCodesToNames() {
        Map<Integer, String> map = new HashMap<>();
        map.put(10, "OFF");
        map.put(11, "TURNING ON");
        map.put(12, "ON");
        map.put(13, "TURNING OFF");
        return map;
    }

    // ---------------------------------------------------------------------------------------------

    public static final Map<Integer, String> bluetoothConnectionStateCodesToNames = fillBluetoothConnectionStateCodesToNames();

    private static Map<Integer, String> fillBluetoothConnectionStateCodesToNames() {
        Map<Integer, String> map = new HashMap<>();
        map.put(0, "DISCONNECTED");
        map.put(1, "CONNECTING");
        map.put(2, "CONNECTED");
        map.put(3, "DISCONNECTING");
        return map;
    }

    // =============================================================================================
    // Thing-specific
    // =============================================================================================

    public static final String Camera = "Camera";
    public static final String CameraPort = "0";
    public static final String CameraTriggerPort = "1";

    public static final String Buttons = "Buttons";
    public static final String ButtonPort0 = "0";
    public static final String ButtonPort1 = "1";
    public static final String ButtonPort2 = "2";

    public static final String Switches = "Switches";
    public static final String SwitchPort0 = "0";
    public static final String SwitchPort1 = "1";
    public static final String SwitchPort2 = "2";

    public static final String Slider = "Slider";
    public static final String SliderPort = "0";

    public static final String Proximity = "Proximity";
    public static final String ProximityPort = "0";

    public static final String BrightnessSensor = "BrightnessSensor";
    public static final String BrightnessSensorPort = "0";

    public static final String Accelerometer = "Accelerometer";
    public static final String AccelerometerX = "0";
    public static final String AccelerometerY = "1";
    public static final String AccelerometerZ = "2";

    public static final String ShakeDetector = "ShakeDetector";
    public static final String ShakeDetectorPort = "0";

    public static final String Gyroscope = "Gyroscope";
    public static final String GyroscopeX = "0";
    public static final String GyroscopeY = "1";
    public static final String GyroscopeZ = "2";
    public static final String GyroscopeW = "3";

    public static final String Rotation = "Rotation";
    public static final String RotationAzimuth = "0";
    public static final String RotationPitch = "1";
    public static final String RotationRoll = "2";

    public static final String GPS = "GPS";
    public static final String GPSLatPort = "0";
    public static final String GPSLonPort = "1";
    public static final String GPSAddressPort = "2";
    public static final String GPSCountryPort = "3";
    public static final String GPSPostalCodePort = "4";
    public static final String GPSTriggerPort = "5";
    public static final String GPSAccuracy = "Accuracy";
    public static final String GPSPowerRequirements = "Power requirements";
    public static final String GPSBetweenUpdatesInterval = "Between location updates interval (ms)";
    public static final String GPSBetweenUpdatesIntervalValue = "0";
    public static final String GPSBetweenUpdatesDistance = "Between location updates distance (m)";
    public static final String GPSBetweenUpdatesDistanceValue = "0";
    public static final String MediumValue = "Medium";
    public static final String LowValue = "Low";
    public static final String HighValue = "High";
    public static final String CoarseValue = "Coarse";
    public static final String FineValue = "Fine";

    public static final String WiFiStatus = "WiFi";
    public static final String WiFiStatusConnectionStatusPort = "0";
    public static final String WiFiStatusSSIDPort = "1";
    public static final String WiFiStatusRSSIPort = "2";

    public static final String Bluetooth = "Bluetooth";
    public static final String BluetoothPowerStatusPort = "0";
    public static final String BluetoothConnectionStatusPort = "1";
    public static final String BluetoothPairedDevicesPort = "2";
    public static final String BluetoothDiscoveredDevicesPort = "3";
    public static final String BluetoothTriggerSingleShotDiscoveryPort = "4";
    public static final String BluetoothRequestPairedDevicesPort = "5";

    public static final String BleBeacon = "Beacon";
    public static final String BleBeaconProximityUuidPort = "0";
    public static final String BleBeaconMajorIDPort = "1";
    public static final String BleBeaconMinorIDPort = "2";
    public static final String BleBeaconTxPowerPort = "3";
    public static final String BleBeaconDistancePort = "4";
    public static final String BleBeaconRssiPort = "5";
    public static final String BleBeaconTypeCodePort = "6";
    public static final String BleBeaconType = "Beacon type";
    public static final String BleBeaconScanPeriod = "Scan period (ms)";
    public static final int BleBeaconScanPeriodValue = 1000;
    public static final int BleBeaconBetweenScanPeriodValue = 5000;
    public static final String BleBeaconBetweenScanPeriod = "Between scan period (ms)";
    public static final String iBeacon = "iBeacon";
    public static final String EddystoneUIDBeacon = "Eddystone-UID";
    public static final String EddystoneTLMBeacon = "Eddystone-TLM";
    public static final String EddystoneURLBeacon = "Eddystone-URL ";
    public static final String EddystoneEIDBeacon = "Eddystone-EID ";
    public static final String AltBeacon = "AltBeacon";
    public static final String AllBeacons = "All";

    public static final String NFC = "NFC";
    public static final String NFCPort = "0";

    public static final String ProgressBar = "ProgressBar";
    public static final String ProgressBarPort = "0";

    public static final String InputSwitches = "InputSwitches";
    public static final String InputSwitchPort0 = "0";
    public static final String InputSwitchPort1 = "1";
    public static final String InputSwitchPort2 = "2";

    public static final String Leds = "Leds";
    public static final String LedsPort0 = "0";
    public static final String LedsPort1 = "1";
    public static final String LedsPort2 = "2";

    public static final String InputTextBox = "Text";
    public static final String InputTextBoxPort = "0";

    public static final String AndroidIntent = "AndroidIntent";
    public static final String AndroidIntentPort = "0";

    public static final String Torch = "Torch";
    public static final String TorchPort = "0";

    public static final String PhotoViewer = "PhotoViewer";
    public static final String PhotoViewerPort = "0";

    // ---------------------------------------------------------------------------------------------

    private ArrayList<Thing> setThings() {
        ArrayList<Thing> preDefinedThings = new ArrayList<>();
        ArrayList<ConfigParameter> Config;
        ConfigParameter conf;

        if (settingsProvider == null)
            settingsProvider = SettingsProvider.getInstance(this);

        String nodeKey = settingsProvider.getNodeKey();
        String thingKey;
        Thing thing;

        // Virtual Buttons
        Config = new ArrayList<>();

        thingKey = ThingKey.CreateKey(nodeKey, Buttons);
        thing = new Thing(thingKey,
                Buttons,
                Config,
                new ArrayList<Port>(),
                "com.yodiwo.out.buttons.default",
                "",
                new ThingUIHints("/Content/img/icons/Generic/thing-gobutton.png", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, ButtonPort0),
                "Button1", "",
                ioPortDirection.Output, ePortType.Boolean, NodeService.PortValue_Boolean_False, 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, ButtonPort1),
                "Button2", "",
                ioPortDirection.Output, ePortType.Boolean, NodeService.PortValue_Boolean_False, 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, ButtonPort2),
                "Button3", "",
                ioPortDirection.Output, ePortType.Boolean, NodeService.PortValue_Boolean_False, 0, ePortConf.None));

        preDefinedThings.add(thing);

        // Virtual Switches
        Config = new ArrayList<>();

        thingKey = ThingKey.CreateKey(nodeKey, Switches);
        thing = new Thing(thingKey,
                Switches,
                Config,
                new ArrayList<Port>(),
                "com.yodiwo.out.switches.onoff.default",
                "",
                new ThingUIHints("/Content/img/icons/Generic/switch.png", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, SwitchPort0),
                "Switch1", "",
                ioPortDirection.Output, ePortType.Boolean, NodeService.PortValue_Boolean_False, 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, SwitchPort1),
                "Switch2", "",
                ioPortDirection.Output, ePortType.Boolean, NodeService.PortValue_Boolean_False, 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, SwitchPort2),
                "Switch3", "",
                ioPortDirection.Output, ePortType.Boolean, NodeService.PortValue_Boolean_False, 0, ePortConf.None));

        preDefinedThings.add(thing);

        // Camera thing + trigger
        Config = new ArrayList<>();

        thingKey = ThingKey.CreateKey(nodeKey, Camera);
        thing = new Thing(thingKey,
                Camera,
                Config,
                new ArrayList<Port>(),
                "",
                // TODO: insert Type according to ThingTypeModelLibrary
                "",
                new ThingUIHints("/Content/img/icons/Generic/thing-camera.png", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, CameraPort),
                "Output", "",
                ioPortDirection.Output, ePortType.String, "", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, CameraTriggerPort),
                "Trigger", "",
                ioPortDirection.Input, ePortType.Boolean, NodeService.PortValue_Boolean_False, 0, ePortConf.PropagateAllEvents));

        preDefinedThings.add(thing);

        // Slider
        Config = new ArrayList<>();

        thingKey = ThingKey.CreateKey(nodeKey, Slider);
        thing = new Thing(thingKey,
                Slider,
                Config,
                new ArrayList<Port>(),
                "com.yodiwo.out.seekbars.default",
                "",
                new ThingUIHints("/Content/img/icons/Generic/thing-slider.png", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, SliderPort),
                "Value", "",
                ioPortDirection.Output, ePortType.Decimal, "0", 0, ePortConf.None));

        preDefinedThings.add(thing);

        // BrightnessSensor
        Config = new ArrayList<>();

        thingKey = ThingKey.CreateKey(nodeKey, BrightnessSensor);
        thing = new Thing(thingKey,
                BrightnessSensor,
                Config,
                new ArrayList<Port>(),
                "com.yodiwo.out.sensors.light.nonNormalized",
                "",
                new ThingUIHints("/Content/img/icons/Generic/brightness.png", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, BrightnessSensorPort),
                "Value", "",
                ioPortDirection.Output, ePortType.Decimal, "0", 0, ePortConf.None));

        preDefinedThings.add(thing);

        // Proximity
        Config = new ArrayList<>();

        thingKey = ThingKey.CreateKey(nodeKey, Proximity);
        thing = new Thing(thingKey,
                Proximity,
                Config,
                new ArrayList<Port>(),
                "com.yodiwo.out.sensors.proximity.default",
                "",
                new ThingUIHints("/Content/img/icons/Generic/thing-proximity.png", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, ProximityPort),
                "State", "",
                ioPortDirection.Output, ePortType.Boolean, "false", 0, ePortConf.IsTrigger));

        preDefinedThings.add(thing);

        // Accelerometer
        Config = new ArrayList<>();

        thingKey = ThingKey.CreateKey(nodeKey, Accelerometer);
        thing = new Thing(thingKey,
                Accelerometer,
                Config,
                new ArrayList<Port>(),
                "com.yodiwo.out.sensors.accelerometer.default",
                "",
                new ThingUIHints("/Content/img/icons/Generic/accelerometer.jpg", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, AccelerometerX),
                "X", "",
                ioPortDirection.Output, ePortType.DecimalHigh, "0", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, AccelerometerY),
                "Y", "",
                ioPortDirection.Output, ePortType.DecimalHigh, "0", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, AccelerometerZ),
                "Z", "",
                ioPortDirection.Output, ePortType.DecimalHigh, "0", 0, ePortConf.None));

        preDefinedThings.add(thing);

        // Shake detector
        Config = new ArrayList<>();

        thingKey = ThingKey.CreateKey(nodeKey, ShakeDetector);
        thing = new Thing(thingKey,
                ShakeDetector,
                Config,
                new ArrayList<Port>(),
                "com.yodiwo.out.shakedetectors.default",
                "",
                new ThingUIHints("/Content/img/icons/Generic/thing-shake.png", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, ShakeDetectorPort),
                "Event", "",
                ioPortDirection.Output, ePortType.Boolean, NodeService.PortValue_Boolean_False, 0, ePortConf.IsTrigger));

        preDefinedThings.add(thing);

        // Rotation
        Config = new ArrayList<>();

        thingKey = ThingKey.CreateKey(nodeKey, Rotation);
        thing = new Thing(thingKey,
                Rotation,
                Config,
                new ArrayList<Port>(),
                "com.yodiwo.out.sensors.rotation.default",
                "",
                new ThingUIHints("/Content/img/icons/Generic/accelerometer.jpg", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, RotationAzimuth),
                "Azimuth", "",
                ioPortDirection.Output, ePortType.DecimalHigh, "0", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, RotationPitch),
                "Pitch", "",
                ioPortDirection.Output, ePortType.DecimalHigh, "0", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, RotationRoll),
                "Roll", "",
                ioPortDirection.Output, ePortType.DecimalHigh, "0", 0, ePortConf.None));

        preDefinedThings.add(thing);

        // Gyroscope
        Config = new ArrayList<>();

        thingKey = ThingKey.CreateKey(nodeKey, Gyroscope);
        thing = new Thing(thingKey,
                Gyroscope,
                Config,
                new ArrayList<Port>(),
                "com.yodiwo.out.sensors.gyroscope.default",
                "",
                new ThingUIHints("/Content/img/icons/Generic/accelerometer.jpg", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, GyroscopeX),
                "X", "",
                ioPortDirection.Output, ePortType.DecimalHigh, "0", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, GyroscopeY),
                "Y", "",
                ioPortDirection.Output, ePortType.DecimalHigh, "0", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, GyroscopeZ),
                "Z", "",
                ioPortDirection.Output, ePortType.DecimalHigh, "0", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, GyroscopeW),
                "W", "",
                ioPortDirection.Output, ePortType.DecimalHigh, "0", 0, ePortConf.None));

        preDefinedThings.add(thing);

        // NFC support
        Config = new ArrayList<>();

        if (nfcHandler != null && nfcHandler.getNfcAdapter() != null) {
            thingKey = ThingKey.CreateKey(nodeKey, NFC);
            thing = new Thing(thingKey,
                    NFC,
                    Config,
                    new ArrayList<Port>(),
                    "com.yodiwo.out.nfc.default",
                    "",
                    new ThingUIHints("/Content/img/icons/Generic/nfc.png", ""));

            thing.Ports.add(new Port(PortKey.CreateKey(thingKey, NFCPort),
                    "Value", "",
                    ioPortDirection.Output, ePortType.String, "", 0, ePortConf.IsTrigger));

            preDefinedThings.add(thing);
        }

        // GPS support + trigger
        Config = new ArrayList<>();
        conf = new ConfigParameter(GPSAccuracy, MediumValue);
        Config.add(conf);
        conf = new ConfigParameter(GPSPowerRequirements, LowValue);
        Config.add(conf);
        conf = new ConfigParameter(GPSBetweenUpdatesDistance, String.valueOf(GPSBetweenUpdatesDistanceValue));
        Config.add(conf);
        conf = new ConfigParameter(GPSBetweenUpdatesInterval, String.valueOf(GPSBetweenUpdatesIntervalValue));
        Config.add(conf);

        thingKey = ThingKey.CreateKey(nodeKey, GPS);
        thing = new Thing(thingKey,
                GPS,
                Config,
                new ArrayList<Port>(),
                "",
                // TODO: insert Type according to ThingTypeModelLibrary
                "",
                new ThingUIHints("/Content/img/icons/Generic/gps.png", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, GPSLatPort),
                "Latitude", "GPS Latitude coordinate",
                ioPortDirection.Output, ePortType.DecimalHigh, "", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, GPSLonPort),
                "Longitude", "GPS Longitude coordinate",
                ioPortDirection.Output, ePortType.DecimalHigh, "", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, GPSAddressPort),
                "Address", "GPS acquired address",
                ioPortDirection.Output, ePortType.String, "", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, GPSCountryPort),
                "Country", "GPS acquired country name",
                ioPortDirection.Output, ePortType.String, "", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, GPSPostalCodePort),
                "Postal code", "GPS acquired postal code",
                ioPortDirection.Output, ePortType.String, "", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, GPSTriggerPort),
                "Trigger", "",
                ioPortDirection.Input, ePortType.Boolean, NodeService.PortValue_Boolean_False, 0, ePortConf.PropagateAllEvents));

        preDefinedThings.add(thing);

        // WiFi status thing
        Config = new ArrayList<>();

        thingKey = ThingKey.CreateKey(nodeKey, WiFiStatus);
        thing = new Thing(thingKey,
                WiFiStatus,
                Config,
                new ArrayList<Port>(),
                "",
                // TODO: insert Type according to ThingTypeModelLibrary
                "",
                new ThingUIHints("/Content/img/icons/Generic/icon-thing-genericwifi.png", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, WiFiStatusConnectionStatusPort),
                "Connection status", "WiFi connection status",
                ioPortDirection.Output, ePortType.String, "", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, WiFiStatusSSIDPort),
                "SSID", "The SSID of the WiFi access point the device is connected to",
                ioPortDirection.Output, ePortType.String, "", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, WiFiStatusRSSIPort),
                "RSSI", "Received Signal Strength Indication (RSSI) of the WiFi access point the device is connected to (dBm)",
                ioPortDirection.Output, ePortType.Integer, "", 0, ePortConf.None));

        preDefinedThings.add(thing);

        // Bluetooth status thing
        Config = new ArrayList<>();

        thingKey = ThingKey.CreateKey(nodeKey, Bluetooth);
        thing = new Thing(thingKey,
                Bluetooth,
                Config,
                new ArrayList<Port>(),
                "",
                // TODO: insert Type according to ThingTypeModelLibrary
                "",
                new ThingUIHints("/Content/img/icons/Generic/icon-thing-genericbluetooth.png", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, BluetoothPowerStatusPort),
                "Power status", "Bluetooth module power status",
                ioPortDirection.Output, ePortType.String, "", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, BluetoothConnectionStatusPort),
                "Connection status", "Indicates whether the bluetooth module is connected to any profile of any remote device",
                ioPortDirection.Output, ePortType.String, "", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, BluetoothPairedDevicesPort),
                "Paired devices", "Names of all remote devices paired to the bluetooth module",
                ioPortDirection.Output, ePortType.String, "", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, BluetoothDiscoveredDevicesPort),
                "Discovered devices", "Names of all discovered remote devices",
                ioPortDirection.Output, ePortType.String, "", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, BluetoothTriggerSingleShotDiscoveryPort),
                "Trigger device discovery", "Initiate a single-shot device discovery procedure",
                ioPortDirection.Input, ePortType.Boolean, "", 0, ePortConf.PropagateAllEvents));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, BluetoothRequestPairedDevicesPort),
                "Request paired devices", "Request all remote devices currently paired to the bluetooth module",
                ioPortDirection.Input, ePortType.Boolean, "", 0, ePortConf.PropagateAllEvents));

        preDefinedThings.add(thing);

        // BLE beacon detector status thing
        Config = new ArrayList<>();
        conf = new ConfigParameter(BleBeaconType,
                AllBeacons,
                "Specify beacon type to read: " +
                        iBeacon + ", " +
                        EddystoneUIDBeacon + ", " +
                        EddystoneURLBeacon + ", " +
                        EddystoneTLMBeacon + ", " +
                        EddystoneEIDBeacon + ", " +
                        AltBeacon + ", " +
                        AllBeacons
        );
        Config.add(conf);
        conf = new ConfigParameter(BleBeaconScanPeriod, String.valueOf(BleBeaconScanPeriodValue));
        Config.add(conf);
        conf = new ConfigParameter(BleBeaconBetweenScanPeriod, String.valueOf(BleBeaconBetweenScanPeriodValue));
        Config.add(conf);

        thingKey = ThingKey.CreateKey(nodeKey, BleBeacon);
        thing = new Thing(thingKey,
                BleBeacon,
                Config,
                new ArrayList<Port>(),
                "com.yodiwo.out.beacon.default",
                "",
                new ThingUIHints("/Content/img/icons/Generic/ibeacon.png", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, BleBeaconProximityUuidPort),
                "Proximity UUID", "Proximity UUID of the detected beacon",
                ioPortDirection.Output, ePortType.String, "", 0, ePortConf.SupressIdenticalEvents));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, BleBeaconMajorIDPort),
                "Major value", "Major value of the detected beacon",
                ioPortDirection.Output, ePortType.String, "", 0, ePortConf.SupressIdenticalEvents));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, BleBeaconMinorIDPort),
                "Minor value", "Minor value of the detected beacon",
                ioPortDirection.Output, ePortType.String, "", 0, ePortConf.SupressIdenticalEvents));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, BleBeaconTxPowerPort),
                "Tx Power", "Measured power of the detected beacon",
                ioPortDirection.Output, ePortType.String, "", 0, ePortConf.SupressIdenticalEvents));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, BleBeaconDistancePort),
                "Distance", "Distance from beacon in meters",
                ioPortDirection.Output, ePortType.String, "", 0, ePortConf.SupressIdenticalEvents));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, BleBeaconRssiPort),
                "Rssi", "Received Signal Strength Indicator of of the beacon's signal",
                ioPortDirection.Output, ePortType.String, "", 0, ePortConf.SupressIdenticalEvents));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, BleBeaconTypeCodePort),
                "Beacon type code", "Beacon type code",
                ioPortDirection.Output, ePortType.String, "", 0, ePortConf.SupressIdenticalEvents));

        preDefinedThings.add(thing);

        // Progress Bar
        Config = new ArrayList<>();

        thingKey = ThingKey.CreateKey(nodeKey, ProgressBar);
        thing = new Thing(thingKey,
                ProgressBar,
                Config,
                new ArrayList<Port>(),
                "com.yodiwo.in.seekbars.default",
                "",
                new ThingUIHints("/Content/img/icons/Generic/progress_bar.png", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, ProgressBarPort),
                "Value", "",
                ioPortDirection.Input, ePortType.Decimal, "0", 0, ePortConf.None));

        preDefinedThings.add(thing);

        // Virtual Switches
        Config = new ArrayList<>();

        thingKey = ThingKey.CreateKey(nodeKey, InputSwitches);
        thing = new Thing(thingKey,
                InputSwitches,
                Config,
                new ArrayList<Port>(),
                "com.yodiwo.in.switches.onoff.default",
                "",
                new ThingUIHints("/Content/img/icons/Generic/switch.png", ""));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, InputSwitchPort0),
                "Switch1", "",
                ioPortDirection.Input, ePortType.Boolean, NodeService.PortValue_Boolean_False, 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, InputSwitchPort1),
                "Switch2", "",
                ioPortDirection.Input, ePortType.Boolean, NodeService.PortValue_Boolean_False, 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, InputSwitchPort2),
                "Switch3", "",
                ioPortDirection.Input, ePortType.Boolean, NodeService.PortValue_Boolean_False, 0, ePortConf.None));

        preDefinedThings.add(thing);

        // Virtual Color
        Config = new ArrayList<>();

        thingKey = ThingKey.CreateKey(nodeKey, Leds);
        thing = new Thing(thingKey,
                Leds,
                Config,
                new ArrayList<Port>(),
                "com.yodiwo.in.lights.NormalizedDimmer",
                "",
                new ThingUIHints("/Content/img/icons/Generic/thing-genericlight.png", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, LedsPort0),
                "Led1", "",
                ioPortDirection.Input, ePortType.Decimal, "", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, LedsPort1),
                "Led2", "",
                ioPortDirection.Input, ePortType.Decimal, "", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, LedsPort2),
                "Led3", "",
                ioPortDirection.Input, ePortType.Decimal, "", 0, ePortConf.None));

        preDefinedThings.add(thing);

        // Text Box
        Config = new ArrayList<>();

        thingKey = ThingKey.CreateKey(nodeKey, InputTextBox);
        thing = new Thing(thingKey,
                InputTextBox,
                Config,
                new ArrayList<Port>(),
                "com.yodiwo.in.text.default",
                "",
                new ThingUIHints("/Content/img/icons/Generic/thing-text.png", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, InputTextBoxPort),
                "Text", "",
                ioPortDirection.Input, ePortType.String, "", 0, ePortConf.PropagateAllEvents));

        preDefinedThings.add(thing);

        // Android Intent
        Config = new ArrayList<>();

        thingKey = ThingKey.CreateKey(nodeKey, AndroidIntent);
        thing = new Thing(thingKey,
                AndroidIntent,
                Config,
                new ArrayList<Port>(),
                "com.yodiwo.in.androidintent.default",
                "",
                new ThingUIHints("/Content/img/icons/Generic/android.png", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, AndroidIntentPort),
                "Intent", "",
                ioPortDirection.Input, ePortType.String, "", 0, ePortConf.PropagateAllEvents));

        preDefinedThings.add(thing);

        // Torch
        Config = new ArrayList<>();

        if (ThingsModuleService.hasTorch) {
            thingKey = ThingKey.CreateKey(nodeKey, Torch);
            thing = new Thing(thingKey,
                    Torch,
                    Config,
                    new ArrayList<Port>(),
                    "com.yodiwo.in.lights.lights",
                    "",
                    new ThingUIHints("/Content/img/icons/Generic/icon-thing-generictorch.png", ""));

            thing.Ports.add(new Port(PortKey.CreateKey(thingKey, TorchPort),
                    "State", "",
                    ioPortDirection.Input, ePortType.Boolean, NodeService.PortValue_Boolean_False, 0, ePortConf.None));

            preDefinedThings.add(thing);
        }

        // PhotoViewer
        Config = new ArrayList<>();

        thingKey = ThingKey.CreateKey(nodeKey, PhotoViewer);
        thing = new Thing(thingKey,
                PhotoViewer,
                Config,
                new ArrayList<Port>(),
                "com.yodiwo.in.lcds.default",
                "",
                new ThingUIHints("/Content/img/icons/Generic/thing-imageviewer.png", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, PhotoViewerPort),
                "Photo", "",
                ioPortDirection.Input, ePortType.String, "", 0, ePortConf.PropagateAllEvents));

        preDefinedThings.add(thing);

        return preDefinedThings;
    }

    // =============================================================================================
    // Photo Viewer
    // =============================================================================================

    public void onUserImageHandling(final Activity activity, final Bitmap bitmap) {
        final ViewFlipper vf = (ViewFlipper) findViewById(R.id.viewFlipper);
        AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(this, R.style.MyDialogStyle);
        alertDialogBuilder.setTitle("An image has been received.");
        alertDialogBuilder
                .setPositiveButton("Show", new DialogInterface.OnClickListener() {
                    public void onClick(final DialogInterface dialog, int id) {
                        dialog.cancel();
                        vf.showNext();
                        ImageView imageToDisplay = (ImageView) findViewById(R.id.imageToDisplay);
                        imageToDisplay.setScaleType(ImageView.ScaleType.FIT_CENTER);
                        imageToDisplay.setImageBitmap(bitmap);
                        Button imageSaveButton = (Button) findViewById(R.id.image_save_button);
                        Button imageDeleteButton = (Button) findViewById(R.id.image_delete_button);

                        // Save button
                        if (imageSaveButton != null) {
                            imageSaveButton.setOnClickListener(new View.OnClickListener() {
                                @Override
                                public void onClick(View view) {
                                    vf.showPrevious();
                                    PhotoHandler.createImageFile(activity, bitmap);
                                }
                            });
                        }

                        // Delete Button
                        if (imageDeleteButton != null) {
                            imageDeleteButton.setOnClickListener(new View.OnClickListener() {
                                @Override
                                public void onClick(View view) {
                                    Helpers.log(Log.DEBUG, TAG, "Image deleted");
                                    dialog.cancel();
                                    vf.showPrevious();
                                    //Toast.makeText(getApplicationContext(), "Image deleted...", Toast.LENGTH_SHORT).show();
                                }
                            });
                        }
                    }
                })
                .setNegativeButton("Delete", new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int id) {
                        dialog.cancel();
                        Helpers.log(Log.DEBUG, TAG, "Image deleted");
                        //Toast.makeText(getApplicationContext(), "Image deleted...", Toast.LENGTH_SHORT).show();
                    }
                })
                .create().show();
    }
}
