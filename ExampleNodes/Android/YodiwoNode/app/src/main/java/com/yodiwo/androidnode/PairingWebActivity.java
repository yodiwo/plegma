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


public class PairingWebActivity extends Activity {

    private static final String TAG = PairingWebActivity.class.getSimpleName();

    private WebView webView;

    // ---------------------------------------------------------------------------------------------

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        final Activity activity = this;
        final String redirectNextURL = PairingService.getPairingWebUrl(SettingsProvider.getInstance(this)) + "/pairing/success";

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_pairing_web);


        webView = (WebView) findViewById(R.id.webView);
        webView.getSettings().setJavaScriptEnabled(true);
        webView.setWebViewClient(new MyWebViewClient(this));

        webView.setWebViewClient(new WebViewClient() {
            public void onReceivedError(WebView view, int errorCode, String description, String failingUrl) {
                Toast.makeText(activity, description, Toast.LENGTH_SHORT).show();
            }
        });

        // Get the pairing UI
        String url = PairingService.getPairingWebURL(this, redirectNextURL);
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
        if (url.contains("success")) {
            Toast.makeText(this, "Pairing phase 2 finished.", Toast.LENGTH_SHORT).show();
            PairingService.FinishPairing(this.getApplicationContext());
        }
        this.finish();
    }

    // =============================================================================================

    public class MyWebViewClient extends WebViewClient {

        final String redirectNextURL = "http://local.yodiwo.com/pairing/next";
        Activity activity;

        public MyWebViewClient(Activity activity) {
            this.activity = activity;
        }

        @Override
        public boolean shouldOverrideUrlLoading(WebView view, String url) {
            Log.i("======================>", "shouldOverrideUrlLoading Url is: " + url);
            // do your handling codes here, which url is the requested url
            // probably you need to open that url rather than redirect:

            return false; // then it is not handled by default action
        }

        @Override
        public void onPageStarted(WebView view, String url, Bitmap favicon) {
            Log.i("======================>", "current Url is: " + url);
            if (url.equals(redirectNextURL)) {
                Toast.makeText(activity, "Pairing phase 2 finished.", Toast.LENGTH_SHORT).show();
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
        }

        @Override
        public void onReceivedError(WebView view, int errorCode,
                                    String description, String failingUrl) {
            Log.i("======================>", "onReceivedError Url: " + failingUrl);
            if (failingUrl.equals(redirectNextURL)) {
                Toast.makeText(activity, "Pairing phase 2 finished.", Toast.LENGTH_SHORT).show();
                activity.finish();
            }
        }
    }
}
