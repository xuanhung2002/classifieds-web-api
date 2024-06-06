using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Data.DTOs.ChatDtos
{

    public class LeaveCurrentChatModel
    {
        public Guid ChatId { get; set; }
        public Guid UserId { get; set; }
        public Guid CurrentUserId { get; set; }
        public List<Guid> NotifyUsers { get; set; }
    }
}
