package com.yodiwo.androidnode;

import android.content.Context;
import android.nfc.NfcAdapter;

import com.yodiwo.plegma.ConfigParameter;
import com.yodiwo.plegma.Port;
import com.yodiwo.plegma.PortKey;
import com.yodiwo.plegma.Thing;
import com.yodiwo.plegma.ThingKey;
import com.yodiwo.plegma.ThingUIHints;
import com.yodiwo.plegma.ePortConf;
import com.yodiwo.plegma.ePortType;
import com.yodiwo.plegma.ioPortDirection;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;

import javax.xml.namespace.NamespaceContext;

/**
 * Created by FxBit on 7/8/2015.
 */
public class ThingManager {


    // =============================================================================================
    // Static information

    public static final String TAG = ThingManager.class.getSimpleName();
    private static ThingManager thingManager = null;

    private ArrayList<ConfigParameter> Config;
    private ConfigParameter conf;


    public static ThingManager getInstance(Context context) {
        if (thingManager == null) {
            thingManager = new ThingManager(context.getApplicationContext());
        }

        return thingManager;
    }

    // =============================================================================================
    // Things names

    public static final String Camera = "Camera";
    public static final int CameraPort = 0;
    public static final int CameraTriggerPort = 1;

    public static final String Buttons = "Buttons";
    public static final int ButtonPort0 = 0;
    public static final int ButtonPort1 = 1;
    public static final int ButtonPort2 = 2;

    public static final String Switches = "Switches";
    public static final int SwitchPort0 = 0;
    public static final int SwitchPort1 = 1;
    public static final int SwitchPort2 = 2;

    public static final String Slider1 = "Slider1";
    public static final int SliderPort = 0;

    public static final String Proximity = "Proximity";
    public static final int ProximityPort = 0;

    public static final String BrightnessSensor = "BrightnessSensor";
    public static final int BrightnessSensorPort = 0;

    public static final String Accelerometer = "Accelerometer";
    public static final int AccelerometerX = 0;
    public static final int AccelerometerY = 1;
    public static final int AccelerometerZ = 2;
    public static final int AccelerometerShaken = 3;

    public static final String Gyroscope = "Gyroscope";
    public static final int GyroscopeX = 0;
    public static final int GyroscopeY = 1;
    public static final int GyroscopeZ = 2;
    public static final int GyroscopeW = 3;

    public static final String Rotation = "Rotation";
    public static final int RotationAzimuth = 0;
    public static final int RotationPitch = 1;
    public static final int RotationRoll = 2;

    public static final String GPS = "GPS";
    public static final int GPSLatPort = 0;
    public static final int GPSLonPort = 1;
    public static final int GPSAddressPort = 2;
    public static final int GPSCountryPort = 3;
    public static final int GPSPostalCodePort = 4;
    public static final int GPSTriggerPort = 5;

    public static final String WiFiStatus = "WiFi";
    public static final int WiFiStatusConnectionStatusPort = 0;
    public static final int WiFiStatusSSIDPort = 1;
    public static final int WiFiStatusRSSIPort = 2;

    public static final String Bluetooth = "Bluetooth";
    public static final int BluetoothPowerStatusPort = 0;
    public static final int BluetoothConnectionStatusPort = 1;
    public static final int BluetoothPairedDevicesPort = 2;
    public static final int BluetoothDiscoveredDevicesPort = 3;
    public static final int BluetoothTriggerSingleShotDiscoveryPort = 4;
    public static final int BluetoothRequestPairedDevicesPort = 5;

    //----------------------------------------------------------------------------------------------

    public static final String OutputNFC = "NFC";
    public static final int OutputNFCPort = 0;

    public static final String InputProgressBar = "ProgressBar";
    public static final int InputProgressBarPort = 0;

    public static final String InputSwitches = "InputSwitches";
    public static final int InputSwitchPort0 = 0;
    public static final int InputSwitchPort1 = 1;
    public static final int InputSwitchPort2 = 2;

    public static final String InputColors = "Leds";
    public static final int InputColorPort0 = 0;
    public static final int InputColorPort1 = 1;
    public static final int InputColorPort2 = 2;

    public static final String InputTextBox = "Text";
    public static final int InputTextBoxPort = 0;

