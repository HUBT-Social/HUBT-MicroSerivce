
using AutoMapper;
using Chat_Data_API.Src.Service;
using HUBT_Social_Chat_Resources.Dtos.Collections.Enum;
using HUBT_Social_Chat_Resources.Dtos.Request.ChatRequest;
using HUBT_Social_Chat_Resources.Dtos.Response;
using HUBT_Social_Chat_Resources.Models;
using HUBT_Social_Chat_Service.Extention;
using HUBT_Social_Chat_Service.Helper;
using HUBT_Social_Chat_Service.Interfaces;
using HUBT_Social_Core.Decode;
using HUBT_Social_Core.Models.Requests.Firebase;
using HUBT_Social_Core.Settings;
using HUBT_Social_MongoDb_Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;
using System.Threading.Channels;

namespace Chat_Data_API.Src.Hubs
{
    [Authorize] // Yêu cầu token hợp lệ
    public class ChatHub : Hub
    {
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly IMongoService<ChatGroupModel> _chatGroups;
        private readonly IUploadService _uploadService;
        private readonly IMapper _mapper;
        private readonly JwtSetting _jwtSettings;
        private readonly INotition _notition;

        public ChatHub(
            IUserConnectionManager userConnectionManager,
            IMongoService<ChatGroupModel> chatGroups,
            IUploadService uploadService,
            IMapper mapper,
            IOptions<JwtSetting> jwtSettings,
            INotition notition
        )
        {
            _userConnectionManager = userConnectionManager;
            _chatGroups = chatGroups;
            _uploadService = uploadService;
            _mapper = mapper;
            _jwtSettings = jwtSettings.Value;
            _notition = notition;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                Console.WriteLine("Client connected with ConnectionId: " + Context.ConnectionId);
                var httpContext = Context.GetHttpContext();

                // Log header và query string để debug
                var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
                var queryToken = httpContext.Request.Query["access_token"].FirstOrDefault();
                Console.WriteLine($"Authorization Header: {authHeader}");
                Console.WriteLine($"Query Token: {queryToken}");
                //////////////////////////////////////////

                var userInfo = httpContext.Request.ExtractTokenInfo(_jwtSettings);
                if (userInfo == null)
                {
                    Console.WriteLine("Invalid or missing token.");
                    throw new HubException("Token không hợp lệ.");
                }

                Console.WriteLine($"User connected: UserId = {userInfo.UserId}, Username = {userInfo.Username}");
                var groupIds = await _chatGroups.GetUserGroupConnectedAsync(userInfo.Username);
                Console.WriteLine($"Group IDs: {string.Join(", ", groupIds)}");

                _userConnectionManager.AddConnection(userInfo.Username, Context.ConnectionId);
                await Task.WhenAll(groupIds.Select(groupId => Groups.AddToGroupAsync(Context.ConnectionId, groupId)));

                // Gửi danh sách người dùng online cho client mới kết nối
                var onlineUsers = _userConnectionManager.GetAllOnlineUsers();
                await Clients.Caller.SendAsync("OnlineUsersList", onlineUsers);

                // Thông báo tới các client khác rằng người dùng đã online
                await Clients.Others.SendAsync("UserStatusChanged", new { Username = userInfo.Username, isOnline = true });

                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection error: {ex.Message}");
                throw;
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userInfo = Context.GetHttpContext()?.Request.ExtractTokenInfo(_jwtSettings);
            if (userInfo != null)
            {
                await Clients.Others.SendAsync("UserStatusChanged", new { userId = userInfo.Username, isOnline = false });
                _userConnectionManager.RemoveConnection(userInfo.Username);
            }

            await base.OnDisconnectedAsync(exception);
        }



