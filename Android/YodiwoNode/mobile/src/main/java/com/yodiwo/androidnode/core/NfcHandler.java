package com.yodiwo.androidnode.core;

import android.app.Activity;
import android.app.PendingIntent;
import android.content.Intent;
import android.content.IntentFilter;
import android.nfc.NdefMessage;
import android.nfc.NdefRecord;
import android.nfc.NfcAdapter;
import android.nfc.Tag;
import android.nfc.tech.Ndef;
import android.os.AsyncTask;
import android.util.Log;

import com.yodiwo.androidagent.core.Helpers;
import com.yodiwo.androidagent.core.NodeService;

import java.io.UnsupportedEncodingException;
import java.util.Arrays;

/**
 * Created by vaskanas on 07-Jul-16.
 */
public class NfcHandler {

    // =============================================================================================
    // Variables
    // =============================================================================================

    private static final String TAG = NfcHandler.class.getSimpleName();

    private NfcAdapter mNfcAdapter = null;

    public static final String MIME_TEXT_PLAIN = "text/plain";

    // =============================================================================================
    // Constructor
    // =============================================================================================

    public NfcHandler(Activity activity) {
        if (mNfcAdapter == null)
            mNfcAdapter = NfcAdapter.getDefaultAdapter(activity);
    }

    // =============================================================================================
    // Helpers
    // =============================================================================================

    /**
     * generateFetcher NfcAdapter
     */
    public NfcAdapter getNfcAdapter(){
        return mNfcAdapter;
    }
    /**
     * requesting the foreground dispatch.
     */
    public void setupForegroundNFCDispatch(Activity activity) {
        if (mNfcAdapter != null) {
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

            mNfcAdapter.enableForegroundDispatch(activity, pendingIntent, filters, techList);
        }
    }

    /**
     * request to stop the foreground dispatch.
     */
    public void stopForegroundNFCDispatch(Activity activity) {
        if (mNfcAdapter != null)
            mNfcAdapter.disableForegroundDispatch(activity);
    }

    /**
     *
     * @param intent NFC intent to handle
     */
    public void handleNFCIntent(Intent intent, Activity activity) {
        String action = intent.getAction();
        if (NfcAdapter.ACTION_NDEF_DISCOVERED.equals(action)) {

            String type = intent.getType();
            if (MIME_TEXT_PLAIN.equals(type)) {

                Tag tag = intent.getParcelableExtra(NfcAdapter.EXTRA_TAG);
                new NdefReaderTask(activity).execute(tag);

            } else {
                Helpers.log(Log.DEBUG, TAG, "Wrong mime type1: " + type);
            }
        } else if (NfcAdapter.ACTION_TECH_DISCOVERED.equals(action)) {

            // In case we would still use the Tech Discovered Intent
            Tag tag = intent.getParcelableExtra(NfcAdapter.EXTRA_TAG);
            String[] techList = tag.getTechList();
            String searchedTech = Ndef.class.getName();

            for (String tech : techList) {
                if (searchedTech.equals(tech)) {
                    new NdefReaderTask(activity).execute(tag);
                    break;
                }
            }
        }
    }

    /**
     * Background task for reading the data. Do not block the UI thread while reading.
     *
     * @author Ralf Wondratschek
     */
    private class NdefReaderTask extends AsyncTask<Tag, Void, String> {
        private Activity activity;

        public NdefReaderTask(Activity activity){
            this.activity = activity;
        }

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
                        Helpers.logException(TAG, e) ;
                    }
                }
            }

            return null;
        }

        /*
         * See NFC forum specification for "Text Record Type Definition" at 3.2.1
         *
         * http://www.nfc-forum.org/specs/
         *
         * bit_7 defines encoding
         * bit_6 reserved for future use, must be 0
         * bit_5..0 length of IANA language code
         */
        private String readText(NdefRecord record) throws UnsupportedEncodingException {
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
                Helpers.log(Log.ERROR, TAG, "NFC:" + result);
                NodeService.SendPortMsg(activity,
                        MainActivity.NFC,
                        MainActivity.NFCPort,
                        result);
            }
        }
    }
}
