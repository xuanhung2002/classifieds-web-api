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
        [EnumDataType(typeof(ItemCondition))]
        public ItemCondition ItemCondition { get; set; }
        [EnumDataType(typeof(ItemStatus))]
        public ItemStatus Status { get; set; }
        public string AddressJson { get; set; } = null!;
        [NotMapped]
        public Address? Address
        {
            get => JsonConvert.DeserializeObject<Address>(AddressJson);
            set => AddressJson = JsonConvert.SerializeObject(value);
        }

        public Guid UserId{ get; set; }

        public Guid CategoryId { get; set; }

        [EnumDataType(typeof(PostType))]
        public PostType PostType { get; set; }

        public DateTime? EndTime { get; set; }
        public decimal? StartAmount { get; set; }
        public decimal? CurrentAmount { get; set; }
        public Guid? CurrentBidderId { get; set; }

        [EnumDataType(typeof(AuctionStatus))]
        public AuctionStatus? AuctionStatus { get; set; }

    }
}
