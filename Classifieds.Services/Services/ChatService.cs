using Classifieds.Data.DTOs;
using Classifieds.Data.DTOs.ChatDtos;
using Classifieds.Data.Entities;
using Classifieds.Repository;
using Classifieds.Services.IServices;
using Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Services.Services
{
    public class ChatService : IChatService
    {
        private readonly IDBRepository _repository;
        private readonly IImageService _imageService;
        public ChatService(IDBRepository repository, IImageService imageService)
        {
            _repository = repository;
            _imageService = imageService;
        }

        public async Task<List<GetChatByUserIdResponse>> GetChatByUserId(Guid id)
        {
            var chatUsers = await _repository.GetAsync<ChatUser>(s => s.UserId == id);
            var chatIds = chatUsers.Select(cu => cu.ChatId).Distinct().ToList();
            var chatUsersContainChatWithUsers = await _repository.GetAsync<ChatUser>(cu => chatIds.Contains(cu.ChatId));
            var chats = await _repository.GetAsync<Chat>(c => chatIds.Contains(c.Id));
            var userIds = chatUsersContainChatWithUsers.Select(cu => cu.UserId).Distinct().ToList();
            var users = await _repository.GetAsync<User>(a => userIds.Contains(a.Id));

            var messages = await _repository.GetSet<ChatMessage>(cm => chatIds.Contains(cm.ChatId)).Take(20).ToListAsync();

            var res = new List<GetChatByUserIdResponse>();

            foreach (var chat in chats)
            {
                var chatUser = chatUsers.FirstOrDefault(cu => cu.UserId == id && cu.ChatId == chat.Id);

                var userIdsChatWith = chatUsersContainChatWithUsers
                    .Where(cu => cu.ChatId == chat.Id && cu.UserId != id)
                    .Select(cu => cu.UserId)
                    .ToList();

                var usersChatWith = users
                    .Where(u => userIdsChatWith.Contains(u.Id))
                    .ToList();

                var userResponseList = usersChatWith
                    .Select(user => new UserResponse
                    {
                        Id = user.Id,
                        Avatar = user.Avatar,
                        Name = user.Name,
                        Email = user.Email,
                        ChatUser = new ChatUserResponse
                        {
                            ChatId = chatUsersContainChatWithUsers
                                .FirstOrDefault(cu => cu.ChatId == chat.Id && cu.UserId == user.Id)?
                                .ChatId ?? Guid.NewGuid(),
                            UserId = user.Id,
                        }
                    })
                    .ToList();

                var messageResponseList = messages
                    .Where(m => m.ChatId == chat.Id)
                    .Select(m => new MessageResponse
                    {
                        Id = m.Id,
                        Type = m.Type,
                        Message = m.Message,
                        ChatId = m.ChatId,
                        FromUserId = m.FromUserId,
                        CreatedAt = m.CreatedAt,
                        UpdatedAt = m.CreatedAt,
                        User = new UserInMessage
                        {
                            Id = m.FromUserId,
                            Avatar = users.FirstOrDefault(u => u.Id == m.FromUserId)?.Avatar,
                            Email = users.FirstOrDefault(u => u.Id == m.FromUserId)?.Email,
                        }
                    })
                    .ToList();

                if (chatUser != null)
                {
                    var result = new GetChatByUserIdResponse
                    {
                        Id = chat.Id,
                        Type = chat.Type,
                        ChatUser = new ChatUserResponse
                        {
                            UserId = chatUser.UserId,
                            ChatId = chatUser.ChatId,
                        },
                        Users = userResponseList,
                        Messages = messageResponseList.OrderBy(s => s.CreatedAt).ToList(),
                    };
                    res.Add(result);
                }
            }

            return res;

        }
        public Task<object> AddUserToGroup(Guid chatId, Guid userId, Guid currentUserId)
        {
            throw new NotImplementedException();
        }


        public async Task<CreateChatResponse> Create(Guid partnerId, Guid userId)
        {
            if (partnerId == userId)
            {
                throw new AppException("Cannot chat with yourself!");
            }

            // Fetch the partner and user
            var user = (await _repository.GetAsync<User>(u => u.Id == userId)).FirstOrDefault();
            var partner = (await _repository.GetAsync<User>(u => u.Id == partnerId)).FirstOrDefault();

            // Check if either the partner or user is null and handle appropriately
            if (partner == null || user == null)
            {
                throw new AppException("User or partner not found.");
            }

            // Fetch the chat users for both user and partner
            var userChatUsers = await _repository.GetAsync<ChatUser>(cu => cu.UserId == userId);
            var partnerChatUsers = await _repository.GetAsync<ChatUser>(cu => cu.UserId == partnerId);

            var userChatIds = userChatUsers.Select(cu => cu.ChatId).ToList();
            var partnerChatIds = partnerChatUsers.Select(cu => cu.ChatId).ToList();

            // Fetch existing dual chats for user and partner
            var existingChats = await _repository.GetAsync<Chat>(c =>
                userChatIds.Contains(c.Id) && partnerChatIds.Contains(c.Id) && c.Type == "dual"
            );

            var commonChat = existingChats.FirstOrDefault();

            if (commonChat != null)
            {
                return new CreateChatResponse
                {
                    CreateChatResponseModels = new List<CreateChatResponseModel>
            {
                new CreateChatResponseModel
                {
                    Id = commonChat.Id,
                    Type = "dual",
                    ChatUser = new
                    {
                        chatId = commonChat.Id,
                        userId,
                        createdAt = DateTime.UtcNow,
                        updatedAt = DateTime.UtcNow
                    },
                    Users = new List<UserInMessage>
                    {
                        new UserInMessage
                        {
                            Id = userId,
                            Avatar = user.Avatar,
                            Name = user.Name,
                            Email = user.Email,
                        },
                        new UserInMessage
                        {
                            Id = partnerId,
                            Avatar = partner.Avatar,
                            Name = partner.Name,
                            Email = partner.Email,
                        }
                    }
                }
            }
                };
            }

            // Create new chat
            var chat = new Chat
            {
                Type = "dual"
            };

            await _repository.AddAsync(chat);

            var chatUsersToAdd = new List<ChatUser>
    {
        new ChatUser { UserId = userId, ChatId = chat.Id },
        new ChatUser { UserId = partnerId, ChatId = chat.Id }
    };

            await _repository.AddRangeAsync(chatUsersToAdd);

            // Prepare the response
            var forCreator = new CreateChatResponseModel
            {
                Id = chat.Id,
                Type = "dual",
                ChatUser = new
                {
                    chatId = chat.Id,
                    userId = userId,
                    createdAt = DateTime.UtcNow,
                    updatedAt = DateTime.UtcNow
                },
                Users = new List<UserInMessage>
        {
            new UserInMessage
            {
                Id = partner.Id,
                Avatar = partner.Avatar,
                Name = partner.Name,
                Email = partner.Email,
            }
        }
            };

            var forReceiver = new CreateChatResponseModel
            {
                Id = chat.Id,
                Type = "dual",
                ChatUser = new
                {
                    chatId = chat.Id,
                    userId = partnerId,
                    createdAt = DateTime.UtcNow,
                    updatedAt = DateTime.UtcNow
                },
                Users = new List<UserInMessage>
        {
            new UserInMessage
            {
                Id = user.Id,
                Avatar = user.Avatar,
                Name = user.Name,
                Email = user.Email
            }
        }
            };

            return new CreateChatResponse
            {
                CreateChatResponseModels = new List<CreateChatResponseModel> { forCreator, forReceiver }
            };
        }




        public async Task<object> Delete(Guid chatId)
        {
            // Retrieve all ChatUsers associated with the specified chatId
            var chatUsersToDelete = await _repository.GetAsync<ChatUser>(cu => cu.ChatId == chatId);

            await _repository.DeleteRangeAsync(chatUsersToDelete);

            // Retrieve all ChatMessages associated with the specified chatId
            var messagesToDelete = await _repository.GetAsync<ChatMessage>(cm => cm.ChatId == chatId);

            await _repository.DeleteRangeAsync(messagesToDelete);

            // Retrieve the Chat entity associated with the specified chatId
            var chatToDelete = await _repository.GetAsync<Chat>(c => c.Id == chatId);

            if (chatToDelete.Any())
            {
                await _repository.DeleteRangeAsync(chatToDelete);
            }

            return new
            {
                ChatId = chatId,
                NotifyUsers = chatUsersToDelete.Select(cu => cu.UserId)
            };
        }


        public Task<object> LeaveCurrentChat(Guid chatId, Guid currentUserId)
        {
            throw new NotImplementedException();
        }

        public async Task<object> Messages(Guid chatId, int page = 1)
        {
            var limit = 10;
            var offset = (page - 1) * limit;

            // Fetch total messages count
            var totalMessages = (await _repository.GetAsync<ChatMessage>(cm => cm.ChatId == chatId)).Count();

            var totalPages = Math.Ceiling(totalMessages / (double)limit);

            if (page > totalPages)
            {
                return new { messages = new List<MessageResponse>() };
            }

            // Fetch messages with pagination
            var messagesQuery = _repository.GetSet<ChatMessage>(
                cm => cm.ChatId == chatId).OrderByDescending(cm => cm.CreatedAt)
                    .Skip(offset)
                    .Take(limit); ;
            var messages = await messagesQuery.ToListAsync();
            // Fetch users information
            var fromUserIds = messages.Select(cm => cm.FromUserId).ToList();
            var userInfos = await _repository.GetAsync<User>(a => fromUserIds.Contains(a.Id));

            var userInfosDict = userInfos.ToDictionary(a => a.Id);

            // Prepare response
            var messageResponses = messages.Select(m => new MessageResponse
            {
                Id = m.Id,
                Type = m.Type,
                Message = m.Message,
                ChatId = m.ChatId,
                FromUserId = m.FromUserId,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.CreatedAt,
                User = userInfosDict.TryGetValue(m.FromUserId, out var userInfo)
                    ? new UserInMessage
                    {
                        Id = userInfo.Id,
                        Avatar = userInfo.Avatar,
                        Name = userInfo.Name,
                        Email = userInfo.Email,
                    }
                    : null
            }).ToList();

            return new { messages = messageResponses, pagination = new { page, totalPages } };
        }


        public async Task<string> UpLoadImage(Stream image)
        {
            return await _imageService.UploadImage(image);
        }

        public async Task<GetChatByUserIdResponse> GetChatById(Guid chatId)
        {
            // Retrieve the chat with the specified ID
            var chat = await _repository.FindAsync<Chat>(c => c.Id == chatId);
            if (chat == null)
            {
                return null; // or throw an exception, based on your error handling preference
            }

            // Retrieve all ChatUsers associated with the specified chatId
            var chatUsers = await _repository.GetAsync<ChatUser>(cu => cu.ChatId == chatId);
            if (chatUsers == null || !chatUsers.Any())
            {
                return null; // or handle the scenario where there are no chat users
            }
            var userIds = chatUsers.Select(cu => cu.UserId).Distinct().ToList();

            // Retrieve user details for these ChatUsers
            var users = await _repository.GetAsync<User>(u => userIds.Contains(u.Id));
            var usersDict = users.ToDictionary(u => u.Id);

            // Retrieve the latest 20 messages for this chat
            var messages = await _repository.GetSet<ChatMessage>(cm => cm.ChatId == chatId)
                                            .OrderBy(cm => cm.CreatedAt)
                                            .Take(20)
                                            .ToListAsync();

            // Prepare user response list
            var userResponseList = users.Select(user => new UserResponse
            {
                Id = user.Id,
                Avatar = user.Avatar,
                Name = user.Name,
                Email = user.Email,
                ChatUser = new ChatUserResponse
                {
                    ChatId = chatId,
                    UserId = user.Id,
                }
            }).ToList();

            // Prepare message response list
            var messageResponseList = messages.Select(m => new MessageResponse
            {
                Id = m.Id,
                Type = m.Type,
                Message = m.Message,
                ChatId = m.ChatId,
                FromUserId = m.FromUserId,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt,
                User = usersDict.TryGetValue(m.FromUserId, out var user) ? new UserInMessage
                {
                    Id = user.Id,
                    Avatar = user.Avatar,
                    Name = user.Name,
                    Email = user.Email,
                } : null
            }).ToList();

            // Prepare the response object
            var response = new GetChatByUserIdResponse
            {
                Id = chat.Id,
                Type = chat.Type,
                ChatUser = new ChatUserResponse
                {
                    UserId = chatUsers.FirstOrDefault()?.UserId ?? Guid.Empty,
                    ChatId = chatUsers.FirstOrDefault()?.ChatId ?? Guid.Empty,
                },
                Users = userResponseList,
                Messages = messageResponseList,
            };

            return response;
        }

        public async Task<List<UserDto>> GetExistUserChatByUserId(Guid id)
        {
            // Fetching ChatUsers by UserId
            var chatUsers = await _repository.GetAsync<ChatUser>(s => s.UserId == id);
            var chatIds = chatUsers.Select(cu => cu.ChatId).Distinct().ToList();

            // Fetching all ChatUsers who are part of these chats and are not the given user
            var chatUsersContainChatWithUsers = await _repository.GetAsync<ChatUser>(cu => chatIds.Contains(cu.ChatId) && cu.UserId != id);

            // Fetching distinct user IDs who had chat with the given user
            var userIds = chatUsersContainChatWithUsers.Select(cu => cu.UserId).Distinct().ToList();

            // Fetching chats that contain messages
            var chatsWithMessages = await _repository.GetSet<ChatMessage>(cm => chatIds.Contains(cm.ChatId))
                                                      .Select(cm => cm.ChatId)
                                                      .Distinct()
                                                      .ToListAsync();

            // Filter chat users who are in chats with messages
            var filteredChatUsers = chatUsersContainChatWithUsers
                                    .Where(cu => chatsWithMessages.Contains(cu.ChatId))
                                    .ToList();

            // Fetching user details for these users
            userIds = filteredChatUsers.Select(cu => cu.UserId).Distinct().ToList();
            var users = await _repository.GetAsync<User>(a => userIds.Contains(a.Id));

            // Creating a list of UserDto to return
            var res = users.Select(user => new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Avatar = user.Avatar
            }).ToList();

            return res;
        }

    }
}
