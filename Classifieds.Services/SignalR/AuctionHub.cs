using Classifieds.Services.IServices;
using Classifieds.Services.Services;
using Classifieds.Services.SignalR;
using Common;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;
using System.Text;
using System;

public class AuctionHub : Hub
{
    public const string USER_JOIN_AUCTION = "USER_JOIN_AUCTION";
    public const string USER_LEAVE_AUCTION = "USER_LEAVE_AUCTION";

    private readonly IHubContext<AuctionHub> _hubContext;
    private readonly IWatchListService _watchListService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPostService _postService;
    private readonly IUserService _userService;
    public static readonly ConcurrentDictionary<Guid?, HubUser> Users = new();

    public AuctionHub(IWatchListService watchListService, IHubContext<AuctionHub> hubContext, ICurrentUserService currentUserService, IPostService postService, IUserService userService)
    {
        _watchListService = watchListService;
        _hubContext = hubContext;
        _currentUserService = currentUserService;
        _postService = postService;
        _userService = userService;
    }

    public override Task OnConnectedAsync()
    {
        //try
        //{
        //    var httpContext = Context.GetHttpContext();
        //    var token = httpContext.Request.Query["access_token"].ToString();
        //    if (string.IsNullOrEmpty(token))
        //    {
        //        throw new UnauthorizedAccessException("Invalid token!");
        //    }

        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.ASCII.GetBytes(AppSettings.JwtSecretKey);
        //    tokenHandler.ValidateToken(token, new TokenValidationParameters
        //    {
        //        ValidateIssuerSigningKey = true,
        //        IssuerSigningKey = new SymmetricSecurityKey(key),
        //        ValidateIssuer = false,
        //        ValidateAudience = false,
        //        RequireExpirationTime = false,
        //        ClockSkew = TimeSpan.Zero
        //    }, out SecurityToken validatedToken);

        //    var jwtToken = (JwtSecurityToken)validatedToken;
        //    var accountId = Guid.Parse(jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.NameId).Value);

        //    var userConnection = Users.GetOrAdd(
        //        accountId,
        //        new HubUser { UserId = accountId, ConnectionIds = new HashSet<string>() }
        //    );

        //    lock (userConnection.ConnectionIds)
        //    {
        //        userConnection.ConnectionIds.Add(Context.ConnectionId);
        //    }

        //    await base.OnConnectedAsync();
        //}
        //catch (Exception exception)
        //{
        //    Console.WriteLine($"{exception.Message} in auctionHub");
        //    throw;
        //}
        return base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        try
        {
            if (_currentUserService.User == null)
            {
                throw new UnauthorizedAccessException("Invalid token!");
            }
            var userId = _currentUserService.User.Id;
            var connectionId = Context.ConnectionId;
            var userConnection = Users.GetOrAdd(
                userId,
                new HubUser { UserId = userId, ConnectionIds = new HashSet<string>() }
            );
            lock (userConnection.ConnectionIds)
            {
                userConnection.ConnectionIds.Remove(connectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message} in auctionHub");
            throw e;
        }
    }

    public async Task JoinGroup(Guid postId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Post-{postId}");
    }

    public async Task LeaveGroup(Guid postId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Post-{postId}");
    }

    public async Task UserJoinAuctionAsync(Guid postId, List<Guid> userIds)
    {
        try
        {
            foreach (var userId in userIds)
            {
                Users.TryGetValue(userId, out var hubUser);
                if (hubUser is not null && hubUser.ConnectionIds.Any())
                {
                    await Clients
                        .Clients(hubUser.ConnectionIds.ToList())
                        .SendAsync(USER_JOIN_AUCTION, postId);

                    foreach (var connectionId in hubUser.ConnectionIds)
                    {
                        await _hubContext.Groups.AddToGroupAsync(
                            connectionId,
                            $"Post-{postId}"
                        );
                    }
                }
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine($"{exception.Message} in auctionHub");
            throw;
        }
    }

    public async Task UserLeaveAuctionAsync(Guid postId, List<Guid> userIds)
    {
        try
        {
            foreach (var userId in userIds)
            {
                Users.TryGetValue(userId, out var hubUser);
                if (hubUser is not null && hubUser.ConnectionIds.Any())
                {
                    await Clients
                        .Clients(hubUser.ConnectionIds.ToList())
                        .SendAsync(USER_LEAVE_AUCTION, postId);

                    foreach (var connectionId in hubUser.ConnectionIds)
                    {
                        await _hubContext.Groups.RemoveFromGroupAsync(
                            connectionId,
                            $"Post-{postId}"
                        );
                    }
                }
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine($"{exception.Message} in auctionHub");
            throw;
        }
    }

    public static Task<IEnumerable<string>> GetConnectionsByUserId(Guid userId)
    {
        Users.TryGetValue(userId, out var hubUser);
        return Task.FromResult(
            hubUser?.ConnectionIds.AsEnumerable() ?? Enumerable.Empty<string>()
        );
    }

    public async Task UpdatePriceAsync(Guid postId, decimal price)
    {
        await Clients.Group($"Post-{postId}").SendAsync("BidPlaced", postId, price);
    }

    public async Task StartAuction(string postId)
    {
        await Clients.Group($"Post-{postId}").SendAsync("AuctionStarted", postId);
    }

    public async Task ClosedAuction(string postId)
    {
        await Clients.Group($"Post-{postId}").SendAsync("AuctionClosed", postId);
    }
}
