using Classifieds.Data;
using Classifieds.Data.Entites;
using Classifieds.Repository.Interface;

namespace Classifieds.Repository.Impl
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(DataContext context) : base(context)
        {
        }

        public async Task<User?> GetUserByAccountName(string accountName)
        {
            var res = await FindAsync(u => u.AccountName == accountName);
            return res == null ? null : res;
        }
    }
}
