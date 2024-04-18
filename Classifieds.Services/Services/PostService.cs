using AutoMapper;
using Classifieds.Data.DTOs;
using Classifieds.Data.Entities;
using Classifieds.Data.Models;
using Classifieds.Repository;
using Classifieds.Services.IServices;

namespace Classifieds.Services.Services
{
    public class PostService : IPostService
    {
        private readonly IDBRepository _repository;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;

        public PostService(IDBRepository repository, IMapper mapper, IImageService imageService)
        {
            _repository = repository;
            _mapper = mapper;
            _imageService = imageService;
        }

        public async Task<Post> AddAsync(AddPostDto dto)
        {
            var post = _mapper.Map<Post>(dto);
            post.Status = ItemStatus.Unsold;
            foreach(var image in dto.Images)
            {
                using var stream = image.OpenReadStream();
                var url = _imageService.UploadImage(stream);
                post.Images.Add(url);
            }

            return await _repository.AddAsync(post);

        }
    }
}
