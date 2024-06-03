using Classifieds.Data.DTOs;
using Classifieds.Data.DTOs.UserDtos;
using Classifieds.Data.Entities;
using Classifieds.Data.Enums;
using System.Linq.Expressions;

namespace Classifieds.Services.IServices
{
    public interface IUserService
    {
        Task<UserDto> GetById(Guid id);
        Task<UserDto> GetByUsername(string username);
        Task ChangePermission(Guid id, Role role);
        Task AddAdmin(RegisterDto dto);
        Task<List<UserDto>> GetUsers(Expression<Func<User, bool>> expression);
    }
}
