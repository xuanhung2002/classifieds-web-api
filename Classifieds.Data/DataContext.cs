using Microsoft.EntityFrameworkCore;

namespace Classifieds.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
      
    }
}
