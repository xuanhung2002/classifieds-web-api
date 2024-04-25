using Classifieds.Data.DTOs;
using Classifieds.Data.Entities;

namespace Classifieds.Services.IServices
{
    public interface IPostService
    {
        Task<PostDto> GetByIdAsync(Guid id);
        Task<Post> AddAsync(PostAddDto dto);
        Task<Post> UpdateAsync(PostUpdateDto dto);
        Task<int> DeleteAsync(Guid id);
    }
}
