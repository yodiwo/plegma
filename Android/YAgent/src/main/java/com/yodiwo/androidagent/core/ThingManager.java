package com.yodiwo.androidagent.core;

import android.content.Context;

import com.yodiwo.androidagent.plegma.PortKey;
import com.yodiwo.androidagent.plegma.ThingKey;

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
    // Things Initialization

    private SettingsProvider settingsProvider;
    private Context context;

    public ThingManager(Context context) {
        this.context = context;
        this.settingsProvider = SettingsProvider.getInstance(context);
    }

    // ---------------------------------------------------------------------------------------------

    public String GetThingKey(String thingId) {
        String nodeKey = settingsProvider.getNodeKey();
        return ThingKey.CreateKey(nodeKey, thingId);
    }

    // ---------------------------------------------------------------------------------------------

    public String GetPortKey(String thingId, String portId) {
        String nodeKey = settingsProvider.getNodeKey();
        String thingKey = ThingKey.CreateKey(nodeKey, thingId);
        return PortKey.CreateKey(thingKey, portId);
    }
}
