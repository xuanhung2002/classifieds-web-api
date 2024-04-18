using Classifieds.Data.Models;
using Microsoft.AspNetCore.Http;

namespace Classifieds.Data.DTOs
{
    public class AddPostDto
    {
        public string Subject { get; set; } = null!;
        public List<IFormFile> Images { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public ItemCondition ItemCondition { get; set; }

        public Address Address {  get; set; } = null!;
        public Guid CategoryId { get; set; }
        public Guid UserId { get; set; }
    }
}
