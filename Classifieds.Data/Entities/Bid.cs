using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Data.Entities
{
    public class Bid : BaseEntity
    {
        public Guid PostId { get; set; }
        public Guid BidderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime BidTime { get; set; }
    }
}
