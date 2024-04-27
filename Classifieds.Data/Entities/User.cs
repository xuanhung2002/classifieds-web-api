using Classifieds.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Classifieds.Data.Entities
{   
    public class User : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string AccountName { get; set; } = null!;
        public byte[] PasswordHash { get; set; } = null!;
        public byte[] PasswordSalt { get; set; } = null!;
        public string? Avatar { get; set; }
        public Role Role { get; set; }
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        public DateTime? PasswordReset { get; set; }

    }
}
