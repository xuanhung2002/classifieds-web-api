using Classifieds.Data.DTOs;
using Classifieds.Data.Entites;
using Classifieds.Services.IServices;
using Classifieds.UnitOfWork;
using System.Security.Cryptography;
using System.Text;

namespace Classifieds.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(ITokenService tokenService, IUnitOfWork unitOfWork)
        {
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }
        public async Task<string> Login(LoginDto loginDto)
        {
            loginDto.AccountName = loginDto.AccountName.ToLower();
            var existedUser = await _unitOfWork.Users.GetUserByAccountName(loginDto.AccountName);
            if (existedUser is null) return null;
            using var hashFunc = new HMACSHA256(existedUser.PasswordSalt);
            var passwordBytes = Encoding.UTF8.GetBytes(loginDto.Password);
            var passwordHash = hashFunc.ComputeHash(passwordBytes);
            for (int i = 0; i < passwordHash.Length; i++)
            {
                if (passwordHash[i] != existedUser.PasswordHash[i])
                    return null;
            }
            return _tokenService.GenerateToken(existedUser);
        }

        public async Task<bool> Register(RegisterDto registerDto)
        {

            registerDto.AccountName = registerDto.AccountName.ToLower();
            if (await _unitOfWork.Users.GetUserByAccountName(registerDto.AccountName) is not null)
                return false;

            using var hashFunc = new HMACSHA256();
            var passwordBytes = Encoding.UTF8.GetBytes(registerDto.Password);

            var newUser = new User
            {
                AccountName = registerDto.AccountName,
                Name = registerDto.Name,
                PhoneNumber = registerDto.PhoneNumber,
                Email = registerDto.Email,               
                PasswordHash = hashFunc.ComputeHash(passwordBytes),
                PasswordSalt = hashFunc.Key
            };
            await _unitOfWork.Users.Add(newUser);
            await _unitOfWork.Complete();
            return true;
        }
    }
}
