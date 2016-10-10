package com.yodiwo.androidnode.core;

import android.content.Context;
import android.os.Environment;
import android.util.Log;

import com.yodiwo.androidagent.core.Helpers;

import java.io.File;
import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Locale;

/**
 * Created by vaskanas on 22-Jul-16.
 */
class UncaughtExceptionHandler implements Thread.UncaughtExceptionHandler {

    // =============================================================================================
    // Variables
    // =============================================================================================

    private static final String TAG = UncaughtExceptionHandler.class.getSimpleName();

    private Context context;

    // =============================================================================================
    // Constructor
    // =============================================================================================

    UncaughtExceptionHandler(Context context){
        this.context = context;
    }

    // =============================================================================================
    // Overrides
    // =============================================================================================

    @Override
    public void uncaughtException(Thread thread, Throwable throwable) {
        Helpers.log(Log.ERROR, TAG, throwable.getMessage());
        System.out.println( throwable.getMessage());
        saveLogcatToFile("NeBiT_LogCrash");
    }

    // =============================================================================================
    // Save log file
    // =============================================================================================
    static void saveLogcatToFile(String fileName) {
        try {
            SimpleDateFormat sdf = new SimpleDateFormat("yyyyMMdd.HHmmss", Locale.getDefault());
            String currentDateandTime = sdf.format(new Date());
            File file = new File(Environment.getExternalStorageDirectory(), fileName + "_" + currentDateandTime + ".txt");
            Runtime.getRuntime().exec("logcat -d -v time -f " + file.getAbsolutePath());
        }catch (IOException e){
            Helpers.logException(TAG, e);
        }
    }
}
