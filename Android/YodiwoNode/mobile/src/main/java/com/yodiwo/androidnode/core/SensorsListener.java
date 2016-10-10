package com.yodiwo.androidnode.core;


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

import com.yodiwo.androidagent.core.Helpers;
import com.yodiwo.androidagent.core.NodeService;
import com.yodiwo.androidagent.core.ThingManager;

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

    // Brightness sensor
    private static Sensor mBrightnessSensor;
    private static float mBrightnessLast = 0;
    private static long mBrightnessTS = 0;

    // Rotation Sensor
    private static Sensor mRotationSensor;
    private final double[] mRotationVector = new double[3];

    // Gyroscope
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
            // Get an instance of the sensor service, and use that to generateFetcher an instance of a particular sensor.
            mSensorManager = (SensorManager) context.getSystemService(Context.SENSOR_SERVICE);

            // Init Accel sensor
            mAccel = 0.00f;
            mAccelCurrent = SensorManager.GRAVITY_EARTH;
            mAccelLast = SensorManager.GRAVITY_EARTH;

            // Rx message
            BroadcastReceiver mMsgRx = new BroadcastReceiver() {
                @Override
                public void onReceive(Context context, Intent intent) {
                    String action = intent.getAction();
                    Helpers.log(Log.INFO, TAG, "Broadcast received: " + action);

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
                                    if (thingKey.equals(thingManager.GetThingKey(MainActivity.Accelerometer)) || thingKey.equals(thingManager.GetThingKey(MainActivity.ShakeDetector))) {
                                        StartService(SensorType.Accelerometer);
                                    } else if (thingKey.equals(thingManager.GetThingKey(MainActivity.BrightnessSensor))) {
                                        StartService(SensorType.Brightness);
                                    } else if (thingKey.equals(thingManager.GetThingKey(MainActivity.Proximity))) {
                                        StartService(SensorType.Proximity);
                                    } else if (thingKey.equals(thingManager.GetThingKey(MainActivity.Gyroscope))) {
                                        StartService(SensorType.Gyroscope);
                                    } else if (thingKey.equals(thingManager.GetThingKey(MainActivity.Rotation))) {
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

            LocalBroadcastManager.getInstance(context).registerReceiver(mMsgRx,
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
                NodeService.SendPortMsg(context, MainActivity.ShakeDetector, MainActivity.ShakeDetectorPort, "True");
                mLastAccelTS = timestamp;
                //Toast.makeText(context, "Device has shaken", Toast.LENGTH_SHORT).show();
            }

            // send accelerometer data to node service
            if (Math.abs(mAccel) > 0.9f) {

                NodeService.SendPortMsg(context, MainActivity.Accelerometer,
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

                NodeService.SendPortMsg(context, MainActivity.Proximity, MainActivity.ProximityPort, "True");
                NodeService.SendPortMsg(context, MainActivity.Proximity, MainActivity.ProximityPort, "False");
                Helpers.log(Log.DEBUG, TAG, "Proximity sensor fired");
            }

        } else if (sensorEvent.sensor.getType() == Sensor.TYPE_LIGHT) {
            float brightness = sensorEvent.values[0];

            // filter out too frequent readings
            // and send data to node service
            if (timestamp - mBrightnessTS > 2 * NANOSEC_IN_SEC) {
                if (Math.abs(brightness - mBrightnessLast) > Math.abs(brightness / 5)) {
                    mBrightnessTS = timestamp;
                    NodeService.SendPortMsg(context,
                            MainActivity.BrightnessSensor,
                            MainActivity.BrightnessSensorPort,
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

                // Normalize the rotation vector if it's big enough to generateFetcher the axis
                // (that is, EPSILON should represent your maximum allowable margin of error)
                if (omegaMagnitude > 0.00001) {
                    axisX /= omegaMagnitude;
                    axisY /= omegaMagnitude;
                    axisZ /= omegaMagnitude;
                }

                // Integrate around this axis with the angular speed by the timestep
                // in order to generateFetcher a delta rotation from this sample over the timestep
                // We will convert this axis-angle representation of the delta rotation
                // into a quaternion before turning it into the rotation matrix.
                float thetaOverTwo = omegaMagnitude * dT / 2.0f;
                double sinThetaOverTwo = Math.sin(thetaOverTwo);
                double cosThetaOverTwo = Math.cos(thetaOverTwo);
                deltaRotationVector[0] = (float) sinThetaOverTwo * axisX;
                deltaRotationVector[1] = (float) sinThetaOverTwo * axisY;
                deltaRotationVector[2] = (float) sinThetaOverTwo * axisZ;
                deltaRotationVector[3] = (float) cosThetaOverTwo;

                NodeService.SendPortMsg(context, MainActivity.Gyroscope,
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

                NodeService.SendPortMsg(context, MainActivity.Rotation,
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
                mAccelSensor = mSensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER);
                if (mAccelSensor != null)
                    mSensorManager.registerListener(this, mAccelSensor, SensorManager.SENSOR_DELAY_NORMAL);
                break;
            case Rotation:
                mRotationSensor = mSensorManager.getDefaultSensor(Sensor.TYPE_ROTATION_VECTOR);
                if (mRotationSensor != null)
                    mSensorManager.registerListener(this, mRotationSensor, SensorManager.SENSOR_DELAY_NORMAL);
                break;
            case Gyroscope:
                mGyroSensor = mSensorManager.getDefaultSensor(Sensor.TYPE_GYROSCOPE);
                if (mGyroSensor != null)
                    mSensorManager.registerListener(this, mGyroSensor, SensorManager.SENSOR_DELAY_NORMAL);
                break;
            case Brightness:
                mBrightnessSensor = mSensorManager.getDefaultSensor(Sensor.TYPE_LIGHT);
                if (mBrightnessSensor != null)
                    mSensorManager.registerListener(this, mBrightnessSensor, SensorManager.SENSOR_DELAY_NORMAL);
                break;
            case Proximity:
                mProximitySensor = mSensorManager.getDefaultSensor(Sensor.TYPE_PROXIMITY);
                if (mProximitySensor != null)
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

        // broadcast event
        Intent intent = new Intent(MainActivity.BROADCAST_NEBIT_TEARDOWN);
        intent.putExtra(MainActivity.EXTRA_TEARDOWN_INFO, TAG);
        LocalBroadcastManager
                .getInstance(context)
                .sendBroadcast(intent);
    }

    public void Start() {
        // Force active key update
        NodeService.ForceActiveKeyUpdate(context);
    }

    // =============================================================================================
}
