using StegaXam.Views;
using System;
using StegaXam.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace StegaXam
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            Application.Current.Properties.TryGetValue(AppConstants.FirstVisit, out object firstVisit);
            if ((bool?)firstVisit != false)
            {
                Application.Current.Properties[AppConstants.FirstVisit] = false;
            }
            //else
            //{
            //    AnimateInfoSection();
            //}

            txtVersion.Text = $"Version: {Xamarin.Essentials.VersionTracking.CurrentVersion}";
            base.OnAppearing();
        }

        private void NaveToDecode(object sender, EventArgs e)
        {
            Navigation.PushAsync(new DecodePage());
        }

        private void NavToEncode(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EncodePage());
        }
        private void Info_Tapped(object sender, EventArgs e)
        {
            AnimateInfoSection();
        }
        static Size? fSize = null;
        bool isExpanded = true;
        private void AnimateInfoSection()
        {
            if (fSize == null)
                fSize = new Size(frameInfo.Width, frameInfo.Height);
            var size = fSize.Value;
            uint animLength = 500;
            Animation widthAnimation;
            Animation heightAnimation;
            if (!isExpanded)
                Expand(animLength, size, out widthAnimation, out heightAnimation);
            else
                Collapse(animLength, out widthAnimation, out heightAnimation);

            frameInfo.Animate("w_anim", widthAnimation, length: animLength, easing: Easing.CubicOut);
            frameInfo.Animate("h_anim", heightAnimation, length: animLength, easing: Easing.CubicOut);
            isExpanded = !isExpanded;
        }

        private void Collapse(uint animLength, out Animation widthAnimation, out Animation heightAnimation)
        {
            widthAnimation = new Animation(value => frameInfo.WidthRequest = value,
            start: frameInfo.WidthRequest,
            end: 50);

            heightAnimation = new Animation(value => frameInfo.HeightRequest = value,
start: frameInfo.HeightRequest,
end: 50);
            infoIconImage.ColorTo((Color)Application.Current.Resources["Secondary"],
                (Color)Application.Current.Resources["PrimaryLight"],
                c => infoIcon.Color = c, animLength, easing: Easing.CubicOut);
        }

        private void Expand(uint animLength, Size size, out Animation widthAnimation, out Animation heightAnimation)
        {
            widthAnimation = new Animation(value => frameInfo.WidthRequest = value,
    start: frameInfo.WidthRequest,
    end: size.Width);

            heightAnimation = new Animation(value => frameInfo.HeightRequest = value,
    start: frameInfo.HeightRequest,
    end: size.Height);
            infoIconImage.ColorTo((Color)Application.Current.Resources["PrimaryLight"],
                (Color)Application.Current.Resources["Secondary"],
                c => infoIcon.Color = c, animLength, easing: Easing.CubicOut);
        }
    }
}