using AutoMapper;
using Classifieds.Data.DTOs;
using Classifieds.Data.DTOs.PostDTOs;
using Classifieds.Data.Entities;
using Classifieds.Data.Enums;
using Classifieds.Repository;
using Classifieds.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;

namespace Classifieds.Services.Services
{
    public class PostService : IPostService
    {
        private readonly IDBRepository _repository;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        public readonly ICurrentUserService _currentUserService;
        public readonly ILogger<PostService> _logger;

        public PostService(IDBRepository repository, IMapper mapper, IImageService imageService, ICurrentUserService currentUserService, ILogger<PostService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _imageService = imageService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<PostDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.FindAsync<Post>(s => s.Id == id);
            return _mapper.Map<PostDto>(entity);
        }

        public async Task<Post> AddAsync(PostAddDto dto)
        {
            if (_currentUserService.User == null)
            {
                throw new UnauthorizedAccessException("Invalid token");
            }
            var post = _mapper.Map<Post>(dto);
            post.User = _currentUserService.User;
            post.Status = ItemStatus.Unsold;
            foreach (var image in dto.Images)
            {
                using var stream = image.OpenReadStream();
                var url = await _imageService.UploadImage(stream);
                post.Images.Add(url);
            }
            if (dto.PostType == PostType.Auction)
            {   
                if(dto.EndTime <= DateTime.UtcNow)
                {
                    throw new InvalidDataException("End time cannot ealier than now");
                }
                post.Price = 0;
                post.AuctionStatus = AuctionStatus.Opening;

            }

            var entity = await _repository.AddAsync(post);
            _logger.LogInformation($"Add post: post id: {entity.Id}");
            return entity;

        }

        public async Task<Post> UpdateAsync(PostUpdateDto dto)
        {
            if (_currentUserService.User == null)
            {
                throw new UnauthorizedAccessException("Invalid token");
            }

            var post = await _repository.FindForUpdateAsync<Post>(s => s.Id == dto.Id);

            if (post == null)
            {
                throw new Exception("Post is not existed");
            }
            if (post.User != _currentUserService.User)
            {
                throw new UnauthorizedAccessException("No permission in this post");
            }

            foreach (var oldImage in post.Images)
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
            if (post == null)
            {
                return 0;
            }
            return await _repository.DeleteAsync(post);
        }

        public async Task<List<PostDto>> GetAllAsync()
        {
            var post = await _repository.GetAsync<Post, PostDto>(s => _mapper.Map<PostDto>(s));
            return post;
        }

        public async Task OpenAuction(OpenAuctionDto dto, Guid userId)
        {
            var post = await _repository.FindForUpdateAsync<Post>(s => s.Id == dto.Id);
            if (post == null)
            {
                throw new Exception("Post is not existed");
            }
            if (post.UserId != userId)
            {
                throw new UnauthorizedAccessException("No permission in this post");
            }

            post.PostType = PostType.Auction;
            post.AuctionStatus = AuctionStatus.Opening;
            post.StartAmount = dto.StartAmount;

            await _repository.UpdateAsync(post);
        }

        public async Task CloseAuction(Guid id, Guid userId)
        {
            var post = await _repository.FindForUpdateAsync<Post>(s => s.Id == id);
            if (post == null)
            {
                throw new Exception("Post is not existed");
            }
            if (post.UserId != userId)
            {
                throw new UnauthorizedAccessException("No permission in this post");
            }

            post.PostType = PostType.Normal;
            post.AuctionStatus = AuctionStatus.Closed;
            post.Price = post.CurrentAmount;

            await _repository.UpdateAsync(post);

        }

        public async Task<TableInfo<PostDto>> GetPagingAsync(PostPagingRequest request)
        {
            var data = _repository.GetSet<Post>();

            // Apply sorting
            IOrderedQueryable<Post> posts;
            if (!request.Parameter.SortKey.IsNullOrEmpty())
            {
                Expression<Func<Post, object>> orderByExpression = p => EF.Property<object>(p, request.Parameter.SortKey);
                posts = Convert.ToBoolean(request.Parameter.IsAccending) ? data.OrderBy(orderByExpression) : data.OrderByDescending(orderByExpression);
            }
            else
            {
                posts = data.OrderBy(s => s.CreatedAt);
            }

            // Apply search filter
            if (request.Parameter.SearchContent != null)
            {
                Expression<Func<Post, bool>> searchExpression = p => p.Subject.Contains(request.Parameter.SearchContent);
                posts = (IOrderedQueryable<Post>)posts.Where(searchExpression);
            }

            if(request.CategoryId != null)
            {
                posts = (IOrderedQueryable<Post>)posts.Where(p => p.CategoryId == request.CategoryId);
            }
            if(request.PostType != null)
            {
                posts = (IOrderedQueryable<Post>)posts.Where(p => p.PostType == request.PostType);
            }

            // Retrieve all filtered and sorted data from the database
            var postList = await posts.ToListAsync();

            // Apply client-side filtering based on Address
            if (!string.IsNullOrEmpty(request.Address?.Province))
            {
                postList = postList.Where(p => p.Address?.Province == request.Address.Province).ToList();
            }

            if (!string.IsNullOrEmpty(request.Address?.District))
            {
                postList = postList.Where(p => p.Address?.District == request.Address.District).ToList();
            }

            if (!string.IsNullOrEmpty(request.Address?.Ward))
            {
                postList = postList.Where(p => p.Address?.Ward == request.Address.Ward).ToList();
            }

            // Calculate total count after all filters are applied
            int totalCount = postList.Count;

            // Apply paging
            int skipCount = (request.Parameter.PageIndex - 1) * request.Parameter.PageSize;
            var pagedPostList = postList.Skip(skipCount).Take(request.Parameter.PageSize).ToList();

            // Calculate page count
            int pageCount = (int)Math.Ceiling(totalCount / (double)request.Parameter.PageSize);

            var postDtos = _mapper.Map<List<PostDto>>(pagedPostList);

            return new TableInfo<PostDto>
            {
                PageCount = pageCount,
                ItemsCount = totalCount,
                Items = postDtos
            };
        }



    }
}
