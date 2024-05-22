using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Data.DTOs.NotificationDtos
{
    public class NotificationAddResquest
    {
        public Guid UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool Seen { get; set; } = false;
    }
}
