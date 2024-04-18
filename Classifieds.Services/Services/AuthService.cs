using Classifieds.Data.DTOs;
using Classifieds.Data.DTOs.GoogleAuthDtos;
using Classifieds.Data.DTOs.UserDtos;
using Classifieds.Data.Entities;
using Classifieds.Data.Models;
using Classifieds.Repository;
using Classifieds.Services.IServices;
using Common;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace Classifieds.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly IDBRepository _repository;
        private readonly IEmailService _emailService;

        public AuthService(ITokenService tokenService, IDBRepository repository, IEmailService emailService)
        {
            _tokenService = tokenService;
            _repository = repository;
            _emailService = emailService;
        }
        public async Task<string> Login(LoginDto loginDto)
        {
            loginDto.AccountName = loginDto.AccountName.ToLower();
            var existedUser = await _repository.FindAsync<User>(s => s.AccountName == loginDto.AccountName);
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
            if (await _repository.FindAsync<User>(s => s.AccountName == registerDto.AccountName) is not null)
                return false;

            using var hashFunc = new HMACSHA256();
            var passwordBytes = Encoding.UTF8.GetBytes(registerDto.Password);

            var newUser = new User
            {
                AccountName = registerDto.AccountName,
                Name = registerDto.Name,
                PhoneNumber = registerDto.PhoneNumber,
                Email = registerDto.Email,
                Role = Role.User,
                PasswordHash = hashFunc.ComputeHash(passwordBytes),
                PasswordSalt = hashFunc.Key
            };
            await _repository.AddAsync(newUser);
            return true;
        }

        private GoogleUser ValidateGoogleToken(string googleToken)
        {
            using (HttpClient client = new HttpClient())
            {
                // Set up the request to Google's token validation endpoint
                var requestUri = $"https://www.googleapis.com/oauth2/v3/tokeninfo?id_token={googleToken}&client_id={AppSettings.GoogleClientId}";

                var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                // Send the request and get the response
                var response = client.SendAsync(request).Result;

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Parse the response content
                    var responseContent = response.Content.ReadAsStringAsync().Result;

                    // Deserialize the JSON response to retrieve user information
                    var googleUser = JsonConvert.DeserializeObject<GoogleUser>(responseContent);

                    return googleUser;
                }
                else
                {
                    throw new AppException($"Google token validation failed. Status code: {response.StatusCode}");
                }
            }
        }
        public async Task<string> LoginWithGoogle(GoogleLoginRequest model)
        {
            var googleUser = ValidateGoogleToken(model.TokenId);
            var existingUser = await _repository.FindForUpdateAsync<User>(u => u.Email == googleUser.Email);
            if(existingUser == null)
            {
                // User doesn't exist, create a new user
                existingUser = CreateUserFromGoogleData(googleUser);
                await _repository.AddAsync(existingUser);
            }
            else
            {
                existingUser = UpdateUserFromGoogleData(existingUser, googleUser);
                var user = await _repository.UpdateAsync(existingUser);
            }

            // authentication successful so generate jwt
            var token = _tokenService.GenerateToken(existingUser);
            return token;
        }

        private User CreateUserFromGoogleData(GoogleUser googleUser)
        {
            using var hashFunc = new HMACSHA256();
            var passwordBytes = Encoding.UTF8.GetBytes("GoogleStrongPass");
            var newUser = new User
            {
                Name = googleUser.Given_Name + googleUser.Family_Name,
                AccountName = googleUser.Email,
                Email = googleUser.Email,
                Role = Role.User,
                CreatedAt = DateTime.UtcNow,
                Avatar = googleUser.Picture,
                PasswordHash = new HMACSHA256().ComputeHash(passwordBytes),
                PasswordSalt = hashFunc.Key

            };
            return newUser;
        }

        private User UpdateUserFromGoogleData(User existingUser, GoogleUser googleUser)
        {
            existingUser.Email = googleUser.Email;
            existingUser.Name = googleUser.Given_Name;
            existingUser.AccountName = googleUser.Email;
            existingUser.Avatar = googleUser.Picture;
            return existingUser;
        }

        public async Task ForgotPassword(ForgotPasswordRequest model)
        {
            var user = await _repository.FindForUpdateAsync<User>(s => s.Email == model.Email);
            if (user == null) return;

            user.ResetToken = await GenerateResetToken();
            user.ResetTokenExpires = DateTime.UtcNow.AddDays(1);
            await _repository.UpdateAsync(user);
            // send email;
            await SendPasswordResetEmail(user);
        }
        private async Task<string> GenerateResetToken()
        {
            // token is a cryptographically strong random sequence of values
            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

            // ensure token is unique by checking against db
            var tokenIsUnique = (await _repository.FindAsync<User>(s => s.ResetToken == token)) == null ;
            if (!tokenIsUnique)
                return await GenerateResetToken();

            return token;
        }

        private async Task SendPasswordResetEmail(User user/*, string origin*/)
        {
            var directory = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            var filePath = Path.Combine(directory, "Common/HtmlEmails/PasswordReset.html");
            string message = await System.IO.File.ReadAllTextAsync(filePath);
            message = message.Replace("[[name]]", user.Email);

            //if (!string.IsNullOrEmpty(origin))
            //{
            //    var resetUrl = $"{origin}/account/reset-password?token={user.ResetToken}";
            //    message = message.Replace("[[link]]", resetUrl);
            //}
            //else
            {
                message =
                    $@"<p>Please use the below token to reset your password with the <code>/accounts/reset-password</code> api route:</p>
                            <p><code>{user.ResetToken}</code></p>";
            }

            await _emailService.Send(
                to: user.Email,
                subject: "Sign-up Verification API - Reset Password",
                html: message
            );
        }

        public async Task ResetPassword(ResetPasswordRequest model)
        {
            var user = await _repository.FindAsync<User>(s => s.ResetToken == model.Token);
            using var hashFunc = new HMACSHA256();
            var passwordBytes = Encoding.UTF8.GetBytes(model.Password);
            user.PasswordHash = hashFunc.ComputeHash(passwordBytes);
            user.PasswordSalt = hashFunc.Key;
            user.PasswordReset = DateTime.UtcNow;
            user.ResetToken = null;
            user.ResetTokenExpires = null;
            await _repository.UpdateAsync(user);
        }
    }
}
