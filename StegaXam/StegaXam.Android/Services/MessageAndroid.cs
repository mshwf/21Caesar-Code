using Android.Widget;
using StegaXam.Droid;
using StegaXam.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(MessageAndroid))]
namespace StegaXam.Droid
{
    public class MessageAndroid : IMessage
    {
        public void LongAlert(string message)
        {
            Toast.MakeText(MainActivity.Instance, message, ToastLength.Long).Show();
        }

        public void ShortAlert(string message)
        {
            Toast.MakeText(MainActivity.Instance, message, ToastLength.Short).Show();
        }
    }
}