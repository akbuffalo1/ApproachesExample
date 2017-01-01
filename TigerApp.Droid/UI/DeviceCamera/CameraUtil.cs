#region using

using System;
using System.IO;
using Android.Content;
using Android.Media;
using Environment = Android.OS.Environment;

#endregion

namespace TigerApp.Droid.UI.DeviceCamera
{
    public class CameraUtil
    {
        public static FileInfo SaveBitmap(Context context, byte[] data, String folderName,
            MediaScannerConnection.IOnScanCompletedListener onScanCompleteListener)
        {
            DateTime dateTime = DateTime.Now;
            String fileName = String.Format("Receipt_{0}{1}{2}_{3}{4}{5}.jpg", dateTime.Year, dateTime.Month,
                dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);

            FileInfo f =
                new FileInfo(Path.Combine(Environment.ExternalStorageDirectory.ToString(), folderName, fileName));
            if (!f.Directory.Exists)
                f.Directory.Create();

            using (FileStream writer = new FileStream(
                f.FullName,
                FileMode.Create))
            {
                //bmp.Compress(Bitmap.CompressFormat.Jpeg, 90, writer);
                writer.Write(data, 0, data.Length);
            }

            MediaScannerConnection.ScanFile(context, new[] {f.FullName}, null, onScanCompleteListener);
            return f;
        }
    }
}