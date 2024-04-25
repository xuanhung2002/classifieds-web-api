using Classifieds.Data.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Classifieds.Data.DTOs
{
    public class PostUpdateDto : BaseDto
    {
        public string Subject { get; set; } = null!;
        public List<IFormFile> Images { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public ItemCondition ItemCondition { get; set; }
        public ItemStatus Status { get; set; }

        public Address Address { get; set; } = null!;
        public Guid CategoryId { get; set; }
        public Guid UserId { get; set; }
        public PostType PostType { get; set; }

        public DateTime? EndTime { get; set; }
        public decimal? StartAmount { get; set; }
        public AuctionStatus? AuctionStatus { get; set; }
    }
}
