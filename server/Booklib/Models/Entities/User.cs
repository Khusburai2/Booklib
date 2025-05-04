// 3. Models/Entities/User.cs:
using System;
using System.ComponentModel.DataAnnotations;

namespace Booklib.Models.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100, ErrorMessage = "Email length can't be more than 100 characters.")]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        [Required]
        [StringLength(50, ErrorMessage = "First name length can't be more than 50 characters.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First name can only contain letters.")]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(50, ErrorMessage = "Last name length can't be more than 50 characters.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last name can only contain letters.")]
        public string LastName { get; set; } = null!;

        public string Role { get; set; } = "Member";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpires { get; set; }
    }
}