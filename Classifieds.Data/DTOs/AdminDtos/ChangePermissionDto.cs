using Classifieds.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Data.DTOs.AdminDtos
{
    public class ChangePermissionDto
    {
        public Guid UserId { get; set; }
        public Role Role { get; set; }
    }
}
