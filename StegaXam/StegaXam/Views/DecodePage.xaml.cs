using Steganography;
using StegaXam.Models;
using StegaXam.Services;
using StegaXam.ViewModels;
using System;
using System.IO;
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
        }

        private void ReadClick(object sender, EventArgs e)
        {

        }
        private IStegImage steg;
        bool _break = false;

        private async void ReadHiddenText_Tapped(object sender, EventArgs e)
        {
            if (imageRaw == null) return;
            steg = DependencyService.Get<IStegImage>();
            steg.Init(imageRaw);

            int lastX = steg.Width - 1;
            int lastY = steg.Height - 1;
            ColorByte lastPixel = steg.GetPixel(lastX, lastY);

            double txtLength = lastPixel.B;
            string txt = "";
            for (int i = 0; i < steg.Width; i++)
            {
                for (int j = 0; j < steg.Height; j++)
                {
                    if (txt.Length == txtLength)
                    {
                        _break = true;
                        break;
                    }
                    txt += (char)steg.GetPixel(i, j).B;
                }
                if (_break) break;
            }

            string password = await DisplayPromptAsync("Password required", null);
            if (string.IsNullOrEmpty(password))
            {
                await DisplayAlert(null, "A password is a required", "OK");
                return;
            }
            try
            {
                var plain = Crypto.DecryptStringAES(txt, password);
                bool answer = await DisplayAlert(null, plain, "Copy", "Cancel");
                if (answer)
                {
                    await Clipboard.SetTextAsync(plain);
                }
            }
            catch (Exception)
            {
                await DisplayAlert(null, "Either the password is wrong or the image doesn't have secrets.", "OK");
            }

        }
    }
}