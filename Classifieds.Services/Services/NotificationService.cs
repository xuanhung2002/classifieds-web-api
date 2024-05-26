using AutoMapper;
using Classifieds.Data.DTOs.NotificationDtos;
using Classifieds.Data.Entities;
using Classifieds.Repository;
using Classifieds.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Services.Services
{
    public class NotificationService : INotificationSerivce
    {   
        private readonly IDBRepository _repository;
        private readonly IMapper _mapper;

        public NotificationService(IDBRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<Notification?> AddAsync(NotificationAddResquest dto)
        {
            var notification = _mapper.Map<Notification>(dto);
             return await _repository.AddAsync(notification);


        }

        public async Task MarkSeen(Guid id)
        {
            var noti = await _repository.FindAsync<Notification>(s => s.Id == id);
            if(noti != null)
            {
                noti.Seen = true;
            }
            else
            {
                throw new Exception("Notification is not existed");
            }
        }

        public async Task<List<NotificationDto>> GetByUserIdAsync(Guid userId)
        {
            var notification = await _repository.GetAsync<Notification>(s => s.UserId == userId);
            if(notification != null)
            {
                return _mapper.Map<List<NotificationDto>>(notification);
            }
            return null;
        }
    }
}
