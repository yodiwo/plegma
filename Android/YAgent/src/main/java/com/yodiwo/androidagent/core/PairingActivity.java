package com.yodiwo.androidagent.core;

import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.support.v4.content.LocalBroadcastManager;
import android.support.v7.app.AlertDialog;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import com.yodiwo.androidagent.R;


public class PairingActivity extends AppCompatActivity {


    public static final String TAG = PairingActivity.class.getSimpleName();

    // =============================================================================================
    // Activity overrides

    private ProgressBar pairing_progress;
    private TextView status_text;
    private TextView nodekey_text;

    private static Activity activity = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        final Context context = this;
        super.onCreate(savedInstanceState);

        activity = this;

        try {
            overridePendingTransition(R.anim.fade_in, R.anim.fade_out);
            setContentView(R.layout.activity_pairing);
            Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar_pairing);
            setSupportActionBar(toolbar);
        } catch(Exception ex){
            Helpers.logException(TAG, ex);
        }

        // Listen to broadcast from Pairing Service for any updates
        LocalBroadcastManager.getInstance(this).registerReceiver(mMsgRxPairingService,
                new IntentFilter(PairingService.BROADCAST_PHASE1_FINISHED));

        LocalBroadcastManager.getInstance(this).registerReceiver(mMsgRxPairingService,
                new IntentFilter(PairingService.BROADCAST_PAIRING_FINISHED));

        LocalBroadcastManager.getInstance(this).registerReceiver(mMsgRxPairingService,
                new IntentFilter(PairingService.BROADCAST_MODAL_DIALOG_FINISHED));

        // Get UI elements
        pairing_progress = (ProgressBar) findViewById(R.id.pairing_progress);
        pairing_progress.setVisibility(View.GONE);

        status_text = (TextView) findViewById(R.id.status);
        nodekey_text = (TextView) findViewById(R.id.nodekey);

        String nkey = SettingsProvider.getInstance(this).getNodeKey();
        String skey = SettingsProvider.getInstance(this).getNodeSecretKey();

        if(nkey != null && skey != null) {
            status_text.setText(R.string.paired);
            nodekey_text.setText(nkey);
            PairingService.setPairingStatus(PairingService.PairingStatus.PAIRED);
        } else {
            status_text.setText(R.string.unpaired);
            nodekey_text.setText("");
            PairingService.setPairingStatus(PairingService.PairingStatus.UNPAIRED);
        }

        // Set button events
        Button pairButton = (Button) findViewById(R.id.button_pairing);
        pairButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                // Start pairing
                status_text.setText(R.string.pairing_ongoing);
                new Handler(Looper.getMainLooper()).post(new Runnable() {
                    @Override
                    public void run() {
                        pairing_progress.setVisibility(View.VISIBLE);
                    }
                });
                PairingService.StartPairing(context);
            }
        });

        // Unset pairing
        Button unpairButton = (Button) findViewById(R.id.button_unpairing);
        unpairButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                if (!status_text.getText().equals(getString(R.string.unpaired))){
                    AlertDialog.Builder dlgAlert = new AlertDialog.Builder(context);
                    dlgAlert.setMessage("Are you sure that you want to unpair?")
                            .setTitle("Unpair?")
                            // un paired
                            .setPositiveButton("Yes",
                                    new DialogInterface.OnClickListener() {
                                        public void onClick(DialogInterface dialog, int which) {
                                            status_text.setText(R.string.unpaired);
                                            nodekey_text.setText("");
                                            // Send request to node pairing service to clean internal state
                                            PairingService.UnPair(context, true);
                                        }
                                    })
                            .setNegativeButton("No", null)
                            .setCancelable(true)
                            .show();
                }
                else{
                    Toast.makeText(context, "NeBiT is already unpaired", Toast.LENGTH_SHORT).show();
                }
            }
        });
    }

    // ---------------------------------------------------------------------------------------------

    @Override
    public void onResume() {
        super.onResume();
    }

    // ---------------------------------------------------------------------------------------------

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_pairing, menu);
        return true;
    }

    // ---------------------------------------------------------------------------------------------

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();
        Intent intent = null;

        // Select the opened activity
        if (id == R.id.action_settings) {
            intent = new Intent(this, SettingsActivity.class);
        } else if (id == R.id.action_hardware_info) {
            intent = new Intent(this, ModalDialogActivity.class);
        }

        if (intent != null) {
            startActivity(intent);
            return true;
        }

        return super.onOptionsItemSelected(item);
    }

    // ---------------------------------------------------------------------------------------------

    @Override
    public void onBackPressed() {
        super.onBackPressed();
    }

    // =============================================================================================
    // Events from background services

    private BroadcastReceiver mMsgRxPairingService = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            String action = intent.getAction();
            Log.i(TAG, "Broadcast received: " + action);

            if (action.equals(PairingService.BROADCAST_PHASE1_FINISHED)) {
                int status = intent.getExtras().getInt(PairingService.EXTRA_STATUS,
                        PairingService.EXTRA_STATUS_FAILED);

                Log.i(TAG, "Phase 1 finished status: " + status);

                if (status == PairingService.EXTRA_STATUS_SUCCESS) {
                    if (PairingService.getPairingStatus() != PairingService.PairingStatus.PHASE1FINISHED) {
                        PairingService.setPairingStatus(PairingService.PairingStatus.PHASE1FINISHED);
                        Intent intentWeb = new Intent(context, PairingWebActivity.class);
                        startActivity(intentWeb);
                    }
                } else {
                    status_text.setText(R.string.failed_to_get_tokens);
                    PairingService.setPairingStatus(PairingService.PairingStatus.UNPAIRED);
                }

                pairing_progress.setVisibility(View.GONE);
            } else if (action.equals(PairingService.BROADCAST_PAIRING_FINISHED)) {

                int status = intent.getExtras().getInt(PairingService.EXTRA_STATUS,
                        PairingService.EXTRA_STATUS_FAILED);

                Log.i(TAG, "Phase 1 finished status: " + status);

                if (status == PairingService.EXTRA_STATUS_SUCCESS) {
                    // inform UI
                    nodekey_text.setText(SettingsProvider.getInstance(context).getNodeKey());
                    status_text.setText(R.string.pairing_success);

                    PairingService.setPairingStatus(PairingService.PairingStatus.PAIRED);

                    // show modal dialog
                    if (!ModalDialogActivity.isShown) {
                        ModalDialogActivity.isShown = true;
                        startActivity(new Intent(context, ModalDialogActivity.class));
                    }
                } else {
                    nodekey_text.setText("");
                    status_text.setText(R.string.failed_to_get_keys);
                    PairingService.setPairingStatus(PairingService.PairingStatus.UNPAIRED);
                }
            } else if (action.equals(PairingService.BROADCAST_MODAL_DIALOG_FINISHED)) {
                activity.finish();
            }
        }
    };
}
