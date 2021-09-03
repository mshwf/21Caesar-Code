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
    }
}