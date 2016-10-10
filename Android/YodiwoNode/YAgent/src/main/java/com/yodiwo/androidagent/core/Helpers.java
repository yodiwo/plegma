package com.yodiwo.androidagent.core;

import android.content.Context;
import android.os.Environment;
import android.util.Log;

import java.io.BufferedWriter;
import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.text.SimpleDateFormat;
import java.util.Date;

//import com.crashlytics.android.Crashlytics;

/**
 * Created by Nikos on 27/10/2015.
 */
public class Helpers {
    private static Helpers ourInstance = new Helpers();

    private static final String LOG_FILE = "NeBiT_log";

    private static final boolean useCrashlytics = false;
    private static boolean isEmpty = false;

    public static Helpers getInstance() {
        return ourInstance;
    }

    private Helpers() {
    }

    public static void log(int priority, String tag, String msg) {
        String priorityMsg = null;
        if (useCrashlytics) {
            //Crashlytics.log(priority, tag, msg);
        } else {
            switch (priority) {
                case Log.VERBOSE:
                    Log.v(tag, msg);
                    priorityMsg = "VERBOSE/";
                    break;
                case Log.DEBUG:
                    Log.d(tag, msg);
                    priorityMsg = "DEBUG/";
                    break;
                case Log.INFO:
                    Log.i(tag, msg);
                    priorityMsg = "INFO/";
                    break;
                case Log.WARN:
                    Log.w(tag, msg);
                    priorityMsg = "WARN/";
                    break;
                case Log.ERROR:
                    Log.e(tag, msg);
                    priorityMsg = "ERROR/";
                    break;
                case Log.ASSERT:
                    Log.e(tag, msg);
                    priorityMsg = "ASSERT/";
                    break;
                default:
                    break;
            }
            if (!SettingsActivity.isRelease)
                saveLogToFile("--> " + priorityMsg + tag + " : " + msg);
        }
    }

    public static void logException(String tag, Exception e) {
        if (useCrashlytics) {
            //Crashlytics.logException(e);
        } else {
            if (e.getMessage() != null)
                log(Log.ERROR, tag, e.getMessage());
        }
    }

    // =============================================================================================
    // save log file
    // =============================================================================================

    private static void saveLogToFile(String data) {
        if (isExternalStorageReadable() && isExternalStorageWritable()){
            File file = new File(Environment.getExternalStorageDirectory(), LOG_FILE + ".txt");
            BufferedWriter out;
            try {
                if (!isEmpty){
                    out = new BufferedWriter(new FileWriter(file, false), 1024);
                    out.write((""));
                    out.close();
                    isEmpty = true;
                }
                out = new BufferedWriter(new FileWriter(file, true), 1024);
                out.write(data);
                out.newLine();
                out.close();
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
    }

    /* Checks if external storage is available for read and write */
    private static boolean isExternalStorageWritable() {
        String state = Environment.getExternalStorageState();
        return Environment.MEDIA_MOUNTED.equals(state);
    }

    /* Checks if external storage is available to at least read */
    private static boolean isExternalStorageReadable() {
        String state = Environment.getExternalStorageState();
        return Environment.MEDIA_MOUNTED.equals(state) ||
                Environment.MEDIA_MOUNTED_READ_ONLY.equals(state);
    }

}
