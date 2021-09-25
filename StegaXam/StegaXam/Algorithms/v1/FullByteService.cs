using StegaXam.Models;
using System;
using System.Text;
using Xamarin.Forms;

namespace StegaXam.Algorithms
{
    class FullByteService : SteganographyAlgorithm
    {
        public FullByteService() : base(1) { }

        public override byte[] Encode(byte[] imageData, string secretMessage, string password)
        {
            IMetaImage metaImage = DependencyService.Get<IMetaImage>();
            metaImage.Init(imageData);

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
            var header = HeaderMetaData.BuildHeader(hasPassword, secretMessage.Length, Version);

            if (metaImage.Width * metaImage.Height < secretMessage.Length + header.Length)
            {
                throw new TooSmallImageException();
            }

            bool _break = false;
            int currentChar = 0;
            int iteration = 0;
            for (int i = 0; i < metaImage.Width; i++)
            {
                for (int j = 0; j < metaImage.Height; j++)
                {
                    ColorByte pixel = metaImage.GetPixel(i, j);
                    if (iteration < header.Length)
                    {
                        metaImage.SetPixel(i, j, new ColorByte(pixel.R, pixel.G, header[iteration]));
                    }
                    else
                    {
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
                    iteration++;
                }
                if (_break) break;
            }
            return metaImage.Save();
        }
        public override string Decode(string password)
        {
            string secret = "";
            bool _break = false;
            int iteration = 0;
            for (int i = 0; i < MetaImage.Width; i++)
            {
                for (int j = 0; j < MetaImage.Height; j++)
                {
                    if (iteration < HeaderMetaData.Size)
                    {
                        iteration++;
                    }
                    else
                    {
                        if (secret.Length == Header.MessageLength)
                        {
                            _break = true;
                            break;
                        }
                        secret += (char)MetaImage.GetPixel(i, j).B;
                    }
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
    }
}