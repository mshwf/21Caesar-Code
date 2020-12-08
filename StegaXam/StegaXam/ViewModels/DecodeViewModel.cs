using StegaXam.Models;
using StegaXam.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StegaXam.ViewModels
{
    public class DecodeViewModel : BaseViewModel
    {
        public DecodeViewModel()
        {
            Title = "Decode";
        }

        public void OnAppearing()
        {
        }
    }
}