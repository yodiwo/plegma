package com.yodiwo.androidagent.core;

import android.app.ActivityManager;
import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.os.Build;
import android.os.Bundle;
import android.preference.ListPreference;
import android.preference.Preference;
import android.preference.PreferenceActivity;
import android.preference.PreferenceManager;
import android.support.v7.widget.AppCompatCheckBox;
import android.support.v7.widget.AppCompatCheckedTextView;
import android.support.v7.widget.AppCompatEditText;
import android.support.v7.widget.AppCompatRadioButton;
import android.support.v7.widget.AppCompatSpinner;
import android.util.AttributeSet;
import android.util.Log;
import android.view.View;

import com.yodiwo.androidagent.R;

import java.io.File;

/**
 * A {@link PreferenceActivity} that presents a set of application settings.
 * <p/>
 * See <a href="http://developer.android.com/design/patterns/settings.html">
 * Android Design: Settings</a> for design guidelines and the <a
 * href="http://developer.android.com/guide/topics/ui/settings.html">Settings
 * API Guide</a> for more information on developing a Settings UI.
 */
public class SettingsActivity extends PreferenceActivity
        implements Preference.OnPreferenceChangeListener {

    private final static String TAG = SettingsActivity.class.getSimpleName();

    /**
     * select release/debug mode manually
     */
    public static final boolean isRelease = false;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        if (isRelease){
                // Add 'general' preferences, defined in the XML file
                addPreferencesFromResource(R.xml.pref_general_release);

                // For all preferences, attach an OnPreferenceChangeListener so the UI summary can be
                // updated when the preference changes.
                bindPreferenceSummaryToValue(findPreference(getString(R.string.pref_app_version_key)));
                bindPreferenceSummaryToValue(findPreference(getString(R.string.pref_device_name_key)));
                bindPreferenceSummaryToValue(findPreference(getString(R.string.pref_transport_protocol)));
                bindPreferenceSummaryToValue(findPreference(getString(R.string.pref_notification_sound_mode_key)));
                bindPreferenceSummaryToValue(findPreference(getString(R.string.pref_reset_app_key)));
            }
        else {
            // Add 'general' preferences, defined in the XML file
            addPreferencesFromResource(R.xml.pref_general_debug);

            // For all preferences, attach an OnPreferenceChangeListener so the UI summary can be
            // updated when the preference changes.
            bindPreferenceSummaryToValue(findPreference(getString(R.string.pref_app_version_key)));
            bindPreferenceSummaryToValue(findPreference(getString(R.string.pref_server_address_key)));
            bindPreferenceSummaryToValue(findPreference(getString(R.string.pref_device_name_key)));
            bindPreferenceSummaryToValue(findPreference(getString(R.string.pref_transport_protocol)));
            bindPreferenceSummaryToValue(findPreference(getString(R.string.pref_mqtt_address_key)));
            bindPreferenceSummaryToValue(findPreference(getString(R.string.pref_service_yodiup_address_key)));
            bindPreferenceSummaryToValue(findPreference(getString(R.string.pref_notification_sound_mode_key)));
            bindPreferenceSummaryToValue(findPreference(getString(R.string.pref_reset_app_key)));
        }

        // reset app handling
        Preference resetApp = getPreferenceManager().findPreference(getString(R.string.pref_reset_app_key));
        if (resetApp != null) {
            resetApp.setOnPreferenceClickListener(new Preference.OnPreferenceClickListener() {
                @Override
                public boolean onPreferenceClick(Preference arg0) {
                    startReset();
                    return true;
                }
            });
        }
    }

    /**
     * Attaches a listener so the summary is always updated with the preference value.
     * Also fires the listener once, to initialize the summary (so it shows up before the value
     * is changed.)
     */
    private void bindPreferenceSummaryToValue(Preference preference) {
        // Set the listener to watch for value changes.
        preference.setOnPreferenceChangeListener(this);

        // Trigger the listener immediately with the preference's
        // current value.
        onPreferenceChange(preference,
                PreferenceManager
                        .getDefaultSharedPreferences(preference.getContext())
                        .getString(preference.getKey(), ""));
    }

    @Override
    public boolean onPreferenceChange(Preference preference, Object value) {
        String stringValue = value.toString();

        if (preference instanceof ListPreference) {
            // For list preferences, look up the correct display value in
            // the preference's 'entries' list (since they have separate labels/values).
            ListPreference listPreference = (ListPreference) preference;
            int prefIndex = listPreference.findIndexOfValue(stringValue);
            if (prefIndex >= 0) {
                preference.setSummary(listPreference.getEntries()[prefIndex]);
            }
        } else {
            // For other preferences, set the summary to the value's simple string representation.
            preference.setSummary(stringValue);
        }
        return true;
    }

    @Override
    public View onCreateView(String name, Context context, AttributeSet attrs) {
        // Allow super to try and create a view first
        final View result = super.onCreateView(name, context, attrs);
        if (result != null) {
            return result;
        }

        if (Build.VERSION.SDK_INT < Build.VERSION_CODES.LOLLIPOP) {
            // If we're running pre-L, we need to 'inject' our tint aware Views in place of the
            // standard framework versions
            switch (name) {
                case "EditText":
                    return new AppCompatEditText(this, attrs);
                case "Spinner":
                    return new AppCompatSpinner(this, attrs);
                case "CheckBox":
                    return new AppCompatCheckBox(this, attrs);
                case "RadioButton":
                    return new AppCompatRadioButton(this, attrs);
                case "CheckedTextView":
                    return new AppCompatCheckedTextView(this, attrs);
            }
        }

        return null;
    }

    @Override
    public void onBackPressed() {
        super.onBackPressed();
    }

    // ---------------------------------------------------------------------------------------------

    private void startReset(){
        new AlertDialog.Builder(this)
                .setTitle("Reset Data")
                .setMessage("Are you sure that you want to delete your local data?")
                .setPositiveButton("Yes", new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int id) {
                        Helpers.log(Log.DEBUG, "Settings", "Delete data from setting menu...");
                        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.KITKAT) {
                            ((ActivityManager) getApplicationContext().getSystemService(ACTIVITY_SERVICE))
                                    .clearApplicationUserData(); // note: it has a return value!
                        }
                        else {
                            // use old hacky way, which can be removed
                            // once minSdkVersion goes above 19 in a few years.
                            Helpers.log(Log.DEBUG, "Settings", "We can't delete the settings in this version of android..." + Build.VERSION.SDK_INT);
                            clearApplicationData();
                        }
                    }
                })
                .setNegativeButton("No", new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int id) {
                        // User cancelled the dialog
                    }
                })
                .create()
                .show();
    }
    // ---------------------------------------------------------------------------------------------

    private void clearApplicationData() {
        File cache = getCacheDir();
        File appDir = new File(cache.getParent());
        if (appDir.exists()) {
            String[] children = appDir.list();
            for (String s : children) {
                if (!s.equals("lib")) {
                    deleteDir(new File(appDir, s));
                    Helpers.log(Log.INFO, "Settings", "File /data/data/APP_PACKAGE/" + s + " DELETED");
                }
            }
        }
    }

    // ---------------------------------------------------------------------------------------------

    private void deleteDir(File file) {
        if (file.isDirectory())
            for (String child : file.list())
                deleteDir(new File(file, child));
        file.delete();  // delete child file or empty directory
    }
}
