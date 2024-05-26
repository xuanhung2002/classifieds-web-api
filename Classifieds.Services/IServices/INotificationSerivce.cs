using Classifieds.Data.DTOs.NotificationDtos;
using Classifieds.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Services.IServices
{
    public interface INotificationSerivce
    {   
        Task<List<NotificationDto>> GetByUserIdAsync(Guid userId);
        Task<Notification?> AddAsync(NotificationAddResquest dto);
        Task MarkSeen(Guid id);
    }
}
