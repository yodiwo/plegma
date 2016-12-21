package com.yodiwo.androidnode;

import android.app.Activity;
import android.graphics.Bitmap;
import android.os.Bundle;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.Toast;

import com.mikroe.hexiwear_android.R;

public class PairingWebActivity extends Activity {

    private static final String TAG = PairingWebActivity.class.getSimpleName();

    private WebView webView;

    // ---------------------------------------------------------------------------------------------

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        Log.d(TAG, "onCreate entered");

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_pairing_web);

        webView = (WebView) findViewById(R.id.webView);
        webView.getSettings().setJavaScriptEnabled(true);
        webView.clearCache(true);
        webView.clearFormData();
        webView.clearHistory();
        webView.setWebViewClient(new MyWebViewClient(this));

        // Get the pairing UI
        String url = PairingService.getPairingUserCfmUrl(this);
        webView.loadUrl(url);
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
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();

        //noinspection SimplifiableIfStatement
        if (id == R.id.action_settings) {
            return true;
        }

        return super.onOptionsItemSelected(item);
    }

    // ---------------------------------------------------------------------------------------------

    @Override
    public void onBackPressed() {
        String url = webView.getUrl();
        Log.d(TAG, "onBackPressed Called url:" + url);
        this.finish();
    }

    // =============================================================================================

    public class MyWebViewClient extends WebViewClient {

        final String redirectNextURL = ""; //"http://local.yodiwo.com/pairing/next";
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
                PairingService.FinishPairing(activity.getApplicationContext());
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
