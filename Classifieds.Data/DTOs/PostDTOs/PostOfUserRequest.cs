using Classifieds.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Data.DTOs.PostDTOs
{
    public class PostOfUserRequest
    {
        public ItemStatus? Status { get; set; }
        public PostType? PostType { get; set; }

    }
}
