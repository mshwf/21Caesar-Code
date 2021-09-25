using StegaXam.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using Xamarin.Forms;

namespace StegaXam.Algorithms
{
    abstract class SteganographyAlgorithm
    {
        public IEncoder Encrypter { get; set; }
        public IDecoder Decrypter { get; set; }
        public static Dictionary<int, SteganographyAlgorithm> AlgorithmVersions =
            new Dictionary<int, SteganographyAlgorithm>();
        public HeaderMetaData Header { get; set; }
        public IMetaImage MetaImage { get; set; }
        public SteganographyAlgorithm(byte version)
        {
            Version = version;
        }

        internal static void Register(SteganographyAlgorithm steganographyAlgorithm)
        {
            AlgorithmVersions[steganographyAlgorithm.Version] = steganographyAlgorithm;
        }

        public static SteganographyAlgorithm GetLastVersion() => AlgorithmVersions[AlgorithmVersions.Max(x => x.Key)];
        public static SteganographyAlgorithm Build(byte[] imageData)
        {
            IMetaImage metaImage = DependencyService.Get<IMetaImage>();
            metaImage.Init(imageData);

            var header = HeaderMetaData.CheckHeader(metaImage);
            if (header == null) return null;
            if (AlgorithmVersions.TryGetValue(header.AlgorithmVersion, out SteganographyAlgorithm algo))
            {
                algo.Header = header;
                algo.MetaImage = metaImage;
                return algo;
            }
            else return null;
        }

        public byte Version { get; }
        public byte[] Encode(byte[] messageArray, string secretMessage, string password)
        {
            var header = HeaderMetaData.BuildHeader(hasPassword, secretMessage.Length, Version);
            MetaImage.
            Encrypter.Encode

            

        }
        public abstract void Encodee(zzz)xsd
        public abstract string Decode(string password);
    }
}