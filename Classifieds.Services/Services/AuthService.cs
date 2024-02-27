
using Classifieds.Data.DTOs;
using Classifieds.Services.IServices;
using Services.IServices;

namespace Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService userService;
        private readonly ITokenService tokenService;

        public AuthService(IUserService userService, ITokenService tokenService)
        {
            this.userService = userService;
            this.tokenService = tokenService;
        }
        public async Task<string> Login(LoginDto loginDto)
        {
            //loginDto.AccountName = loginDto.AccountName.ToLower();
            //var existedUser = await userService.GetUserByAccountNameAsync(loginDto.AccountName);
            //if (existedUser is null) return null;
            //using var hashFunc = new HMACSHA256(existedUser.PasswordSalt);
            //var passwordBytes = Encoding.UTF8.GetBytes(loginDto.Password);
            //var passwordHash = hashFunc.ComputeHash(passwordBytes);
            //for (int i = 0; i < passwordHash.Length; i++)
            //{
            //    if (passwordHash[i] != existedUser.PasswordHash[i])
            //        return null;
            //}
            //return tokenService.GenerateToken(existedUser);
            throw new NotImplementedException();
        }

        public async Task<bool> Register(RegisterDto registerDto)
        {

            //registerDto.AccountName = registerDto.AccountName.ToLower();
            //if (await userService.GetUserByAccountNameAsync(registerDto.AccountName) is not null)
            //    return false;

            //using var hashFunc = new HMACSHA256();
            //var passwordBytes = Encoding.UTF8.GetBytes(registerDto.Password);

            //var newUser = new User
            //{
            //    PasswordHash = hashFunc.ComputeHash(passwordBytes),
            //    PasswordSalt = hashFunc.Key
            //};
            //await userService.AddUserAsync(newUser);
            //return true;
            throw new NotImplementedException();
        }
    }
}
