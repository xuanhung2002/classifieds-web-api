using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Classifieds.Data.Entities
{
    public class Notification : BaseEntity
    {
        public string Content { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public bool Seen { get; set; }
    }
}
