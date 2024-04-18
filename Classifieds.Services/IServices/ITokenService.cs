using Classifieds.Data.Entities;

namespace Classifieds.Services.IServices
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        Guid? ValidateJwtToken(string token);
    }
}
