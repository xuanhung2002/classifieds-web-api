using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Services.SignalR
{
    public class HubUser
    {
        public Guid? UserId { get; set; }

        public HashSet<string> ConnectionIds { get; set; } = new();
    }
}
