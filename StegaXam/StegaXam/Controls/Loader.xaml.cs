using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace StegaXam.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Loader : Frame
    {

        public static readonly BindableProperty IsRunningProperty =
           BindableProperty.Create(nameof(IsRunning), typeof(bool), typeof(Loader), propertyChanged: IsRunningPropertyChanged);

        private static void IsRunningPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            bool isRunning = (bool)newValue;
            var control = (Loader)bindable;
            control.IsVisible = control.loader.IsRunning = isRunning;
        }

        public bool IsRunning
        {
            get => (bool)GetValue(IsRunningProperty);
            set => SetValue(IsRunningProperty, value);
        }

        public Loader()
        {
            InitializeComponent();
        }
    }
}