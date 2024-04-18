using Classifieds.Data.DTOs;
using Classifieds.Data.Entities;

namespace Classifieds.Services.IServices
{
    public interface IPostService
    {
        Task<Post> AddAsync(AddPostDto dto);
    }
}
