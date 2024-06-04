using Classifieds.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Classifieds.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) {}


        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Bid> Bids{ get; set; }
        public DbSet<WatchList> WatchLists { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ChatUser> ChatUsers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }

    }
}
