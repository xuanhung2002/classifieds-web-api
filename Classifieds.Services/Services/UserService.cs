using AutoMapper;
using Classifieds.Data.DTOs;
using Classifieds.Data.DTOs.UserDtos;
using Classifieds.Data.Entities;
using Classifieds.Data.Enums;
using Classifieds.Repository;
using Classifieds.Services.IServices;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;

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
        public async Task AddAdmin(RegisterDto dto)
        {
            dto.AccountName = dto.AccountName.ToLower();
            if (await _repository.FindAsync<User>(s => s.AccountName == dto.AccountName) is not null)
            {
                throw new Exception("Username is existed");
            }


            using var hashFunc = new HMACSHA256();
            var passwordBytes = Encoding.UTF8.GetBytes(dto.Password);

            var newUser = new User
            {
                AccountName = dto.AccountName,
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                Role = Role.Admin,
                PasswordHash = hashFunc.ComputeHash(passwordBytes),
                PasswordSalt = hashFunc.Key
            };
            await _repository.AddAsync(newUser);
        }

        public async Task ChangePermission(Guid id, Role role)
        {
            var user = await _repository.FindForUpdateAsync<User>(s => s.Id == id);
            if (user == null)
            {
                throw new InvalidDataException("User id is not existed");
            }

            user.Role = role;
            await _repository.UpdateAsync(user);
        }

        public async Task<List<UserDto>> GetUsers(Expression<Func<User, bool>> expression)
        {
            var user = await _repository.GetAsync<User, UserDto>(s => _mapper.Map<UserDto>(s), expression);
            return user;
        }
    }
}
