using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Data.Entities
{
    public class Bid : BaseEntity
    {
        [Required]
        public Guid PostId { get; set; }
        [ForeignKey("PostId")]
        public Post Post { get; set; }
        [Required]
        public Guid BidderId { get; set; }
        [ForeignKey("BidderId")]
        public User User { get; set; }
        public decimal Amount { get; set; }
        public DateTime BidTime { get; set; }
    }
}
