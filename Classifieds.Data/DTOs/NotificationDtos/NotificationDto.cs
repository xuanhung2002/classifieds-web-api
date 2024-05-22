using Classifieds.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Data.DTOs.NotificationDtos
{
    public class NotificationDto
    {
        public string Content { get; set; }
        public UserDto User { get; set; }
        public bool Seen { get; set; }
    }
}
