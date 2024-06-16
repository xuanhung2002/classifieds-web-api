using Classifieds.Data.DTOs.ChatDtos;
using Classifieds.Data.Entities;
using Classifieds.Repository;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Classifieds.Services.SignalR
{
    public class ChatHub : Hub
    {
        private readonly IDictionary<Guid, UserConnection> users;
        private readonly IDictionary<string, Guid> userSockets;
        private readonly IDBRepository _repository;
        private readonly ILogger<ChatHub> _logger;
        private readonly IHubContext<NotificationHub> _notificationHubContext;

        public ChatHub(IDBRepository repository, IDictionary<Guid, UserConnection> _users, IDictionary<string, Guid> _userSockets, ILogger<ChatHub> logger, IHubContext<NotificationHub> notificationHubContext)
        {
            _repository = repository;
            users = _users;
            userSockets = _userSockets;
            _logger = logger;
            _notificationHubContext = notificationHubContext;
        }
        public override Task OnConnectedAsync()
        {
            _logger.LogInformation("Connect chat signalR");
            _logger.LogInformation($"Client connected: {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }
        // Handles when a user joins the chat
        public async Task Join(UserResponse user)
       {
            var sockets = new HashSet<string>();

            if (users.TryGetValue(user.Id, out var existingUser))
            {
                if (existingUser.Sockets?.Add(Context.ConnectionId) == true)
                {
                    userSockets[Context.ConnectionId] = user.Id;
                    sockets = existingUser.Sockets;
                }
            }
            else
            {
                var newUser = new UserConnection { Id = user.Id, Sockets = new HashSet<string> { Context.ConnectionId } };
                users[user.Id] = newUser;
                sockets.Add(Context.ConnectionId);
                userSockets[Context.ConnectionId] = user.Id;
            }

            var onlineFriends = new HashSet<Guid>();
            var chatters = await GetChattersAsync(user.Id);

            foreach (var chatterId in chatters)
            {
                if (users.TryGetValue(chatterId, out var chatter))
                {
                    foreach (var socket in chatter.Sockets)
                    {
                        try
                        {
                            await Clients.Client(socket).SendAsync("online", user);
                        }
                        catch (Exception ex)
                        {
                            // Log or handle the exception appropriately
                        }
                    }
                    onlineFriends.Add(chatter.Id);
                }
            }

            try
            {
                await Task.WhenAll(sockets.Select(socket => Clients.Client(socket).SendAsync("friends", onlineFriends)));
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
            }
        }

        // Handles sending messages
        public async Task Message(MessageSocket message)
        {
            var sockets = new HashSet<string>();

            if (users.TryGetValue(message.FromUser.Id, out var fromUser))
            {
                sockets.UnionWith(fromUser.Sockets);
            }

            foreach (var id in message.ToUserId)
            {
                if (users.TryGetValue(id, out var toUser))
                {
                    sockets.UnionWith(toUser.Sockets);
                }
            }

            try
            {
                var msg = new ChatMessage
                {
                    Type = message.Type,
                    FromUserId = message.FromUser.Id,
                    ChatId = message.ChatId,
                    Message = message.Message,
                };

                await _repository.AddAsync(msg);

                var messageResponse = new MessageResponse
                {
                    Id = msg.Id,
                    ChatId = msg.ChatId,
                    CreatedAt = msg.CreatedAt,
                    UpdatedAt = msg.CreatedAt,
                    Type = msg.Type,
                    FromUserId = message.FromUser.Id,
                    User = message.FromUser,
                    Message = msg.Message
                };

                var sendTasks = sockets.Select(async socket =>
                {
                    try
                    {
                        await Clients.Client(socket).SendAsync("received", messageResponse);
                        await Clients.Clients($"User-{message.ToUserId.ToString()}").SendAsync("ReceiveMessageNotification");
                    }
                    catch (Exception ex)
                    {
                        
                    }
                });

                await Task.WhenAll(sendTasks);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("send socket failed");
            }
        }

        // Handles the typing notification
        public async Task Typing(TypingModel model)
        {
            foreach (var id in model.ToUserId)
            {
                if (users.TryGetValue(id, out var user))
                {
                    var userSockets = user.Sockets;

                    var sendTasks = userSockets.Select(async socket =>
                    {
                        try
                        {
                            await Clients.Client(socket).SendAsync("typing", model);
                        }
                        catch (Exception ex)
                        {
                            // Log or handle the exception appropriately
                        }
                    });

                    await Task.WhenAll(sendTasks);
                }
            }
        }

        // Handles adding a new friend
        public async Task AddFriend(AddFriendModel model)
        {
            try
            {
                const string onlineStatus = "online";

                async Task NotifyUserAboutNewChat(ChatModel chat, Guid userId)
                {
                    if (users.TryGetValue(userId, out var user))
                    {
                        chat.Users[0].Status = onlineStatus;
                        var userSockets = user.Sockets;

                        var sendTasks = userSockets.Select(async socket =>
                        {
                            try
                            {
                                await Clients.Client(socket).SendAsync("new-chat", chat);
                            }
                            catch (Exception ex)
                            {
                                // Log or handle the exception appropriately
                            }
                        });

                        await Task.WhenAll(sendTasks);
                    }
                }

                foreach (var chat in model.Chats)
                {
                    chat.Messages = new List<object>();
                }

                if (users.ContainsKey(model.Chats[1].Users[0].Id))
                {
                    await NotifyUserAboutNewChat(model.Chats[0], model.Chats[1].Users[0].Id);
                }

                if (users.ContainsKey(model.Chats[0].Users[0].Id))
                {
                    await NotifyUserAboutNewChat(model.Chats[1], model.Chats[0].Users[0].Id);
                }
            }
            catch (Exception e)
            {
                // Log or handle the exception appropriately
            }
        }

        // Handles deleting a chat
        public async Task DeleteChat(DeleteChatModel model)
        {
            try
            {
                async Task NotifyUserAboutDeletedChat(Guid userId)
                {
                    if (users.TryGetValue(userId, out var user))
                    {
                        var userSockets = user.Sockets;

                        var sendTasks = userSockets.Select(async socket =>
                        {
                            try
                            {
                                await Clients.Client(socket).SendAsync("delete-chat", model.ChatId);
                            }
                            catch (Exception ex)
                            {
                                // Log or handle the exception appropriately
                            }
                        });

                        await Task.WhenAll(sendTasks);
                    }
                }

                foreach (var userId in model.NotifyUsers)
                {
                    if (users.ContainsKey(userId))
                    {
                        await NotifyUserAboutDeletedChat(userId);
                    }
                }
            }
            catch (Exception e)
            {
                // Log or handle the exception appropriately
            }
        }

        // Handles when a user disconnects
        public async override Task OnDisconnectedAsync(Exception exception)
        {
            if (userSockets.TryGetValue(Context.ConnectionId, out var userId))
            {
                if (users.TryGetValue(userId, out var user))
                {
                    if (user.Sockets.Count > 1)
                    {
                        user.Sockets.Remove(Context.ConnectionId);
                    }
                    else
                    {
                        var chatters = await GetChattersAsync(userId);

                        foreach (var chatterId in chatters)
                        {
                            if (users.TryGetValue(chatterId, out var chatter))
                            {
                                foreach (var socket in chatter.Sockets)
                                {
                                    try
                                    {
                                        await Clients.Client(socket).SendAsync("offline", user);
                                    }
                                    catch (Exception)
                                    {
                                        // Log or handle the exception appropriately
                                    }
                                }
                            }
                        }

                        userSockets.Remove(Context.ConnectionId);
                        users.Remove(userId);
                    }
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        // Retrieves chatters for a given user
        private async Task<IEnumerable<Guid>> GetChattersAsync(Guid userId)
        {
            var chatUsers = await _repository.GetAsync<ChatUser>(cu => cu.UserId == userId);

            var chatIds = chatUsers.Select(cu => cu.ChatId).ToList();

            var userIdsChatWith = await _repository.GetAsync<ChatUser>(cu => chatIds.Contains(cu.ChatId) && cu.UserId != userId);

            return userIdsChatWith.Select(cu => cu.UserId);
        }
    }
}
