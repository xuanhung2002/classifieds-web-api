using Classifieds.Data.DTOs;
using Classifieds.Data.DTOs.GoogleAuthDtos;
using Classifieds.Data.DTOs.UserDtos;

namespace Classifieds.Services.IServices
{
    public interface IAuthService
    {
        Task<bool> Register(RegisterDto registerDto);
        Task<string> Login(LoginDto loginDto);
        Task<string> LoginWithGoogle(GoogleLoginRequest model);
        Task ForgotPassword(ForgotPasswordRequest model);
        Task ResetPassword(ResetPasswordRequest model);
    }
}
