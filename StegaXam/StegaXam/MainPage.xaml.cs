using StegaXam.Views;
using System;
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
            //Application.Current.Properties.TryGetValue(AppConstants.FirstVisit, out object firstVisit);
            //if ((bool?)firstVisit != false)
            //{
            //    Application.Current.Properties[AppConstants.FirstVisit] = false;
            //}
            //else if (isExpanded)
            //{
            //    AnimateInfoSection(false);
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
        static readonly Color primaryLight = (Color)Application.Current.Resources["PrimaryLight"];
        static readonly Color secondary = (Color)Application.Current.Resources["Secondary"];
        private void AnimateInfoSection()
        {
            if (fSize == null) return;
            var size = fSize.Value;
            uint animLength = 500;
            Animation widthAnimation;
            Animation heightAnimation;
            if (!isExpanded)
            {
                Expand(animLength, size, out widthAnimation, out heightAnimation);
            }
            else
            {
                Collapse(animLength, out widthAnimation, out heightAnimation);
            }

            frameInfo.Animate("w_anim", widthAnimation, length: animLength, easing: Easing.CubicOut);
            frameInfo.Animate("h_anim", heightAnimation, length: animLength, easing: Easing.CubicOut);
            isExpanded = !isExpanded;
        }

        private void Collapse(uint animLength, out Animation widthAnimation, out Animation heightAnimation)
        {
            widthAnimation = new Animation(value => frameInfo.WidthRequest = value,
                        start: frameInfo.Width,
                        end: 50);

            heightAnimation = new Animation(value => frameInfo.HeightRequest = value,
                        start: frameInfo.Height,
                        end: 50);
            infoIconImage.ColorTo(secondary, primaryLight, c => infoIcon.Color = c, animLength, easing: Easing.CubicOut);
        }

        private void Expand(uint animLength, Size size, out Animation widthAnimation, out Animation heightAnimation)
        {
            widthAnimation = new Animation(value => frameInfo.WidthRequest = value,
    start: frameInfo.WidthRequest,
    end: size.Width);

            heightAnimation = new Animation(value => frameInfo.HeightRequest = value,
    start: frameInfo.HeightRequest,
    end: size.Height);
            infoIconImage.ColorTo(primaryLight, secondary, c => infoIcon.Color = c, animLength, easing: Easing.CubicOut);
        }

        private void frameInfo_SizeChanged(object sender, EventArgs e)
        {
            if (fSize == null)
                fSize = new Size(frameInfo.Width, frameInfo.Height);
        }
    }
}