using Classifieds.Data;
using Classifieds.Repository.Impl;
using Classifieds.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Classifieds.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        public IUserRepository Users { get; private set; }

        public IPostRepository Posts { get; private set; }

        public UnitOfWork(DataContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Posts = new PostRepository(_context);
        }
        
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
