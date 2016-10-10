package com.yodiwo.androidnode.core;

import android.os.RemoteException;
import android.support.multidex.MultiDexApplication;
import android.util.Log;

import com.yodiwo.androidagent.core.Helpers;
import com.yodiwo.androidagent.core.NodeService;
import com.yodiwo.androidagent.core.SettingsProvider;
import com.yodiwo.androidagent.plegma.ConfigParameter;
import com.yodiwo.androidagent.plegma.Thing;
import com.yodiwo.androidagent.plegma.ThingKey;
import com.yodiwo.androidnode.R;

import org.altbeacon.beacon.Beacon;
import org.altbeacon.beacon.BeaconConsumer;
import org.altbeacon.beacon.BeaconManager;
import org.altbeacon.beacon.BeaconParser;
import org.altbeacon.beacon.RangeNotifier;
import org.altbeacon.beacon.Region;

import java.util.ArrayList;
import java.util.Collection;

/**
 * Created by vaskanas on 09-Mar-16.
 */
public class BeaconHandler extends MultiDexApplication implements BeaconConsumer {

    // =============================================================================================
    // Variables
    // =============================================================================================

    private static final String TAG = BeaconHandler.class.getSimpleName();

    private static final String SPECIFIC_BEACON_REGION = "NeBiTRegion";

    /**
     * https://github.com/google/eddystone/blob/master/protocol-specification.md
     */
    private BeaconManager beaconManager;
    private SettingsProvider settingsProvider;
    private int scanPeriod = MainActivity.BleBeaconScanPeriodValue;
    private int betweenScanPeriod = MainActivity.BleBeaconBetweenScanPeriodValue;
    private Region region = null;
    private int serviceUuidFilter;
    private int typeCodeFilter;
    private boolean detectAll = false;
    public static boolean isBeaconServiceRunning = false;

    // =============================================================================================
    // Basic functions
    // =============================================================================================

    @Override
    public void onCreate() {
        super.onCreate();
        Helpers.log(Log.DEBUG, TAG, "Beacon service started");
        isBeaconServiceRunning = true;

        try {
            beaconManager = BeaconManager.getInstanceForApplication(this);
        }catch (Exception ex){
            Helpers.logException(TAG, ex);
        }
        settingsProvider = SettingsProvider.getInstance(this);

        /**
         * initialize beaconManager to detect all beacons
         */
        beaconManager.getBeaconParsers().clear();
        // iBeacons
        beaconManager.getBeaconParsers().add(new BeaconParser().
                setBeaconLayout(BeaconParser.URI_BEACON_LAYOUT));
        // Eddystone-UID frame:
        beaconManager.getBeaconParsers().add(new BeaconParser().
                setBeaconLayout(BeaconParser.EDDYSTONE_UID_LAYOUT));
        // Eddystone-TLM frame:
        beaconManager.getBeaconParsers().add(new BeaconParser().
                setBeaconLayout(BeaconParser.EDDYSTONE_TLM_LAYOUT));
        // Eddystone-URL frame:
        beaconManager.getBeaconParsers().add(new BeaconParser().
                setBeaconLayout(BeaconParser.EDDYSTONE_URL_LAYOUT));
        // altBeacon
        beaconManager.getBeaconParsers().add(new BeaconParser().
                setBeaconLayout(BeaconParser.ALTBEACON_LAYOUT));

        /**
         * set default scan periods
         */
        beaconManager.setBackgroundScanPeriod(scanPeriod);
        beaconManager.setBackgroundBetweenScanPeriod(betweenScanPeriod);
        beaconManager.setBackgroundMode(true);

        /**
         * bind to beacon service
         */
        beaconManager.bind(this);
        if (beaconManager.isBound(this)) {
            Helpers.log(Log.DEBUG, TAG, "Beacon service is bound");
            // TODO: handling power saving
                    /*
                    beaconManager.setBackgroundMode(true);
                    Helpers.log(Log.DEBUG, TAG, "setting up background monitoring for beacons and power saving");

                    // wake up the app when a beacon is seen with the below specified criteria
                    Region region = new Region("backgroundRegion",
                            null, null, null);
                    regionBootstrap = new RegionBootstrap(this, region);

                    // simply constructing this class and holding a reference to it in your custom Application
                    // class will automatically cause the BeaconLibrary to save battery whenever the application
                    // is not visible.  This reduces bluetooth power usage by about 60%
                    backgroundPowerSaver = new BackgroundPowerSaver(this);
                 */
        }
        else
            Helpers.log(Log.DEBUG, TAG, "Beacon service cannot be bound");
    }

    public void onTerminate(){
        super.onTerminate();
        try {
            if (region != null) {
                beaconManager.stopRangingBeaconsInRegion(region);
                Helpers.log(Log.DEBUG, TAG, "Beacon service stops ranging");
            }
        }catch(RemoteException ex){
            Helpers.logException(TAG, ex);
        }
        beaconManager.unbind(this);
        isBeaconServiceRunning = false;
        Helpers.log(Log.DEBUG, TAG, "Beacon service has been unbound");
    }

    // =============================================================================================
    // BeaconConsumer Overrides
    // =============================================================================================

