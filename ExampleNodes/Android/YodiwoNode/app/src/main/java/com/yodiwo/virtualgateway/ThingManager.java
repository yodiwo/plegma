package com.yodiwo.virtualgateway;

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

/**
 * Created by FxBit on 7/8/2015.
 */
public class ThingManager {


    // =============================================================================================
    // Static information's

    public static final String TAG = ThingManager.class.getSimpleName();
    private static ThingManager thingManager = null;


    public static ThingManager getInstance(Context context) {
        if (thingManager == null) {
            thingManager = new ThingManager(context.getApplicationContext());
        }

        return thingManager;
    }

    // =============================================================================================
    // Things names

    public static final String Button1 = "Button1";
    public static final int ButtonPort = 0;


    public static final String Switch1 = "Switch1";
    public static final int SwitchPort = 0;

    public static final String Slider1 = "Slider1";
    public static final int SliderPort = 0;

    public static final String Brightness = "Brightness";
    public static final int BrightnessPort = 0;

    public static final String Accelerometer = "Accelerometer";
    public static final int AccelerometerX = 0;
    public static final int AccelerometerY = 1;
    public static final int AccelerometerZ = 2;
    public static final int AccelerometerShaken = 3;


    public static final String GPS = "GPS";
    public static final int GPSLatPort = 0;
    public static final int GPSLotPort = 1;
    public static final int GPSCityNamePort = 2;
    public static final int GPSAdrressPort = 3;

    public static final String OutputNFC = "NFC";
    public static final int OutputNFCPort = 0;

    public static final String InputProgressBar1 = "InputProgressBar1";
    public static final int InputProgressBarPort = 0;

    public static final String InputSwitch1 = "InputSwitch1";
    public static final int InputSwitchPort = 0;

    public static final String InputColor1 = "InputColor1";
    public static final int InputColorPort = 0;

    public static final String InputAndroidIntent = "AndroidIntent";
    public static final int InputAndroidIntentPort = 0;

    // =================== ==========================================================================
    // Things Initialization

    private SettingsProvider settingsProvider;
    private Context context;

    public ThingManager(Context context) {
        this.context = context;
        this.settingsProvider = SettingsProvider.getInstance(context);
    }

    // ---------------------------------------------------------------------------------------------

