using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Data.DTOs.BidDtos
{
    public class CreateBidRequest
    {
        public Guid PostId { get; set; }
        public decimal Amount { get; set; }
    }
}