    @Override
    public void onBeaconServiceConnect() {
        region = new Region(SPECIFIC_BEACON_REGION, null, null, null);
        Helpers.log(Log.DEBUG, TAG, "onBeaconServiceConnect");

        /**
         * define range notifier
         */
        beaconManager.setRangeNotifier(new RangeNotifier() {
            @Override
            public void didRangeBeaconsInRegion(Collection<Beacon> beacons, Region region) {

                // return if not connected/paired
                if (settingsProvider.getNodeKey() == null && settingsProvider.getNodeSecretKey() == null)
                    return;

                String thingKey = ThingKey.CreateKey(settingsProvider.getNodeKey(), MainActivity.BleBeacon);
                Thing thing = NodeService.getThing(thingKey);

                // return if thing not registered
                if (thing == null)
                    return;

                /**
                 * user defined configuration
                 */
                setFilters(thing.Config);

                // update beacon scan periods
                beaconManager.setBackgroundScanPeriod(scanPeriod);
                beaconManager.setBackgroundBetweenScanPeriod(betweenScanPeriod);

                /**
                 * send port event message
                 */
                try {
                    for (Beacon beacon : beacons) {
                        Helpers.log(Log.DEBUG, TAG, "Beacon detected UUID: " + beacon.getId1().toString());
                        Boolean toSend;
                        toSend = detectAll ||
                                !detectAll && beacon.getServiceUuid() == serviceUuidFilter && beacon.getBeaconTypeCode() == typeCodeFilter;

                        if (toSend && NodeService.isActive(thing.Ports)) {
                            NodeService.SendPortMsg(getApplicationContext(), MainActivity.BleBeacon, createPortMsg(beacon));
                            Helpers.log(Log.DEBUG, TAG, "Send to cloud beacon UUID: " + beacon.getId1().toString());
                        }

                    }
                }catch (Exception ex){
                    Helpers.logException(TAG, ex);
                }
            }
        });

        /**
         * start ranging/scanning
         */
        try {
            beaconManager.startRangingBeaconsInRegion(region);
            // beaconManager.startRangingBeaconsInRegion(new Region(region, Identifier.parse("2F234454-CF6D-4A0F-ADF2-F4911BA9FFA6"), Identifier.parse("1"), null));
            // This sample ranges all beacons matching id1=2F234454-CF6D-4A0F-ADF2-F4911BA9FFA6, id2=1, id3=*
        }
        catch (RemoteException e) {
            Helpers.logException(TAG, e);
        }
    }

    // =============================================================================================
    // Helpers
    // =============================================================================================
    private void setFilters(ArrayList<ConfigParameter> configParameters){
        for (ConfigParameter conf : configParameters) {
            // beacon type
            switch (conf.Name) {
                case MainActivity.BleBeaconType:
                    if (conf.Value.equalsIgnoreCase(MainActivity.iBeacon)) {
                        typeCodeFilter = 0x0215;
                        serviceUuidFilter = -1;
                        detectAll = false;
                    } else if (conf.Value.equalsIgnoreCase(MainActivity.EddystoneUIDBeacon)) {
                        serviceUuidFilter = 0xfeaa;
                        typeCodeFilter = 0x00;
                        detectAll = false;
                    } else if (conf.Value.equalsIgnoreCase(MainActivity.EddystoneTLMBeacon)) {
                        serviceUuidFilter = 0xfeaa;
                        typeCodeFilter = 0x20;
                        detectAll = false;
                    } else if (conf.Value.equalsIgnoreCase(MainActivity.EddystoneURLBeacon)) {
                        serviceUuidFilter = 0xfeaa;
                        typeCodeFilter = 0x10;
                        detectAll = false;
                    } else if (conf.Value.equalsIgnoreCase(MainActivity.EddystoneEIDBeacon)) {
                        typeCodeFilter = 0xfeaa;
                        serviceUuidFilter = 0x30;
                        detectAll = false;
                    }else if (conf.Value.equalsIgnoreCase(MainActivity.AltBeacon)) {
                        typeCodeFilter = 0xbeac;
                        serviceUuidFilter = -1;
                        detectAll = false;
                    }else if (conf.Value.equalsIgnoreCase(MainActivity.AllBeacons)){
                        detectAll = true;
                    }
                    else {
                        typeCodeFilter = 0x0215;
                        serviceUuidFilter = -1;
                    }
                    break;
                // beacon scan period configuration
                case MainActivity.BleBeaconScanPeriod:
                    if (!conf.Value.equals(""))
                        scanPeriod = Integer.parseInt(conf.Value);
                    break;
                // beacon between scan period configuration
                case MainActivity.BleBeaconBetweenScanPeriod:
                    if (!conf.Value.equals(""))
                        betweenScanPeriod = Integer.parseInt(conf.Value);
                    break;
            }
        }
    }

    private String[] createPortMsg(Beacon beacon) {
        String[] msg;
        // TODO: how to handle Eddystone-URL frame?
        // String url = UrlBeaconUrlCompressor.uncompress(beacon.getId1().toByteArray());
        if (beacon.getIdentifiers().size() == 1) {
            msg = new String[]{
                    beacon.getId1().toString(),
                    getString(R.string.not_supported_beacon_type),
                    getString(R.string.not_supported_beacon_type),
                    String.valueOf(beacon.getTxPower()),
                    String.valueOf(beacon.getDistance()),
                    String.valueOf(beacon.getRssi()),
                    String.valueOf(beacon.getBeaconTypeCode())
            };
        } else if (beacon.getIdentifiers().size() == 2) {
            msg = new String[]{
                    beacon.getId1().toString(),
                    beacon.getId2().toString(),
                    getString(R.string.not_supported_beacon_type),
                    String.valueOf(beacon.getTxPower()),
                    String.valueOf(beacon.getDistance()),
                    String.valueOf(beacon.getRssi()),
                    String.valueOf(beacon.getBeaconTypeCode())
            };
        } else {
            msg = new String[]{
                    beacon.getId1().toString(),
                    beacon.getId2().toString(),
                    beacon.getId3().toString(),
                    String.valueOf(beacon.getTxPower()),
                    String.valueOf(beacon.getDistance()),
                    String.valueOf(beacon.getRssi()),
                    String.valueOf(beacon.getBeaconTypeCode())
            };
        }
        return msg;
    }
}
