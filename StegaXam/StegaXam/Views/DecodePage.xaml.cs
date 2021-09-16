using StegaXam.Services;
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

        public DecodePage(byte[] imageData) : this()
        {
            picker.LoadImage(imageData);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }

        private async void RevealMessage_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (picker.ImageData == null)
                {
                    await DisplayAlert(null, "First, pick up the photo that contains the secret message", "OK");
                    return;
                }
                var algo = SteganographyAlgorithm.Build(picker.ImageData);
                if (algo == null)
                {
                    await DisplayAlert("No secrets", "No secrets can be found in this image", "OK");
                    return;
                }
                string password = string.Empty;

                if (algo.Header.HasPassword)
                {
                    password = await DisplayPromptAsync("Password required", null);
                    if (string.IsNullOrEmpty(password))
                    {
                        if (password == "") await DisplayAlert(null, "Password is required", "OK");
                        return;
                    }
                }
                string secret = string.Empty;
                try
                {
                    secret = algo.Decode(password);
                }
                catch (IncorrectPasswordException)
                {
                    await DisplayAlert("Wrong password ✘", "You didn't enter the correct password", "OK");
                    return;
                }

                bool copy = await DisplayAlert("👁", secret, "Copy", "Cancel");
                if (copy) await Clipboard.SetTextAsync(secret);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Exception", ex.ToString(), "OK");
            }
        }
    }
}