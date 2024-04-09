using Classifieds.Services.IServices;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Services.Services
{
    public class ImageService : IImageService
    {
        private readonly Cloudinary _cloudinary;

        public ImageService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public ImageUploadResult? UploadFile(Stream fileStream, string fileName)
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileName, fileStream),
            };

            var uploadResult = _cloudinary.Upload(uploadParams);

            return uploadResult;
        }

        public bool DeleteFile(string publicId)
        {
            var deletionParams = new DeletionParams(publicId);
            var deletionResult = _cloudinary.Destroy(deletionParams);

            if (deletionResult.Result == "ok")
            {
                return true;
            }

            return false;
        }
    }
}
