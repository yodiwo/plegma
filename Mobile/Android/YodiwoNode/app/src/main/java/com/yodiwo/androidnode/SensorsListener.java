package com.yodiwo.androidnode;


import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.os.Bundle;
import android.os.Vibrator;
import android.support.v4.content.LocalBroadcastManager;
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
        Gyroscope,
        Rotation,
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


    // Rotation Sensore
    private static Sensor mRotationSensor;
    private final double[] mRotationVector = new double[3];


    private static Sensor mGyroSensor;
    private static long mGyroTS = 0;
    private final float[] deltaRotationVector = new float[4];
    private final float[] oldDeltaRotationVector = new float[4];
    private final float[] rotationCurrent = new float[4];

    //Proximity
    private static Sensor mProximitySensor;
    private long mProximityTS = 0;

    private final long MILLISEC_IN_SEC = 1000;
    private final long MICROSEC_IN_SEC = 1000 * MILLISEC_IN_SEC;
    private final long NANOSEC_IN_SEC = 1000 * MICROSEC_IN_SEC;

    private static final float NS2S = 1.0f / 1000000000.0f;

    // ---------------------------------------------------------------------------------------------

    public SensorsListener(Context context) {
        SensorsListener.context = context;

        if (mSensorManager == null) {
            // Get an instance of the sensor service, and use that to get an instance of a particular sensor.
            mSensorManager = (SensorManager) context.getSystemService(Context.SENSOR_SERVICE);

            // Init Accel sensor
            mAccel = 0.00f;
            mAccelCurrent = SensorManager.GRAVITY_EARTH;
            mAccelLast = SensorManager.GRAVITY_EARTH;

            // Rx message
            BroadcastReceiver mMessageReceiver = new BroadcastReceiver() {
                @Override
                public void onReceive(Context context, Intent intent) {
                    String action = intent.getAction();
                    Log.i(TAG, "Broadcast received: " + action);

                    try {
                        switch (action) {
                            case NodeService.BROADCAST_ACTIVE_PORT_UPDATE: {
                                Bundle b = intent.getExtras();
                                ThingManager thingManager = ThingManager.getInstance(context);

                                String[] thingKeys = b.getStringArray(NodeService.EXTRA_UPDATED_ACTIVE_THINGS_KEYS);
                                if (thingKeys == null)
                                    break;

                                String[] portKeys = b.getStringArray(NodeService.EXTRA_UPDATED_ACTIVE_PORT_KEYS);
                                if (portKeys == null)
                                    break;

                                // Stop all services
                                StopService(SensorType.Accelerometer);
                                StopService(SensorType.Brightness);
                                StopService(SensorType.Proximity);
                                StopService(SensorType.Gyroscope);
                                StopService(SensorType.Rotation);

                                // Start the services that we have active
                                for (String thingKey : thingKeys) {
                                    if (thingKey.equals(thingManager.GetThingKey(ThingManager.Accelerometer))) {
                                        StartService(SensorType.Accelerometer);
                                    } else if (thingKey.equals(thingManager.GetThingKey(ThingManager.BrightnessSensor))) {
                                        StartService(SensorType.Brightness);
                                    } else if (thingKey.equals(thingManager.GetThingKey(ThingManager.Proximity))) {
                                        StartService(SensorType.Proximity);
                                    } else if (thingKey.equals(thingManager.GetThingKey(ThingManager.Gyroscope))) {
                                        StartService(SensorType.Gyroscope);
                                    } else if (thingKey.equals(thingManager.GetThingKey(ThingManager.Rotation))) {
                                        StartService(SensorType.Rotation);
                                    }
                                }
                            }
                            break;
                        }
                    } catch (Exception e) {
                        Helpers.logException(TAG, e);
                    }
                }
            };

            LocalBroadcastManager.getInstance(context).registerReceiver(mMessageReceiver,
                    new IntentFilter(NodeService.BROADCAST_ACTIVE_PORT_UPDATE));
        }
    }


    // ---------------------------------------------------------------------------------------------

    @Override
    public void onSensorChanged(SensorEvent sensorEvent) {

        if (mSensorManager == null)
            return;

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

            //send shake event independently (time filtered)
            if (isShake && (timestamp - mLastAccelTS > 2 * NANOSEC_IN_SEC)) {
                NodeService.SendPortMsg(context, ThingManager.Accelerometer, ThingManager.AccelerometerShaken, "True");
                mLastAccelTS = timestamp;
                Toast.makeText(context, "Device has shaken", Toast.LENGTH_SHORT).show();
            }

            // send accelerometer data to node service
            if (Math.abs(mAccel) > 0.9f) {

                NodeService.SendPortMsg(context, ThingManager.Accelerometer,
                        new String[]{
                                Float.toString(x),
                                Float.toString(y),
                                Float.toString(z)
                        },
                        0);
            }

        } else if (sensorEvent.sensor.getType() == Sensor.TYPE_PROXIMITY) {
            float centimeters = sensorEvent.values[0];
            //boolean val = (centimeters<2.0) ? true : false;

            if ((centimeters < 1.0) &&
                    (timestamp - mProximityTS > 1 * NANOSEC_IN_SEC)) {

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
            if (timestamp - mBrightnessTS > 2 * NANOSEC_IN_SEC) {
                if (Math.abs(brightness - mBrightnessLast) > Math.abs(brightness / 5)) {
                    mBrightnessTS = timestamp;
                    NodeService.SendPortMsg(context,
                            ThingManager.BrightnessSensor,
                            ThingManager.BrightnessSensorPort,
                            Float.toString(brightness),
                            0);
                    mBrightnessLast = brightness;
                }
            }
        } else if (sensorEvent.sensor.getType() == Sensor.TYPE_GYROSCOPE) {

            if (mGyroTS != 0) {
                final float dT = (sensorEvent.timestamp - mGyroTS) * NS2S;
                // Axis of the rotation sample, not normalized yet.
                float axisX = sensorEvent.values[0];
                float axisY = sensorEvent.values[1];
                float axisZ = sensorEvent.values[2];

                // Calculate the angular speed of the sample
                float omegaMagnitude = (float) Math.sqrt(axisX * axisX + axisY * axisY + axisZ * axisZ);

                // Normalize the rotation vector if it's big enough to get the axis
                // (that is, EPSILON should represent your maximum allowable margin of error)
                if (omegaMagnitude > 0.00001) {
                    axisX /= omegaMagnitude;
                    axisY /= omegaMagnitude;
                    axisZ /= omegaMagnitude;
                }

                // Integrate around this axis with the angular speed by the timestep
                // in order to get a delta rotation from this sample over the timestep
                // We will convert this axis-angle representation of the delta rotation
                // into a quaternion before turning it into the rotation matrix.
                float thetaOverTwo = omegaMagnitude * dT / 2.0f;
                double sinThetaOverTwo = Math.sin(thetaOverTwo);
                double cosThetaOverTwo = Math.cos(thetaOverTwo);
                deltaRotationVector[0] = (float) sinThetaOverTwo * axisX;
                deltaRotationVector[1] = (float) sinThetaOverTwo * axisY;
                deltaRotationVector[2] = (float) sinThetaOverTwo * axisZ;
                deltaRotationVector[3] = (float) cosThetaOverTwo;

                NodeService.SendPortMsg(context, ThingManager.Gyroscope,
                        new String[]{
                                Float.toString(deltaRotationVector[0]),
                                Float.toString(deltaRotationVector[1]),
                                Float.toString(deltaRotationVector[2]),
                                Float.toString(deltaRotationVector[3]),
                        },
                        0);
            }
            mGyroTS = sensorEvent.timestamp;
        } else if (sensorEvent.sensor.getType() == Sensor.TYPE_ROTATION_VECTOR) {

            float rotationMatrix[] = new float[16];
            SensorManager.getRotationMatrixFromVector(rotationMatrix, sensorEvent.values);

            float[] orientationValues = new float[3];
            SensorManager.getOrientation(rotationMatrix, orientationValues);
            double azimuth = Math.toDegrees(orientationValues[0]);
            double pitch = Math.toDegrees(orientationValues[1]);
            double roll = Math.toDegrees(orientationValues[2]);

            double minChange = 2;
            if (Math.abs(mRotationVector[0] - azimuth) > minChange ||
                    Math.abs(mRotationVector[1] - pitch) > minChange ||
                    Math.abs(mRotationVector[2] - roll) > minChange) {
                mRotationVector[0] = azimuth;
                mRotationVector[1] = pitch;
                mRotationVector[2] = roll;

                NodeService.SendPortMsg(context, ThingManager.Rotation,
                        new String[]{
                                Double.toString(mRotationVector[0]),
                                Double.toString(mRotationVector[1]),
                                Double.toString(mRotationVector[2])
                        },
                        0);
            }
        }
    }


    // ---------------------------------------------------------------------------------------------

    @Override
    public void onAccuracyChanged(Sensor sensor, int i) {

    }

    // =============================================================================================
    // Public Functions

    private void StopService(SensorType type) {
        switch (type) {
            case Accelerometer:
                if (mAccelSensor != null) {
                    mSensorManager.unregisterListener(this, mAccelSensor);
                    mAccelSensor = null;
                }
                break;
            case Rotation:
                if (mRotationSensor != null) {
                    mSensorManager.unregisterListener(this, mRotationSensor);
                    mRotationSensor = null;
                }
                break;
            case Gyroscope:
                if (mGyroSensor != null) {
                    mSensorManager.unregisterListener(this, mGyroSensor);
                    mGyroSensor = null;
                }
                break;
            case Brightness:
                if (mBrightnessSensor != null) {
                    mSensorManager.unregisterListener(this, mBrightnessSensor);
                    mBrightnessSensor = null;
                }
                break;
            case Proximity:
                if (mProximitySensor != null) {
                    mSensorManager.unregisterListener(this, mProximitySensor);
                    mProximitySensor = null;
                }
                break;
        }
    }

    // ---------------------------------------------------------------------------------------------

    private void StartService(SensorType type) {
        switch (type) {
            case Accelerometer:
                if (SettingsProvider.getInstance(context).getServiceAccelerometerEnabled()) {
                    mAccelSensor = mSensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER);
                    mSensorManager.registerListener(this, mAccelSensor, SensorManager.SENSOR_DELAY_NORMAL);
                }
                break;
            case Rotation:
                if (SettingsProvider.getInstance(context).getServiceRotationEnabled()) {
                    mRotationSensor = mSensorManager.getDefaultSensor(Sensor.TYPE_ROTATION_VECTOR);
                    mSensorManager.registerListener(this, mRotationSensor, SensorManager.SENSOR_DELAY_NORMAL);
                }
                break;
            case Gyroscope:
                if (SettingsProvider.getInstance(context).getServiceGyroscopeEnabled()) {
                    mGyroSensor = mSensorManager.getDefaultSensor(Sensor.TYPE_GYROSCOPE);
                    mSensorManager.registerListener(this, mGyroSensor, SensorManager.SENSOR_DELAY_NORMAL);
                }
                break;
            case Brightness:
                if (SettingsProvider.getInstance(context).getServiceBrightnessEnabled()) {
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

    public void Stop() {
        StopService(SensorType.Accelerometer);
        StopService(SensorType.Brightness);
        StopService(SensorType.Proximity);
        StopService(SensorType.Gyroscope);
        StopService(SensorType.Rotation);
    }

    public void Start() {
        // Force active key update
        NodeService.ForceActiveKeyUpdate(context);
    }

    // =============================================================================================
}
