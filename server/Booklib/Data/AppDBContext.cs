using Microsoft.EntityFrameworkCore;
using Booklib.Models.Entities;

namespace Booklib.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }

         public DbSet<User> User { get; set; }

        // Add your DbSets here
        // public DbSet<Book> Books { get; set; }
    }
}
