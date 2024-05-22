using Classifieds.Data.Entities;
using Microsoft.AspNetCore.SignalR;

namespace Classifieds.Services.SignalR
{
    public class NotificationHub : Hub
    {
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
