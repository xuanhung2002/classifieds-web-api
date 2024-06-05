using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Data.DTOs.WatchListDtos
{
    public class WatchListDto : BaseDto
    {
        public Guid UserId { get; set; }
        public List<PostDto> Posts { get; set; } = new List<PostDto>();
    }
}
