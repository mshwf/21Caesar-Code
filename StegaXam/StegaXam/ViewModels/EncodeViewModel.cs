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
            Title = "Encode";
        }

        public ICommand Encode { get; set; } = new Command(OnEncode);

        private static void OnEncode()
        {
            
        }
    }
}