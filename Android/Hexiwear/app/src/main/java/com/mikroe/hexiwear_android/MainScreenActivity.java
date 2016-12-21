package com.mikroe.hexiwear_android;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.Bundle;
import android.support.v7.app.ActionBarActivity;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;

import com.yodiwo.androidbleagent.BluetoothLeService;
import com.yodiwo.androidnode.NodeService;
import com.yodiwo.androidnode.PairingActivity;
import com.yodiwo.androidnode.SettingsActivity;
import com.yodiwo.androidnode.SettingsProvider;

public class MainScreenActivity extends ActionBarActivity {

    public static boolean isUnpairedByUser;
    private static boolean ActivityInitialized;
    private SettingsProvider settingsProvider = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.main_screen);

        if (getIntent().getBooleanExtra("EXIT", false)) {
            android.os.Process.killProcess(android.os.Process.myPid());
            System.exit(0);
            finish();
            return;
        }

        ActivityInitialized = false;

        if (settingsProvider == null)
            settingsProvider = SettingsProvider.getInstance(this);

        // Check if device is paired
        if (settingsProvider.getNodeKey() != null && settingsProvider.getNodeSecretKey() != null) {
            ActivityInitialized = true;
            isUnpairedByUser = false;
            NodeService.Startup(this);
        }
    }

    @Override
    protected void onResume() {
        super.onResume();
        registerReceivers();
    }

    @Override
    protected void onPause() {
        super.onPause();
        unregisterReceiver(mGattUpdateReceiver);
    }

    @Override
    public void onBackPressed() {
        super.onBackPressed();
        Intent intent = new Intent(getApplicationContext(), IntroActivity.class);
        intent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
        intent.putExtra("EXIT", true);
        startActivity(intent);
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.main, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
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

        return super.onOptionsItemSelected(item);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////// SUB SCREENS /////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////

    public void startTempScreen(View view) {
        Intent intentAct = new Intent(MainScreenActivity.this, WeatherActivity.class);
        startActivity(intentAct);
    }

    public void startAccelScreen(View view) {
        Intent intentAct = new Intent(MainScreenActivity.this, AccelActivity.class);
        startActivity(intentAct);
    }

    public void startGyroScreen(View view) {
        Intent intentAct = new Intent(MainScreenActivity.this, GyroscopeActivity.class);
        startActivity(intentAct);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////// BROADCAST RECEIVESR /////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Handles various events fired by the Service.
    // ACTION_GATT_DISCONNECTED: disconnected from a GATT server.
    // ACTION_DATA_AVAILABLE: received data from the device.  This can be a result of read
    //                        or notification operations.

    private final BroadcastReceiver mGattUpdateReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            final String action = intent.getAction();

            if (BluetoothLeService.ACTION_GATT_DISCONNECTED.equals(action)) {
                invalidateOptionsMenu();
//                finish();
                Intent intentAct = new Intent(MainScreenActivity.this, IntroActivity.class);
                startActivity(intentAct);
            }
            else if(ConnectivityManager.CONNECTIVITY_ACTION.equals(action)) {
                //------------------------ IP Connectivity -----------------------------------------
                int networkType = intent.getIntExtra(ConnectivityManager.EXTRA_NETWORK_TYPE, -1);

                ConnectivityManager cm = (ConnectivityManager) context.getSystemService(Context.CONNECTIVITY_SERVICE);
                NetworkInfo activeNetInfo = cm.getActiveNetworkInfo();

                if (activeNetInfo == null || !activeNetInfo.isConnected()) {
                    /* if null: airplane mode, or mobile data & wifi off, or out of signal */

                    //update global connectivity so that we don't constantly try to connect to cloud
                    NodeService.SetNetworkConnStatus(context, false);

                    return;
                }

                //update global connectivity: we do have the internets; rejoice
                NodeService.SetNetworkConnStatus(context, true);
            }
        }
    };

    ////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////// HELPERS ////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////

    private void registerReceivers(){
        registerReceiver(mGattUpdateReceiver, makeGattUpdateIntentFilter());
        registerReceiver(mGattUpdateReceiver, makeConnectivityUpdateIntentFilter());
    }

    private static IntentFilter makeGattUpdateIntentFilter() {
        final IntentFilter intentFilter = new IntentFilter();
        intentFilter.addAction(BluetoothLeService.ACTION_GATT_DISCONNECTED);
        return intentFilter;
    }

    private static IntentFilter makeConnectivityUpdateIntentFilter() {
        final IntentFilter intentFilter = new IntentFilter();
        intentFilter.addAction(ConnectivityManager.CONNECTIVITY_ACTION);
        return intentFilter;
    }
}