    public static final String InputAndroidIntent = "AndroidIntent";
    public static final int InputAndroidIntentPort = 0;

    public static final String Torch = "Torch";
    public static final int InputTorchPort0 = 0;

    public static final String PhotoViewer = "PhotoViewer";
    public static final int PhotoViewerPort0 = 0;


    // =============================================================================================
    // Thing-specific helpers

    // Bluetooth
    public static final Map<Integer, String> bluetoothPowerStateCodesToNames = fillBluetoothPowerStateCodesToNames();

    private static Map<Integer, String> fillBluetoothPowerStateCodesToNames() {
        Map<Integer, String> map = new HashMap<Integer, String>();
        map.put(10, "OFF");
        map.put(11, "TURNING ON");
        map.put(12, "ON");
        map.put(13, "TURNING OFF");
        return map;
    }

    public static final Map<Integer, String> bluetoothConnectionStateCodesToNames = fillBluetoothConnectionStateCodesToNames();

    private static Map<Integer, String> fillBluetoothConnectionStateCodesToNames() {
        Map<Integer, String> map = new HashMap<Integer, String>();
        map.put(0, "DISCONNECTED");
        map.put(1, "CONNECTING");
        map.put(2, "CONNECTED");
        map.put(3, "DISCONNECTING");
        return map;
    }


    // =============================================================================================
    // Things Initialization

    private SettingsProvider settingsProvider;
    private Context context;

    public ThingManager(Context context) {
        this.context = context;
        this.settingsProvider = SettingsProvider.getInstance(context);
    }

    // ---------------------------------------------------------------------------------------------

