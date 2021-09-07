using StegaXam.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var widthAnimation = new Animation();
            var heightAnimation = new Animation();
            if (!isExpanded)
            {
                widthAnimation = new Animation(value => frameInfo.WidthRequest = value,
        start: frameInfo.WidthRequest,
        end: 500);

                heightAnimation = new Animation(value => frameInfo.HeightRequest = value,
        start: frameInfo.HeightRequest,
        end: 200);
            }
            else
            {
                widthAnimation = new Animation(value => frameInfo.WidthRequest = value,
start: frameInfo.WidthRequest,
end: 50);

                heightAnimation = new Animation(value => frameInfo.HeightRequest = value,
start: frameInfo.HeightRequest,
end: 50);

            }
            frameInfo.Animate("anim", widthAnimation, length: 500, easing: Easing.CubicOut);
            frameInfo.Animate("anim2", heightAnimation, length: 500, easing: Easing.CubicOut);
            isExpanded = !isExpanded;
        }
    }
}