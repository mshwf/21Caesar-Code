using StegaXam.Algorithms;
using StegaXam.Services;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace StegaXam.Views
{
    public partial class EncodePage : ContentPage
    {
        readonly ISteganography stega;
        public EncodePage()
        {
            InitializeComponent();
            stega = new SteganographyServiceV1();
        }

        string textToHide;
        static string filename;
        private async void HideClick(object sender, EventArgs e)
        {
            try
            {
                textToHide = entryMsg.Text;
                if (string.IsNullOrEmpty(textToHide))
                {
                    await DisplayAlert("What's your message?", "Type the message you wish to hide from enemies", "OK");
                    return;
                }
                if (picker.ImageData == null)
                {
                    await DisplayAlert("No image selected!", "Pick up an image to hide your message in.", "OK");
                    return;
                }

                string password = await DisplayPromptAsync("Password?", "Enter a password, or leave empty to embed the message in plain (not encrypted)");
                if (password == null) return;

                var imageData = stega.Encode(picker.ImageData, textToHide, password);

                filename = DependencyService.Get<IPicture>().SavePictureToDisk("21Caesar", imageData);
                var pwdMesg = password != "" ? "they can't read the message without knowing the password, " : null;
                await DisplayAlert("Your secret has been embeded successfully",
                    $"You can now share it with anybody, {pwdMesg}the image name: {filename}", "OK");
                btnHide.IsVisible = false;
                btnShare.IsVisible = true;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Exception", ex.ToString(), "OK");
            }
        }

        private async void Share_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(filename))
                await DisplayAlert("Oops!", "No file found!", "OK");

            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "Hello",
                File = new ShareFile(filename)
            });
        }

        private void picker_ImageDataChanged(object sender, EventArgs e)
        {
            if (picker.ImageData == null)
            {
                btnShare.IsVisible = false;
                btnHide.IsVisible = true;
            }
        }
    }
}