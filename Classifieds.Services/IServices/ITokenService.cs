using Classifieds.Data.Entites;

namespace Classifieds.Services.IServices
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
