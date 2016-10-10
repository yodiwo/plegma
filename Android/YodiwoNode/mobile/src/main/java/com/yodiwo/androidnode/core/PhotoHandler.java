package com.yodiwo.androidnode.core;

/**
 * Created by vaskanas on 28-Jun-16.
 */

import android.Manifest;
import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.content.res.Resources;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.hardware.Camera;
import android.hardware.Camera.PictureCallback;
import android.net.Uri;
import android.os.Environment;
import android.support.v4.app.ActivityCompat;
import android.util.Log;
import android.widget.Toast;

import com.yodiwo.androidagent.core.Helpers;
import com.yodiwo.androidagent.core.NodeService;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.List;
import java.util.Locale;

public class PhotoHandler implements PictureCallback {

    // =============================================================================================
    // Variables
    // =============================================================================================

    private static final String TAG = PhotoHandler.class.getSimpleName();

    private final Activity activity;

    // =============================================================================================
    // Constructor
    // =============================================================================================

    public PhotoHandler(Activity activity) {
        this.activity = activity;
    }

    // =============================================================================================
    // PictureCallback
    // =============================================================================================

    @Override
    public void onPictureTaken(byte[] data, Camera camera) {

        // Create the File where the photo should go
        File photoFile = createImageFile(activity, data);

        // Continue only if the File was successfully created
        if (photoFile != null) {
            // save the file path for result
            MainActivity.mCurrentPhotoPath = photoFile.getAbsolutePath();
            // Send the request
            NodeService.UploadFile(activity,
                    MainActivity.mCurrentPhotoPath,
                    MainActivity.Camera,
                    MainActivity.CameraPort);

            ThingsModuleService.setTorch(activity.getBaseContext(), false, false);
            ThingsModuleService.releaseCamera();
        }
    }

    // =============================================================================================
    // Create image file
    // =============================================================================================

    public static File createImageFile(Activity activity, byte[] data) {
        File pictureFile = createFile(activity, "Picture_", ".jpg", Environment.DIRECTORY_PICTURES);
        if (pictureFile == null)
            return null ;

        try {
            // resize image
            Bitmap bitmap = resizeBitmap(activity, data);

            // convert to byteArray
            byte[] byteArray = BitmapToByteArray(bitmap);

            // save image file
            saveFile(pictureFile, byteArray);
        } catch (Exception ex) {
            Helpers.logException(TAG, ex);
            return pictureFile;
        }

        return pictureFile;
    }

    // ---------------------------------------------------------------------------------------------

