using CredentialsApi.Models;
using CredentialsApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
namespace CredentialsApi.Data
{
    public class AppDbContext : DbContext
    {
        private readonly EncryptionService _encryption;
        public AppDbContext(DbContextOptions<AppDbContext> options, EncryptionService encryption    ) : base(options)
        {
            _encryption = encryption;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var provider = this.GetService<IServiceProvider>();
            var encryption = provider.GetService<EncryptionService>();

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
