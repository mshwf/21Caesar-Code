using StegaXam.Services;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace StegaXam.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImagePicker : Frame
    {
        public event EventHandler ImageDataChanged;

        public ImagePicker()
        {
            InitializeComponent();
        }
        public static readonly BindableProperty ImageDataProperty = 
            BindableProperty.Create(nameof(ImageData), typeof(byte[]), typeof(ImagePicker), propertyChanged: ImageDataPropertyChanged);

        private static void ImageDataPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (ImagePicker)bindable;

            control.ImageDataChanged?.Invoke(control, EventArgs.Empty);
        }

        public byte[] ImageData
        {
            get => (byte[])GetValue(ImageDataProperty);
            set => SetValue(ImageDataProperty, value);
        }

        private async void OnPickPhotoButtonClicked(object sender, EventArgs e)
        {
            (sender as VisualElement).IsEnabled = false;

            Stream stream = await DependencyService.Get<IPhotoPickerService>().GetImageStreamAsync();
            if (stream == null)
            {
                (sender as VisualElement).IsEnabled = true;
                return;
            }
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            if (stream != null)
            {
                using (var ms = new MemoryStream())
                {
                    //await stream.CopyToAsync(ms);
                    ImageData = memoryStream.ToArray();
                    image.Source = ImageSource.FromStream(() => new MemoryStream(ImageData));
                }
            }
            (sender as VisualElement).IsEnabled = true;
            closeImg.IsVisible = true;
            imageIcon.IsVisible = false;
        }

        private void DismissImage_Tapped(object sender, EventArgs e)
        {
            image.Source = null;
            ImageData = null;
            imageIcon.IsVisible = true;
            closeImg.IsVisible = false;
        }
    }
}