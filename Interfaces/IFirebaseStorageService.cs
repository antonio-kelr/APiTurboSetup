using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APiTurboSetup.Interfaces
{
    public interface IFirebaseStorageService
    {
        Task<List<string>> UploadImagesAsync(List<IFormFile> imageFiles, string destinationPath);
        Task<string> GetSignedUrlAsync(string objectName);
    }
} 