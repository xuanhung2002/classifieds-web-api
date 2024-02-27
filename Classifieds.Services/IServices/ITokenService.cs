
using Classifieds.Data.Entites;

namespace Services.IServices
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
