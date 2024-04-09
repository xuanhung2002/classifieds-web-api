using System.ComponentModel.DataAnnotations;

namespace Classifieds.Data.Entities
{
    public class IdEntity
    {
        [Key]
        public Guid Id { get; set; }
    }
    public class BaseEntity : IdEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
