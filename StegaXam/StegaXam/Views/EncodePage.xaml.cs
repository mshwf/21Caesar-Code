using Steganography;
using StegaXam.Models;
using StegaXam.Services;
using System;
using System.IO;
using Xamarin.Forms;

namespace StegaXam.Views
{
    public partial class EncodePage : ContentPage
    {
        public EncodePage()
        {
            InitializeComponent();
        }
        byte[] imageRaw;
        private async void OnPickPhotoButtonClicked(object sender, EventArgs e)
        {
            (sender as Button).IsEnabled = false;

            Stream stream = await DependencyService.Get<IPhotoPickerService>().GetImageStreamAsync();
            if (stream == null)
            {
                (sender as Button).IsEnabled = true;
                return;
            }
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            if (stream != null)
            {
                image.Source = ImageSource.FromStream(() => new MemoryStream(memoryStream.ToArray()));
                using (var ms = new MemoryStream())
                {
                    //await stream.CopyToAsync(ms);
                    imageRaw = memoryStream.ToArray();
                }
            }
            (sender as Button).IsEnabled = true;
            hideBtn.IsEnabled = true;
        }

        string textToHide;
        int currentChar = 0;
        private IStegImage steg;

        private async void HideClick(object sender, EventArgs e)
        {
            textToHide = entryMsg.Text;
            if (string.IsNullOrEmpty(textToHide) || imageRaw == null) return;

            string password = await DisplayPromptAsync("Password", "Enter a password to encrypt the message.");

            if (string.IsNullOrEmpty(password))
            {
                await DisplayAlert(null, "A password is a required", "OK");
                return;
            }
            textToHide = Crypto.EncryptStringAES(textToHide, password);
            steg = DependencyService.Get<IStegImage>();
            steg.Init(imageRaw);
            if (steg.Width * steg.Height < textToHide.Length + 1)
            {
                await DisplayAlert(null, "The image is too small for the message, choose bigger one.", "OK");
                return;
            }

            bool _break = false;
            for (int i = 0; i < steg.Width; i++)
            {
                for (int j = 0; j < steg.Height; j++)
                {
                    ColorByte pixel = steg.GetPixel(i, j);
                    if (currentChar < textToHide.Length)
                    {
                        int ch = textToHide[currentChar++];
                        steg.SetPixel(i, j, new ColorByte(pixel.R, pixel.G, ch));
                    }
                    else
                    {
                        _break = true;
                        break;
                    }
                }
                if (_break) break;
            }
            int lastX = steg.Width - 1;
            int lastY = steg.Height - 1;
            ColorByte lastPixel = steg.GetPixel(lastX, lastY);

            steg.SetPixel(lastX, lastY, new ColorByte(lastPixel.R, lastPixel.G, textToHide.Length));
            await DisplayAlert("The message was successfully hidden!", "Tap 'Save' to save the image and share it with anybody, they can't read the message without knowing the password", "OK");
            hideBtn.IsEnabled = entryMsg.Text.Length > 0 && imageRaw != null;
            saveBtn.IsVisible = true;
        }

        private async void SaveClicked(object sender, EventArgs e)
        {
            saveBtn.IsEnabled = false;
            if (steg == null) return;
            var fileName = DependencyService.Get<IPicture>().SavePictureToDisk("ChartImage", steg.Save());
            await DisplayAlert("Photo saved successfully", $"You can find it in: {fileName}", "OK");
            saveBtn.IsEnabled = true;
        }

        private void EntryMsg_TextChanged(object sender, TextChangedEventArgs e)
        {
            hideBtn.IsEnabled = entryMsg.Text.Length > 0 && imageRaw != null;
        }
    }
}