    // Here we initialize all things and add them to node service
    // Any new things must be add here.
    public void RegisterThings() {
        String nodeKey = settingsProvider.getNodeKey();
        String thingKey = "";
        Thing thing = null;

        String deviceName = settingsProvider.getDeviceName();


        // Clean old local things
        NodeService.CleanThings(context);

        // -----------------------------------------------------------------------------
        // Output

        // ----------------------------------------------
        // Virtual Button
        thingKey = ThingKey.CreateKey(nodeKey, Button1);
        thing = new Thing(thingKey,
                Button1,
                new ArrayList<ConfigParameter>(),
                new ArrayList<Port>(),
                "",
                "",
                new ThingUIHints("/Content/VirtualGateway/img/icon-thing-gobutton.png", ""));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, "0"),
                "ButtonState",
                "",
                ioPortDirection.Output,
                ePortType.Boolean,
                NodeService.PortValue_Boolean_False,
                0,
                ePortConf.None));
        NodeService.AddThing(context, thing);


        // ----------------------------------------------
        // Virtual Switch
        thingKey = ThingKey.CreateKey(nodeKey, Switch1);
        thing = new Thing(thingKey,
                Switch1,
                new ArrayList<ConfigParameter>(),
                new ArrayList<Port>(),
                "",
                "",
                new ThingUIHints("/Content/VirtualGateway/img/switch.png", ""));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, "0"),
                "ButtonState",
                "",
                ioPortDirection.Output,
                ePortType.Boolean,
                NodeService.PortValue_Boolean_False,
                0,
                ePortConf.None
        ));
        NodeService.AddThing(context, thing);


        // ----------------------------------------------
        // Slider
        thingKey = ThingKey.CreateKey(nodeKey, Slider1);
        thing = new Thing(thingKey,
                Slider1,
                new ArrayList<ConfigParameter>(),
                new ArrayList<Port>(),
                "",
                "",
                new ThingUIHints("/Content/VirtualGateway/img/icon-thing-slider.png", ""));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, "0"),
                "Value",
                "",
                ioPortDirection.Output,
                ePortType.Decimal,
                "0",
                0,
                ePortConf.None));
        NodeService.AddThing(context, thing);


        // ----------------------------------------------
        // Brightness

        if (settingsProvider.getServiceBrightnessEnabled()) {
            thingKey = ThingKey.CreateKey(nodeKey, Brightness);
            thing = new Thing(thingKey,
                    Brightness,
                    new ArrayList<ConfigParameter>(),
                    new ArrayList<Port>(),
                    "",
                    "",
                    new ThingUIHints("/Content/VirtualGateway/img/brightness.png", ""));
            thing.Ports.add(new Port(PortKey.CreateKey(thingKey, "0"),
                    "Value",
                    "",
                    ioPortDirection.Output,
                    ePortType.Decimal,
                    "0",
                    0,
                    ePortConf.None));
            NodeService.AddThing(context, thing);
        }

        // ----------------------------------------------
        // Accelerometer
        if (settingsProvider.getServiceAccelerometerEnabled()) {
            thingKey = ThingKey.CreateKey(nodeKey, Accelerometer);
            thing = new Thing(thingKey,
                    Accelerometer,
                    new ArrayList<ConfigParameter>(),
                    new ArrayList<Port>(),
                    "",
                    "",
                    new ThingUIHints("/Content/VirtualGateway/img/accelerometer.jpg", ""));
            thing.Ports.add(new Port(PortKey.CreateKey(thingKey, "0"), "X", "", ioPortDirection.Output, ePortType.Decimal, "0", 0, ePortConf.None));
            thing.Ports.add(new Port(PortKey.CreateKey(thingKey, "1"), "Y", "", ioPortDirection.Output, ePortType.Decimal, "0", 0, ePortConf.None));
            thing.Ports.add(new Port(PortKey.CreateKey(thingKey, "2"), "Z", "", ioPortDirection.Output, ePortType.Decimal, "0", 0, ePortConf.None));
            thing.Ports.add(new Port(PortKey.CreateKey(thingKey, "3"), "onShake", "", ioPortDirection.Output, ePortType.Boolean, NodeService.PortValue_Boolean_False, 0, ePortConf.IsTrigger));
            NodeService.AddThing(context, thing);
        }

        // ----------------------------------------------
        // NFC support

        NfcAdapter mNfcAdapter = NfcAdapter.getDefaultAdapter(context);
        if (mNfcAdapter != null) {
            thingKey = ThingKey.CreateKey(nodeKey, OutputNFC);
            thing = new Thing(thingKey,
                    OutputNFC,
                    new ArrayList<ConfigParameter>(),
                    new ArrayList<Port>(),
                    "",
                    "",
                    new ThingUIHints("/Content/VirtualGateway/img/nfc.png", ""));
            thing.Ports.add(new Port(PortKey.CreateKey(thingKey, "0"),
                    "Value",
                    "",
                    ioPortDirection.Output,
                    ePortType.String,
                    "",
                    0,
                    ePortConf.IsTrigger));
            NodeService.AddThing(context, thing);
        }

        // ----------------------------------------------
        // GPS support

        thingKey = ThingKey.CreateKey(nodeKey, GPS);
        thing = new Thing(thingKey,
                GPS,
                new ArrayList<ConfigParameter>(),
                new ArrayList<Port>(),
                "",
                "",
                new ThingUIHints("/Content/VirtualGateway/img/gps.png", ""));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, "0"),
                "Position",
                "",
                ioPortDirection.Output,
                ePortType.String,
                "",
                0,
                ePortConf.IsTrigger));
        NodeService.AddThing(context, thing);

        /*
        thingKey = ThingKey.CreateKey(nodeKey, GPS);
        thing = new Thing(thingKey,
                deviceName + " " + GPS,
                new ArrayList<ConfigParameter>(),
                new ArrayList<Port>(),
                "",
                new ThingUIHints("/Content/VirtualGateway/img/nfc.png"));
        thing.Ports.add(new Port("Lat",
                ePortType.DecimalHigh,
                ioPortDirection.Output,
                "0",
                "",
                PortKey.CreateKey(thingKey, "0"),
                0));
        thing.Ports.add(new Port("Lot",
                ePortType.DecimalHigh,
                ioPortDirection.Output,
                "0",
                "",
                PortKey.CreateKey(thingKey, "1"),
                0));
        thing.Ports.add(new Port("CityName",
                ePortType.String,
                ioPortDirection.Output,
                "0",
                "",
                PortKey.CreateKey(thingKey, "2"),
                0));
        thing.Ports.add(new Port("Address",
                ePortType.String,
                ioPortDirection.Output,
                "0",
                "",
                PortKey.CreateKey(thingKey, "3"),
                0));
        NodeService.AddThing(context, thing);
        */

        // -----------------------------------------------------------------------------
        // Inputs


        // ----------------------------------------------
        // Progress
        thingKey = ThingKey.CreateKey(nodeKey, InputProgressBar1);
        thing = new Thing(thingKey,
                InputProgressBar1,
                new ArrayList<ConfigParameter>(),
                new ArrayList<Port>(),
                "",
                "",
                new ThingUIHints("/Content/VirtualGateway/img/progress_bar.png", ""));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, "0"),
                "Value",
                "",
                ioPortDirection.Input,
                ePortType.Decimal,
                "0",
                0,
                ePortConf.None));
        NodeService.AddThing(context, thing);


        // ----------------------------------------------
        // Virtual Switch
        thingKey = ThingKey.CreateKey(nodeKey, InputSwitch1);
        thing = new Thing(thingKey,
                InputSwitch1,
                new ArrayList<ConfigParameter>(),
                new ArrayList<Port>(),
                "",
                "",
                new ThingUIHints("/Content/VirtualGateway/img/switch.png", ""));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, "0"),
                "ButtonState",
                "",
                ioPortDirection.Input,
                ePortType.Boolean,
                NodeService.PortValue_Boolean_False,
                0,
                ePortConf.None));
        NodeService.AddThing(context, thing);


        // ----------------------------------------------
        // Virtual Color
        thingKey = ThingKey.CreateKey(nodeKey, InputColor1);
        thing = new Thing(thingKey,
                InputColor1,
                new ArrayList<ConfigParameter>(),
                new ArrayList<Port>(),
                "",
                "",
                new ThingUIHints("/Content/VirtualGateway/img/icon-thing-genericlight.png", ""));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, "0"),
                "ButtonState",
                "",
                ioPortDirection.Input,
                ePortType.Color,
                "",
                0,
                ePortConf.None));
        NodeService.AddThing(context, thing);

        // ----------------------------------------------
        // Android Intent
        thingKey = ThingKey.CreateKey(nodeKey, InputAndroidIntent);
        thing = new Thing(thingKey,
                InputAndroidIntent,
                new ArrayList<ConfigParameter>(),
                new ArrayList<Port>(),
                "",
                "",
                new ThingUIHints("/Content/VirtualGateway/img/android.png", ""));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, "0"),
                "ButtonState",
                "",
                ioPortDirection.Input,
                ePortType.String,
                "",
                0,
                ePortConf.ReceiveAllEvents));
        NodeService.AddThing(context, thing);
    }


    // ---------------------------------------------------------------------------------------------

    public String GetThingKey(String thingId) {
        String nodeKey = settingsProvider.getNodeKey();
        return ThingKey.CreateKey(nodeKey, thingId);
    }

    // ---------------------------------------------------------------------------------------------


    // ---------------------------------------------------------------------------------------------

}
