using Classifieds.Data.Entites;
using Microsoft.EntityFrameworkCore;

namespace Classifieds.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
      

        public DbSet<User> Users { get; set; }
    }
}
