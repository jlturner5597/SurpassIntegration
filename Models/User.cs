using System.ComponentModel.DataAnnotations;

namespace SurpassIntegration.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = default!;

        // In production, this should be a hashed password (not plaintext!)
        [Required]
        [MaxLength(200)]
        public string Password { get; set; } = default!;
    }
}
