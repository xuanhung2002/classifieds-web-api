using CloudinaryDotNet.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Services.IServices
{
    public interface IImageService
    {
        Task<ImageUploadResult?> UploadFile(Stream fileStream, string fileName);
        Task<bool> DeleteFile(string url);
        Task<string> UploadImage(Stream stream);
    }
}
