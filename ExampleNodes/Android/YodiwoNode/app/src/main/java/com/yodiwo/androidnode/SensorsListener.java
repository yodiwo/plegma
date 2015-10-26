package com.yodiwo.androidnode;


import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.os.Vibrator;
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


    private static SensorManager mSensorManager = null;
    private static Context context;

    // Accelerometer
    private static Sensor mAccelSensor;
    private static double mAccelLast; // last acceleration including gravity
    private static long mLastAccelTS = 0;
    private double mAccel; // acceleration apart from gravity
    private double mAccelCurrent; // current acceleration including gravity

    // Brightness
    private static Sensor mBrightnessSensor;
    private static float mBrightnessLast = 0;
    private static long mBrightnessTS = 0;

    //Proximity
    private static Sensor mProximitySensor;
    private long mProximityTS = 0;

    private final long MILLISEC_IN_SEC = 1000;
    private final long MICROSEC_IN_SEC = 1000 * MILLISEC_IN_SEC;
    private final long NANOSEC_IN_SEC = 1000 * MICROSEC_IN_SEC;

    // ---------------------------------------------------------------------------------------------

    public SensorsListener(Context context) {
        this.context = context;

        if (mSensorManager == null) {
            // Get an instance of the sensor service, and use that to get an instance of a particular sensor.
            mSensorManager = (SensorManager) context.getSystemService(Context.SENSOR_SERVICE);

            // Init Accel sensor
            mAccel = 0.00f;
            mAccelCurrent = SensorManager.GRAVITY_EARTH;
            mAccelLast = SensorManager.GRAVITY_EARTH;
        }
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
            mAccelCurrent = Math.sqrt(x * x + y * y + z * z);
            double delta = mAccelCurrent - mAccelLast;
            mAccel = mAccel * 0.9 + delta; // perform low-cut filter

            // Check for shake event
            if (mAccel > 10) {
                isShake = true;
            }

            // filter out too frequent readings
            // and send data to node service
            if ((timestamp - mLastAccelTS > 2*NANOSEC_IN_SEC) &&
                    ((Math.abs(mAccel) > 0.9f) || isShake)) {

                if (isShake) {
                    Toast.makeText(context, "Device has shaken", Toast.LENGTH_SHORT).show();
                }
                Log.d(TAG, "Accel Delta:" + Double.toString(delta));
                mLastAccelTS = timestamp;
                NodeService.SendPortMsg(context, ThingManager.Accelerometer,
                        new String[]{
                                Float.toString(x),
                                Float.toString(y),
                                Float.toString(x),
                                (isShake) ? "True" : "False"
                        });
            }

        } else if (sensorEvent.sensor.getType() == Sensor.TYPE_PROXIMITY) {
            float centimeters = sensorEvent.values[0];
            //boolean val = (centimeters<2.0) ? true : false;

            if ( (centimeters < 1.0) &&
                 (timestamp - mProximityTS > 1*NANOSEC_IN_SEC) ) {

                Vibrator vibrator = (Vibrator) context.getSystemService(Context.VIBRATOR_SERVICE);
                vibrator.vibrate(50);

                mProximityTS = timestamp;

                NodeService.SendPortMsg(context, ThingManager.Proximity, ThingManager.ProximityPort, "True");
                NodeService.SendPortMsg(context, ThingManager.Proximity, ThingManager.ProximityPort, "False");
                Log.d(TAG, "Proximity sensor fired");
            }

        } else if (sensorEvent.sensor.getType() == Sensor.TYPE_LIGHT) {
            float brightness = sensorEvent.values[0];

            // filter out too frequent readings
            // and send data to node service
            if (timestamp - mBrightnessTS > 2*NANOSEC_IN_SEC) {
                if (Math.abs(brightness - mBrightnessLast) > Math.abs(brightness / 5)) {
                    mBrightnessTS = timestamp;
                    NodeService.SendPortMsg(context,
                            ThingManager.Brightness,
                            ThingManager.BrightnessPort,
                            Float.toString(brightness));
                    mBrightnessLast = brightness;
                    Log.d(TAG, "Brightness:" + brightness);
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
                if (mAccelSensor != null) {
                    mAccelSensor = null;
                    mSensorManager.unregisterListener(this, mAccelSensor);
                }
                break;
            case Brightness:
                if (mBrightnessSensor != null) {
                    mBrightnessSensor = null;
                    mSensorManager.unregisterListener(this, mBrightnessSensor);
                }
                break;
            case Proximity:
                if(mProximitySensor != null) {
                    mProximitySensor = null;
                    mSensorManager.unregisterListener(this, mProximitySensor);
                }
                break;
        }
    }

    // ---------------------------------------------------------------------------------------------

    public void StartService(SensorType type) {
        switch (type) {
            case Accelerometer:
                if(SettingsProvider.getInstance(context).getServiceAccelerometerEnabled()) {
                    mAccelSensor = mSensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER);
                    mSensorManager.registerListener(this, mAccelSensor, SensorManager.SENSOR_DELAY_NORMAL);
                }
                break;
            case Brightness:
                if(SettingsProvider.getInstance(context).getServiceBrightnessEnabled()) {
                    mBrightnessSensor = mSensorManager.getDefaultSensor(Sensor.TYPE_LIGHT);
                    mSensorManager.registerListener(this, mBrightnessSensor, SensorManager.SENSOR_DELAY_NORMAL);
                }
                break;
            case Proximity:
                //TODO: Check that device actually has a proximity sensor (tablets don't)
                mProximitySensor = mSensorManager.getDefaultSensor(Sensor.TYPE_PROXIMITY);
                mSensorManager.registerListener(this, mProximitySensor, SensorManager.SENSOR_DELAY_NORMAL);
                break;
        }
    }

    // =============================================================================================
}
