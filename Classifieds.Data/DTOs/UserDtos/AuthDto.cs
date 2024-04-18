using System.ComponentModel.DataAnnotations;

namespace Classifieds.Data.DTOs.UserDtos
{

    public class LoginDto
    {
        public string AccountName { get; set; } = null!;

        [MaxLength(100)]
        public string Password { get; set; } = null!;
    }

    public class RegisterDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string AccountName { get; set; }
        public string Password { get; set; }
    }
}
