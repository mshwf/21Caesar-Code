using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace StegaXam.Models
{
   public interface IStegImage
    {
        byte[] Raw { get; set; }
        int Width { get; set; }
        int Height { get; set; }

        void SetPixel(int x, int y, Color color);

        Color GetPixel(int x, int y);
    }
}
