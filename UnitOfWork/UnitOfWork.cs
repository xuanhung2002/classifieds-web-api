using Classifieds.Data;
using Classifieds.Repository.Impl;
using Classifieds.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        public IUserRepository Users { get; private set; }
        public UnitOfWork(DataContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
        }
        
        public async Task<bool> Complete()
        {
           var res = await _context.SaveChangesAsync() > 0;
           return res;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
