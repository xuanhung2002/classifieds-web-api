using AutoMapper;
using Classifieds.Data.DTOs.WatchListDtos;
using Classifieds.Data.Entities;
using Classifieds.Repository;
using Classifieds.Services.IServices;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Services.Services
{
    public class WatchListService : IWatchListService
    {
        private readonly IDBRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public WatchListService(IDBRepository repository, IMapper mapper, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }
        public async Task AddWatchPost(AddWatchPostDto dto)
        {
            var watchPost = _mapper.Map<WatchList>(dto);
            var watchOfUser = await GetPostIdsByUserId(watchPost.UserId);
            if(watchOfUser != null)
            {
                if (watchOfUser.Contains(watchPost.Id))
                {
                    throw new Exception("You has already watched this post");
                }
            }
            await _repository.AddAsync(watchPost);
        }

        public async Task<List<Guid>?> GetPostIdsByUserId(Guid userId)
        {
            var postIds = await _repository.GetAsync<WatchList, Guid>(s => s.PostId, s => s.UserId == userId);
            return postIds;
        }

        public async Task<List<Guid>?> GetUserIdsByPostId(Guid postId)
        {
            var userIds = await _repository.GetAsync<WatchList, Guid>(s => s.UserId, s => s.PostId == postId);
            return userIds;
        }

        public async Task<bool> IsWatching(Guid postId)
        {
            if (_currentUserService.User == null)
            {
                throw new UnauthorizedAccessException("No permission");
            }

            var isWatching = await _repository.FindAsync<WatchList>(s => s.UserId == _currentUserService.User.Id) != null;
            return isWatching;
        }

        public async Task UnWatch(Guid postId)
        {
            if(_currentUserService.User == null)
            {
                throw new UnauthorizedAccessException("No permission");
            }

            var watchPost = _repository.FindAsync<WatchList>(s => s.PostId == postId && s.UserId == _currentUserService.User.Id);
            if( watchPost != null)
            {
                await _repository.DeleteAsync(watchPost);
            }
        }
    }
}
