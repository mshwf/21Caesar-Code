using Android.Content;
using Android.Graphics;
using Java.IO;
using Java.Nio;
using StegaXam.Droid;
using StegaXam.Models;
using System;
using System.IO;
using Xamarin.Forms;
using Color = Android.Graphics.Color;

[assembly: Dependency(typeof(StegImage))]
namespace StegaXam.Droid
{
    class StegImage : IStegImage
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        Bitmap bitmap;
        public ColorByte GetPixel(int x, int y)
        {
            var pixel = bitmap.GetPixel(x, y);
            var r = Color.GetRedComponent(pixel);
            var g = Color.GetGreenComponent(pixel);
            var b = Color.GetBlueComponent(pixel);
            return new ColorByte(r, g, b);
        }

        public void SetPixel(int x, int y, ColorByte color)
        {
            bitmap.SetPixel(x, y, Color.Rgb(color.R, color.G, color.B));
        }
        public void Init(byte[] raw)
        {
            bitmap = BitmapFactory.DecodeByteArray(raw, 0, raw.Length);
            bitmap = bitmap.Copy(bitmap.GetConfig(), true);
            Width = bitmap.Width;
            Height = bitmap.Height;
        }

        public byte[] Save()
        {
            using var stream = new MemoryStream();
            bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
            byte[] byteArray = stream.ToArray();
            return byteArray;
            //ByteBuffer byteBuffer = ByteBuffer.Allocate(bitmap.ByteCount);
            //bitmap.CopyPixelsToBuffer(byteBuffer);
            //byte[] bytes = byteBuffer.ToArray<byte>();
            //return bytes;
        }
    }
}