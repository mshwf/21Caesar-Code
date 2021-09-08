using Steganography;
using StegaXam.Models;
using StegaXam.ViewModels;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace StegaXam.Views
{
    public partial class DecodePage : ContentPage
    {
        DecodeViewModel _viewModel;

        public DecodePage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new DecodeViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }

        private IStegImage steg;
        bool _break = false;

        private async void RevealMessage_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (picker.ImageData == null)
                {
                    await DisplayAlert(null, "First, pick up the photo that contains the secret message", "OK");
                    return;
                }
                steg = DependencyService.Get<IStegImage>();
                steg.Init(picker.ImageData);

                if (!(LengthCheck(steg) && StampCheck(steg) && PasswordCheck(steg)))
                {
                    await DisplayAlert("No secrets", "No secrets can be found in this image", "OK");
                    return;
                }
                var hasPassword = steg.GetPixel(0, 3).B == 1;

                var txtLength = BitConverter.ToInt32(new byte[] {
                (byte)steg.GetPixel(0, 4).B,
                (byte)steg.GetPixel(0, 5).B,
                (byte)steg.GetPixel(0, 6).B,
                (byte)steg.GetPixel(0, 7).B}, 0);

                string txt = "";
                for (int i = 0; i < steg.Width; i++)
                {
                    for (int j = 0; j < steg.Height; j++)
                    {
                        if (i == 0 && j < 8) continue;
                        if (txt.Length == txtLength)
                        {
                            _break = true;
                            break;
                        }
                        txt += (char)steg.GetPixel(i, j).B;
                    }
                    if (_break) break;
                }
                if (hasPassword)
                {
                    string password = await DisplayPromptAsync("Password required", null);
                    if (string.IsNullOrEmpty(password))
                    {
                        if (password == "")
                            await DisplayAlert(null, "Password is required", "OK");
                        return;
                    }
                    try
                    {
                        txt = Crypto.DecryptStringAES(txt, password);
                    }
                    catch (Exception)
                    {
                        await DisplayAlert("Wrong password ✘", "You didn't enter the correct password", "OK");
                        return;
                    }
                }
                else
                {
                    txt = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(txt));
                }

                bool copy = await DisplayAlert("👁", txt, "Copy", "Cancel");
                if (copy)
                {
                    await Clipboard.SetTextAsync(txt);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Exception", ex.ToString(), "OK");
            }
        }

        private bool LengthCheck(IStegImage steg) => steg.Width * steg.Height > 8;

        private bool StampCheck(IStegImage steg)
        => steg.GetPixel(0, 0).B == App.AppStamp[0] ||
            steg.GetPixel(0, 1).B == App.AppStamp[1] ||
            steg.GetPixel(0, 2).B == App.AppStamp[2];
        private bool PasswordCheck(IStegImage steg) => steg.GetPixel(0, 3).B == 0 || steg.GetPixel(0, 3).B == 1;
    }
}