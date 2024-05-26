using AutoMapper;
using Classifieds.Data.DTOs;
using Classifieds.Data.DTOs.NotificationDtos;
using Classifieds.Data.DTOs.PostDTOs;
using Classifieds.Data.Entities;
using Classifieds.Data.Enums;
using Classifieds.Repository;
using Classifieds.Services.IServices;
using Classifieds.Services.SignalR;
using Microsoft.AspNetCore.SignalR;
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
        private readonly IHubContext<AuctionHub> _hubContext;
        private readonly IHubContext<NotificationHub> _notificationHubContext;
        private readonly INotificationSerivce _notificationSerivce;

        public PostService(IDBRepository repository,
            IMapper mapper, IImageService imageService,
            ICurrentUserService currentUserService,
            ILogger<PostService> logger,
            IHubContext<AuctionHub> hubContext,
            IHubContext<NotificationHub> notificationHubContext,
            INotificationSerivce notificationSerivce)
        {
            _repository = repository;
            _mapper = mapper;
            _imageService = imageService;
            _currentUserService = currentUserService;
            _logger = logger;
            _hubContext = hubContext;
            _notificationHubContext = notificationHubContext;
            _notificationSerivce = notificationSerivce;
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
            if(dto.PostType == PostType.Normal && dto.Price < 0)
            {
                throw new Exception("The amount must bigger than 0");
            }
            if (dto.PostType == PostType.Auction && dto.StartAmount < 0)
            {
                throw new Exception("The start amount must bigger than 0");
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
            post.EndTime = dto.EndTime;

            await _repository.UpdateAsync(post);
        }
        public async Task ReOpenAuction(OpenAuctionDto dto, Guid userId)
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

            var bidsOfPost = await _repository.GetAsync<Bid>(s => s.PostId == post.Id);
            if (bidsOfPost != null)
            {
                await _repository.DeleteRangeAsync(bidsOfPost);
                _logger.LogInformation($"Delete all bids of post: {post.Id}");
            }

            post.PostType = PostType.Auction;
            post.AuctionStatus = AuctionStatus.Opening;
            post.StartAmount = dto.StartAmount;
            post.CurrentAmount = null;
            post.EndTime = dto.EndTime;

            await _repository.UpdateAsync(post);
            _logger.LogInformation($"Update post: {post.Id}");
        }

        public async Task CloseAuction(Guid id, Guid userId)
        {
            var post = await _repository.FindForUpdateAsync<Post>(s => s.Id == id);
            if (post == null)
            {
                throw new Exception("Post is not existed");
            }
            if(post.PostType != PostType.Auction)
            {
                throw new Exception("Post is not auction post");
            }
            if (post.UserId != userId)
            {
                throw new UnauthorizedAccessException("No permission in this post");
            }

            post.AuctionStatus = AuctionStatus.Closed;
            post.Price = post.CurrentAmount;
            var entity = await _repository.UpdateAsync(post);
            await _hubContext.Clients.Group($"Post-{id}").SendAsync("AuctionClosed");
            if(entity != null && entity.CurrentBidderId != null)
            {
                var notification = await _notificationSerivce.AddAsync(new NotificationAddResquest
                {
                    UserId = (Guid)entity.CurrentBidderId,
                    Content = $"You win in auction {post.Id} : {post.Subject}",
                    Seen = false
                });
                await _notificationHubContext.Clients.Group($"User-{entity.CurrentBidderId}").SendAsync("WinAuction", _mapper.Map<NotificationDto>(notification));
            }
          
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
