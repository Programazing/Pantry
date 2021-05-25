using Microsoft.EntityFrameworkCore;
using Pantry.Models;

namespace Pantry
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ImageLocation> ImageLocations { get; set; }
        public DbSet<CurrentUser> CurrentUser { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}
