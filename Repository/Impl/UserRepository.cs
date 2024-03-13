using Classifieds.Data;
using Classifieds.Data.Entites;
using Classifieds.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Repository.Impl
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(DataContext context) : base(context)
        {
        }

        public async Task<User?> GetUserByAccountName(string accountName)
        {
            var res = await Find(u => u.AccountName == accountName);
            return res == null ? null : res.FirstOrDefault();
        }
    }
}
