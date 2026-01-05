using Microsoft.EntityFrameworkCore;
namespace CredentialsApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<CredentialsApi.Models.User> Users { get; set; }
    }
}
