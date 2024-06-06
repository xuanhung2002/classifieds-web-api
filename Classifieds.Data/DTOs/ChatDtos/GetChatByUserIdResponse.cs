using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Data.DTOs.ChatDtos
{
public class GetChatByUserIdResponse : BaseDto
{
    public string Type { get; set; }
    public ChatUserResponse ChatUser { get; set; }
    // list of user info we chat with
    public List<UserResponse> Users { get; set; }
    public List<MessageResponse> Messages { get; set; }
}

public class ChatUserResponse
{
    public Guid ChatId { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
    
public class UserResponse : BaseDto
{
    public string Avatar { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public ChatUserResponse ChatUser { get; set; }
}

public class MessageResponse : BaseDto
{
    public string Type { get; set; }
    public string Message { get; set; }
    public Guid ChatId { get; set; }
    public Guid FromUserId { get; set; }
    public UserInMessage User { get; set; }
}
    
public class UserInMessage : BaseDto
{

    public string Avatar { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}
}
