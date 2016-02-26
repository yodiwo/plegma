package com.yodiwo.androidnode;

import android.util.Log;

import com.crashlytics.android.Crashlytics;

/**
 * Created by Nikos on 27/10/2015.
 */
public class Helpers {
    private static Helpers ourInstance = new Helpers();

    private static final boolean useCrashlytics = true;

    public static Helpers getInstance() {
        return ourInstance;
    }

    private Helpers() {
    }

    public static void log(int priority, String tag, String msg) {

        if (useCrashlytics) {
            Crashlytics.log(priority, tag, msg);
        } else {
            switch (priority) {
                case Log.VERBOSE:
                    Log.v(tag, msg);
                    break;
                case Log.DEBUG:
                    Log.d(tag, msg);
                    break;
                case Log.INFO:
                    Log.i(tag, msg);
                    break;
                case Log.WARN:
                    Log.w(tag, msg);
                    break;
                case Log.ERROR:
                    Log.e(tag, msg);
                    break;
                case Log.ASSERT:
                    Log.e(tag, msg);
                    break;
                default:
                    break;
            }
        }
    }

    public static void logException(String tag, Exception e) {
        if (useCrashlytics) {
            Crashlytics.logException(e);
        } else {
            Log.e(tag, e.getMessage());
        }
    }
}