    public static void createImageFile(Activity activity, Bitmap bitmap) {
        File pictureFile = createFile(activity, "Picture_", ".jpg", Environment.DIRECTORY_PICTURES);
        if (pictureFile == null)
            return;

        try {
            // resize and save file
            saveFile(pictureFile, resizeBitmap(activity, bitmap));

            // inform UI
            Intent mediaScanIntent = new Intent(Intent.ACTION_MEDIA_SCANNER_SCAN_FILE);
            Uri contentUri = Uri.fromFile(pictureFile);
            mediaScanIntent.setData(contentUri);
            activity.sendBroadcast(mediaScanIntent);
            Toast.makeText(activity.getApplicationContext(), "Image saved", Toast.LENGTH_SHORT).show();
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    // ---------------------------------------------------------------------------------------------

    public static File createFile(Activity activity, String prefix, String extension, String environmentDir) {
        File file = null;

        try {
            // Verify storage permissions
            int REQUEST_EXTERNAL_STORAGE = 1;
            String[] PERMISSIONS_STORAGE = {
                    Manifest.permission.READ_EXTERNAL_STORAGE,
                    Manifest.permission.WRITE_EXTERNAL_STORAGE
            };
            int permission = ActivityCompat.checkSelfPermission(activity, Manifest.permission.WRITE_EXTERNAL_STORAGE);
            if (permission != PackageManager.PERMISSION_GRANTED) {
                // We don't have permission so prompt the user
                ActivityCompat.requestPermissions(
                        activity,
                        PERMISSIONS_STORAGE,
                        REQUEST_EXTERNAL_STORAGE
                );
            }

            // creating
            String timeStamp = new SimpleDateFormat("ddMMyyyy_HH:mm:ss", Locale.getDefault()).format(new Date());
            String photoFile = prefix + timeStamp;
            File pictureFileDir = null;
            if (environmentDir.equals(Environment.DIRECTORY_PICTURES))
                pictureFileDir = getPictureDir();

            if (pictureFileDir != null && !pictureFileDir.exists() && !pictureFileDir.mkdirs()) {
                Helpers.log(Log.DEBUG, TAG, "Can't create directory to save image.");
                return null;
            }

            file = File.createTempFile(
                    photoFile,
                    extension,
                    pictureFileDir);
        } catch (Exception ex) {
            Helpers.logException(TAG, ex);
        }

        return file;
    }

    // ---------------------------------------------------------------------------------------------

    private static File getPictureDir() {
        File sdDir = Environment
                .getExternalStoragePublicDirectory(Environment.DIRECTORY_PICTURES);
        return new File(sdDir, "CameraApiDemo");
    }

    // =============================================================================================
    // Image resize
    // =============================================================================================

    private static Bitmap resizeBitmap(Activity activity, Bitmap bitmap){
        List<Camera.Size> supportedRes = ThingsModuleService.getResolutions(activity);
        Camera.Size desiredRes = supportedRes.get(supportedRes.size() / 2);
        return resizeBitmap(bitmap, desiredRes.width, desiredRes.height);
    }

    private static Bitmap resizeBitmap(Bitmap bitmap, int width, int height){
        return Bitmap.createScaledBitmap(bitmap, width, height, true);
    }

    // ---------------------------------------------------------------------------------------------

    private static Bitmap resizeBitmap(Activity activity, byte[] data){
        List<Camera.Size> supportedRes = ThingsModuleService.getResolutions(activity);
        Camera.Size desiredRes = supportedRes.get(supportedRes.size() / 2);
        return resizeBitmap(data, desiredRes.width, desiredRes.height);
    }

    public static Bitmap resizeBitmap(byte[] data, int width, int height){
        Bitmap bitmap = null;
        if (data != null) {
            BitmapFactory.Options options = new BitmapFactory.Options();
            try {
                // First decode with inJustDecodeBounds=true to check dimensions
                options.inJustDecodeBounds = true;
                BitmapFactory.decodeByteArray(data, 0, data.length, options);

                // Calculate inSampleSize
                options.inSampleSize = calculateInSampleSize(options, width, height);

                // Decode bitmap with inSampleSize set
                options.inJustDecodeBounds = false;
                options.inTempStorage = new byte[16 * 1024];
                bitmap = BitmapFactory.decodeByteArray(data, 0, data.length, options);

            } catch (Exception ex) {
                Helpers.logException(TAG, ex);
            }
        }
        return bitmap;
    }

    // =============================================================================================
    // Decode Bitmap From Resource
    // =============================================================================================

    public static Bitmap decodeBitmapFromResource(Context context, int resId){
        List<Camera.Size> supportedRes = ThingsModuleService.getResolutions(context);
        Camera.Size desiredRes = supportedRes.get(supportedRes.size() / 2);
        return decodeBitmapFromResource(context.getApplicationContext().getResources(), resId, desiredRes.width, desiredRes.height);
    }

    private static Bitmap decodeBitmapFromResource(Resources res, int resId, int width, int height){
        Bitmap bitmap = null;
        try {
            // First decode with inJustDecodeBounds=true to check dimensions
            final BitmapFactory.Options options = new BitmapFactory.Options();
            options.inJustDecodeBounds = true;
            BitmapFactory.decodeResource(res, resId, options);

            // Calculate inSampleSize
            options.inSampleSize = PhotoHandler.calculateInSampleSize(options, width, height);

            // Decode bitmap with inSampleSize set
            options.inJustDecodeBounds = false;
            BitmapFactory.decodeResource(res, resId, options);

            bitmap = BitmapFactory.decodeResource(res, resId, options);
        } catch (Exception ex){
            Helpers.logException(TAG, ex);
        }
        return bitmap;
    }

    // ---------------------------------------------------------------------------------------------

    private static int calculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight) {
        // Raw height and width of image
        final int height = options.outHeight;
        final int width = options.outWidth;
        int inSampleSize = 1;

        if (height > reqHeight || width > reqWidth) {

            final int halfHeight = height / 2;
            final int halfWidth = width / 2;

            // Calculate the largest inSampleSize value that is a power of 2 and keeps both
            // height and width larger than the requested height and width.
            while ((halfHeight / inSampleSize) > reqHeight
                    && (halfWidth / inSampleSize) > reqWidth) {
                inSampleSize *= 2;
            }
        }

        return inSampleSize;
    }

    // =============================================================================================
    // Converters
    // =============================================================================================

    public static byte[] BitmapToByteArray(Bitmap imageBitmap) {
        ByteArrayOutputStream stream = new ByteArrayOutputStream();
        imageBitmap.compress(Bitmap.CompressFormat.JPEG, 100, stream);
        return stream.toByteArray();
    }

    // =============================================================================================
    // Save
    // =============================================================================================

    private static void saveFile(File file, byte[] bytes) {
        try {
            FileOutputStream fos = new FileOutputStream(file);
            fos.write(bytes);
            fos.close();
        } catch (IOException e) {
            Helpers.logException(TAG, e);
        }
    }

    private static void saveFile(File file, Bitmap bitmap) {
        try {
            FileOutputStream fout = new FileOutputStream(file);
            bitmap.compress(Bitmap.CompressFormat.JPEG, 100, fout);
            fout.flush();
            fout.close();
        } catch (IOException e) {
            Helpers.logException(TAG, e);
        }
    }

}
