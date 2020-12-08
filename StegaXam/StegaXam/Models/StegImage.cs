using System.IO;
using Xamarin.Forms;

namespace StegaXam.Models
{
    public struct ColorByte
    {
        public ColorByte(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
        }
        public int R { get; }
        public int G { get; }
        public int B { get; }
    }
    public interface IStegImage
    {
        int Width { get; }
        int Height { get; }
        void SetPixel(int x, int y, ColorByte color);
        ColorByte GetPixel(int x, int y);
        void Init(byte[] raw);
        byte[] Save();
    }
}
