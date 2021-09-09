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
    interface ISteganography
    {
        byte[] Encode(byte[] imageData, string secretMessage, string password);
        string Decode(byte[] imageData, string password);
        bool CheckIntegrity(byte[] imageData, out bool hasPassword);
    }
}