    // Here we initialize all things and add them to node service
    // Any new things must be added here.
    public void RegisterThings() {
        String nodeKey = settingsProvider.getNodeKey();
        String thingKey = "";
        Thing thing = null;

        // Clean old local things
        NodeService.CleanThings(context);

        // ----------------------------------------------
        // Virtual Buttons
        Config = new ArrayList<ConfigParameter>();

        thingKey = ThingKey.CreateKey(nodeKey, Buttons);
        thing = new Thing(thingKey,
                Buttons,
                Config,
                new ArrayList<Port>(),
                "yodiwo.output.buttons",
                "",
                new ThingUIHints("/Content/VirtualGateway/img/icon-thing-gobutton.png", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(ButtonPort0)),
                "Button1", "",
                ioPortDirection.Output, ePortType.Boolean, NodeService.PortValue_Boolean_False, 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(ButtonPort1)),
                "Button2", "",
                ioPortDirection.Output, ePortType.Boolean, NodeService.PortValue_Boolean_False, 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(ButtonPort2)),
                "Button3", "",
                ioPortDirection.Output, ePortType.Boolean, NodeService.PortValue_Boolean_False, 0, ePortConf.None));

        NodeService.AddThing(context, thing);


        // ----------------------------------------------
        // Virtual Switches
        Config = new ArrayList<ConfigParameter>();

        thingKey = ThingKey.CreateKey(nodeKey, Switches);
        thing = new Thing(thingKey,
                Switches,
                Config,
                new ArrayList<Port>(),
                "yodiwo.output.switches.onoff",
                "",
                new ThingUIHints("/Content/VirtualGateway/img/switch.png", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(SwitchPort0)),
                "Switch1", "",
                ioPortDirection.Output, ePortType.Boolean, NodeService.PortValue_Boolean_False, 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(SwitchPort1)),
                "Switch2", "",
                ioPortDirection.Output, ePortType.Boolean, NodeService.PortValue_Boolean_False, 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(SwitchPort2)),
                "Switch3", "",
                ioPortDirection.Output, ePortType.Boolean, NodeService.PortValue_Boolean_False, 0, ePortConf.None));

        NodeService.AddThing(context, thing);

        // ----------------------------------------------
        // Camera thing + trigger
        Config = new ArrayList<ConfigParameter>();

        thingKey = ThingKey.CreateKey(nodeKey, Camera);
        thing = new Thing(thingKey,
                Camera,
                Config,
                new ArrayList<Port>(),
                "",
                // TODO: insert Type according to ThingTypeModelLibrary
                "",
                new ThingUIHints("/Content/VirtualGateway/img/icon-thing-camera.png", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(CameraPort)),
                "Camera", "",
                ioPortDirection.Output, ePortType.String, "", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(CameraTriggerPort)),
                "Trigger", "",
                ioPortDirection.Input, ePortType.Boolean, NodeService.PortValue_Boolean_False, 0, ePortConf.ReceiveAllEvents));

        NodeService.AddThing(context, thing);

        // ----------------------------------------------
        // Slider
        Config = new ArrayList<ConfigParameter>();

        thingKey = ThingKey.CreateKey(nodeKey, Slider1);
        thing = new Thing(thingKey,
                Slider1,
                Config,
                new ArrayList<Port>(),
                "yodiwo.output.seekbars",
                "",
                new ThingUIHints("/Content/VirtualGateway/img/icon-thing-slider.png", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(SliderPort)),
                "Value", "",
                ioPortDirection.Output, ePortType.Decimal, "0", 0, ePortConf.None));

        NodeService.AddThing(context, thing);


        // ----------------------------------------------
        // BrightnessSensor
        Config = new ArrayList<ConfigParameter>();

        if (settingsProvider.getServiceBrightnessEnabled()) {
            thingKey = ThingKey.CreateKey(nodeKey, BrightnessSensor);
            thing = new Thing(thingKey,
                    BrightnessSensor,
                    Config,
                    new ArrayList<Port>(),
                    "yodiwo.output.sensors.light",
                    "",
                    new ThingUIHints("/Content/VirtualGateway/img/brightness.png", ""));

            thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(BrightnessSensorPort)),
                    "Value", "",
                    ioPortDirection.Output, ePortType.Decimal, "0", 0, ePortConf.None));

            NodeService.AddThing(context, thing);
        }

        // ----------------------------------------------
        // Proximity
        Config = new ArrayList<ConfigParameter>();

        thingKey = ThingKey.CreateKey(nodeKey, Proximity);
        thing = new Thing(thingKey,
                Proximity,
                Config,
                new ArrayList<Port>(),
                "yodiwo.output.sensors.proximity",
                "",
                new ThingUIHints("/Content/VirtualGateway/img/icon-thing-proximity.png", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, "0"),
                "Close", "",
                ioPortDirection.Output, ePortType.Boolean, "false", 0, ePortConf.IsTrigger));

        NodeService.AddThing(context, thing);

        // ----------------------------------------------
        // Accelerometer
        Config = new ArrayList<ConfigParameter>();

        if (settingsProvider.getServiceAccelerometerEnabled()) {
            thingKey = ThingKey.CreateKey(nodeKey, Accelerometer);
            thing = new Thing(thingKey,
                    Accelerometer,
                    Config,
                    new ArrayList<Port>(),
                    "",
                    // TODO: insert Type according to ThingTypeModelLibrary
                    "",
                    new ThingUIHints("/Content/VirtualGateway/img/accelerometer.jpg", ""));

            thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(AccelerometerX)),
                    "X", "",
                    ioPortDirection.Output, ePortType.DecimalHigh, "0", 0, ePortConf.None));
            thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(AccelerometerY)),
                    "Y", "",
                    ioPortDirection.Output, ePortType.DecimalHigh, "0", 0, ePortConf.None));
            thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(AccelerometerZ)),
                    "Z", "",
                    ioPortDirection.Output, ePortType.DecimalHigh, "0", 0, ePortConf.None));
            thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(AccelerometerShaken)),
                    "Shaken", "",
                    ioPortDirection.Output, ePortType.Boolean, NodeService.PortValue_Boolean_False, 0, ePortConf.IsTrigger));

            NodeService.AddThing(context, thing);
        }

        // ----------------------------------------------
        // Rotation
        Config = new ArrayList<ConfigParameter>();

        if (settingsProvider.getServiceRotationEnabled()) {
            thingKey = ThingKey.CreateKey(nodeKey, Rotation);
            thing = new Thing(thingKey,
                    Rotation,
                    Config,
                    new ArrayList<Port>(),
                    "yodiwo.output.sensors.rotation",
                    "",
                    new ThingUIHints("/Content/VirtualGateway/img/accelerometer.jpg", ""));

            thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(RotationAzimuth)),
                    "Azimuth", "",
                    ioPortDirection.Output, ePortType.DecimalHigh, "0", 0, ePortConf.None));
            thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(RotationPitch)),
                    "Pitch", "",
                    ioPortDirection.Output, ePortType.DecimalHigh, "0", 0, ePortConf.None));
            thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(RotationRoll)),
                    "Roll", "",
                    ioPortDirection.Output, ePortType.DecimalHigh, "0", 0, ePortConf.None));

            NodeService.AddThing(context, thing);
        }

        // ----------------------------------------------
        // Gyroscope
        Config = new ArrayList<ConfigParameter>();

        if (settingsProvider.getServiceGyroscopeEnabled()) {
            thingKey = ThingKey.CreateKey(nodeKey, Gyroscope);
            thing = new Thing(thingKey,
                    Gyroscope,
                    Config,
                    new ArrayList<Port>(),
                    "yodiwo.output.sensors.gyroscope",
                    "",
                    new ThingUIHints("/Content/VirtualGateway/img/accelerometer.jpg", ""));

            thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(GyroscopeX)),
                    "X", "",
                    ioPortDirection.Output, ePortType.DecimalHigh, "0", 0, ePortConf.None));
            thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(GyroscopeY)),
                    "Y", "",
                    ioPortDirection.Output, ePortType.DecimalHigh, "0", 0, ePortConf.None));
            thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(GyroscopeZ)),
                    "Z", "",
                    ioPortDirection.Output, ePortType.DecimalHigh, "0", 0, ePortConf.None));
            thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(GyroscopeW)),
                    "W", "",
                    ioPortDirection.Output, ePortType.DecimalHigh, "0", 0, ePortConf.None));

            NodeService.AddThing(context, thing);
        }

        // ----------------------------------------------
        // NFC support
        Config = new ArrayList<ConfigParameter>();

        NfcAdapter mNfcAdapter = NfcAdapter.getDefaultAdapter(context);
        if (mNfcAdapter != null) {
            thingKey = ThingKey.CreateKey(nodeKey, OutputNFC);
            thing = new Thing(thingKey,
                    OutputNFC,
                    Config,
                    new ArrayList<Port>(),
                    "yodiwo.output.nfc",
                    "",
                    new ThingUIHints("/Content/VirtualGateway/img/nfc.png", ""));

            thing.Ports.add(new Port(PortKey.CreateKey(thingKey, "0"),
                    "Value", "",
                    ioPortDirection.Output, ePortType.String, "", 0, ePortConf.IsTrigger));

            NodeService.AddThing(context, thing);
        }

        // ----------------------------------------------
        // GPS support + trigger
        Config = new ArrayList<ConfigParameter>();
        conf = new ConfigParameter("Accuracy", "MEDIUM");
        Config.add(conf);
        conf = new ConfigParameter("Power requirements", "MEDIUM");
        Config.add(conf);

        thingKey = ThingKey.CreateKey(nodeKey, GPS);
        thing = new Thing(thingKey,
                GPS,
                Config,
                new ArrayList<Port>(),
                "",
                // TODO: insert Type according to ThingTypeModelLibrary
                "",
                new ThingUIHints("/Content/VirtualGateway/img/gps.png", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(GPSLatPort)),
                "Latitude", "GPS Latitude coordinate",
                ioPortDirection.Output, ePortType.DecimalHigh, "", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(GPSLonPort)),
                "Longitude", "GPS Longitude coordinate",
                ioPortDirection.Output, ePortType.DecimalHigh, "", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(GPSAddressPort)),
                "Address", "GPS acquired address",
                ioPortDirection.Output, ePortType.String, "", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(GPSCountryPort)),
                "Country name", "GPS acquired country name",
                ioPortDirection.Output, ePortType.String, "", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(GPSPostalCodePort)),
                "Postal code", "GPS acquired postal code",
                ioPortDirection.Output, ePortType.String, "", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(GPSTriggerPort)),
                "Trigger", "",
                ioPortDirection.Input, ePortType.Boolean, NodeService.PortValue_Boolean_False, 0, ePortConf.ReceiveAllEvents));

        NodeService.AddThing(context, thing);

        // ----------------------------------------------
        // WiFi status thing
        Config = new ArrayList<ConfigParameter>();

        thingKey = ThingKey.CreateKey(nodeKey, WiFiStatus);
        thing = new Thing(thingKey,
                WiFiStatus,
                Config,
                new ArrayList<Port>(),
                "",
                // TODO: insert Type according to ThingTypeModelLibrary
                "",
                new ThingUIHints("/Content/VirtualGateway/img/icon-thing-genericwifi.svg", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(WiFiStatusConnectionStatusPort)),
                "Connection status", "WiFi connection status",
                ioPortDirection.Output, ePortType.String, "", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(WiFiStatusSSIDPort)),
                "SSID", "The SSID of the WiFi access point the device is connected to",
                ioPortDirection.Output, ePortType.String, "", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(WiFiStatusRSSIPort)),
                "RSSI", "Received Signal Strength Indication (RSSI) of the WiFi access point the device is connected to (dBm)",
                ioPortDirection.Output, ePortType.Integer, "", 0, ePortConf.None));

        NodeService.AddThing(context, thing);

        // ----------------------------------------------
        // Bluetooth status thing
        Config = new ArrayList<ConfigParameter>();

        thingKey = ThingKey.CreateKey(nodeKey, Bluetooth);
        thing = new Thing(thingKey,
                Bluetooth,
                Config,
                new ArrayList<Port>(),
                "",
                // TODO: insert Type according to ThingTypeModelLibrary
                "",
                new ThingUIHints("/Content/VirtualGateway/img/icon-thing-genericbluetooth.svg", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(BluetoothPowerStatusPort)),
                "Power status", "Bluetooth module power status",
                ioPortDirection.Output, ePortType.String, "", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(BluetoothConnectionStatusPort)),
                "Connection status", "Indicates whether the bluetooth module is connected to any profile of any remote device",
                ioPortDirection.Output, ePortType.String, "", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(BluetoothPairedDevicesPort)),
                "Paired devices", "Names of all remote devices paired to the bluetooth module",
                ioPortDirection.Output, ePortType.String, "", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(BluetoothDiscoveredDevicesPort)),
                "Discovered devices", "Names of all discovered remote devices",
                ioPortDirection.Output, ePortType.String, "", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(BluetoothTriggerSingleShotDiscoveryPort)),
                "Trigger device discovery", "Initiate a single-shot device discovery procedure",
                ioPortDirection.Input, ePortType.Boolean, "", 0, ePortConf.ReceiveAllEvents));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(BluetoothRequestPairedDevicesPort)),
                "Request paired devices", "Request all remote devices currently paired to the bluetooth module",
                ioPortDirection.Input, ePortType.Boolean, "", 0, ePortConf.ReceiveAllEvents));

        NodeService.AddThing(context, thing);

        // ----------------------------------------------
        // Progress Bar
        Config = new ArrayList<ConfigParameter>();

        thingKey = ThingKey.CreateKey(nodeKey, InputProgressBar);
        thing = new Thing(thingKey,
                InputProgressBar,
                Config,
                new ArrayList<Port>(),
                "yodiwo.input.seekbars",
                "",
                new ThingUIHints("/Content/VirtualGateway/img/progress_bar.png", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey,  Integer.toString(InputProgressBarPort)),
                "Value", "",
                ioPortDirection.Input, ePortType.Decimal, "0", 0, ePortConf.None));

        NodeService.AddThing(context, thing);


        // ----------------------------------------------
        // Virtual Switches
        Config = new ArrayList<ConfigParameter>();

        thingKey = ThingKey.CreateKey(nodeKey, InputSwitches);
        thing = new Thing(thingKey,
                InputSwitches,
                Config,
                new ArrayList<Port>(),
                "yodiwo.input.switches.onoff",
                "",
                new ThingUIHints("/Content/VirtualGateway/img/switch.png", ""));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(InputSwitchPort0)),
                "Switch1", "",
                ioPortDirection.Input, ePortType.Boolean, NodeService.PortValue_Boolean_False, 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(InputSwitchPort1)),
                "Switch2", "",
                ioPortDirection.Input, ePortType.Boolean, NodeService.PortValue_Boolean_False, 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(InputSwitchPort2)),
                "Switch3", "",
                ioPortDirection.Input, ePortType.Boolean, NodeService.PortValue_Boolean_False, 0, ePortConf.None));

        NodeService.AddThing(context, thing);


        // ----------------------------------------------
        // Virtual Color
        Config = new ArrayList<ConfigParameter>();

        thingKey = ThingKey.CreateKey(nodeKey, InputColors);
        thing = new Thing(thingKey,
                InputColors,
                Config,
                new ArrayList<Port>(),
                "yodiwo.input.leds.dimmable",
                "",
                new ThingUIHints("/Content/VirtualGateway/img/icon-thing-genericlight.png", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(InputColorPort0)),
                "Light1", "",
                ioPortDirection.Input, ePortType.Decimal, "", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(InputColorPort1)),
                "Light2", "",
                ioPortDirection.Input, ePortType.Decimal, "", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(InputColorPort2)),
                "Light3", "",
                ioPortDirection.Input, ePortType.Decimal, "", 0, ePortConf.None));

        NodeService.AddThing(context, thing);

        // ----------------------------------------------
        // Text Box
        Config = new ArrayList<ConfigParameter>();

        thingKey = ThingKey.CreateKey(nodeKey, InputTextBox);
        thing = new Thing(thingKey,
                InputTextBox,
                Config,
                new ArrayList<Port>(),
                "yodiwo.input.text",
                "",
                new ThingUIHints("/Content/VirtualGateway/img/icon-thing-text.png", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(InputTextBoxPort)),
                "Text", "",
                ioPortDirection.Input, ePortType.String, "", 0, ePortConf.ReceiveAllEvents));

        NodeService.AddThing(context, thing);

        // ----------------------------------------------
        // Android Intent
        Config = new ArrayList<ConfigParameter>();

        thingKey = ThingKey.CreateKey(nodeKey, InputAndroidIntent);
        thing = new Thing(thingKey,
                InputAndroidIntent,
                Config,
                new ArrayList<Port>(),
                "yodiwo.input.androidintent",
                "",
                new ThingUIHints("/Content/VirtualGateway/img/android.png", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(InputAndroidIntentPort)),
                "Intent", "",
                ioPortDirection.Input, ePortType.String, "", 0, ePortConf.ReceiveAllEvents));

        NodeService.AddThing(context, thing);

        // ----------------------------------------------
        // Torch
        Config = new ArrayList<ConfigParameter>();

        if (ThingsModuleService.hasTorch) {
            thingKey = ThingKey.CreateKey(nodeKey, Torch);
            thing = new Thing(thingKey,
                    Torch,
                    Config,
                    new ArrayList<Port>(),
                    "yodiwo.input.torches",
                    "",
                    new ThingUIHints("/Content/VirtualGateway/img/icon-thing-generictorch.svg", ""));

            thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(InputTorchPort0)),
                    "Torch state", "",
                    ioPortDirection.Input, ePortType.Boolean, NodeService.PortValue_Boolean_False, 0, ePortConf.None));

            NodeService.AddThing(context, thing);
        }

        // ----------------------------------------------
        // PhotoViewer
        Config = new ArrayList<ConfigParameter>();

        thingKey = ThingKey.CreateKey(nodeKey, PhotoViewer);
        thing = new Thing(thingKey,
                PhotoViewer,
                Config,
                new ArrayList<Port>(),
                "yodiwo.input.lcds",
                "",
                new ThingUIHints("/Content/VirtualGateway/img/icon-thing-imageviewer.png", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(PhotoViewerPort0)),
                "PhotoViewer", "",
                ioPortDirection.Input, ePortType.String, "", 0, ePortConf.ReceiveAllEvents));

        NodeService.AddThing(context, thing);

    }

    // ---------------------------------------------------------------------------------------------

    public String GetThingKey(String thingId) {
        String nodeKey = settingsProvider.getNodeKey();
        return ThingKey.CreateKey(nodeKey, thingId);
    }

    // ---------------------------------------------------------------------------------------------

    public String GetPortKey(String thingId, int portId) {
        String nodeKey = settingsProvider.getNodeKey();
        String thingKey = ThingKey.CreateKey(nodeKey, thingId);
        return PortKey.CreateKey(thingKey, Integer.toString(portId));
    }

    // ---------------------------------------------------------------------------------------------

}
