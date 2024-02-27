using Classifieds.Data.DTOs;

namespace Services.IServices
{
    public interface IAuthService
    {
        Task<Boolean> Register(RegisterDto registerDto);
        Task<String> Login(LoginDto loginDto);
    }
}
