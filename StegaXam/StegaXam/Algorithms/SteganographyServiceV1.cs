using Steganography;
using StegaXam.Models;
using StegaXam.Services;
using System;
using System.Text;
using Xamarin.Forms;

namespace StegaXam.Algorithms
{
    class SteganographyServiceV1 : ISteganography
    {
        int currentChar = 0;

        public byte[] Encode(byte[] imageData, string secretMessage, string password)
        {
            IMetaImage metaImage = DependencyService.Get<IMetaImage>();

            bool hasPassword = false;
            if (string.IsNullOrEmpty(password))
            {
                secretMessage = Convert.ToBase64String(Encoding.UTF8.GetBytes(secretMessage));
            }
            else
            {
                hasPassword = true;
                secretMessage = Crypto.EncryptStringAES(secretMessage, password);
            }
            metaImage.Init(imageData);
            if (metaImage.Width * metaImage.Height < secretMessage.Length + 8)
            {
                throw new Exception("The image is too small for the message, choose bigger one.");
            }
            bool _break = false;
            for (int i = 0; i < metaImage.Width; i++)
            {
                for (int j = 0; j < metaImage.Height; j++)
                {
                    ColorByte pixel = metaImage.GetPixel(i, j);
                    if (i == 0 && j < 8)
                    {
                        if (j < 3)
                            metaImage.SetPixel(i, j, new ColorByte(pixel.R, pixel.G, App.AppStamp[j]));
                        else if (j == 3)
                            metaImage.SetPixel(i, j, new ColorByte(pixel.R, pixel.G, hasPassword ? 1 : 0));
                        else
                        {
                            byte[] lengthBytes = BitConverter.GetBytes(secretMessage.Length);
                            metaImage.SetPixel(i, j, new ColorByte(pixel.R, pixel.G, lengthBytes[j - 4]));
                        }
                        continue;
                    }

                    if (currentChar < secretMessage.Length)
                    {
                        int ch = secretMessage[currentChar++];
                        metaImage.SetPixel(i, j, new ColorByte(pixel.R, pixel.G, ch));
                    }
                    else
                    {
                        _break = true;
                        break;
                    }
                }
                if (_break) break;
            }
            return metaImage.Save();
        }
        public string Decode(byte[] imageData, string password)
        {
            IMetaImage steg = DependencyService.Get<IMetaImage>();
            steg.Init(imageData);

            var txtLength = BitConverter.ToInt32(new byte[] {
                (byte)steg.GetPixel(0, 4).B,
                (byte)steg.GetPixel(0, 5).B,
                (byte)steg.GetPixel(0, 6).B,
                (byte)steg.GetPixel(0, 7).B}, 0);

            string secret = "";
            bool _break = false;
            for (int i = 0; i < steg.Width; i++)
            {
                for (int j = 0; j < steg.Height; j++)
                {
                    if (i == 0 && j < 8) continue;
                    if (secret.Length == txtLength)
                    {
                        _break = true;
                        break;
                    }
                    secret += (char)steg.GetPixel(i, j).B;
                }
                if (_break) break;
            }
            if (string.IsNullOrEmpty(password))
                secret = Encoding.UTF8.GetString(Convert.FromBase64String(secret));
            else
            {
                try
                {
                    secret = Crypto.DecryptStringAES(secret, password);
                }
                catch (Exception)
                {
                    throw new IncorrectPasswordException();
                }
            }
            return secret;
        }
        public bool CheckIntegrity(byte[] imageData, out bool hasPassword)
        {
            IMetaImage steg = DependencyService.Get<IMetaImage>();
            steg.Init(imageData);

            if (!(LengthCheck(steg) && StampCheck(steg) && PasswordCheck(steg)))
            {
                hasPassword = false;
                return false;
            }
            hasPassword = steg.GetPixel(0, 3).B == 1;
            return true;
        }

        private bool LengthCheck(IMetaImage steg) => steg.Width * steg.Height > 8;

        private bool StampCheck(IMetaImage steg)
        => steg.GetPixel(0, 0).B == App.AppStamp[0] ||
            steg.GetPixel(0, 1).B == App.AppStamp[1] ||
            steg.GetPixel(0, 2).B == App.AppStamp[2];
        private bool PasswordCheck(IMetaImage steg) => steg.GetPixel(0, 3).B == 0 || steg.GetPixel(0, 3).B == 1;
    }

    class IncorrectPasswordException : Exception
    {
    }
}