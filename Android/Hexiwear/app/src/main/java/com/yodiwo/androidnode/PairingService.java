package com.yodiwo.androidnode;

import android.app.IntentService;
import android.content.Context;
import android.content.Intent;
import android.os.Build;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;
import android.webkit.CookieManager;
import android.webkit.CookieSyncManager;
import android.webkit.ValueCallback;

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
    public static final String BROADCAST_PAIRING_FINISHED = "PairingService.BROADCAST_PAIRING_FINISHED";
    public static final String BROADCAST_NODE_UNPAIRED = "PairingService.BROADCAST_NODE_UNPAIRED";
    public static final String BROADCAST_MODAL_DIALOG_FINISHED = "PairingService.BROADCAST_MODAL_DIALOG_FINISHED";

    public static final int REQUEST_START_PAIRING = 0;
    public static final int REQUEST_FINISH_PAIRING = 1;

    public static final int EXTRA_STATUS_SUCCESS = 0;
    public static final int EXTRA_STATUS_FAILED = 1;

    private static PairingStatus pairingStatus;
    // keep previous nodekey (in case of unpairing-pairing)
    private static String previousNodekey = null;
    private static boolean isReset = false;

    public enum PairingStatus{
        UNPAIRING,
        UNPAIRED,
        PHASE1ONGOING,
        PHASE1FINISHED,
        PAIRED
    }

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
       // Logger.getInstance(this).log(Log.DEBUG, TAG, "Handle message");
        settingsProvider = SettingsProvider.getInstance(getApplicationContext());

        if (apiRestAccess == null)
            apiRestAccess = new ApiRestAccess(getPairingWebUrl(settingsProvider));

        int request_type = intent.getExtras().getInt(EXTRA_REQUEST_TYPE);
        switch (request_type) {
            case REQUEST_START_PAIRING:
                pairingStatus = PairingStatus.PHASE1ONGOING;
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
   //     Logger.getInstance(this).log(Log.DEBUG, TAG, "Start Pairing");
        Intent intent = new Intent(BROADCAST_PHASE1_FINISHED);

        // Get Tokens from server
        try {
            PairingNodeGetTokensRequest req = new PairingNodeGetTokensRequest();
            req.uuid = settingsProvider.getNodeUUID();
            req.name = settingsProvider.getDeviceName();
            req.RedirectUri = PairingService.getPairingWebUrl(SettingsProvider.getInstance(this)) + "/pairing/1/success";
            req.NoUUIDAuthentication = true;

            PairingServerTokensResponse resp = apiRestAccess.service.SendPairingGetTokens(req);
        //    Logger.getInstance(this).log(Log.DEBUG, TAG, "Tokens: " + resp.token1 + ", " + resp.token2);

            // Save tokens
            settingsProvider.setNodeTokens(resp.token1, resp.token2);

            // Add extra status
            intent.putExtra(EXTRA_STATUS, EXTRA_STATUS_SUCCESS);
        } catch (Exception e) {
       //     Logger.getInstance(this).logException(TAG, e);

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
      //  Logger.getInstance(this).log(Log.DEBUG, TAG, "Finish Pairing");

        // Get Tokens from server
        try {
            PairingNodeGetKeysRequest req = new PairingNodeGetKeysRequest();
            req.uuid = settingsProvider.getNodeUUID();
            req.token1 = settingsProvider.getNodeToken1();
            req.token2 = settingsProvider.getNodeToken2();

            PairingServerKeysResponse resp = apiRestAccess.service.SendPairingGetKeys(req);

            // Save tokens
            settingsProvider.setNodeKeys(resp.nodeKey, resp.secretKey);

            // Add extra status
            intent.putExtra(EXTRA_STATUS, EXTRA_STATUS_SUCCESS);

            // set previousNodekey value
            if (previousNodekey == null)
                previousNodekey = SettingsProvider.getInstance(this).getNodeKey();

            // if nodekey has been changed, unpairing -> pairing
            if (previousNodekey != null && !previousNodekey.equals(SettingsProvider.getInstance(this).getNodeKey())) {
                isReset(true);
                previousNodekey = SettingsProvider.getInstance(this).getNodeKey();
            }
        } catch (Exception e) {
       //     Logger.getInstance(this).logException(TAG, e);

            if (settingsProvider.getNodeKey() != null && settingsProvider.getNodeSecretKey() != null)
                intent.putExtra(EXTRA_STATUS, EXTRA_STATUS_SUCCESS);
            else
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

    public static String getPairingUserCfmUrl(Context context) {
        SettingsProvider settingsProvider = SettingsProvider.getInstance(context);
        String path = getPairingWebUrl(settingsProvider);

        return path
                + "/pairing/1/userconfirm"
                + "?token2=" + settingsProvider.getNodeToken2()
                + "&uuid=" + settingsProvider.getNodeUUID();
    }

    public static String getPairingWebUrl(SettingsProvider settingsProvider) {
        boolean useSSL = settingsProvider.getServerUseSSL();
        return (useSSL ? "https://" : "http://") +
                settingsProvider.getServerAddress() + ":" +
                Integer.toString(useSSL ? 443 : 3334);
    }

    // ---------------------------------------------------------------------------------------------

    public static void FinishPairing(Context context) {
        Intent intent = new Intent(context, PairingService.class);
        intent.putExtra(EXTRA_REQUEST_TYPE, REQUEST_FINISH_PAIRING);
        context.startService(intent);
    }

    // ---------------------------------------------------------------------------------------------

    public static PairingStatus getPairingStatus(){
        return pairingStatus;
    }

    // ---------------------------------------------------------------------------------------------

    public static void setPairingStatus(PairingStatus status) {
        pairingStatus = status;
    }

    // ---------------------------------------------------------------------------------------------

    public static void UnPair(Context context, boolean isUnpairedByUser) {
        pairingStatus = PairingStatus.UNPAIRING;
        isReset(false);

        // Remove cookies
        CookieManager cookieManager = CookieManager.getInstance();
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP) {
            cookieManager.removeAllCookies(new ValueCallback<Boolean>() {
                @Override
                public void onReceiveValue(Boolean aBoolean) {
                }
            });
        } else {
            CookieSyncManager.createInstance(context);
            cookieManager.removeAllCookie();
        }

        if (isUnpairedByUser) {
         //   NodeService.SendNodeStatusChangedReq(context,
        //            new NodeStatusChangedReq(0,
        //                    eNodeNewStatus.Unpaired,
         //                   eNodeStatusChangeReason.UserRequested,
        //                    "")
        //    );
        }
        else{
            LocalBroadcastManager
                    .getInstance(context)
                    .sendBroadcast(new Intent(BROADCAST_NODE_UNPAIRED));
        }
    }

    // ---------------------------------------------------------------------------------------------

    private static final Object isResetLock = new Object();

    public static boolean isReset() {
        return isReset;
    }

    private static void isReset(boolean status) {
        synchronized (isResetLock) {
            isReset = status;
        }
    }
}