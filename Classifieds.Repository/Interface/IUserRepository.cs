using Classifieds.Data.Entites;

namespace Classifieds.Repository.Interface
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetUserByAccountName(string accountName);
    }
}
