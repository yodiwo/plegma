package com.yodiwo.androidnode;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.net.Uri;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.CompoundButton;
import android.widget.ProgressBar;
import android.widget.SeekBar;
import android.widget.Switch;
import android.widget.TextView;


/**
 * A placeholder fragment containing a simple view.
 */
public class MainActivityFragment extends Fragment {

    // =============================================================================================
    // Static

    public static final String TAG = MainActivityFragment.class.getSimpleName();

    // =============================================================================================

    private ThingManager thingManager;
    private ProgressBar progressBar;
    private Switch inputSwitch1;
    private Switch inputSwitch2;
    private Switch inputSwitch3;
    private Button colorButton1;
    private Button colorButton2;
    private Button colorButton3;

    private TextView outputStr;
    private TextView inputStr;

    public MainActivityFragment() { }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {

        final Context context = this.getActivity().getApplicationContext();

        View view = inflater.inflate(R.layout.fragment_main, container, false);

        SettingsProvider settingsProvider = SettingsProvider.getInstance(context);
        thingManager = ThingManager.getInstance(context);

        // Link button1 to code
        Button button1 = (Button) view.findViewById(R.id.button1);
        button1.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View view, MotionEvent motionEvent) {
                int action = motionEvent.getAction();
                NodeService.SendPortMsg(context, ThingManager.Buttons, ThingManager.ButtonPort0,
                        action == MotionEvent.ACTION_DOWN ? NodeService.PortValue_Boolean_True : NodeService.PortValue_Boolean_False);
                return false;
            }
        });

        // Link button2 to code
        Button button2 = (Button) view.findViewById(R.id.button2);
        button2.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View view, MotionEvent motionEvent) {
                int action = motionEvent.getAction();
                NodeService.SendPortMsg(context, ThingManager.Buttons, ThingManager.ButtonPort1,
                        action == MotionEvent.ACTION_DOWN ? NodeService.PortValue_Boolean_True : NodeService.PortValue_Boolean_False);
                return false;
            }
        });

        // Link button3 to code
        Button button3 = (Button) view.findViewById(R.id.button3);
        button3.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View view, MotionEvent motionEvent) {
                int action = motionEvent.getAction();
                NodeService.SendPortMsg(context, ThingManager.Buttons, ThingManager.ButtonPort2,
                        action == MotionEvent.ACTION_DOWN ? NodeService.PortValue_Boolean_True : NodeService.PortValue_Boolean_False);
                return false;
            }
        });

        // Link the slider
        SeekBar seek = (SeekBar) view.findViewById(R.id.seekBar);
        seek.setOnSeekBarChangeListener(new SeekBar.OnSeekBarChangeListener() {
            @Override
            public void onProgressChanged(SeekBar seekBar, int progress, boolean fromUser) {
                // Send normalize value
                String value = Float.toString(progress / (float) seekBar.getMax());
                NodeService.SendPortMsg(context, ThingManager.Slider1, ThingManager.SliderPort, value);
            }

            @Override
            public void onStartTrackingTouch(SeekBar seekBar) {

            }

            @Override
            public void onStopTrackingTouch(SeekBar seekBar) {

            }
        });

        // Link the switches
        Switch s1 = (Switch) view.findViewById(R.id.output_switch1);
        s1.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
                // do something, the isChecked will be true if the switch is in the On position
                NodeService.SendPortMsg(context, ThingManager.Switches, ThingManager.ButtonPort0,
                        (isChecked) ? NodeService.PortValue_Boolean_True : NodeService.PortValue_Boolean_False);
            }
        });

        Switch s2 = (Switch) view.findViewById(R.id.output_switch2);
        s2.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
                // do something, the isChecked will be true if the switch is in the On position
                NodeService.SendPortMsg(context, ThingManager.Switches, ThingManager.ButtonPort1,
                        (isChecked) ? NodeService.PortValue_Boolean_True : NodeService.PortValue_Boolean_False);
            }
        });

        Switch s3 = (Switch) view.findViewById(R.id.output_switch3);
        s3.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
                // do something, the isChecked will be true if the switch is in the On position
                NodeService.SendPortMsg(context, ThingManager.Switches, ThingManager.ButtonPort2,
                        (isChecked) ? NodeService.PortValue_Boolean_True : NodeService.PortValue_Boolean_False);
            }
        });

        // GPS button
        Button gpsButton = (Button) view.findViewById(R.id.button_GPS);
        gpsButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {

            }
        });

        // Link rx path
        progressBar = (ProgressBar) view.findViewById(R.id.input_progressBar);
        inputSwitch1 = (Switch) view.findViewById(R.id.input_switch1);
        inputSwitch2 = (Switch) view.findViewById(R.id.input_switch2);
        inputSwitch3 = (Switch) view.findViewById(R.id.input_switch3);
        colorButton1 = (Button) view.findViewById(R.id.input_button_color1);
        colorButton2 = (Button) view.findViewById(R.id.input_button_color2);
        colorButton3 = (Button) view.findViewById(R.id.input_button_color3);

        LocalBroadcastManager.getInstance(context).registerReceiver(mMessageReceiverMainActivityService,
                new IntentFilter(NodeService.BROADCAST_THING_UPDATE));

        // UI elements to signify connectivity
        outputStr = (TextView) view.findViewById(R.id.textView);
        inputStr = (TextView) view.findViewById(R.id.textView2);

        //start out greyed out (no connection)
        outputStr.setAlpha(0.3f);
        inputStr.setAlpha(0.3f);

        LocalBroadcastManager.getInstance(context).registerReceiver(mMessageReceiverMainActivityService,
                new IntentFilter(aServerAPI.CONNECTIVITY_UI_UPDATE));

        return view;
    }

    // =============================================================================================
    // Events from background services

    private BroadcastReceiver mMessageReceiverMainActivityService = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            String action = intent.getAction();
            Log.i(TAG, "Broadcast received: " + action);

            try {
                if (action.equals(NodeService.BROADCAST_THING_UPDATE)) {
                    Bundle b = intent.getExtras();
                    int portID = b.getInt(NodeService.EXTRA_UPDATED_PORT_ID, -1);
                    String thingKey = b.getString(NodeService.EXTRA_UPDATED_THING_KEY);
                    String thingName = b.getString(NodeService.EXTRA_UPDATED_THING_NAME);
                    String portState = b.getString(NodeService.EXTRA_UPDATED_STATE);
                    Boolean isEvent = b.getBoolean(NodeService.EXTRA_UPDATED_IS_EVENT);
                    Log.i(TAG, "Update from Thing:" + thingName);

                    if (thingKey.equals(thingManager.GetThingKey(ThingManager.InputProgressBar))) {
                        float progress = Float.parseFloat(portState);
                        if(progress >= 0 && progress <= 1)
                            progressBar.setProgress((int) (progressBar.getMax() * progress));

                    }
                    else if (thingKey.equals(thingManager.GetThingKey(ThingManager.InputSwitches))) {
                        if(portID == 0)
                            inputSwitch1.setChecked(Boolean.parseBoolean(portState));
                        else if(portID==1)
                            inputSwitch2.setChecked(Boolean.parseBoolean(portState));
                        else if(portID==2)
                            inputSwitch3.setChecked(Boolean.parseBoolean(portState));

                    }
                    else if (thingKey.equals(thingManager.GetThingKey(ThingManager.InputColors))) {
                        double val = Double.parseDouble(portState);
                        int color = 0xff000000 | (int)((double)0xffffff * val);

                        if(portID == 0) {
                            colorButton1.setBackgroundColor(color);
                        } else if(portID == 1) {
                            colorButton2.setBackgroundColor(color);
                        } else if(portID == 2) {
                            colorButton3.setBackgroundColor(color);
                        }
                    }
                    else if (thingKey.equals(thingManager.GetThingKey(ThingManager.InputAndroidIntent))) {
                        if(isEvent) {
                            Intent i = new Intent(android.content.Intent.ACTION_VIEW,
                                    Uri.parse(portState));
                            startActivity(i);
                        }
                    }
                    else if (thingKey.equals(thingManager.GetThingKey(ThingManager.Torch))) {
                        Intent i = new Intent(context, ThingsModuleService.class);

                        i.putExtra(ThingsModuleService.EXTRA_INTENT_FOR_THING, ThingsModuleService.EXTRA_TORCH_THING);
                        i.putExtra(ThingsModuleService.EXTRA_TORCH_THING_STATE, Boolean.parseBoolean(portState));
                        context.startService(i);
                    }
                }
                else if (action.equals(aServerAPI.CONNECTIVITY_UI_UPDATE)) {
                    Bundle b = intent.getExtras();
                    Boolean rxActive = b.getBoolean(aServerAPI.EXTRA_UPDATED_RX_STATE);
                    Boolean txActive = b.getBoolean(aServerAPI.EXTRA_UPDATED_TX_STATE);

                    outputStr.setAlpha(txActive ? 1.0f : 0.3f);
                    inputStr.setAlpha(rxActive ? 1.0f : 0.3f);
                }
            } catch (Exception ex) {
                Log.e(TAG, "Failed to get update data: " + ex.getMessage());
            }
        }
    };
}
