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
        ImageUploadResult? UploadFile(Stream fileStream, string fileName);
        bool DeleteFile(string publicId);
        public string UploadImage(Stream stream);
    }
}
