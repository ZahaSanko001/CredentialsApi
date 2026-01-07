using CredentialsApi.Models;
using Microsoft.EntityFrameworkCore;
namespace CredentialsApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<SecureItem> SecureItems { get; set; }
    }
}
