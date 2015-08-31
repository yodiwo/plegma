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
    private Switch inputSwitch;
    private Button colorButton;

    public MainActivityFragment() {
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        final Context context = this.getActivity().getApplicationContext();

        View view = inflater.inflate(R.layout.fragment_main, container, false);

        SettingsProvider settingsProvider = SettingsProvider.getInstance(context);
        thingManager = ThingManager.getInstance(context);

        // Link button to code
        Button button = (Button) view.findViewById(R.id.button);
        button.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View view, MotionEvent motionEvent) {
                switch (motionEvent.getAction()) {
                    case MotionEvent.ACTION_DOWN:
                        NodeService.SendPortMsg(context,
                                ThingManager.Button1,
                                ThingManager.ButtonPort,
                                NodeService.PortValue_Boolean_True
                        );
                        break;
                    case MotionEvent.ACTION_UP:
                        NodeService.SendPortMsg(context,
                                ThingManager.Button1,
                                ThingManager.ButtonPort,
                                NodeService.PortValue_Boolean_False
                        );
                        break;
                }
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
                NodeService.SendPortMsg(context,
                        ThingManager.Slider1,
                        ThingManager.SliderPort,
                        value);
            }

            @Override
            public void onStartTrackingTouch(SeekBar seekBar) {

            }

            @Override
            public void onStopTrackingTouch(SeekBar seekBar) {

            }
        });


        // Link the switch
        Switch s = (Switch) view.findViewById(R.id.output_switch);
        s.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
                // do something, the isChecked will be
                // true if the switch is in the On position
                NodeService.SendPortMsg(context,
                        ThingManager.Switch1,
                        ThingManager.ButtonPort,
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
        inputSwitch = (Switch) view.findViewById(R.id.input_switch);
        colorButton = (Button) view.findViewById(R.id.input_button_color);

        LocalBroadcastManager.getInstance(context).registerReceiver(mMessageReceiverNodeService,
                new IntentFilter(NodeService.BROADCAST_THING_UPDATE));

        return view;
    }


    // =============================================================================================
    // Events from background services

    private BroadcastReceiver mMessageReceiverNodeService = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            String action = intent.getAction();
            Log.i(TAG, "Broadcast received: " + action);

            try {
                if (action.equals(NodeService.BROADCAST_THING_UPDATE)) {
                    Bundle b = intent.getExtras();
                    int portID = b.getInt(NodeService.EXTRA_UPDATED_PORT_ID, -1);
                    String thing = b.getString(NodeService.EXTRA_UPDATED_THING);
                    String thingName = b.getString(NodeService.EXTRA_UPDATED_THING_NAME);
                    String portState = b.getString(NodeService.EXTRA_UPDATED_STATE);
                    Boolean isEvent = b.getBoolean(NodeService.EXTRA_UPDATED_ISEVENT);

                    Log.i(TAG, "Update from Thing:" + thingName);

                    if (thing.equals(thingManager.GetThingKey(ThingManager.InputProgressBar1))) {
                        float progress = Float.parseFloat(portState);
                        progressBar.setProgress((int) (progressBar.getMax() * progress));
                    } else if (thing.equals(thingManager.GetThingKey(ThingManager.InputSwitch1))) {
                        inputSwitch.setChecked(Boolean.parseBoolean(portState));
                    } else if (thing.equals(thingManager.GetThingKey(ThingManager.InputColor1))) {

                    } else if (thing.equals(thingManager.GetThingKey(ThingManager.InputAndroidIntent))) {
                        if(isEvent) {
                            Intent i = new Intent(android.content.Intent.ACTION_VIEW,
                                    Uri.parse(portState));
                            startActivity(i);
                        }
                    }

                }
            } catch (Exception ex) {
                Log.e(TAG, "Failed to get update data");
            }
        }
    };
}
