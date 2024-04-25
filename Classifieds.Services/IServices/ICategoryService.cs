using Classifieds.Data.DTOs;
using Classifieds.Data.Entities;

namespace Classifieds.Services.IServices
{
    public interface ICategoryService
    {
        Task<Category> AddAsync(AddCategoryRequest category);
        Task<int> DeleteAsync(Guid id);
        Task<List<CategoryDto>> GetAllAsync();
    }
}
