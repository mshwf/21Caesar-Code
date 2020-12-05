using Android.Graphics;
using System;

namespace StegaXam.Droid
{
    class StegImage : Models.IStegImage
    {

        public byte[] Raw { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Width { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Height { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Bitmap bitmap;
        public Xamarin.Forms.Color GetPixel(int x, int y)
        {
            bitmap ??= BitmapFactory.DecodeByteArray(Raw, 0, Raw.Length);
            var pixel = bitmap.GetPixel(x, y);
            var r = Color.GetRedComponent(pixel) / 255d;
            var g = Color.GetGreenComponent(pixel) / 255d;
            var b = Color.GetBlueComponent(pixel) / 255d;
            return new Xamarin.Forms.Color(r, g, b);
        }

        public void SetPixel(int x, int y, Xamarin.Forms.Color color)
        {
            bitmap ??= BitmapFactory.DecodeByteArray(Raw, 0, Raw.Length);
            bitmap.SetPixel(x, y, Color.Rgb((int)(255 * color.R), (int)(255 * color.G), (int)(255 * color.B)));
        }
    }
}