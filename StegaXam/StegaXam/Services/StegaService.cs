using StegaXam.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace StegaXam.Services
{
    class StegaService : IStega
    {
        Image image;
        int currentChar = 0;
        public IStegImage Bury(IStegImage image, string secret)
        {
            currentChar = 0;
            if (image == null || string.IsNullOrWhiteSpace(secret))
            {
                Application.Current.MainPage.DisplayAlert("Failed", "Choose image and type text", "OK");
                return null;
            }
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    ColorByte pixel = image.GetPixel(i, j);
                    if (currentChar < secret.Length)
                    {
                        int ch = secret[currentChar++];
                        image.SetPixel(i, j, new ColorByte(pixel.R, pixel.G, ch));
                    }
                    if (i == image.Width - 1 && j == image.Height - 1)
                        image.SetPixel(i, j, new ColorByte(pixel.R, pixel.G, secret.Length));
                }
            }
            Application.Current.MainPage.DisplayAlert("Success", "Image Saved Successfully", "OK");
            return image;
        }
    }
}
