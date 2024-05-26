using Classifieds.Data.Entities;
using Classifieds.Services.Services;
using Common;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Classifieds.Services.SignalR
{
    public class NotificationHub : Hub
    {
        public readonly ConcurrentDictionary<Guid?, HubUser> Users = new();
        private readonly ICurrentUserService _currentUserService;

        public NotificationHub(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var httpContext = Context.GetHttpContext();
                var token = httpContext.Request.Query["access_token"].ToString();
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
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var accountId = Guid.Parse(jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.NameId).Value);

                var userConnection = Users.GetOrAdd(
                    accountId,
                    new HubUser { UserId = accountId, ConnectionIds = new HashSet<string>() }
                );

                lock (userConnection.ConnectionIds)
                {
                    userConnection.ConnectionIds.Add(Context.ConnectionId);
                }

                await base.OnConnectedAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{exception.Message} in notificationHub");
                throw;
            }
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
                Console.WriteLine($"{e.Message} in notificationHub");
                throw e;
            }
        }

        public async Task JoinGroup(Guid userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User-{userId}");
        }

        public async Task LeaveGroup(Guid userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User-{userId}");
        }
    }

}
