using Classifieds.Data.DTOs;
using Classifieds.Data.Entities;
using Classifieds.Repository;

namespace Classifieds.Services.IServices
{
    public interface IPostService
    {
        Task<List<PostDto>> GetAllAsync();
        Task<TableInfo<PostDto>> GetPagingAsync(TableQParameter<Post> parameter);
        Task<PostDto> GetByIdAsync(Guid id);
        Task<Post> AddAsync(PostAddDto dto);
        Task<Post> UpdateAsync(PostUpdateDto dto, Guid userId);
        Task<int> DeleteAsync(Guid id);

        Task OpenAuction(OpenAuctionDto dto, Guid userId);
        Task CloseAuction(Guid id, Guid userId);
    }
}
