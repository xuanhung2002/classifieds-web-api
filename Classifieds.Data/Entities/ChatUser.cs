using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Data.Entities
{
    public class ChatUser : BaseEntity
    {
        public Guid UserId { get; set; }
        public required User User { get; set; }
        public Guid ChatId { get; set; }
        public required Chat Chat { get; set; }
    }
}
