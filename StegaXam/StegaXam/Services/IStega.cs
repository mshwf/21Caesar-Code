using StegaXam.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
/*
1. take string from user
2. take image path from the user
3. optionally take password, to combine with the encryption algorithm
4. use symmeteric/assymetric algorithm to encrypt text 
5. take the cipher and burry in the image
6.save the image/share
**************
*Services:
1. Encryption service:
- take string return cipher
2. Steganography service (burrying the secret)
- take cipher and image return image with cipher
*/

namespace StegaXam.Services
{
    interface IStega
    {
        IStegImage Bury(IStegImage image, string secret);
    }
    interface IEncrypt
    {
        string Encrypt(string text, string password);

    }
}
