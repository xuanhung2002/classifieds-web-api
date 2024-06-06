using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Data.DTOs.ChatDtos
{
    public class AddUserToGroupModel
    {
        public ChatModel Chat { get; set; }
        public UserInChatSignalR NewChatter { get; set; }
    }
}
