using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Entities
{
    [Index(nameof(UserId), IsUnique = true)]
    [Index(nameof(Username), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Guid UserId { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(254)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(512)]
        public string EncryptedPassword { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        [Required]
        [MaxLength(10)]
        [Column("Leng")]
        public string Language { get; set; } = string.Empty;
    }
}
