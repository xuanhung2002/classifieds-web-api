using AutoMapper;
using Classifieds.Data.DTOs;
using Classifieds.Data.DTOs.PostDTOs;
using Classifieds.Data.Entities;
using Classifieds.Data.Enums;
using Classifieds.Repository;
using Classifieds.Services.IServices;
using Microsoft.EntityFrameworkCore;
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

        public PostService(IDBRepository repository, IMapper mapper, IImageService imageService, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _mapper = mapper;
            _imageService = imageService;
            _currentUserService = currentUserService;
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
                post.Price = 0;
                post.AuctionStatus = AuctionStatus.Opening;
            }

            return await _repository.AddAsync(post);

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
            int pageCount = 0;
            int totalCount = 0;

            var data = _repository.GetSet<Post>();
            int skipCount = (request.Parameter.PageIndex - 1) * request.Parameter.PageSize;
            IOrderedQueryable<Post> posts;
            if (!request.Parameter.SortKey.IsNullOrEmpty())
            {
                Expression<Func<Post, object>> orderByExpression = p => EF.Property<object>(p, request.Parameter.SortKey);
                if (Convert.ToBoolean(request.Parameter.IsAccending))
                {
                    posts = data.OrderBy(orderByExpression);
                }
                else
                {
                    posts = data.OrderByDescending(orderByExpression);
                }
            }
            else
            {
                posts = data.OrderBy(s => s.CreatedAt);
            }



            var allCount = totalCount = await posts.CountAsync();
            if (allCount == 0)
            {
                pageCount = 1;
            }
            else
            {
                pageCount = allCount % request.Parameter.PageSize == 0
                    ? (allCount / request.Parameter.PageSize)
                    : (allCount / request.Parameter.PageSize) + 1; ;
            }

            IQueryable<Post> query = skipCount == 0
                ? posts.Take(request.Parameter.PageSize)
                : posts.Skip(skipCount).Take(request.Parameter.PageSize);

            if (request.Parameter.SearchContent != null)
            {
                Expression<Func<Post, bool>> searchExpression = p => p.Subject.Contains(request.Parameter.SearchContent);

                query = query.Where(searchExpression);
            }

            if (!string.IsNullOrEmpty(request.Address?.Province))
            {

                posts = (IOrderedQueryable<Post>)posts.Where(p => p.Address.Province == request.Address.Province);
            }

            if (!string.IsNullOrEmpty(request.Address?.District))
            {

                posts = (IOrderedQueryable<Post>)posts.Where(p => p.Address.District == request.Address.District);
            }

            if (!string.IsNullOrEmpty(request.Address?.Ward))
            {
                posts = (IOrderedQueryable<Post>)posts.Where(p => p.Address.Ward == request.Address.Ward);
            }
            var postDtos = _mapper.Map<List<PostDto>>(query);
            return new TableInfo<PostDto>
            {
                PageCount = pageCount,
                ItemsCount = allCount,
                Items = postDtos
            };
        }
    }
}
