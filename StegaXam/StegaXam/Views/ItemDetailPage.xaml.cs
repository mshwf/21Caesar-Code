using StegaXam.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace StegaXam.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}