using Classifieds.Data.Entites;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Repository.Interface
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetUserByAccountName(string accountName);
    }
}
