using Classifieds.Data.DTOs;
using Classifieds.Data.Entities;
using Classifieds.Repository;
using Classifieds.Services.IServices;

namespace Classifieds.Services.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IDBRepository _repository;
        private readonly IImageService _imageService;

        public CategoryService(IDBRepository repository, IImageService imageService)
        {
            _repository = repository;
            _imageService = imageService;
        }
        public async Task<Category> AddAsync(AddCategoryRequest dto)
        {
            using var stream = dto.Image.OpenReadStream();
            var imageUrl = _imageService.UploadImage(stream);
            var entity = new Category
            {
                Name = dto.Name,
                Image = imageUrl,
            };
            return await _repository.AddAsync(entity);
        }

        public async Task<int> DeleteAsync(Guid id)
        {
            var entity = _repository.FindAsync<Category>(s => s.Id == id);
            return await _repository.DeleteAsync(entity);
        }
    }
}
