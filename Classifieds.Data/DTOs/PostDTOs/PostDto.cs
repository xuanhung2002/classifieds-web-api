using Classifieds.Data.Enums;
using Classifieds.Data.Models;
using System.ComponentModel.DataAnnotations;

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

        public PostType PostType { get; set; }
        public Address Address { get; set; } = null!;
        public Guid CategoryId { get; set; }
        public Guid UserId { get; set; }

        public DateTime? EndTime { get; set; }
        public decimal? StartAmount { get; set; }
        public decimal? CurrentAmount { get; set; }
        public Guid? CurrentBidderId { get; set; }
        public AuctionStatus? AuctionStatus { get; set; }

    }
}
