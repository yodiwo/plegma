package com.yodiwo.androidnode;

import android.app.IntentService;
import android.content.Context;
import android.content.Intent;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;

import com.yodiwo.plegma.ApiRestAccess;
import com.yodiwo.plegma.PairingNodeGetKeysRequest;
import com.yodiwo.plegma.PairingNodeGetTokensRequest;
import com.yodiwo.plegma.PairingServerKeysResponse;
import com.yodiwo.plegma.PairingServerTokensResponse;

public class PairingService extends IntentService {

    // =============================================================================================
    // Static information's

    public static final String TAG = PairingService.class.getSimpleName();

    public static final String EXTRA_REQUEST_TYPE = "EXTRA_REQUEST_TYPE";
    public static final String EXTRA_STATUS = "EXTRA_STATUS";

    public static final String BROADCAST_PHASE1_FINISHED = "PairingService.BROADCAST_PHASE1_FINISHED";
    public static final String BROADCAST_PAIRING_FINISHED = "PairingService.BROADCAST_PAIRINGH_FINISHED";

    public static final int REQUEST_START_PAIRING = 0;
    public static final int REQUEST_FINISH_PAIRING = 1;

    public static final int EXTRA_STATUS_SUCCESS = 0;
    public static final int EXTRA_STATUS_FAILED = 1;


    // =============================================================================================
    // Service overrides

    private ApiRestAccess apiRestAccess = null;
    private SettingsProvider settingsProvider;


    public PairingService() {
        super("PairingService");
    }

    public PairingService(String name) {
        super(name);
    }

    @Override
    protected void onHandleIntent(Intent intent) {

        Log.d(TAG, "Handle message");
        settingsProvider = SettingsProvider.getInstance(getApplicationContext());


        if (apiRestAccess == null)
            apiRestAccess = new ApiRestAccess(getPairingWebUrl(settingsProvider));

        int request_type = intent.getExtras().getInt(EXTRA_REQUEST_TYPE);
        switch (request_type) {
            case REQUEST_START_PAIRING:
                StartPairing(settingsProvider);
                break;
            case REQUEST_FINISH_PAIRING:
                FinishPairing(settingsProvider);
                break;
        }

    }

    // =============================================================================================
    // Service execution (background thread)

    private void StartPairing(SettingsProvider settingsProvider) {

        Log.d(TAG, "Start Pairing");
        Intent intent = new Intent(BROADCAST_PHASE1_FINISHED);

        // Get Tokens from server
        try {
            PairingNodeGetTokensRequest req = new PairingNodeGetTokensRequest();
            req.uuid = settingsProvider.getNodeUUID();
            req.name = settingsProvider.getDeviceName();

            PairingServerTokensResponse resp = apiRestAccess.service.SendPairingGetTokens(req);
            Log.d(TAG, "Tokens: " + resp.token1 + ", " + resp.token2);

            // Save tokens
            settingsProvider.setNodeTokens(resp.token1, resp.token2);

            // Add extra status
            intent.putExtra(EXTRA_STATUS, EXTRA_STATUS_SUCCESS);
        } catch (Exception ex) {
            Log.e(TAG, ex.getMessage());

            // Add extra status for failed
            intent.putExtra(EXTRA_STATUS, EXTRA_STATUS_FAILED);
        }

        // Broadcast the finish of the first pairing
        LocalBroadcastManager
                .getInstance(getApplicationContext())
                .sendBroadcast(intent);
    }

    // ---------------------------------------------------------------------------------------------

    private void FinishPairing(SettingsProvider settingsProvider) {
        Intent intent = new Intent(BROADCAST_PAIRING_FINISHED);

        // Get Tokens from server
        try {
            PairingNodeGetKeysRequest req = new PairingNodeGetKeysRequest();
            req.uuid = settingsProvider.getNodeUUID();
            req.token1 = settingsProvider.getNodeToken1();

            PairingServerKeysResponse resp = apiRestAccess.service.SendPairingGetKeys(req);
            //Log.d(TAG, "Keys: " + resp.nodeKey + ", " + resp.secretKey);

            // Save tokens
            settingsProvider.setNodeKeys(resp.nodeKey, resp.secretKey);

            // Add extra status
            intent.putExtra(EXTRA_STATUS, EXTRA_STATUS_SUCCESS);
        } catch (Exception ex) {
            Log.e(TAG, ex.getMessage());

            // Add extra status for failed
            intent.putExtra(EXTRA_STATUS, EXTRA_STATUS_FAILED);
        }

        // Broadcast the finish of the first pairing
        LocalBroadcastManager
                .getInstance(getApplicationContext())
                .sendBroadcast(intent);
    }

    // =============================================================================================
    // Public Functions

    public static void StartPairing(Context context) {
        Intent intent = new Intent(context, PairingService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_START_PAIRING);
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    public static String getPairingWebURL(Context context, String redirectURL) {
        SettingsProvider settingsProvider = SettingsProvider.getInstance(context);
        String path = getPairingWebUrl(settingsProvider);

        return path
                + "/pairing/1/userconfirm"
                + "?token2=" + settingsProvider.getNodeToken2()
                + "&noderedirect=" + redirectURL
                + "&uuid=" + settingsProvider.getNodeUUID();
    }

    public static String getPairingWebUrl(SettingsProvider settingsProvider) {
        return ((settingsProvider.getServerUseSSL()) ? "https://" : "http://") +
                settingsProvider.getServerAddress() + ":" +
                Integer.toString(settingsProvider.getServerPort());
    }

    // ---------------------------------------------------------------------------------------------

    public static void FinishPairing(Context context) {
        Intent intent = new Intent(context, PairingService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_FINISH_PAIRING);
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    public static void UnPair(Context context) {
        SettingsProvider settingsProvider = SettingsProvider.getInstance(context);
        settingsProvider.setNodeKeys(null, null);
        settingsProvider.setNodeTokens(null, null);
    }

    // =============================================================================================
}
