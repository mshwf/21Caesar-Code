using System;

namespace StegaXam.Models
{
    //[6 (Stamp) | 1 (hasPassword) | 4 (msgLength) | 1 (Version)]
    class HeaderMetaData
    {
        public long AppStamp { get; set; }//6
        public bool HasPassword { get; set; }//1
        public int MessageLength { get; set; }//4
        public int AlgorithmVersion { get; set; }//1

        public const int Size = 12;

        public static byte[] BuildHeader(bool hasPassword, int msgLength, int version)
        {
            var passwordByte = (byte)(hasPassword ? 1 : 0);
            var msgLengthBytes = BitConverter.GetBytes(msgLength);
            byte[] header = new byte[Size];
            Array.Copy(App.AppStamp, header, App.AppStamp.Length);
            header[App.AppStamp.Length] = passwordByte;
            Array.Copy(msgLengthBytes, 0, header, App.AppStamp.Length + 1, msgLengthBytes.Length);
            header[Size - 1] = (byte)version;
            return header;
        }

        public static HeaderMetaData CheckHeader(IMetaImage metaImage)
        {
            int iteration = 0;
            byte[] appStamp = new byte[App.AppStamp.Length];
            byte? hasPassword = null;
            byte? version = null;
            byte[] messageLength = new byte[4];
            for (int i = 0; i < metaImage.Width; i++)
            {
                if (iteration >= Size) break;
                for (int j = 0; j < metaImage.Height; j++)
                {
                    if (iteration < Size)
                    {
                        var pixel = metaImage.GetPixel(i, j);

                        if (iteration < App.AppStamp.Length)
                            appStamp[iteration] = (byte)pixel.B;
                        else if (iteration == App.AppStamp.Length)
                            hasPassword = (byte)pixel.B;
                        else if (iteration > App.AppStamp.Length && iteration < Size - 1)
                            messageLength[iteration - (App.AppStamp.Length + 1)] = (byte)pixel.B;
                        else
                            version = (byte)pixel.B;
                        iteration++;
                    }
                    else break;
                }
            }

            if (!StampCheck(appStamp) ||
                metaImage.Width * metaImage.Height < Size + 1 ||
                hasPassword == null || hasPassword > 1 ||
                version == null)
                return null;

            var header = new HeaderMetaData
            {
                AlgorithmVersion = version.Value,
                HasPassword = hasPassword == 1,
                MessageLength = BitConverter.ToInt32(messageLength, 0)
            };

            return header;
        }

        private static bool StampCheck(byte[] appStamp)
        {
            if (appStamp.Length != App.AppStamp.Length) return false;
            for (int i = 0; i < App.AppStamp.Length; i++)
            {
                if (appStamp[i] != App.AppStamp[i]) return false;
            }
            return true;
        }
    }
}