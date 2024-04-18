using Classifieds.Services.IServices;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;

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

        public string UploadImage(Stream image)
        {
            var uploadResult = UploadFile(image, Guid.NewGuid().ToString());
            return uploadResult != null ? uploadResult.Url.ToString() : string.Empty;
        }

    }
}
