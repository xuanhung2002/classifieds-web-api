using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Data.Entities
{
    public class ChatMessage : BaseEntity
    {
        public required string Type { get; set; }
        public required string Message { get; set; }
        public Guid ChatId { get; set; }
        public required Chat Chat { get; set; }
        public int FromUserId { get; set; }
    }
}
