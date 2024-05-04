using AutoMapper;
using Classifieds.Data.DTOs.WatchListDtos;
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
    public class WatchListService : IWatchListService
    {
        private readonly IDBRepository _repository;
        private readonly IMapper _mapper;

        public WatchListService(IDBRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task AddWatchPost(AddWatchPostDto dto)
        {
            var watchPost = _mapper.Map<WatchList>(dto);
            await _repository.AddAsync(watchPost);
        }
    }
}
