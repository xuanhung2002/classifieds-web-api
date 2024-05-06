using Classifieds.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace Classifieds.Data.DTOs
{
    public class UserDto : BaseDto
    {
        public string Name { get; set; } = null!;
        public string? PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string AccountName { get; set; } = null!;
        public string? Avatar { get; set; }
        [EnumDataType(typeof(Role))]
        public Role Role { get; set; }
    }
}
