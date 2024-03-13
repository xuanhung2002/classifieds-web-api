using Classifieds.Data.DTOs;

namespace Classifieds.Services.IServices
{
    public interface IAuthService
    {
        Task<bool> Register(RegisterDto registerDto);
        Task<string> Login(LoginDto loginDto);
    }
}
