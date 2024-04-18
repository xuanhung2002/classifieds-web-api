using AutoMapper;
using Classifieds.Data.DTOs;
using Classifieds.Data.Entities;
using Classifieds.Repository;
using Classifieds.Services.IServices;

namespace Classifieds.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IDBRepository _repository;
        private readonly IMapper _mapper;

        public UserService(IDBRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<UserDto> GetById(Guid id)
        {
            var user = await _repository.GetAsync<User>(s => s.Id == id);
            var dto = _mapper.Map<UserDto>(user);
            return dto;
        }
    }
}
