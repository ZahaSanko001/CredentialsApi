using CredentialsApi.Models;
using CredentialsApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
namespace CredentialsApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SecureItem>()
                .Property(e => e.EncryptedContent)
                .HasConversion(
                    v => _encryption.Encrypt(v),
                    v => _encryption.Decrypt(v));
        }

        public DbSet<User> Users { get; set; }
        public DbSet<SecureItem> SecureItems { get; set; }
    }
}
