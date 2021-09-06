using StegaXam.Services;
using StegaXam.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: ExportFont("FontAwesome5Regular-400.otf", Alias = "FontAwesome5Regular")]
[assembly: ExportFont("FontAwesome5Solid-900.otf", Alias = "FontAwesome5Solid")]

namespace StegaXam
{
    public partial class App : Application
    {
        internal static byte[] AppStamp = new byte[3] { 0x83, 0xfc, 0x0a };
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
