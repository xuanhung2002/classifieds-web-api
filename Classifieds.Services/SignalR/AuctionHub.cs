using Classifieds.Services.IServices;
using Classifieds.Services.Services;
using Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Services.SignalR
{

    public class AuctionHub : Hub
    {
        public const string ERROR = "ERROR";
        public const string USER_JOIN_AUCTION = "USER_JOIN_AUCTION";
        public const string USER_LEAVE_AUCTION = "USER_LEAVE_AUCTION";
        public const string RECEIVE_NOTIFICATION = "RECEIVE_NOTIFICATION";

        private readonly IHubContext<AuctionHub> _hubContext;
        private readonly IWatchListService _watchListService;
        private readonly ICurrentUserService _currentUserService;
        public static readonly ConcurrentDictionary<Guid?, HubUser> Users = new();

        public AuctionHub(IWatchListService watchListService, IHubContext<AuctionHub> hubContext)
        {
            _watchListService = watchListService;
            _hubContext = hubContext;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var httpContext = Context.GetHttpContext();
                var configuration = httpContext.RequestServices.GetService<IConfiguration>();
                var token = httpContext.Request.Query["access_token"].ToString();
                Console.WriteLine("token", token);
                var tmp = Context.ConnectionId;
                if (string.IsNullOrEmpty(token))
                {
                    throw new UnauthorizedAccessException("Invalid token!");
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(AppSettings.JwtSecretKey);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var accountId = Guid.Parse(jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.NameId).Value);

                Console.WriteLine("User connected");
                var userId = _currentUserService.User.Id;
                var watchList = await _watchListService.GetPostIdsByUserId(userId);

                var userConnection = Users.GetOrAdd(
                        accountId,
                        new HubUser { UserId = accountId, ConnectionIds = new HashSet<string>() }
                );

                lock (userConnection.ConnectionIds)
                {
                    userConnection.ConnectionIds.Add(Context.ConnectionId);
                }

                foreach (var postId in watchList)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, postId.ToString());
                }

                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public void SendMessage(string message)
        {
            Clients.All.SendAsync(message);
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

                var watchList = await _watchListService.GetPostIdsByUserId(userId);
                foreach (var postId in watchList)
                {
                    await Groups.RemoveFromGroupAsync(connectionId, postId.ToString());
                }

                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception e)
            {
                throw e;
            }
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
                                postId.ToString()
                            );
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                throw;
            }
        }

        public async Task UserLeaveAuctionAsync(Guid channelId, List<Guid> userIds)
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
                            .SendAsync(USER_LEAVE_AUCTION, channelId);

                        foreach (var connectionId in hubUser.ConnectionIds)
                        {
                            await _hubContext.Groups.RemoveFromGroupAsync(
                                connectionId,
                                channelId.ToString()
                            );
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
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


        public async Task StartAuction(string auctionId)
        {
            // Phát sự kiện bắt đầu đấu giá đến tất cả các client
            await Clients.All.SendAsync("AuctionStarted", auctionId);
        }

        public async Task PlaceBid(Guid postId, decimal amount)
        {   
            if(_currentUserService.User == null)
            {
                throw new UnauthorizedAccessException("Invalid token");
            }
            //logic
            await Clients.All.SendAsync("BidPlace", postId, _currentUserService.User.Name, amount);
        }
    }
}
