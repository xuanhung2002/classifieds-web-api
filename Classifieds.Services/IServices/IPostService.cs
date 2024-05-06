using Classifieds.Data.DTOs;
using Classifieds.Data.Entities;
using Classifieds.Repository;

namespace Classifieds.Services.IServices
{
    public interface IPostService
    {
        Task<List<PostDto>> GetAllAsync();
        Task<TableInfo<PostDto>> GetPagingAsync(TablePageParameter parameter);
        Task<PostDto> GetByIdAsync(Guid id);
        Task<Post> AddAsync(PostAddDto dto);
        Task<Post> UpdateAsync(PostUpdateDto dto);
        Task<int> DeleteAsync(Guid id);

        Task OpenAuction(OpenAuctionDto dto, Guid userId);
        Task CloseAuction(Guid id, Guid userId);
    }
}
