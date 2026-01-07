using System.ComponentModel.DataAnnotations;

namespace CredentialsApi.Models
{
    public class SecureItem
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string EncryptedContent { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
