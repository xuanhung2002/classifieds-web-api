using Classifieds.Data.Models;

namespace Classifieds.Data.DTOs
{
    public class PostDto : BaseDto
    {   
        public string Subject { get; set; } = null!;
        public List<string> Images { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public ItemCondition ItemCondition { get; set; }
        public ItemStatus Status { get; set; }

        public Address Address { get; set; } = null!;
        public Guid CategoryId { get; set; }
        public Guid UserId { get; set; }
    }
}
