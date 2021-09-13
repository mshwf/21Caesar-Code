using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using System.Threading.Tasks;
using System.IO;
using Android.Content;
using Plugin.CurrentActivity;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using Android;
using Xamarin.Forms;
using System;

namespace StegaXam.Droid
{
    [Activity(Label = "21Caesar Code", Icon = "@mipmap/logo", Theme = "@style/MainTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize,
        Name = "com.mshawaf.x21caesarcode.MainActivity")]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        internal static MainActivity Instance { get; private set; }

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            if (Intent.ActionSend.Equals(Intent.Action) && Intent.Type != null && Intent.Type.StartsWith("image/"))
            {
                await ImageDispatcher.HandleSendImage(ContentResolver, Intent, "Encode");
                //HandleSendImages();
            }
            Instance = this;
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            CheckAppPermissions();

        }
        private void CheckAppPermissions()
        {
            var activity = CrossCurrentActivity.Current.Activity;

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != (int)Android.Content.PM.Permission.Granted)
            {
                ActivityCompat.RequestPermissions(activity, new string[] { Manifest.Permission.WriteExternalStorage }, 1);
            }
        }
        // Field, property, and method for Picture Picker
        public static readonly int PickImageId = 1000;
        public TaskCompletionSource<Stream> PickImageTaskCompletionSource { set; get; }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent intent)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, intent);

                if (requestCode == PickImageId)
                {
                    if ((resultCode == Result.Ok) && (intent != null))
                    {
                        Android.Net.Uri uri = intent.Data;
                        Stream stream = ContentResolver.OpenInputStream(uri);

                        // Set the Stream as the completion of the Task
                        PickImageTaskCompletionSource.SetResult(stream);
                    }
                    else
                    {
                        PickImageTaskCompletionSource.SetResult(null);
                    }
                }
            }
            catch (Exception ex)
            {
                PickImageTaskCompletionSource.SetResult(null);
                Android.Widget.Toast.MakeText(this, "Can't retrieve image, check the path", Android.Widget.ToastLength.Long).Show();
            }
        }


        private async void HandleSendImages()
        {

            Android.Net.Uri imageUri = (Android.Net.Uri)Intent.GetParcelableExtra(Intent.ExtraStream);
            if (imageUri != null)
            {

                Stream stream = ContentResolver.OpenInputStream(imageUri);
                byte[] byteArray;

                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    byteArray = memoryStream.ToArray();
                    await Xamarin.Forms.Application.Current.MainPage.Navigation.PushModalAsync(new Views.EncodePage(byteArray));
                    //FinishAndRemoveTask();
                    //FinishAffinity();
                }
            }

            //var view = new LinearLayout(this) { Orientation = Orientation.Vertical };
            //var url = Intent.GetStringExtra(Intent.ExtraText);

            //var urlTextView = new TextView(this) { Gravity = GravityFlags.Center };
            //urlTextView.Text = url;

            //view.AddView(urlTextView);
            //var description = new EditText(this) { Gravity = GravityFlags.Top };
            //view.AddView(description);

            //new AlertDialog.Builder(this)
            //         .SetTitle("Save a URL Link")
            //         .SetMessage("Type a description for your link")
            //         .SetView(view)
            //         .SetPositiveButton("Add", (dialog, whichButton) =>
            //         {
            //             var desc = description.Text;
            //        //Save off the url and description here
            //        //Remove dialog and navigate back to app or browser that shared                 
            //        //the link
            //        FinishAndRemoveTask();
            //             FinishAffinity();
            //         })
            //         .Show();
        }
    }


    [Activity(Name = "com.mshawaf.x21caesarcode.DecodeActivity")]
    public class DecodeActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
            if (Intent.ActionSend.Equals(Intent.Action) && Intent.Type != null && Intent.Type.StartsWith("image/"))
            {
                await ImageDispatcher.HandleSendImage(ContentResolver, Intent, "Decode");
            }
        }
    }
}