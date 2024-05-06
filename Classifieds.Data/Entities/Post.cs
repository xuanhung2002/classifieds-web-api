using Classifieds.Data.Enums;
using Classifieds.Data.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Classifieds.Data.Entities
{
    public class Post : BaseEntity
    {
        public string Subject { get; set; } = null!;
        public List<string> Images { get; set; } = new List<string>();
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public ItemCondition ItemCondition { get; set; }
        public ItemStatus Status { get; set; }
        public string AddressJson { get; set; } = null!;
        [NotMapped]
        public Address? Address
        {
            get => JsonConvert.DeserializeObject<Address>(AddressJson);
            set => AddressJson = JsonConvert.SerializeObject(value);
        }

        [Required]
        public Guid UserId{ get; set; }
        public User User { get; set; }

        [Required]
        public Guid CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        public PostType PostType { get; set; }

        public DateTime? EndTime { get; set; }
        public decimal? StartAmount { get; set; }
        public decimal? CurrentAmount { get; set; }
        public Guid? CurrentBidderId { get; set; }

        public AuctionStatus? AuctionStatus { get; set; }

    }
}
