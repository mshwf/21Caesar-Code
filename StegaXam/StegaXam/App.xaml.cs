using StegaXam.Algorithms;
using StegaXam.Services;
using Xamarin.Forms;

[assembly: ExportFont("FontAwesome5Regular-400.otf", Alias = "FontAwesome5Regular")]
[assembly: ExportFont("FontAwesome5Solid-900.otf", Alias = "FontAwesome5Solid")]
[assembly: ExportFont("HammersmithOne-Regular.ttf", Alias = "HammersmithOneRegular")]
namespace StegaXam
{
    public partial class App : Application
    {
        internal static byte[] AppStamp = new byte[6] { 0x83, 0xfc, 0x0a, 0xcc, 0x1f, 0xda };
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage());
            SteganographyAlgorithm.Register(new SteganographyServiceV1());
            //SteganographyAlgorithm.Register(new LSBServiceV2());
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