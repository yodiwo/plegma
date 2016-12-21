package com.yodiwo.androidnode;

import android.content.Context;
import android.content.SharedPreferences;
import android.preference.PreferenceManager;
import android.util.Log;

import com.mikroe.hexiwear_android.R;

import java.util.UUID;

public class SettingsProvider {

    // =============================================================================================
    // Static

    public static final String TAG = SettingsProvider.class.getSimpleName();

    // Keep local global entrypoint for any request with Server.
    private static SettingsProvider settings = null;
    private String nodeToken2;

    // get the instance of the settings
    // if for some reason the instance is not valid create a new one.
    public static SettingsProvider getInstance(Context context) {
        if (settings == null) {
            settings = new SettingsProvider(context.getApplicationContext());
        }
        return settings;
    }

    // =============================================================================================

    // link to the application context that we have the instance of the db
    private Context context;
    private SharedPreferences prefs;

    // ---------------------------------------------------------------------------------------------

    public SettingsProvider(Context context) {
        this.context = context;
        prefs = PreferenceManager.getDefaultSharedPreferences(context);
        SharedPreferences.Editor editor = prefs.edit();

        // Init the UUID if is null
        String uuid = prefs.getString(context.getString(R.string.pref_pairing_uuid_key), null);
        if (uuid == null) {
            // Create a random uuid
            uuid = "hexiwear";//UUID.randomUUID().toString();
            editor.putString(context.getString(R.string.pref_pairing_uuid_key), uuid);
            Log.d(TAG, "Init UUID to " + uuid);
        }
        Log.d(TAG, "Current UUID is " + uuid);

        editor.commit();
    }

    // =============================================================================================
    // Settings from UI

    public String getServerAddress() {
        return prefs.getString(context.getString(R.string.pref_server_address_key),
                context.getString(R.string.pref_server_address_default));
    }

    // ---------------------------------------------------------------------------------------------

    public Boolean getServerUseSSL() {
        return prefs.getBoolean(context.getString(R.string.pref_server_use_ssl_key),
                Boolean.parseBoolean(context.getString(R.string.pref_server_use_ssl_default)));
    }

    // ---------------------------------------------------------------------------------------------

    public String getDeviceName() {
        return prefs.getString(context.getString(R.string.pref_device_name_key),
                context.getString(R.string.pref_device_name_default));
    }


    // =============================================================================================
    // Settings for YodiUp

    public String getYodiUpAddress() {
        return prefs.getString(context.getString(R.string.pref_service_yodiup_address_key),
                context.getString(R.string.pref_service_yodiup_address_default));
    }

    // =============================================================================================
    // Settings for MQTT


    public Boolean getMqttUseSSL() {
        return prefs.getBoolean(context.getString(R.string.pref_mqtt_use_ssl_key),
                Boolean.parseBoolean(context.getString(R.string.pref_mqtt_use_ssl_default)));
    }

    // ---------------------------------------------------------------------------------------------
    public String getMqttAddress() {
        return prefs.getString(context.getString(R.string.pref_mqtt_address_key),
                context.getString(R.string.pref_mqtt_address_default));
    }

    // =============================================================================================
    // Private Settings

    public String getNodeUUID() {
        return prefs.getString(context.getString(R.string.pref_pairing_uuid_key), null);
    }

    // ---------------------------------------------------------------------------------------------

    public void setNodeTokens(String token1, String token2) {
        SharedPreferences.Editor editor = prefs.edit();
        editor.putString(context.getString(R.string.pref_pairing_token1_key), token1);
        editor.putString(context.getString(R.string.pref_pairing_token2_key), token2);
        editor.commit();
    }

    // ---------------------------------------------------------------------------------------------

    public String getNodeToken2() {
        return prefs.getString(context.getString(R.string.pref_pairing_token2_key), null);
    }

    // ---------------------------------------------------------------------------------------------

    public String getNodeToken1() {
        return prefs.getString(context.getString(R.string.pref_pairing_token1_key), null);
    }

    // ---------------------------------------------------------------------------------------------

    public String getNodeThingsRevNum() {
        return prefs.getString(context.getString(R.string.pref_node_things_revnum), "1");
    }

    // ---------------------------------------------------------------------------------------------

    public void setNodeThingsRevNum(String revnum) {
        SharedPreferences.Editor editor = prefs.edit();
        editor.putString(context.getString(R.string.pref_node_things_revnum), revnum);
        editor.commit();
    }

    // ---------------------------------------------------------------------------------------------
    public void setNodeKeys(String nodeKey, String secretKey) {
        SharedPreferences.Editor editor = prefs.edit();
        editor.putString(context.getString(R.string.pref_pairing_nodekey_key), nodeKey);
        editor.putString(context.getString(R.string.pref_pairing_nodesecret_key), secretKey);
        editor.commit();
    }

    // ---------------------------------------------------------------------------------------------

    public String getNodeKey() {
        return prefs.getString(context.getString(R.string.pref_pairing_nodekey_key), null);
    }

    // ---------------------------------------------------------------------------------------------

    public String getUserKey() {
        // TODO: make function to the basic API
        String nodeKey = prefs.getString(context.getString(R.string.pref_pairing_nodekey_key), null);
        if (nodeKey != null) {
            String[] array = nodeKey.split("\\-");
            if (array.length > 0)
                return array[0];
        }

        return null;
    }

    // ---------------------------------------------------------------------------------------------

    public String getNodeSecretKey() {
        return prefs.getString(context.getString(R.string.pref_pairing_nodesecret_key), null);
    }

    // ---------------------------------------------------------------------------------------------


    // ---------------------------------------------------------------------------------------------


    // ---------------------------------------------------------------------------------------------
}
