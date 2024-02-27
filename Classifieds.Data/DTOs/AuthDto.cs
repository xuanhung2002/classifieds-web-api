using System.ComponentModel.DataAnnotations;

namespace Classifieds.Data.DTOs
{

    public class LoginDto
    {
        public string AccountName { get; set; } = null!;

        [MaxLength(100)]
        public string Password { get; set; } = null!;
    }

    public class RegisterDto
    {

    }
}
