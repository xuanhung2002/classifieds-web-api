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

        public async Task<ImageUploadResult?> UploadFile(Stream fileStream, string fileName)
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileName, fileStream),
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            return uploadResult;
        }

        public async Task<bool> DeleteFile(string url)
        {
            var lastSlashIndex = url.LastIndexOf("/");
            var lastDotIndex = url.LastIndexOf(".");
            var publicId = url.Substring(lastSlashIndex + 1, lastDotIndex);

            var deletionParams = new DeletionParams(publicId);
            var deletionResult = await _cloudinary.DestroyAsync(deletionParams);

            if (deletionResult.Result == "ok")
            {
                return true;
            }

            return false;
        }

        public async Task<string> UploadImage(Stream image)
        {
            var uploadResult = await UploadFile(image, Guid.NewGuid().ToString());
            return uploadResult != null ? uploadResult.Url.ToString() : string.Empty;
        }

    }
}
