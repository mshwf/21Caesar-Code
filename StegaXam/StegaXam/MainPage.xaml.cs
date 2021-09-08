using StegaXam.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private void NaveToDecode(object sender, EventArgs e)
        {
            Navigation.PushAsync(new DecodePage());
        }

        private void NavToEncode(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EncodePage());
        }
        bool isExpanded = false;
        private void Info_Tapped(object sender, EventArgs e)
        {
            uint animLength = 500;
            var widthAnimation = new Animation();
            var heightAnimation = new Animation();
            var infoColorAnimation = new Animation();
            if (!isExpanded)
            {
                widthAnimation = new Animation(value => frameInfo.WidthRequest = value,
        start: frameInfo.WidthRequest,
        end: 500);

                heightAnimation = new Animation(value => frameInfo.HeightRequest = value,
        start: frameInfo.HeightRequest,
        end: 200);
                infoIconImage.ColorTo((Color)Application.Current.Resources["PrimaryLight"], (Color)Application.Current.Resources["PrimaryDark"],
                    c => infoIcon.Color = c, animLength, easing: Easing.CubicOut);
            }
            else
            {
                widthAnimation = new Animation(value => frameInfo.WidthRequest = value,
start: frameInfo.WidthRequest,
end: 50);

                heightAnimation = new Animation(value => frameInfo.HeightRequest = value,
start: frameInfo.HeightRequest,
end: 50);
            infoIconImage.ColorTo((Color)Application.Current.Resources["PrimaryDark"], (Color)Application.Current.Resources["PrimaryLight"], 
                c => infoIcon.Color = c, animLength, easing: Easing.CubicOut);

            }
            frameInfo.Animate("anim", widthAnimation, length: animLength, easing: Easing.CubicOut);
            frameInfo.Animate("anim2", heightAnimation, length: animLength, easing: Easing.CubicOut);
            isExpanded = !isExpanded;
        }
    }
}