using Microsoft.EntityFrameworkCore;

namespace OOP_Fair_Fare.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<SavedRoute> SavedRoutes { get; set; }
    }
}