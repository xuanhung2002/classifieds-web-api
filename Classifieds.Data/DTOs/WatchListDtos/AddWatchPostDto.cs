using Classifieds.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Data.DTOs.WatchListDtos
{
    public class AddWatchPostDto
    {
        public Guid PostId { get; set; }
        public WatchType WatchType { get; set; }
    }
}
