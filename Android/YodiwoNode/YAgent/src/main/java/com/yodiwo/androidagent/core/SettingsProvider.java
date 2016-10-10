package com.yodiwo.androidagent.core;

import android.content.Context;
import android.content.SharedPreferences;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.os.Build;
import android.preference.PreferenceManager;
import android.util.Log;

import com.yodiwo.androidagent.R;

import java.util.HashSet;
import java.util.Set;
import java.util.UUID;

public class SettingsProvider {

    // =============================================================================================
    // Static

    public static final String TAG = SettingsProvider.class.getSimpleName();

    private static final String FIXED_UUID = "NeBiT_Android";
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
            uuid = FIXED_UUID;//UUID.randomUUID().toString();
            editor.putString(context.getString(R.string.pref_pairing_uuid_key), uuid);
            Helpers.log(Log.DEBUG, TAG, "Init UUID to " + uuid);
        }
        Helpers.log(Log.DEBUG, TAG, "Current UUID is " + uuid);

        // Init app version
        String version;
        try {
            PackageManager manager = context.getPackageManager();
            PackageInfo info = manager.getPackageInfo(
                    context.getPackageName(), 0);
            version = info.versionName;
            editor.putString(context.getString(R.string.pref_app_version_key), version);
            Helpers.log(Log.INFO, TAG, "Application.Version = " + version);
        } catch (Exception e) {
            Helpers.log(Log.ERROR, TAG, "Error getting version");
        }

        // Init device name
        try {
            editor.putString(context.getString(R.string.pref_device_name_key), Build.MODEL);
            Helpers.log(Log.INFO, TAG, "Device name = " + Build.MODEL);
        } catch (Exception e) {
            Helpers.log(Log.ERROR, TAG, "Error setting device name/model");
        }

        editor.apply();
    }

    // =============================================================================================
    // Settings from UI
    public String getAppVersion() {
        return prefs.getString(context.getString(R.string.pref_app_version_key),
                context.getString(R.string.pref_app_version));
    }

    // ---------------------------------------------------------------------------------------------

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

    public String getTransportProtocol() {
        return prefs.getString(context.getString(R.string.pref_transport_protocol),
                context.getString(R.string.pref_transport_protocol_default));
    }

    // ---------------------------------------------------------------------------------------------

    public String getDeviceName() {
        return prefs.getString(context.getString(R.string.pref_device_name_key),
                context.getString(R.string.pref_device_name_default));
    }

    // =============================================================================================
    // Settings for notifications

    public String getNotificationSoundMode() {
        return prefs.getString(context.getString(R.string.pref_notification_sound_mode_key),
                context.getString(R.string.pref_notification_sound_mode_default));
    }

    // ---------------------------------------------------------------------------------------------

    public Boolean getNotificationEnable() {
        return prefs.getBoolean(context.getString(R.string.pref_notifications_enable_key),
                Boolean.parseBoolean(context.getString(R.string.pref_notifications_enable_default)));
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
        editor.apply();
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
        return prefs.getString(context.getString(R.string.pref_node_things_revnum), "0");
    }

    // ---------------------------------------------------------------------------------------------

    public void setNodeThingsRevNum(String revnum) {
        SharedPreferences.Editor editor = prefs.edit();
        editor.putString(context.getString(R.string.pref_node_things_revnum), revnum);
        editor.apply();
    }

    // ---------------------------------------------------------------------------------------------

    public void setNodeKeys(String nodeKey, String secretKey) {
        SharedPreferences.Editor editor = prefs.edit();
        editor.putString(context.getString(R.string.pref_pairing_nodekey_key), nodeKey);
        editor.putString(context.getString(R.string.pref_pairing_nodesecret_key), secretKey);
        editor.apply();
    }

    // ---------------------------------------------------------------------------------------------

    public String getNodeKey() {
        return prefs.getString(context.getString(R.string.pref_pairing_nodekey_key), null);
    }
    // ---------------------------------------------------------------------------------------------

    public void setUserUUID(String uuid) {
        SharedPreferences.Editor editor = prefs.edit();
        editor.putString(context.getString(R.string.pref_pairing_uuid_key), uuid);
        editor.apply();
    }
    // ---------------------------------------------------------------------------------------------

    public String getUserKey() {
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

    public Set<String> getSubNodeKeys() {
        return prefs.getStringSet(context.getString(R.string.pref_pairing_subnodekey_key), null);
    }

    // ---------------------------------------------------------------------------------------------

    public String getSubNodeKeys(String subnodeId) {
        for (String key : getSubNodeKeys()){
            if (key.contains(subnodeId))
                return key;
        }

        return null;
    }

    // ---------------------------------------------------------------------------------------------
    public String setSubNodeKey(String subnodeId) {
        String subKey = createSubNodeKey(subnodeId);

        if (subKey != null){
            SharedPreferences.Editor editor = prefs.edit();
            Set<String> subnodeKeys = getSubNodeKeys();
            if (subnodeKeys == null)
                subnodeKeys = new HashSet<>();
            subnodeKeys.add(subKey);
            editor.putStringSet(context.getString(R.string.pref_pairing_subnodekey_key), subnodeKeys);
            editor.apply();
        }
        return subKey;
    }

    // ---------------------------------------------------------------------------------------------

    private String createSubNodeKey(String subnodeId){
        if (getNodeKey() != null && getNodeSecretKey() != null && subnodeId != null)
            return getNodeKey() + "/" + subnodeId;

        return null;
    }

    // =============================================================================================
    // Settings for Logging data

    public Boolean getLogDataEnable() {
        return prefs.getBoolean(context.getString(R.string.pref_log_data_key),
                Boolean.parseBoolean(context.getString(R.string.pref_log_data_default)));
    }

    // ---------------------------------------------------------------------------------------------


    // ---------------------------------------------------------------------------------------------
}
