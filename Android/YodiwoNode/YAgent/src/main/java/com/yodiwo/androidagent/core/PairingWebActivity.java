package com.yodiwo.androidagent.core;

import android.app.Activity;
import android.content.Intent;
import android.graphics.Bitmap;
import android.os.Bundle;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.webkit.WebView;
import android.webkit.WebViewClient;

import com.yodiwo.androidagent.R;


public class PairingWebActivity extends Activity {

    private static final String TAG = PairingWebActivity.class.getSimpleName();

    private WebView webView;

    public static final String EXTRA_URI = "EXTRA_URI";
    public static final String BROADCAST_WEBPAGE_FINISHED = "PairingWebActivity.BROADCAST_WEBPAGE_FINISHED";

    private Boolean isNebit = false;

    // ---------------------------------------------------------------------------------------------

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_pairing_web);

        Helpers.log(Log.DEBUG, TAG, "onCreate entered");
        String url = null;
        try {
            url = getIntent().getExtras().getString(EXTRA_URI);
        } catch (Exception ex){
            Helpers.logException(TAG, ex);
        }

        isNebit = url == null;

        webView = (WebView) findViewById(R.id.webView);
        webView.getSettings().setJavaScriptEnabled(true);
        webView.clearCache(true);
        webView.clearFormData();
        webView.clearHistory();
        webView.setWebViewClient(new MyWebViewClient(this));

        // Get the pairing UI
        webView.loadUrl(isNebit ? PairingService.getPairingUserCfmUrl(this) : url);
    }

    // ---------------------------------------------------------------------------------------------

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_pairing_web, menu);
        return true;
    }

    // ---------------------------------------------------------------------------------------------

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        int id = item.getItemId();

        if (id == R.id.action_settings) {
            return true;
        }

        return super.onOptionsItemSelected(item);
    }

    // ---------------------------------------------------------------------------------------------

    @Override
    public void onBackPressed() {
        String url = webView.getUrl();
        Helpers.log(Log.DEBUG, TAG, "onBackPressed Called url:" + url);
        this.finish();
    }

    // =============================================================================================

    public class MyWebViewClient extends WebViewClient {

        final String redirectNextURL = "";
        Activity activity;

        MyWebViewClient(Activity activity) {
            this.activity = activity;
        }

        @Override
        public boolean shouldOverrideUrlLoading(WebView view, String url) {
            Log.i("======================>", "shouldOverrideUrlLoading Url is: " + url);
            // do your handling codes here, which url is the requested url
            // probably you need to open that url rather than redirect:
            webView.loadUrl(url);
            return true; // then it is not handled by default action
        }

        @Override
        public void onPageStarted(WebView view, String url, Bitmap favicon) {
            Log.i("======================>", "current Url is: " + url);
            if (url.equals(redirectNextURL)) {
                 activity.finish();
            }
        }

        @Override
        public void onLoadResource(WebView view, String url) {
            Log.i("======================>", "onLoadResource Url: " + url);
        }


        @Override
        public void onPageFinished(WebView view, String url) {
            Log.d("======================>", "onPageFinished:" + url);
            if (url.contains("success")) {
                if (isNebit)
                    PairingService.FinishPairing(activity.getApplicationContext());
                else
                    LocalBroadcastManager
                            .getInstance(getApplicationContext())
                            .sendBroadcast(new Intent(BROADCAST_WEBPAGE_FINISHED));
                activity.finish();
            }
        }

        @Override
        public void onReceivedError(WebView view, int errorCode,
                                    String description, String failingUrl) {
            Log.i("======================>", "onReceivedError Url: " + failingUrl);
            if (failingUrl.equals(redirectNextURL)) {
                activity.finish();
            }
        }
    }
}
