using System.IO;
using System.Threading.Tasks;

namespace StegaXam.Services
{

    public interface IPhotoPickerService
    {
        Task<Stream> GetImageStreamAsync();
    }
}
