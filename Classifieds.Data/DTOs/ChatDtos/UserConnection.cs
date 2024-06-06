using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Data.DTOs.ChatDtos
{
    public class UserConnection
    {
        public Guid Id { get; set; }
        public HashSet<string> Sockets { get; set; }
    }
}
