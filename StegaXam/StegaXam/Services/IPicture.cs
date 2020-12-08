using System;
using System.Collections.Generic;
using System.Text;

namespace StegaXam.Services
{
    public interface IPicture
    {
        void SavePictureToDisk(string filename, byte[] imageData);
    }
}
