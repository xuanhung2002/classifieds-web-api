using Classifieds.Data;
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

        //public UnitOfWork(DataContext context)
        //{
        //    _context = context;
        //    Users = new UserRepository(_context);
        //}
        //public IUserRepository User { get; private set; }
        public int Complete()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
