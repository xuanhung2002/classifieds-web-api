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
        public ChatService(IDBRepository repository)
        {
            _repository = repository;
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
                        Messages = messageResponseList,
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

            var chatUsers = await _repository.GetAsync<ChatUser>(cu => cu.UserId == partnerId || cu.UserId == userId);
            var chatIdInChatUserOfPartner = chatUsers.Where(cu => cu.UserId == partnerId).Select(cu => cu.ChatId);
            var chatIdInChatUserOfUser = chatUsers.Where(cu => cu.UserId == userId).Select(cu => cu.ChatId);
            var isChatIdOfUserAndPartnerIntersect = chatIdInChatUserOfPartner.Intersect(chatIdInChatUserOfUser).Any();
            var isChatTypeDual = isChatIdOfUserAndPartnerIntersect
                ? (await _repository.GetAsync<Chat>(c => chatIdInChatUserOfPartner.Intersect(chatIdInChatUserOfUser).Contains(c.Id) && c.Type == "dual")).Any()
                : false;
            var isUserAndPartnerAlreadyChat = isChatIdOfUserAndPartnerIntersect && isChatTypeDual;

            if (isUserAndPartnerAlreadyChat)
            {
                throw new AppException("Chat with this user already exists!");
            }

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

            var userInfos = await _repository.GetAsync<User>(a => a.Id == userId || a.Id == partnerId);
            var userInfo = userInfos.FirstOrDefault(a => a.Id == userId);
            var partnerInfo = userInfos.FirstOrDefault(a => a.Id == partnerId);

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
                        Id = partnerInfo.Id,
                        Avatar = partnerInfo.Avatar,
                        Name = partnerInfo.Name,
                        Email = partnerInfo.Email,
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
                        Id = userInfo.Id,
                        Avatar = userInfo.Avatar,
                        Name = userInfo.Name,
                        Email = userInfo.Email
                    }
                },
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
                return new { messages = new List<MessageResponse>()};
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


        public object UpLoadImage(Stream image)
        {
            throw new NotImplementedException();
        }
    }
}
