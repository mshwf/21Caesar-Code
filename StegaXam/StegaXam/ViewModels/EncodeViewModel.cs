using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace StegaXam.ViewModels
{
    public class EncodeViewModel : BaseViewModel
    {
        public EncodeViewModel()
        {
            Title = "About";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));
        }

        public ICommand OpenWebCommand { get; }
    }
}