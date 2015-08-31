package com.yodiwo.virtualgateway;

import android.app.Activity;
import android.app.PendingIntent;
import android.content.Intent;
import android.content.IntentFilter;
import android.location.Criteria;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.location.LocationProvider;
import android.nfc.NdefMessage;
import android.nfc.NdefRecord;
import android.nfc.NfcAdapter;
import android.nfc.Tag;
import android.nfc.tech.Ndef;
import android.os.AsyncTask;
import android.os.Bundle;
import android.support.v7.app.ActionBarActivity;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Toast;

import java.io.UnsupportedEncodingException;
import java.util.Arrays;
import java.util.List;


public class MainActivity extends ActionBarActivity implements LocationListener {

    private final static String TAG = MainActivity.class.getSimpleName();

    private SettingsProvider settingsProvider = null;
    private NfcAdapter mNfcAdapter = null;

    public static final String MIME_TEXT_PLAIN = "text/plain";

    private LocationManager locationManager;
    private String bestGPSProvider;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        // Check if we are pair the device
        if (settingsProvider == null)
            settingsProvider = SettingsProvider.getInstance(this);

        if (settingsProvider.getNodeKey() == null || settingsProvider.getNodeSecretKey() == null) {
            // We are not paired yet, start pairing activity
            Intent intent = new Intent(this, PairingActivity.class);
            startActivity(intent);
        } else {
            // Check for nfc
            mNfcAdapter = NfcAdapter.getDefaultAdapter(this);
            if (mNfcAdapter == null) {
                Toast.makeText(this, "This device doesn't support NFC.", Toast.LENGTH_LONG).show();
            } else {
                if (!mNfcAdapter.isEnabled()) {
                    Toast.makeText(this, "NFC is disabled.", Toast.LENGTH_LONG).show();
                } else {
                    Toast.makeText(this, "NFC is enable.", Toast.LENGTH_LONG).show();
                }
            }



            // Get the location manager
            locationManager = (LocationManager) getSystemService(LOCATION_SERVICE);

            // List all providers:
            List<String> providers = locationManager.getAllProviders();
            for (String provider : providers) {
                LocationProvider info = locationManager.getProvider(provider);
                Log.d(TAG, "GPS:" + info.toString());
            }

            Criteria criteria = new Criteria();
            bestGPSProvider = locationManager.getBestProvider(criteria, false);
            LocationProvider info = locationManager.getProvider(bestGPSProvider);
            Log.d(TAG, "GPS BEST Provider:" + info.toString());

            Location location = locationManager.getLastKnownLocation(bestGPSProvider);
            if (location == null)
                Log.d(TAG, "GPS Locations (starting with last known): [unknown]\n\n");
            else
                Log.d(TAG, "GPS Locations (starting with last known):" + location.toString());
        }
    }

    // ---------------------------------------------------------------------------------------------

    @Override
    public void onResume() {
        super.onResume();

        if (settingsProvider.getNodeKey() == null || settingsProvider.getNodeSecretKey() == null) {
            // TODO: Show message that we are not paired
        } else {
            NodeService.RegisterNode(this, false);

            // Start rx for things
            NodeService.StartRx(this);

            // Get from defines what we need to enable
            NodeService.StartService(this, SensorsListener.SensorType.Accelerometer);
            NodeService.StartService(this, SensorsListener.SensorType.Brightness);

            // Resume NFC
            setupForegroundNFCDispatch(this, mNfcAdapter);

            // Request update location
            if(locationManager!=null)
                locationManager.requestLocationUpdates(bestGPSProvider, 20000, 1, this);

            // Request the state of the things in the cloud
            NodeService.RequesttUpdatedState(this);
        }
    }

    // ---------------------------------------------------------------------------------------------

    @Override
    public void onPause() {

        // Call this before onPause, otherwise an IllegalArgumentException is thrown as well.
        if(mNfcAdapter!=null)
            stopForegroundNFCDispatch(this, mNfcAdapter);

        if (!(settingsProvider.getNodeKey() == null || settingsProvider.getNodeSecretKey() == null)) {
            // Get from defines what we need to enable
            NodeService.StopService(this, SensorsListener.SensorType.Accelerometer);
            NodeService.StopService(this, SensorsListener.SensorType.Brightness);


            // Start rx for things
            NodeService.StopRx(this);
        }

        super.onPause();


        if (!(settingsProvider.getNodeKey() == null || settingsProvider.getNodeSecretKey() == null)) {
            locationManager.removeUpdates(this);
        }
    }

    // ---------------------------------------------------------------------------------------------

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_main, menu);
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
        switch (id) {
            // Start settings activity
            case R.id.action_settings:
                intent = new Intent(this, SettingsActivity.class);
                break;
            // Start Pairing activity
            case R.id.action_repairing:
                intent = new Intent(this, PairingActivity.class);
                break;
        }

        if (intent != null) {
            startActivity(intent);
            return true;
        }

        return super.onOptionsItemSelected(item);
    }

    // ---------------------------------------------------------------------------------------------

    @Override
    protected void onNewIntent(Intent intent) {
        handleNFCIntent(intent);
    }

    // =============================================================================================
    // NFC

    /**
     * @param activity The corresponding {@link Activity} requesting the foreground dispatch.
     * @param adapter  The {@link NfcAdapter} used for the foreground dispatch.
     */
    public static void setupForegroundNFCDispatch(final Activity activity, NfcAdapter adapter) {
        if(adapter!=null) {
            final Intent intent = new Intent(activity.getApplicationContext(), activity.getClass());
            intent.setFlags(Intent.FLAG_ACTIVITY_SINGLE_TOP);

            final PendingIntent pendingIntent = PendingIntent.getActivity(activity.getApplicationContext(), 0, intent, 0);

            IntentFilter[] filters = new IntentFilter[1];
            String[][] techList = new String[][]{};

            // Notice that this is the same filter as in our manifest.
            filters[0] = new IntentFilter();
            filters[0].addAction(NfcAdapter.ACTION_NDEF_DISCOVERED);
            filters[0].addCategory(Intent.CATEGORY_DEFAULT);
            try {
                filters[0].addDataType(MIME_TEXT_PLAIN);
            } catch (IntentFilter.MalformedMimeTypeException e) {
                throw new RuntimeException("Check your mime type.");
            }

            adapter.enableForegroundDispatch(activity, pendingIntent, filters, techList);
        }
    }

    // ---------------------------------------------------------------------------------------------

    /**
     * @param activity The corresponding requesting to stop the foreground dispatch.
     * @param adapter  The {@link NfcAdapter} used for the foreground dispatch.
     */
    public static void stopForegroundNFCDispatch(final Activity activity, NfcAdapter adapter) {
        if(adapter!=null)
            adapter.disableForegroundDispatch(activity);
    }

    // ---------------------------------------------------------------------------------------------

    private void handleNFCIntent(Intent intent) {
        String action = intent.getAction();
        if (NfcAdapter.ACTION_NDEF_DISCOVERED.equals(action)) {

            String type = intent.getType();
            if (MIME_TEXT_PLAIN.equals(type)) {

                Tag tag = intent.getParcelableExtra(NfcAdapter.EXTRA_TAG);
                new NdefReaderTask().execute(tag);

            } else {
                Log.d(TAG, "Wrong mime type: " + type);
            }
        } else if (NfcAdapter.ACTION_TECH_DISCOVERED.equals(action)) {

            // In case we would still use the Tech Discovered Intent
            Tag tag = intent.getParcelableExtra(NfcAdapter.EXTRA_TAG);
            String[] techList = tag.getTechList();
            String searchedTech = Ndef.class.getName();

            for (String tech : techList) {
                if (searchedTech.equals(tech)) {
                    new NdefReaderTask().execute(tag);
                    break;
                }
            }
        }
    }
    // ---------------------------------------------------------------------------------------------

    /**
     * Background task for reading the data. Do not block the UI thread while reading.
     *
     * @author Ralf Wondratschek
     */
    private class NdefReaderTask extends AsyncTask<Tag, Void, String> {

        @Override
        protected String doInBackground(Tag... params) {
            Tag tag = params[0];

            Ndef ndef = Ndef.get(tag);
            if (ndef == null) {
                // NDEF is not supported by this Tag.
                return null;
            }

            NdefMessage ndefMessage = ndef.getCachedNdefMessage();

            NdefRecord[] records = ndefMessage.getRecords();
            for (NdefRecord ndefRecord : records) {
                if (ndefRecord.getTnf() == NdefRecord.TNF_WELL_KNOWN && Arrays.equals(ndefRecord.getType(), NdefRecord.RTD_TEXT)) {
                    try {
                        return readText(ndefRecord);
                    } catch (UnsupportedEncodingException e) {
                        Log.e(TAG, "Unsupported Encoding", e);
                    }
                }
            }

            return null;
        }

        private String readText(NdefRecord record) throws UnsupportedEncodingException {
        /*
         * See NFC forum specification for "Text Record Type Definition" at 3.2.1
         *
         * http://www.nfc-forum.org/specs/
         *
         * bit_7 defines encoding
         * bit_6 reserved for future use, must be 0
         * bit_5..0 length of IANA language code
         */

            byte[] payload = record.getPayload();

            // Get the Text Encoding
            String textEncoding = ((payload[0] & 128) == 0) ? "UTF-8" : "UTF-16";

            // Get the Language Code
            int languageCodeLength = payload[0] & 0063;

            // String languageCode = new String(payload, 1, languageCodeLength, "US-ASCII");
            // e.g. "en"

            // Get the Text
            return new String(payload, languageCodeLength + 1, payload.length - languageCodeLength - 1, textEncoding);
        }

        @Override
        protected void onPostExecute(String result) {
            if (result != null) {
                Log.e(TAG, "NFC:" + result);
                NodeService.SendPortMsg(getApplicationContext(),
                        ThingManager.OutputNFC,
                        ThingManager.OutputNFCPort,
                        result);
            }
        }
    }


    // =============================================================================================
    // GPS

    public  void onSendGPS(View view)
    {

    }

    private static final String[] S = { "Out of Service",
            "Temporarily Unavailable", "Available" };


    @Override
    public void onLocationChanged(Location location) {

        location = locationManager.getLastKnownLocation(bestGPSProvider);
        if (location == null)
            Log.d(TAG, "GPS Locations (starting with last known): [unknown]\n\n");
        else
            Log.d(TAG, "GPS Locations (starting with last known):" + location.toString());
    }

    @Override
    public void onStatusChanged(String provider, int status, Bundle extras) {
        Log.d(TAG, "GPS Provider Status Changed: " + provider + ", Status="
                + S[status] + ", Extras=" + extras);
    }

    @Override
    public void onProviderEnabled(String provider) {
        // is provider better than bestProvider?
        // is yes, bestProvider = provider
        Log.d(TAG, "GPS Provider Enabled: " + provider);
    }

    @Override
    public void onProviderDisabled(String provider) {
        Log.d(TAG, "GPS Provider Disabled: " + provider);
    }

    // ---------------------------------------------------------------------------------------------
}
