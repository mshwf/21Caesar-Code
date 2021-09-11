using System;

namespace StegaXam.Models
{
    //[3 (Stamp) | 1 (hasPassword) | 4 (msgLength) | 1 (Version)]
    class HeaderMetaData
    {
        public int AppStamp { get; set; }//3
        public bool HasPassword { get; set; }//1
        public int MessageLength { get; set; }//4
        public int AlgorithmVersion { get; set; }//1
        public const int Size = 9;

        public static byte[] ToArray(bool hasPassword, int msgLength, int version)
        {
            byte[] header = new byte[Size];
            Array.Copy(App.AppStamp, header, App.AppStamp.Length);
            header[App.AppStamp.Length] = (byte)(hasPassword ? 1 : 0);
            Array.Copy(BitConverter.GetBytes(msgLength), 0, header, 4, 4);
            header[8] = (byte)version;
            return header;
        }

        public static HeaderMetaData CheckHeader(IMetaImage metaImage)
        {
            int iteration = 0;
            byte[] appStamp = new byte[3];
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

                        if (iteration < 3)
                            appStamp[iteration] = (byte)pixel.B;
                        else if (iteration == 3)
                            hasPassword = (byte)pixel.B;
                        else if (iteration > 3 && iteration < 8)
                            messageLength[iteration - 4] = (byte)pixel.B;
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