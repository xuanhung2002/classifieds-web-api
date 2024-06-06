using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Data.DTOs.ChatDtos
{
    public class DeleteChatModel
    {
        public Guid ChatId { get; set; }
        public List<Guid> NotifyUsers { get; set; }
    }
}
