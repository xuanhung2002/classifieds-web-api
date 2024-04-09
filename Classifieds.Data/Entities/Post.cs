using Classifieds.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Data.Entities
{
    public class Post : BaseEntity
    {
        public string Subject { get; set; } = null!;
        public List<string> Images { get; set; } = new List<string>();
        public string? Description { get; set; }
        public decimal Price { get; set; }
        [EnumDataType(typeof(ItemCondition))]
        public ItemCondition ItemCondition { get; set; }
        public string AddressJson { get; set; } = null!;
        [NotMapped]
        public Address? Address
        {
            get => JsonConvert.DeserializeObject<Address>(AddressJson);
            set => AddressJson = JsonConvert.SerializeObject(value);
        }

        public Guid UserId{ get; set; }
        public User User { get; set; }
    }
}
