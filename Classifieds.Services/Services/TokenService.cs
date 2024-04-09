using Classifieds.Data.Entities;
using Classifieds.Services.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Classifieds.Services.Services
{
    public class TokenService : ITokenService
    {
        private readonly string jwtSecretKey;

        public TokenService(IConfiguration configuration)
        {
            jwtSecretKey = configuration["JwtSecretKey"] ?? string.Empty;
        }

        public string GenerateToken(User user)
        {
            var claims = new List<Claim>()
            {
                new (JwtRegisteredClaimNames.Name, user.AccountName),
                new (JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(ClaimTypes.Role, "")

            };
            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new(symmetricKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}

