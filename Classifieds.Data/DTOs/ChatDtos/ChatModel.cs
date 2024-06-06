using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Classifieds.Data.DTOs.ChatDtos
{
    public class ChatModel
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public List<UserInChatSignalR> Users { get; set; } // Represents the users involved in the chat
        public List<object> Messages { get; set; } // Represents the chat message content
    }

    public class UserInChatSignalR : BaseDto
    {
        public string Avatar { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Gender = "male";
        [JsonIgnore]
        public string Status { get; set; }
    }

    public class AddFriendModel
    {
        public List<ChatModel> Chats { get; set; }
    }
}
