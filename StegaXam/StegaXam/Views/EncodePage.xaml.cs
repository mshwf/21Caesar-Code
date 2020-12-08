﻿using StegaXam.Models;
using StegaXam.Services;
using System;
using System.IO;
using Xamarin.Forms;

namespace StegaXam.Views
{
    public partial class EncodePage : ContentPage
    {
        public EncodePage()
        {
            InitializeComponent();
        }
        byte[] bytes;
        private async void OnPickPhotoButtonClicked(object sender, EventArgs e)
        {
            (sender as Button).IsEnabled = false;

            Stream stream = await DependencyService.Get<IPhotoPickerService>().GetImageStreamAsync();
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            if (stream != null)
            {
                image.Source = ImageSource.FromStream(() => new MemoryStream(memoryStream.ToArray()));
                using (var ms = new MemoryStream())
                {
                    //await stream.CopyToAsync(ms);
                    bytes = memoryStream.ToArray();
                }
            }
            (sender as Button).IsEnabled = true;
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

        }
        string textToHide;
        int currentChar = 0;
        private IStegImage steg;

        private void HideClick(object sender, EventArgs e)
        {
            steg = DependencyService.Get<IStegImage>();
            steg.Init(bytes);

            textToHide = text.Text;

            //W=4032, H=3024
            bool _break = false;
            for (int i = 0; i < steg.Width; i++)
            {
                for (int j = 0; j < steg.Height; j++)
                {
                    ColorByte pixel = steg.GetPixel(i, j);
                    if (currentChar < textToHide.Length)
                    {
                        int ch = textToHide[currentChar++];
                        steg.SetPixel(i, j, new ColorByte(pixel.R, pixel.G, ch));
                    }
                    else
                    {
                        _break = true;
                        break;
                    }
                }
                if (_break) break;
            }
            int lastX = steg.Width - 1;
            int lastY = steg.Height - 1;
            ColorByte lastPixel = steg.GetPixel(lastX, lastY);

            int length = textToHide.Length;
            steg.SetPixel(lastX, lastY, new ColorByte(lastPixel.R, lastPixel.G, length));
        }

        private void SaveClicked(object sender, EventArgs e)
        {
            DependencyService.Get<IPicture>().SavePictureToDisk("ChartImage", steg.Save());
        }
    }
}