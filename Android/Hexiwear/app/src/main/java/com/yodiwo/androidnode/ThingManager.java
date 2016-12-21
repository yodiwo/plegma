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

/**
 * Created by FxBit on 7/8/2015.
 */
public class ThingManager {

    // =============================================================================================
    // Static information

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

    public static final String HexiwearGyro = "HexiwearGyro";
    public static final int HexiwearGyroPortX = 0;
    public static final int HexiwearGyroPortY = 1;
    public static final int HexiwearGyroPortZ = 2;

    public static final String HexiwearAccel = "HexiwearAccel";
    public static final int HexiwearAccelPortX = 0;
    public static final int HexiwearAccelPortY = 1;
    public static final int HexiwearAccelPortZ = 2;

    public static final String HexiwearWeather = "HexiwearWeather";
    public static final int HexiwearWeatherPortTemperature = 0;
    public static final int HexiwearWeatherPortHumidity = 1;
    public static final int HexiwearWeatherPortPressure = 2;


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
        // Gyro

        thingKey = ThingKey.CreateKey(nodeKey, HexiwearGyro);
        thing = new Thing(thingKey, HexiwearGyro, new ArrayList<ConfigParameter>(), new ArrayList<Port>(), "", "",
                new ThingUIHints("/Content/VirtualGateway/img/icon-gyroscope.png", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(HexiwearGyroPortX)),
                "X-axis (deg/s)", "",
                ioPortDirection.Output, ePortType.String, "0", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(HexiwearGyroPortY)),
                "Y-axis (deg/s)", "",
                ioPortDirection.Output, ePortType.String, "0", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(HexiwearGyroPortZ)),
                "Z-axis (deg/s)", "",
                ioPortDirection.Output, ePortType.String, "0", 0, ePortConf.None));

        NodeService.AddThing(context, thing);

        // ----------------------------------------------
        // Accel

        thingKey = ThingKey.CreateKey(nodeKey, HexiwearAccel);
        thing = new Thing(thingKey, HexiwearAccel, new ArrayList<ConfigParameter>(), new ArrayList<Port>(), "", "",
                new ThingUIHints("/Content/VirtualGateway/img/accelerometer.jpg", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(HexiwearAccelPortX)),
                "X-axis (g)", "",
                ioPortDirection.Output, ePortType.String, "0", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(HexiwearAccelPortY)),
                "Y-axis (g)", "",
                ioPortDirection.Output, ePortType.String, "0", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(HexiwearAccelPortZ)),
                "Z-axis (g)", "",
                ioPortDirection.Output, ePortType.String, "0", 0, ePortConf.None));

        NodeService.AddThing(context, thing);

        // ----------------------------------------------
        // Weather

        thingKey = ThingKey.CreateKey(nodeKey, HexiwearWeather);
        thing = new Thing(thingKey, HexiwearWeather, new ArrayList<ConfigParameter>(), new ArrayList<Port>(), "", "",
                new ThingUIHints("/Content/Designer/img/BlockImages/icon-openweathermap.png", ""));

        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(HexiwearWeatherPortTemperature)),
                "Temperature (Celsius)", "",
                ioPortDirection.Output, ePortType.String, "0", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(HexiwearWeatherPortHumidity)),
                "Humidity (%)", "",
                ioPortDirection.Output, ePortType.String, "0", 0, ePortConf.None));
        thing.Ports.add(new Port(PortKey.CreateKey(thingKey, Integer.toString(HexiwearWeatherPortPressure)),
                "Pressure (kPa)", "",
                ioPortDirection.Output, ePortType.String, "0", 0, ePortConf.None));

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
