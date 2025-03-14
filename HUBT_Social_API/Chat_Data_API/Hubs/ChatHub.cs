
using AutoMapper;
using Chat_Data_API.Hubs;
using HUBT_Social_Chat_Resources.Dtos.Collections.Enum;
using HUBT_Social_Chat_Resources.Dtos.Request.ChatRequest;
using HUBT_Social_Chat_Resources.Dtos.Response;
using HUBT_Social_Chat_Resources.Models;
using HUBT_Social_Chat_Service.Extention;
using HUBT_Social_Chat_Service.Helper;
using HUBT_Social_Chat_Service.Interfaces;
using HUBT_Social_Core.Decode;
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

        public ChatHub(
            IUserConnectionManager userConnectionManager,
            IMongoService<ChatGroupModel> chatGroups,
            IUploadService uploadService,
            IMapper mapper,
            IOptions<JwtSetting> jwtSettings
        )
        {
            _userConnectionManager = userConnectionManager;
            _chatGroups = chatGroups;
            _uploadService = uploadService;
            _mapper = mapper;
            _jwtSettings = jwtSettings.Value;
            Console.WriteLine($"Jwt: {_jwtSettings.ToJson()}");
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
                var groupIds = await _chatGroups.GetUserGroupConnectedAsync(userInfo.UserId);
                Console.WriteLine($"Group IDs: {string.Join(", ", groupIds)}");

                _userConnectionManager.AddConnection(userInfo.UserId, Context.ConnectionId);
                await Task.WhenAll(groupIds.Select(groupId => Groups.AddToGroupAsync(Context.ConnectionId, groupId)));

                // Gửi danh sách người dùng online cho client mới kết nối
                var onlineUsers = _userConnectionManager.GetAllOnlineUsers();
                await Clients.Caller.SendAsync("OnlineUsersList", onlineUsers);

                // Thông báo tới các client khác rằng người dùng đã online
                await Clients.Others.SendAsync("UserStatusChanged", new { userId = userInfo.UserId, isOnline = true });

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
                await Clients.Others.SendAsync("UserStatusChanged", new { userId = userInfo.UserId, isOnline = false });
                _userConnectionManager.RemoveConnection(userInfo.UserId);
            }

            await base.OnDisconnectedAsync(exception);
        }



        public async Task SendItemChat(SendChatRequest inputRequest)
        {
            var userInfo = Context.GetHttpContext()?.Request.ExtractTokenInfo(_jwtSettings);
            if (userInfo == null)
            {
                await Clients.Caller.SendAsync("SendErr", "Token không hợp lệ");
                return;
            }
            var chatRequest = new ChatRequest
            {
                UserId = userInfo.UserId,
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
                
            if(string.IsNullOrEmpty(inputRequest.Content))
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

            // Đọc kết quả từ channel và gửi về client ngay khi có
            await foreach (var (success, message, itemId) in channel.Reader.ReadAllAsync())
            {
                var status = success ? MessageStatus.Sent : MessageStatus.Failed;

                if (message is not null)
                {
                    await Clients.Caller.SendAsync("ReceiveProcess", new
                    {
                        requestId = inputRequest.RequestId,
                        itemId = itemId, // Giữ nguyên itemId của frontend
                        type = message.messageType,
                        status = status
                    });

                    var messageResponse = new MessageResponse<MessageDTO>
                    {
                        groupId = inputRequest.GroupId,
                        message = _mapper.Map<MessageDTO>(message)
                    };
                    await Clients.Group(inputRequest.GroupId).SendAsync("ReceiveChat", messageResponse);
                }
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


