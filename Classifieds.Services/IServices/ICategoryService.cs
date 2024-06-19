using Classifieds.Data.DTOs;
using Classifieds.Data.DTOs.CategoryDTOs;
using Classifieds.Data.Entities;

namespace Classifieds.Services.IServices
{
    public interface ICategoryService
    {
        Task<Category> AddAsync(AddCategoryRequest category);
        Task<Category> UpdateAsync(UpdateCategoryRequest category);
        Task<int> DeleteAsync(Guid id);
        Task<List<CategoryDto>> GetAllAsync();
        Task<CategoryDto> GetByIdAsync(Guid id);
    }
}
