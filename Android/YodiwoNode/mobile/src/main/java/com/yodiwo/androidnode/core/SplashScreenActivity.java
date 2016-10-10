package com.yodiwo.androidnode.core;

import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.view.animation.Animation;
import android.view.animation.AnimationUtils;
import android.widget.ImageView;

import com.yodiwo.androidagent.core.Helpers;
import com.yodiwo.androidnode.R;

/**
 * Created by vaskanas on 07-Sep-16.
 */
public class SplashScreenActivity extends AppCompatActivity {

    private static final String TAG = SplashScreenActivity.class.getSimpleName();

    // Splash screen timer
    private static int SPLASH_TIME_OUT = 3000;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_splash_screen);
        final Context context = this.getApplicationContext();

        /****** Create Thread that will sleep for 5 seconds *************/
        Thread background = new Thread() {
            public void run() {
                try {

                    // set animation
                    ImageView logo = (ImageView) findViewById(R.id.imgLogo);

                    Animation animation = AnimationUtils.loadAnimation(context,
                            R.anim.anim_splash_screen);
                    logo.startAnimation(animation);

                    // Thread will sleep for 5 seconds
                    sleep(SPLASH_TIME_OUT);

                    // After 5 seconds redirect to another intent
                    Intent i=new Intent(getBaseContext(), MainActivity.class);
                    startActivity(i);

                    //Remove activity
                    finish();

                } catch (Exception ex) {
                    Helpers.logException(TAG, ex);
                }
            }
        };

        // start thread
        background.start();
    }
}