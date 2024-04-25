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

        public async Task<PostDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.FindAsync<Post>(s => s.Id == id);
            return _mapper.Map<PostDto>(entity);
        }

        public async Task<Post> AddAsync(PostAddDto dto)
        {
            var post = _mapper.Map<Post>(dto);
            post.Status = ItemStatus.Unsold;
            foreach(var image in dto.Images)
            {
                using var stream = image.OpenReadStream();
                var url = await _imageService.UploadImage(stream);
                post.Images.Add(url);
            }
            if(dto.PostType == PostType.Auction)
            {
                post.AuctionStatus = AuctionStatus.Opening;
            }

            return await _repository.AddAsync(post);

        }

        public async Task<Post> UpdateAsync(PostUpdateDto dto)
        {
            var post = await _repository.FindForUpdateAsync<Post>(s => s.Id == dto.Id);
            foreach(var oldImage in post.Images)
            {
               await _imageService.DeleteFile(oldImage);                    
            }
            post.Images.Clear();
            foreach (var image in dto.Images)
            {
                using var stream = image.OpenReadStream();
                var url = await _imageService.UploadImage(stream);
                post.Images.Add(url);
            }
            post.Address = dto.Address;
            post.Price = dto.Price;
            post.Description = dto.Description;
            post.Status = dto.Status;
            post.ItemCondition = dto.ItemCondition;
            post.CategoryId = dto.CategoryId;

            return await _repository.UpdateAsync(post);
        }

        public async Task<int> DeleteAsync(Guid id)
        {
            var post = await _repository.FindAsync<Post>(s => s.Id == id);          
            if(post == null)
            {
                return 0;
            }
            return await _repository.DeleteAsync(post);
        }

    }
}
