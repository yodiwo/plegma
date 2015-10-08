package com.yodiwo.androidnode;


import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.util.Log;
import android.widget.Toast;

public class SensorsListener implements SensorEventListener {

    // =============================================================================================
    // Static information's

    public static final String TAG = SensorsListener.class.getSimpleName();
    private static SensorsListener sensorsListener = null;

    public static SensorsListener getInstance(Context context) {
        if (sensorsListener == null) {
            sensorsListener = new SensorsListener(context.getApplicationContext());
        }
        return sensorsListener;
    }

    public enum SensorType {
        Brightness,
        Accelerometer,
        Proximity,
        GPS
    }

    // =============================================================================================
    // Sensor Initialization


    private SensorManager mSensorManager;
    private Context context;
    private SettingsProvider settingsProvider;

    // Accelerometer
    private Sensor mAccelSensor;
    private float mAccel; // acceleration apart from gravity
    private float mAccelCurrent; // current acceleration including gravity
    private float mAccelLast; // last acceleration including gravity
    private long mLastAccelTS = 0;

    // Brightness
    private Sensor mBrightnessSensor;
    private float mBrightnessLast = 0;
    private long mBrightnessTS = 0;

    //Proximity
    private Sensor mProximitySensor;
    private long mProximityTS = 0;

    private final long MILLISEC_IN_SEC = 1000;
    private final long MICROSEC_IN_SEC = 1000 * MILLISEC_IN_SEC;
    private final long NANOSEC_IN_SEC = 1000 * MICROSEC_IN_SEC;

    // ---------------------------------------------------------------------------------------------

    public SensorsListener(Context context) {
        this.context = context;
        settingsProvider = SettingsProvider.getInstance(context);

        // Get an instance of the sensor service, and use that to get an instance of a particular sensor.
        mSensorManager = (SensorManager) context.getSystemService(Context.SENSOR_SERVICE);

        // Init Accel sensor
        mAccel = 0.00f;
        mAccelCurrent = SensorManager.GRAVITY_EARTH;
        mAccelLast = SensorManager.GRAVITY_EARTH;

        Toast.makeText(context, "Sensor listening service created", Toast.LENGTH_SHORT).show();
    }

    // ---------------------------------------------------------------------------------------------

    @Override
    public void onSensorChanged(SensorEvent sensorEvent) {

        long timestamp = sensorEvent.timestamp;

        if (sensorEvent.sensor.getType() == Sensor.TYPE_ACCELEROMETER) {
            float x = sensorEvent.values[0];
            float y = sensorEvent.values[1];
            float z = sensorEvent.values[2];
            boolean isShake = false;
            mAccelLast = mAccelCurrent;
            mAccelCurrent = android.util.FloatMath.sqrt(x * x + y * y + z * z);
            float delta = mAccelCurrent - mAccelLast;
            mAccel = mAccel * 0.9f + delta; // perform low-cut filter

            // Check for shacked
            if (mAccel > 10) {
                isShake = true;
                Toast.makeText(context, "Device has shaken.", Toast.LENGTH_SHORT).show();
            }

            // filter out too frequent readings
            // and send data to node service
            if ((timestamp - mLastAccelTS > 2*NANOSEC_IN_SEC) &&
                    ((Math.abs(mAccel) > 0.9f) || isShake)) {

                Log.d(TAG, "Accel Delta:" + Float.toString(delta));

                mLastAccelTS = timestamp;
                NodeService.SendPortMsg(context, ThingManager.Accelerometer,
                        new String[]{
                                Float.toString(x),
                                Float.toString(y),
                                Float.toString(x),
                                (isShake) ? "True" : "False"
                        });
            } else if (isShake) { // The shakes must override the time filter
                NodeService.SendPortMsg(context,
                        ThingManager.Accelerometer,
                        ThingManager.AccelerometerShaken,
                        "True"
                );
            }

        } else if (sensorEvent.sensor.getType() == Sensor.TYPE_PROXIMITY) {
            float centimeters = sensorEvent.values[0];
            //boolean val = (centimeters<2.0) ? true : false;

            if ( (centimeters < 1.0) &&
                 (timestamp - mProximityTS > 1*NANOSEC_IN_SEC) ) {

                mProximityTS = timestamp;

                NodeService.SendPortMsg(context, ThingManager.Proximity, ThingManager.ProximityPort, "True");
                NodeService.SendPortMsg(context, ThingManager.Proximity, ThingManager.ProximityPort, "False");
            }

        } else if (sensorEvent.sensor.getType() == Sensor.TYPE_LIGHT) {
            float brightness = sensorEvent.values[0];

            Log.d(TAG, "Brightness:" + brightness);

            // filter out too frequent readings
            // and send data to node service
            if (timestamp - mBrightnessTS > 1*NANOSEC_IN_SEC) {
                if (Math.abs(brightness - mBrightnessLast) > Math.abs(brightness / 10)) {
                    mBrightnessTS = timestamp;
                    NodeService.SendPortMsg(context,
                            ThingManager.Brightness,
                            ThingManager.BrightnessPort,
                            Float.toString(brightness));
                    mBrightnessLast = brightness;
                }
            }
        }
    }

    // ---------------------------------------------------------------------------------------------

    @Override
    public void onAccuracyChanged(Sensor sensor, int i) {

    }

    // =============================================================================================
    // Public Functions

    public void StopService(SensorType type) {
        switch (type) {
            case Accelerometer:
                if (mAccelSensor != null)
                    mSensorManager.unregisterListener(this, mAccelSensor);
                break;
            case Brightness:
                if (mBrightnessSensor != null)
                    mSensorManager.unregisterListener(this, mBrightnessSensor);
                break;
            case Proximity:
                if(mProximitySensor!=null)
                    mSensorManager.unregisterListener(this, mProximitySensor);
                break;
        }
    }

    // ---------------------------------------------------------------------------------------------

    public void StartService(SensorType type) {
        switch (type) {
            case Accelerometer:
                if(settingsProvider.getServiceAccelerometerEnabled()) {
                    mAccelSensor = mSensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER);
                    mSensorManager.registerListener(this, mAccelSensor, SensorManager.SENSOR_DELAY_NORMAL);
                }
                break;
            case Brightness:
                if(settingsProvider.getServiceBrightnessEnabled()) {
                    mBrightnessSensor = mSensorManager.getDefaultSensor(Sensor.TYPE_LIGHT);
                    mSensorManager.registerListener(this, mBrightnessSensor, SensorManager.SENSOR_DELAY_NORMAL);
                }
                break;
            case  Proximity:
                mProximitySensor = mSensorManager.getDefaultSensor(Sensor.TYPE_PROXIMITY);
                mSensorManager.registerListener(this, mProximitySensor, SensorManager.SENSOR_DELAY_NORMAL);
                break;
        }
    }

    // =============================================================================================
}
