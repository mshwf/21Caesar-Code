namespace StegaXam.Services
{
    public interface IPicture
    {
        string SavePictureToDisk(string filename, byte[] imageData);
    }
}
