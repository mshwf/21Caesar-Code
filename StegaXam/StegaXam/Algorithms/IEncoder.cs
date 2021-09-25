using StegaXam.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StegaXam.Algorithms
{
    public interface IEncoder
    {
        IMetaImage Encode(IMetaImage metaImage);
    }
}
