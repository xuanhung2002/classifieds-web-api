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
using Microsoft.Extensions.Hosting;
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
        private readonly IEmailService _emailService;

        public PostService(IDBRepository repository,
            IMapper mapper, IImageService imageService,
            ICurrentUserService currentUserService,
            ILogger<PostService> logger,
            IHubContext<AuctionHub> hubContext,
            IHubContext<NotificationHub> notificationHubContext,
            INotificationSerivce notificationSerivce,
            IEmailService emailService)
        {
            _repository = repository;
            _mapper = mapper;
            _imageService = imageService;
            _currentUserService = currentUserService;
            _logger = logger;
            _hubContext = hubContext;
            _notificationHubContext = notificationHubContext;
            _notificationSerivce = notificationSerivce;
            _emailService = emailService;
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
            var uploadTasks = dto.Images.Select(async image =>
            {
                using var stream = image.OpenReadStream();
                var url = await _imageService.UploadImage(stream);
                return url;
            }).ToList();
            var urls = await Task.WhenAll(uploadTasks);
            post.Images.AddRange(urls);

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
                _imageService.DeleteFile(oldImage);
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
            post.ItemCondition = dto.ItemCondition;
            post.CategoryId = dto.CategoryId;

            var entity = await _repository.UpdateAsync(post);
            _logger.LogInformation($"Update post: {entity.Id}");
            return entity;
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
            if(post.Status == ItemStatus.Sold)
            {
                throw new Exception("This item has been sold, can't open auction");
            }

            post.PostType = PostType.Auction;
            post.AuctionStatus = AuctionStatus.Opening;
            post.StartAmount = dto.StartAmount;
            post.EndTime = dto.EndTime;
            await _repository.UpdateAsync(post);
            await _hubContext.Clients.Group($"Post-{dto.Id}").SendAsync("AuctionStarted");
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
            if (post.Status == ItemStatus.Sold)
            {
                throw new Exception("This item has been sold, can't open auction");
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


                var winnerUser = await _repository.FindAsync<User>(s => s.Id == entity.CurrentBidderId);
                
                var message =
                    $@"<p>You wil in auction for item: {entity.Subject}</p>
                            <p>Contact to post owner to have a deal</p>";


                _emailService.Send(
                    to: winnerUser.Email,
                    subject: "Classifieds - Win the auction",
                    html: message
                );
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
            if(request.Status != null)
            {
                posts = (IOrderedQueryable<Post>)posts.Where(p => p.Status == request.Status);
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

        public async Task MarkSold(Guid id)
        {
            var currentUser = _currentUserService.User; 
            var post = await _repository.FindForUpdateAsync<Post>(s =>  s.Id == id);
            if (post == null)
            {
                throw new Exception("Post is null");
            }
            if (currentUser != null && post.UserId == currentUser.Id)
            {
                post.Status = ItemStatus.Sold;
            }
            else
            {
                throw new UnauthorizedAccessException("Unauthorize");
            }

            if(post.PostType == PostType.Auction && post.AuctionStatus == AuctionStatus.Opening)
            {
                post.AuctionStatus = AuctionStatus.Closed;
                var bids = await _repository.GetAsync<Bid>(s => s.PostId == post.Id);
                if(bids != null)
                {
                    _repository.DeleteRangeAsync(bids);
                }
            }
            await _repository.UpdateAsync(post);
            _logger.LogInformation($"Update post: {post.Id}, mark SOLD");
        }

        public async Task<List<PostDto>> GetPostOfCurrentUser(PostOfUserRequest request)
        {   
            var currentUser = _currentUserService?.User;
            if(currentUser == null)
            {
                throw new UnauthorizedAccessException("Unauthorize..");
            }

            var post = await _repository.GetAsync<Post, PostDto>(s => _mapper.Map<PostDto>(s),s => s.UserId == currentUser.Id);
            if (request.Status != null) {
                post = post.Where(s => s.Status == request.Status).ToList();
            }
            if(request.PostType != null)
            {
                post = post.Where(s => s.PostType == request.PostType).ToList();
            }
            return post;
        }
    }
}
