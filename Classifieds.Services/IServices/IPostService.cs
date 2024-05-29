using Classifieds.Data.DTOs;
using Classifieds.Data.DTOs.PostDTOs;
using Classifieds.Data.Entities;
using Classifieds.Repository;

namespace Classifieds.Services.IServices
{
    public interface IPostService
    {
        Task<List<PostDto>> GetPostOfCurrentUser(PostOfUserRequest request);
        Task<List<PostDto>> GetAllAsync();
        Task<TableInfo<PostDto>> GetPagingAsync(PostPagingRequest request);
        Task<PostDto> GetByIdAsync(Guid id);
        Task<Post> AddAsync(PostAddDto dto);
        Task<Post> UpdateAsync(PostUpdateDto dto);
        Task<int> DeleteAsync(Guid id);

        Task OpenAuction(OpenAuctionDto dto, Guid userId);
        Task ReOpenAuction(OpenAuctionDto dto, Guid userId);
        Task CloseAuction(Guid id, Guid userId);
        Task MarkSold(Guid id);
    }
}
