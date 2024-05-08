using Classifieds.Data.Enums;
using Classifieds.Data.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Classifieds.Data.DTOs
{
    public class PostAddDto
    {
        [Required(ErrorMessage = "The Subject field is requiredd.")]
        public string Subject { get; set; }
        [Required(ErrorMessage = "The Images field is requiredd.")]
        public List<IFormFile> Images { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public ItemCondition ItemCondition { get; set; }
        [Required(ErrorMessage = "The Address field is requiredd.")]
        public Address Address {  get; set; }
        public Guid CategoryId { get; set; }

        public PostType PostType { get; set; }

        public DateTime? EndTime { get; set; }
        public decimal? StartAmount { get; set; }

    }
}
