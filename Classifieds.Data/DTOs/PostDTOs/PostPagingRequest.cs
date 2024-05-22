using Classifieds.Data.Enums;
using Classifieds.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Data.DTOs.PostDTOs
{
    public class PostPagingRequest
    {
        public TablePageParameter Parameter { get; set; }
        public Address? Address { get; set; }
        public Guid? CategoryId { get; set; }
        public PostType? PostType { get; set; }
    }
}
