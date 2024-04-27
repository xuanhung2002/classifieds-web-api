using Classifieds.Data.DTOs;

namespace Classifieds.Services.IServices
{
    public interface IUserService
    {
        Task<UserDto> GetById(Guid id);
        Task<UserDto> GetByUsername(string username);
    }
}
