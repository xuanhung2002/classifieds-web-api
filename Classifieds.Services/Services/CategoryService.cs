using AutoMapper;
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
        private readonly IMapper _mapper;

        public CategoryService(IDBRepository repository, IImageService imageService, IMapper mapper)
        {
            _repository = repository;
            _imageService = imageService;
            _mapper = mapper;
        }

        public async Task<List<CategoryDto>> GetAllAsync()
        {
            var result = (await _repository.GetAsync<Category>()).Select(s => _mapper.Map<CategoryDto>(s)).ToList();
            return result;
        }
        public async Task<Category> AddAsync(AddCategoryRequest dto)
        {
            using var stream = dto.Image.OpenReadStream();
            var imageUrl = await _imageService.UploadImage(stream);
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

        public async Task<CategoryDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.FindAsync<Category>(s => s.Id == id);
            if(entity != null)
            {
                return _mapper.Map<CategoryDto>(entity);
            }
            else
            {
                throw new InvalidDataException("Category is not existed");
            }
        }
    }
}
