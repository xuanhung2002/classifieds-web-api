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
            var user = await _repository.FindAsync<User>(s => s.Id == id);
            if (user == null)
            {
                throw new Exception("User is not existed");
            }
            var dto = _mapper.Map<UserDto>(user);
            return dto;
        }

        public async Task<UserDto> GetByUsername(string username)
        {
            var user = await _repository.FindAsync<User>(s => s.AccountName == username);
            if(user == null)
            {
                throw new Exception("User is not existed");
            }
            var dto = _mapper.Map<UserDto>(user);
            return dto;
        }
    }
}
