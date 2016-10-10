package com.yodiwo.androidnode.core;

import android.content.Context;
import android.content.Intent;
import android.location.Address;
import android.location.Criteria;
import android.location.Geocoder;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.location.LocationProvider;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;

import com.yodiwo.androidagent.core.Helpers;
import com.yodiwo.androidagent.core.NodeService;
import com.yodiwo.androidagent.core.SettingsProvider;
import com.yodiwo.androidagent.plegma.ConfigParameter;
import com.yodiwo.androidagent.plegma.Thing;
import com.yodiwo.androidagent.plegma.ThingKey;

import java.io.IOException;
import java.util.List;
import java.util.Locale;

/**
 * Created by vaskanas on 07-Jul-16.
 */
public class LocationHandler implements
        LocationListener {

    // =============================================================================================
    // Variables
    // =============================================================================================

    public static final String TAG = LocationHandler.class.getSimpleName();

    private static LocationManager locationManager;
    private static String bestGPSProvider;
    public static boolean isSetLocationListener = false;

    private static final int TWO_MINUTES = 1000 * 60 * 2;
    private static final int REVERSEGEOCODING_RESULT_SUCCESS = 1;
    private static final String[] S = {"Out of Service", "Temporarily Unavailable", "Available"};

    // Keep local global entrypoint for any request
    private static LocationHandler locHandler = null;

    /**
     * generateFetcher the instance of the locationHandler
     * if for some reason the instance is not valid create a new one.
      */
    public static LocationHandler getInstance(Context context) {
        if (locHandler == null) {
            locHandler = new LocationHandler(context.getApplicationContext());
        }
        return locHandler;
    }

    // link to the application context that we have the instance of the db
    private Context context;

    // =============================================================================================
    // Constructor
    // =============================================================================================

    private LocationHandler(Context context) {
        this.context = context;
        locationManager = (LocationManager) context.getSystemService(Context.LOCATION_SERVICE);
    }

    // =============================================================================================
    // Location Callbacks
    // =============================================================================================

    @Override
    public void onLocationChanged(Location location) {
        //send this location to the cloud
        SendNewLocation(location);
    }

    @Override
    public void onStatusChanged(String provider, int status, Bundle extras) {
        Helpers.log(Log.DEBUG, TAG, "GPS Provider Status Changed: " + provider + ", Status="
                + S[status] + ", Extras=" + extras);
    }

    @Override
    public void onProviderEnabled(String provider) {
        Helpers.log(Log.DEBUG, TAG, "GPS Provider Enabled: " + provider);
        setLocationListener();
    }

    @Override
    public void onProviderDisabled(String provider) {
        Helpers.log(Log.DEBUG, TAG, "GPS Provider Disabled: " + provider);
    }

    // =============================================================================================
    // Helpers
    // =============================================================================================

    /**
     * set listener
     */
    public void setLocationListener() {
        String minDist = String.valueOf(MainActivity.GPSBetweenUpdatesDistanceValue);
        String minTime = String.valueOf(MainActivity.GPSBetweenUpdatesIntervalValue);

        bestGPSProvider = getBestGPSProviderForChosenCriteria();

        try {
            if (locationManager != null) {
                String thingKey = ThingKey.CreateKey(SettingsProvider.getInstance(context).getNodeKey(), MainActivity.GPS);
                Thing thing = NodeService.getThing(thingKey);
                if (thing != null) {
                    for (ConfigParameter conf : thing.Config) {
                        if (conf.Name.equals(MainActivity.GPSBetweenUpdatesDistance))
                            minDist = conf.Value;
                        else if (conf.Name.equals(MainActivity.GPSBetweenUpdatesInterval))
                            minTime = conf.Value;
                    }
                }
                locationManager.requestLocationUpdates(bestGPSProvider, Integer.valueOf(minTime), Integer.valueOf(minDist), this);
                isSetLocationListener = true;
                Helpers.log(Log.DEBUG, TAG, "Location listener set");
            }
            else
                Helpers.log(Log.ERROR, TAG, "entered onProviderEnabled with locationManager unset");
        } catch (SecurityException e) {
            Helpers.logException(TAG, e);
        }
    }

    /**
     * unset listener
     */
    public void unsetLocationListener() {
        try {
            if (locationManager != null && isSetLocationListener) {
                locationManager.removeUpdates(this);
                isSetLocationListener = false;
                Helpers.log(Log.DEBUG, TAG, "Location listener set");
            }
            else
                Helpers.log(Log.ERROR, TAG, "entered unsetLocationListener with locationManager unset");
        } catch (SecurityException e) {
            Helpers.logException(TAG, e);
        } finally {
            // broadcast event
            Intent intent = new Intent(MainActivity.BROADCAST_NEBIT_TEARDOWN);
            intent.putExtra(MainActivity.EXTRA_TEARDOWN_INFO, TAG);
            LocalBroadcastManager
                    .getInstance(context)
                    .sendBroadcast(intent);
        }
    }

    /**
     * Will either send incoming location or grab last known one
     */
    public void SendNewLocation(Location location) {
        Location currentBestLocation = null;

        try {
            if (locationManager != null)
                currentBestLocation = locationManager.getLastKnownLocation(bestGPSProvider);
        } catch (SecurityException e){
            Helpers.logException(TAG, e);
        }

        try {
            if (location == null)
                location = currentBestLocation;
        } catch (Exception e) {
            Helpers.logException(TAG, e);
            location = null;
        }

        if (location == null) {
            Helpers.log(Log.ERROR, TAG, "No location could be retrieved");
        }
        else {
            if (isBetterLocation(location, currentBestLocation))
                location = currentBestLocation;

            if (location != null) {
                Helpers.log(Log.DEBUG, TAG, "GPS Locations (starting with last known):" + location.toString());
                Double latitude = location.getLatitude();
                Double longitude = location.getLongitude();
                getAddressFromLocation(latitude, longitude, context, new ReverseGeocodingHandler());
            }
        }
    }

    /**
     * Determines whether one Location reading is better than the current Location fix (currentBestLocation)
     * @param location  The new Location that you want to evaluate
     * @param currentBestLocation  The current Location fix, to which you want to compare the new one
     */
    protected boolean isBetterLocation(Location location, Location currentBestLocation) {
        if (currentBestLocation == null) {
            // A new location is always better than no location
            return true;
        }

        // Check whether the new location fix is newer or older
        long timeDelta = location.getTime() - currentBestLocation.getTime();
        boolean isSignificantlyNewer = timeDelta > TWO_MINUTES;
        boolean isSignificantlyOlder = timeDelta < -TWO_MINUTES;
        boolean isNewer = timeDelta > 0;

        // If it's been more than two minutes since the current location, use the new location
        // because the user has likely moved
        if (isSignificantlyNewer) {
            return true;
            // If the new location is more than two minutes older, it must be worse
        } else if (isSignificantlyOlder) {
            return false;
        }

        // Check whether the new location fix is more or less accurate
        int accuracyDelta = (int) (location.getAccuracy() - currentBestLocation.getAccuracy());
        boolean isLessAccurate = accuracyDelta > 0;
        boolean isMoreAccurate = accuracyDelta < 0;
        boolean isSignificantlyLessAccurate = accuracyDelta > 200;

        // Check if the old and new location are from the same provider
        boolean isFromSameProvider = isSameProvider(location.getProvider(),
                currentBestLocation.getProvider());

        // Determine location quality using a combination of timeliness and accuracy
        if (isMoreAccurate) {
            return true;
        } else if (isNewer && !isLessAccurate) {
            return true;
        } else if (isNewer && !isSignificantlyLessAccurate && isFromSameProvider) {
            return true;
        }
        return false;
    }

    /**
     * Checks whether two providers are the same
     */
    private boolean isSameProvider(String provider1, String provider2) {
        if (provider1 == null) {
            return provider2 == null;
        }
        return provider1.equals(provider2);
    }

    /**
     * Reverse geocoding related private members
     */
    private static void getAddressFromLocation(final double latitude, final double longitude,
                                               final Context context, final Handler cbHandler) {

        Thread thread = new Thread() {

            @Override
            public void run() {
                Geocoder geocoder = new Geocoder(context, Locale.getDefault());
                try {
                    List<Address> addressList = geocoder.getFromLocation(latitude, longitude, 1);
                    if (addressList != null && addressList.size() > 0) {
                        Address address = addressList.get(0);

                        // Construct message
                        Message message = Message.obtain();
                        message.setTarget(cbHandler);
                        message.what = REVERSEGEOCODING_RESULT_SUCCESS;
                        Bundle bundle = new Bundle();
                        bundle.putDouble("latitude", latitude);
                        bundle.putDouble("longitude", longitude);
                        bundle.putString("address", address.getThoroughfare());
                        bundle.putString("country", address.getCountryName());
                        bundle.putString("postal", address.getPostalCode());
                        message.setData(bundle);

                        // Send message to handler
                        message.sendToTarget();
                    }
                } catch (IOException e) {
                    Helpers.logException(TAG, e);
                }
            }
        };

        thread.start();
    }

    private class ReverseGeocodingHandler extends Handler {

        @Override
        public void handleMessage(Message message) {
            switch (message.what) {
                case REVERSEGEOCODING_RESULT_SUCCESS: {
                    Bundle bundle = message.getData();

                    // Notify NodeService
                    NodeService.SendPortMsg(context,
                            MainActivity.GPS,
                            new String[]{Double.toString(bundle.getDouble("latitude")),
                                    Double.toString(bundle.getDouble("longitude")),
                                    bundle.getString("address"),
                                    bundle.getString("country"),
                                    bundle.getString("postal")});

                    break;
                }
                default: {
                    break;
                }
            }
        }
    }

    private String getBestGPSProviderForChosenCriteria() {

        if (locationManager == null) {
            Helpers.log(Log.ERROR, TAG, "Location requested, but locationManager is null");
            return null;
        }
        // List all providers (for debug purposes)
        /*
        try {
            List<String> providers = locationManager.getAllProviders();
            for (String provider : providers) {
                LocationProvider info = locationManager.getProvider(provider);
                Helpers.log(Log.DEBUG, TAG, "GPS provider:" + info.toString());
            }
        }
        catch (SecurityException e) {
            Helpers.log(Log.ERROR, TAG, "Probably need more permissions");
        }
        */
        String provider = null;
        try {
            String thingKey = ThingKey.CreateKey(SettingsProvider.getInstance(context).getNodeKey(), MainActivity.GPS);
            Thing thing = NodeService.getThing(thingKey);
            Criteria criteria = new Criteria();
            if (thing != null) {
                for (ConfigParameter conf : thing.Config) {
                    // gps accuracy configuration
                    if (conf.Name.equals(MainActivity.GPSAccuracy)) {
                        switch (conf.Value) {
                            case MainActivity.LowValue:
                                criteria.setAccuracy(Criteria.ACCURACY_LOW);
                                break;
                            case MainActivity.MediumValue:
                                criteria.setAccuracy(Criteria.ACCURACY_MEDIUM);
                                break;
                            case MainActivity.HighValue:
                                criteria.setAccuracy(Criteria.ACCURACY_HIGH);
                                break;
                            case MainActivity.CoarseValue:
                                criteria.setAccuracy(Criteria.ACCURACY_COARSE);
                                break;
                            case MainActivity.FineValue:
                                criteria.setAccuracy(Criteria.ACCURACY_FINE);
                                break;
                            default:
                                criteria.setAccuracy(Criteria.ACCURACY_FINE);
                                break;
                        }
                    }
                    // gps power requirements configuration
                    else if (conf.Name.equals(MainActivity.GPSPowerRequirements)){
                        switch (conf.Value) {
                            case MainActivity.LowValue:
                                criteria.setPowerRequirement(Criteria.POWER_LOW);
                                break;
                            case MainActivity.MediumValue:
                                criteria.setPowerRequirement(Criteria.POWER_MEDIUM);
                                break;
                            case MainActivity.HighValue:
                                criteria.setPowerRequirement(Criteria.POWER_HIGH);
                                break;
                            default:
                                criteria.setPowerRequirement(Criteria.POWER_LOW);
                                break;
                        }
                    }
                }
            }
            else {
                criteria.setPowerRequirement(Criteria.POWER_LOW);
                criteria.setAccuracy(Criteria.ACCURACY_MEDIUM);
            }

            provider = locationManager.getBestProvider(criteria, false);
            LocationProvider info = locationManager.getProvider(provider);
            Helpers.log(Log.DEBUG, TAG, "GPS best provider: " + info.toString());

        } catch (Exception e) {
            Helpers.logException(TAG, e);
        }

        return provider;
    }

}
