package com.yodiwo.androidagent.core;

import android.content.Context;
import android.content.Intent;
import android.support.v4.content.LocalBroadcastManager;

// The Interface to interact with Yodiwo server
public abstract class aServerAPI {

    protected Context context;
    protected SettingsProvider settingsProvider;

    public static final String CONNECTIVITY_UI_UPDATE = "ServerAPI.CONNECTIVITY_UI_UPDATE";
    public static final String EXTRA_UPDATED_RX_STATE = "EXTRA_UPDATED_RX_STATE";
    public static final String EXTRA_UPDATED_TX_STATE = "EXTRA_UPDATED_TX_STATE";

    protected boolean RxActive = false;
    protected boolean TxActive = false;

    protected abstract boolean SendMsg(Object msg);   //send async message

    protected abstract boolean SendReq(Object msg);        //send RPC Request

    protected abstract boolean SendRsp(Object msg, int syncId);   //send RPC Response

    protected abstract void StartRx();

    protected abstract void StopRx();

    protected abstract void Connect();

    protected abstract void Teardown();

    public void RequestConnectivityUiUpdate() {
        Intent intent = new Intent(CONNECTIVITY_UI_UPDATE);
        intent.putExtra(EXTRA_UPDATED_RX_STATE, RxActive);
        intent.putExtra(EXTRA_UPDATED_TX_STATE, TxActive);

        LocalBroadcastManager
                .getInstance(context)
                .sendBroadcast(intent);
    }
}
