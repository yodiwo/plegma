package com.yodiwo.androidnode;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.Bundle;
import android.support.v4.content.LocalBroadcastManager;
import android.support.v7.app.ActionBarActivity;
import android.util.Log;
import android.view.KeyEvent;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.WindowManager;
import android.view.inputmethod.InputMethodManager;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

public class PairingActivity extends ActionBarActivity {


    public static final String TAG = PairingActivity.class.getSimpleName();

    // =============================================================================================
    // Activity overrides

    private ProgressBar pairing_progress;
    private TextView status_text;
    private TextView nodekey_text;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        final Context context = this;
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_pairing);

        getWindow().addFlags(WindowManager.LayoutParams.FLAG_FORCE_NOT_FULLSCREEN);
        getWindow().clearFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN);

        // Listen to broadcast from Pairing Service for any updates
        LocalBroadcastManager.getInstance(this).registerReceiver(mMessageReceiverPairingService,
                new IntentFilter(PairingService.BROADCAST_PHASE1_FINISHED));

        LocalBroadcastManager.getInstance(this).registerReceiver(mMessageReceiverPairingService,
                new IntentFilter(PairingService.BROADCAST_PAIRING_FINISHED));

        // Get UI elements
        pairing_progress = (ProgressBar) findViewById(R.id.pairing_progress);
        pairing_progress.setVisibility(View.INVISIBLE);

        status_text = (TextView) findViewById(R.id.status);
        nodekey_text = (TextView) findViewById(R.id.nodekey);

        final EditText UUID_text = (EditText) findViewById(R.id.uuid);
        UUID_text.setText(SettingsProvider.getInstance(this).getNodeUUID());
        UUID_text.setFocusable(false);

        String nkey = SettingsProvider.getInstance(this).getNodeKey();
        String skey = SettingsProvider.getInstance(this).getNodeSecretKey();

        if(nkey != null && skey != null) {
            status_text.setText("Paired");
            nodekey_text.setText(nkey);
        } else {
            status_text.setText("Unpaired");
            nodekey_text.setText("");
        }

        if ((status_text.getText().equals("Pairing Successful")) || (status_text.getText().equals("Paired")))
            UUID_text.setFocusableInTouchMode(false);
        else
            UUID_text.setFocusableInTouchMode(true);

        UUID_text.setOnKeyListener(new View.OnKeyListener() {
            public boolean onKey(View view, int keyCode, KeyEvent event) {
                UUID_text.setCursorVisible(true);
                final InputMethodManager imm = (InputMethodManager) context.getSystemService(Activity.INPUT_METHOD_SERVICE);

                if (keyCode == KeyEvent.KEYCODE_ENTER) {
                    imm.hideSoftInputFromWindow(getWindow().getDecorView().getWindowToken(), 0);
                    UUID_text.setCursorVisible(false);

                    SettingsProvider
                            .getInstance(getApplicationContext())
                            .setUserUUID(UUID_text.getText().toString());
                }
                return true;
            }
        });

        // Set button events
        Button pairButton = (Button) findViewById(R.id.button_pairing);
        pairButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                // Start pairing
                UUID_text.setFocusable(false);
                pairing_progress.setVisibility(View.VISIBLE);
                PairingService.StartPairing(context);

                status_text.setText("Pairing ongoing");
            }
        });

        // Unset pairing
        Button unpairButton = (Button) findViewById(R.id.button_unpairing);
        unpairButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                AlertDialog.Builder dlgAlert = new AlertDialog.Builder(context);
                dlgAlert.setMessage("Are you sure that you want to unpair?")
                        .setTitle("UnPair?")
                                // un paired
                        .setPositiveButton("Yes",
                                new DialogInterface.OnClickListener() {
                                    public void onClick(DialogInterface dialog, int which) {
                                        status_text.setText("Unpaired");
                                        nodekey_text.setText("");
                                        UUID_text.setFocusableInTouchMode(true);
                                        MainActivity.isUnpairedByUser = true;
                                        // Send request to node pairing service to clean internal state
                                        PairingService.UnPair(context);
                                    }
                                })
                        .setNegativeButton("No", null)
                        .setCancelable(true)
                        .show();
            }
        });
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

        //noinspection SimplifiableIfStatement
        if (id == R.id.action_settings) {
            Intent intent = new Intent(this, SettingsActivity.class);
            startActivity(intent);
            return true;
        }

        return super.onOptionsItemSelected(item);
    }


    // =============================================================================================
    // Events from background services

    private BroadcastReceiver mMessageReceiverPairingService = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            String action = intent.getAction();
            Log.i(TAG, "Broadcast received: " + action);

            if (action.equals(PairingService.BROADCAST_PHASE1_FINISHED)) {
                int status = intent.getExtras().getInt(PairingService.EXTRA_STATUS,
                        PairingService.EXTRA_STATUS_FAILED);

                Log.i(TAG, "Phase 1 finished status: " + status);

                if (status == PairingService.EXTRA_STATUS_SUCCESS) {
                    Intent intentWeb = new Intent(context, PairingWebActivity.class);
                    startActivity(intentWeb);
                } else {
                    status_text.setText("Failed to get tokens");
                }

                pairing_progress.setVisibility(View.INVISIBLE);
            }
            else if (action.equals(PairingService.BROADCAST_PAIRING_FINISHED)) {

                int status = intent.getExtras().getInt(PairingService.EXTRA_STATUS,
                        PairingService.EXTRA_STATUS_FAILED);

                Log.i(TAG, "Phase 1 finished status: " + status);

                if (status == PairingService.EXTRA_STATUS_SUCCESS) {
                    nodekey_text.setText(SettingsProvider.getInstance(context).getNodeKey());
                    status_text.setText("Pairing Successful");
                    Toast.makeText(context, "visit https://cyan.yodiwo.com to start creating stories!", Toast.LENGTH_SHORT).show();
                }
                else {
                    nodekey_text.setText("");
                    status_text.setText("Failed to get keys");
                }

                //TODO: create dialog at UI level informing of cyan.yodiwo.com
                /*
                AlertDialog.Builder builder = new AlertDialog.Builder(context);
                builder.setTitle("Pairing complete!")
                        .setMessage("Now visit https://cyan.yodiwo.com to start creating stories!")
                        .setIcon(android.R.drawable.ic_dialog_info)
                        .setNeutralButton("OK", new DialogInterface.OnClickListener() {
                            @Override
                            public void onClick(DialogInterface dialog, int which) {
                                Intent intent = new Intent(PairingActivity.this, MainActivity.class);
                                startActivity(intent);
                            }
                        })
                        .setCancelable(true)
                        .show();
                */
            }
        }
    };
    // ---------------------------------------------------------------------------------------------
}
