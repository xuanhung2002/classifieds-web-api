using Classifieds.Data.DTOs.ChatDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Services.IServices
{
    public interface IChatService
    {
        Task<List<GetChatByUserIdResponse>> GetChatByUserId(Guid id);
        Task<CreateChatResponse> Create(Guid partnerId, Guid userId);
        Task<object> Messages(Guid id, int page = 1);
        Task<object> AddUserToGroup(Guid chatId, Guid userId, Guid currentUserId);
        Task<object> Delete(Guid chatId);
        Task<object> LeaveCurrentChat(Guid chatId, Guid currentUserId);
        object UpLoadImage(Stream image);
    }
}
