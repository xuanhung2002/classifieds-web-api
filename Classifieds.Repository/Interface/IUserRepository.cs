using Classifieds.Data.Entities;

namespace Classifieds.Repository.Interface
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetUserByAccountName(string accountName);
    }
}
