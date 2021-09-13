using System.Threading.Tasks;
using System.IO;
using Android.Content;
using Xamarin.Forms;
using System;

namespace StegaXam.Droid
{
    public static class ImageDispatcher
    {
        public static async Task HandleSendImage(ContentResolver contentResolver, Intent intent, string pageName)
        {

            Android.Net.Uri imageUri = (Android.Net.Uri)intent.GetParcelableExtra(Intent.ExtraStream);
            if (imageUri != null)
            {

                Stream stream = contentResolver.OpenInputStream(imageUri);
                byte[] byteArray;

                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    byteArray = memoryStream.ToArray();
                    if (pageName == "Encode")
                        await Application.Current.MainPage.Navigation.PushModalAsync(new Views.EncodePage(byteArray));
                    else if (pageName == "Decode")
                    {
                        try
                        {
                            await Application.Current.MainPage.Navigation.PushModalAsync(new Views.DecodePage(byteArray));
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
        }
    }
}