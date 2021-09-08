using Steganography;
using StegaXam.Models;
using StegaXam.Services;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace StegaXam.Views
{
    public partial class EncodePage : ContentPage
    {
        public EncodePage()
        {
            InitializeComponent();
        }

        string textToHide;
        int currentChar = 0;
        private IStegImage steg;
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
                bool hasPassword = false;
                if (password == null) return;
                if (password != "")
                {
                    hasPassword = true;
                    textToHide = Crypto.EncryptStringAES(textToHide, password);
                }
                else
                {
                    textToHide = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(textToHide));
                }
                steg = DependencyService.Get<IStegImage>();
                steg.Init(picker.ImageData);
                if (steg.Width * steg.Height < textToHide.Length + 8)
                {
                    await DisplayAlert("Oops!", "The image is too small for the message, choose bigger one.", "OK");
                    return;
                }
                bool _break = false;
                for (int i = 0; i < steg.Width; i++)
                {
                    for (int j = 0; j < steg.Height; j++)
                    {
                        ColorByte pixel = steg.GetPixel(i, j);
                        if (i == 0 && j < 8)
                        {
                            if (j < 3)
                                steg.SetPixel(i, j, new ColorByte(pixel.R, pixel.G, App.AppStamp[j]));
                            else if (j == 3)
                                steg.SetPixel(i, j, new ColorByte(pixel.R, pixel.G, hasPassword ? 1 : 0));
                            else
                            {
                                byte[] lengthBytes = BitConverter.GetBytes(textToHide.Length);
                                steg.SetPixel(i, j, new ColorByte(pixel.R, pixel.G, lengthBytes[j - 4]));
                            }
                            continue;
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
                filename = DependencyService.Get<IPicture>().SavePictureToDisk("21Caesar", steg.Save());
                var pwdMesg = hasPassword ? "they can't read the message without knowing the password, " : null;
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