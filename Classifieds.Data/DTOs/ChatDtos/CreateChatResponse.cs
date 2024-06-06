using Classifieds.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Data.DTOs.ChatDtos
{
    public class CreateChatResponse
    {
        public List<CreateChatResponseModel> CreateChatResponseModels { get; set; }
    }

    public class CreateChatResponseModel : BaseDto
    {
        public string Type { get; set; }
        public Object ChatUser { get; set; }
        public List<UserInMessage> Users { get; set; }
        public List<ChatMessage> Messages = new List<ChatMessage>();
    }
}