        public async Task SendItemChat(SendChatRequest inputRequest)
        {
            var userInfo = Context.GetHttpContext()?.Request.ExtractTokenInfo(_jwtSettings);
            string? token = Context.GetHttpContext()?.Request.Headers.ExtractBearerToken();
            if (userInfo == null && userInfo?.Username == null && token == null)
            {
                await Clients.Caller.SendAsync("SendErr", "Token không hợp lệ");
                return;
            }
            ChatGroupModel? chatGroupModel = await _chatGroups.GroupIdToInfo(inputRequest.GroupId);
            if (chatGroupModel == null)
            {
                await Clients.Caller.SendAsync("SendErr", "Group id sai");
                return;
            }
            var chatRequest = new ChatRequest
            {
                UserName = userInfo.Username,
                GroupId = inputRequest.GroupId,
                Content = inputRequest.Content,
                Medias = inputRequest.Medias,
                Files = inputRequest.Files
            };

            // Gửi trạng thái "Pending" cho tất cả item
            if (inputRequest.Medias is not null && inputRequest.Medias.Any())
            {
                foreach (var media in inputRequest.Medias)
                {
                    await Clients.Caller.SendAsync("ReceiveProcess", new
                    {
                        requestId = inputRequest.RequestId,
                        itemId = media.Id, // Giữ nguyên itemId do frontend gửi lên
                        type = "media",
                        status = MessageStatus.Pending
                    });
                }
            }

            if (inputRequest.Files is not null && inputRequest.Files.Any())
            {
                foreach (var file in inputRequest.Files)
                {
                    await Clients.Caller.SendAsync("ReceiveProcess", new
                    {
                        requestId = inputRequest.RequestId,
                        itemId = file.Id, // Giữ nguyên itemId do frontend gửi lên
                        type = "file",
                        status = MessageStatus.Pending
                    });
                }
            }

            if (string.IsNullOrEmpty(inputRequest.Content))
            {
                await Clients.Caller.SendAsync("ReceiveProcess", new
                {
                    requestId = inputRequest.RequestId,
                    type = "text",
                    status = MessageStatus.Pending
                });
            }

            // Sử dụng Channel để xử lý từng phần
            var channel = Channel.CreateUnbounded<(bool, MessageModel?, string)>();
            _ = _uploadService.SendChatAsync(chatRequest, _chatGroups, channel);

             bool sendSuccessful = false;

            // Đọc kết quả từ channel và gửi về client ngay khi có
            await foreach (var (success, message, itemId) in channel.Reader.ReadAllAsync())
            {
                var status = success ? MessageStatus.Sent : MessageStatus.Failed;
                if(status == MessageStatus.Sent && sendSuccessful == false)
                {
                    sendSuccessful = !sendSuccessful;
                }
                if (message is not null)
                {
                    await Clients.Caller.SendAsync("ReceiveProcess", new
                    {
                        requestId = inputRequest.RequestId,
                        itemId, // Giữ nguyên itemId của frontend
                        type = message.messageType,
                        status
                    });

                    var messageResponse = new MessageResponse<MessageDTO>
                    {
                        groupId = inputRequest.GroupId,
                        message = _mapper.Map<MessageDTO>(message)
                    };
                    await Clients.Group(inputRequest.GroupId).SendAsync("ReceiveChat", messageResponse);
                }
            }
            if (sendSuccessful)
            {
                try
                {
                    string body = inputRequest.Content != null
                        ? inputRequest.Content
                        : "You have unread message!";
                    SendGroupMessageRequest request = new SendGroupMessageRequest
                    {
                        GroupId = inputRequest.GroupId,
                        RequestId = inputRequest.RequestId,
                        Type = "chat",
                        ImageUrl = chatGroupModel.AvatarUrl,
                        Title = chatGroupModel.Name,
                        Body =  body
                    };
                    if (token != null)
                    {
                        await _notition.SendNotationToMany(request, token);
                    }
                }
                catch { }

            }
            
        }


        public async Task TypingText(string groupId)
        {
            var userInfo = Context.GetHttpContext()?.Request.ExtractTokenInfo(_jwtSettings);
            if (userInfo == null)
            {
                await Clients.Caller.SendAsync("TypingErr", "Token không hợp lệ");
                return;
            }

            try
            {
                await Clients.Group(groupId).SendAsync("ReceiveTyping", groupId, userInfo.UserId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi thông báo đang gõ: {ex.Message}");
            }
        }
    }

}


