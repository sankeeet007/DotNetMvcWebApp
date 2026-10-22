
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MVCwebApp.Models;
namespace MVCwebApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // THIS LINE CREATES THE USERS TABLE IN YOUR DATABASE
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
