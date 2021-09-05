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
            try
            {
                textToHide = entryMsg.Text;
                if (string.IsNullOrEmpty(textToHide) || imageRaw == null) return;

                string password = await DisplayPromptAsync("Password?", "Enter a password to encrypt the message. Or leave empty to embed the message in plain text (not encrypted)");
                bool hasPassword = false;
                if (!string.IsNullOrEmpty(password))
                {
                    hasPassword = true;
                    textToHide = Crypto.EncryptStringAES(textToHide, password);
                }
                steg = DependencyService.Get<IStegImage>();
                steg.Init(imageRaw);
                if (steg.Width * steg.Height < textToHide.Length + 8)
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
                        if (i == 0)
                        {
                            if (j < 8)
                            {
                                if (j < 3)
                                    steg.SetPixel(i, j, new ColorByte(pixel.R, pixel.G, App.AppStamp[j]));
                                else if (j == 3)
                                    steg.SetPixel(i, j, new ColorByte(pixel.R, pixel.G, hasPassword ? 1 : 0));
                                else
                                {
                                    byte[] lengthBytes = BitConverter.GetBytes(textToHide.Length);
                                    if (lengthBytes.Length > 4)
                                        await DisplayAlert("Message is too larg", "Try shrinking the message or encode more photos", "OK");
                                    else
                                        steg.SetPixel(i, j, new ColorByte(pixel.R, pixel.G, lengthBytes[j - 4]));
                                }
                                continue;
                            }
                        }

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
                var fileName = DependencyService.Get<IPicture>().SavePictureToDisk("21Caesar", steg.Save());
                var pwdMesg = hasPassword ? "they can't read the message without knowing the password, " : null;
                await DisplayAlert("Your secret has been embeded successfully",
                    $"You can now share it with anybody, {pwdMesg}the image name: {fileName}", "OK");
                hideBtn.IsEnabled = entryMsg.Text.Length > 0 && imageRaw != null;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Exception", ex.ToString(), "OK");
            }
        }

        private void EntryMsg_TextChanged(object sender, TextChangedEventArgs e)
        {
            hideBtn.IsEnabled = entryMsg.Text.Length > 0 && imageRaw != null;
        }
    }
}