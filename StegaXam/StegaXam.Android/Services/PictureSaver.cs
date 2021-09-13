using Android;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Plugin.CurrentActivity;
using StegaXam.Droid;
using StegaXam.Services;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(PictureSaver))]
namespace StegaXam.Droid
{
    public class PictureSaver : IPicture
    {
        public string SavePictureToDisk(string filename, byte[] imageData)
        {
            try
            {
                CheckAppPermissions();
                //var pathToNewFolderx = MainActivity.Instance.GetExternalFilesDir(Environment.DirectoryDcim).AbsolutePath;
                //string dir = null;
                //if (false)
                //{
                //    dir = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDocuments).AbsolutePath;
                //}
                //else
                //{
                //    dir = Environment.ExternalStorageDirectory.AbsolutePath;
                //}

                //var pathToNewFolder = pathToNewFolderx + "/21Caesar";
                //Directory.CreateDirectory(pathToNewFolder);

                ////adding a time stamp time file name to allow saving more than one image... otherwise it overwrites the previous saved image of the same name  
                //string name = System.DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg";
                //string filePath = Path.Combine(pathToNewFolder, name);
                var pathToNewFolder = Environment.ExternalStorageDirectory.AbsolutePath + "/21Caesar";
                Directory.CreateDirectory(pathToNewFolder);

                //adding a time stamp time file name to allow saving more than one image... otherwise it overwrites the previous saved image of the same name  
                string name = System.DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg";
                string filePath = Path.Combine(pathToNewFolder, name);

                System.IO.File.WriteAllBytes(filePath, imageData);
                //mediascan adds the saved image into the gallery  
                var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                mediaScanIntent.SetData(Uri.FromFile(new Java.IO.File(filePath)));
                MainActivity.Instance.SendBroadcast(mediaScanIntent);
                return filePath;
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.ToString());
                return null;
            }
        }
        private void CheckAppPermissions()
        {
            var activity = CrossCurrentActivity.Current.Activity;

            if (ContextCompat.CheckSelfPermission(MainActivity.Instance, Manifest.Permission.WriteExternalStorage) != (int)Android.Content.PM.Permission.Granted)
            {
                ActivityCompat.RequestPermissions(activity, new string[] { Manifest.Permission.WriteExternalStorage }, 1);
            }
        }
    }
}