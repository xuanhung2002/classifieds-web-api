using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Data.DTOs
{
    public class OpenAuctionDto
    {
        public Guid Id { get; set; }
        public decimal StartAmount { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
