
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
            var httpContext = Context.GetHttpContext();
            var userInfo = httpContext?.Request.ExtractTokenInfo(_jwtSettings);
            if (userInfo == null)
                throw new HubException("Token không hợp lệ.");

            var groupIds = await _chatGroups.GetUserGroupConnectedAsync(userInfo.Username);

            _userConnectionManager.AddConnection(userInfo.Username, Context.ConnectionId);
            await Task.WhenAll(groupIds.Select(groupId => Groups.AddToGroupAsync(Context.ConnectionId, groupId)));

            // Gửi danh sách người dùng online cho client mới kết nối
            var onlineUsers = _userConnectionManager.GetAllOnlineUsers();
            await Clients.Caller.SendAsync("OnlineUsersList", onlineUsers);

            // Thông báo tới các client khác rằng người dùng đã online
            await Clients.Others.SendAsync("UserStatusChanged", new { Username = userInfo.Username, isOnline = true });

            await base.OnConnectedAsync();
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
            Console.WriteLine("SendItemChat 1");
            var httpContext = Context.GetHttpContext();
            var userInfo = httpContext?.Request.ExtractTokenInfo(_jwtSettings);

            Console.WriteLine("SendItemChat 2");
            // Kiểm tra token
            if (userInfo == null && userInfo?.Username == null && userInfo?.Token == null)
            {
                await Clients.Caller.SendAsync("SendErr", "Token không hợp lệ");
                return;
            }

            Console.WriteLine("SendItemChat 3");
            // Kiểm tra GroupId
            var chatGroupModel = await _chatGroups.GroupIdToInfo(inputRequest.GroupId);
            if (chatGroupModel == null)
            {
                await Clients.Caller.SendAsync("SendErr", "Group id sai");
                return;
            }

            Console.WriteLine("SendItemChat 4");
            // Tạo yêu cầu chat
            var chatRequest = new ChatRequest
            {
                UserName = userInfo.Username,
                GroupId = inputRequest.GroupId,
                Content = inputRequest.Content,
                Medias = inputRequest.Medias,
                Files = inputRequest.Files
            };

            Console.WriteLine("SendItemChat 5");
            // Gửi trạng thái "Pending" cho media
            if (inputRequest.Medias?.Any() == true)
            {
                foreach (var media in inputRequest.Medias)
                {
                    await Clients.Caller.SendAsync("ReceiveProcess", new
                    {
                        requestId = inputRequest.RequestId,
                        itemId = media.Id,
                        type = "media",
                        status = MessageStatus.Pending
                    });
                }
            }

            Console.WriteLine("SendItemChat 6");
            // Gửi trạng thái "Pending" cho file
            if (inputRequest.Files?.Any() == true)
            {
                foreach (var file in inputRequest.Files)
                {
                    await Clients.Caller.SendAsync("ReceiveProcess", new
                    {
                        requestId = inputRequest.RequestId,
                        itemId = file.Id,
                        type = "file",
                        status = MessageStatus.Pending
                    });
                }
            }

            Console.WriteLine("SendItemChat 7");
            // Nếu chỉ gửi nội dung văn bản
            if (!string.IsNullOrWhiteSpace(inputRequest.Content))
            {
                await Clients.Caller.SendAsync("ReceiveProcess", new
                {
                    requestId = inputRequest.RequestId,
                    type = "text",
                    status = MessageStatus.Pending
                });
            }

            Console.WriteLine("SendItemChat 8");
            // Dùng channel để xử lý message bất đồng bộ
            var channel = Channel.CreateUnbounded<(bool, MessageModel?, string)>();
            _ = _uploadService.SendChatAsync(chatRequest, _chatGroups, channel);

            bool sendSuccessful = false;

            Console.WriteLine("SendItemChat 9");
            // Đọc dữ liệu trả về từ channel và gửi phản hồi ngay cho client
            await foreach (var (success, message, itemId) in channel.Reader.ReadAllAsync())
            {
                var status = success ? MessageStatus.Sent : MessageStatus.Failed;

                if (!sendSuccessful && status == MessageStatus.Sent)
                {
                    sendSuccessful = true;
                }

                if (message != null)
                {
                    Console.WriteLine("SendItemChat check 1");

                    // Gửi trạng thái xử lý
                    await Clients.Caller.SendAsync("ReceiveProcess", new
                    {
                        requestId = inputRequest.RequestId,
                        itemId,
                        type = message.messageType,
                        status
                    });

                    Console.WriteLine("SendItemChat check 2");
                    // Gửi message cho nhóm
                    var messageResponse = new MessageResponse<MessageDTO>
                    {
                        groupId = inputRequest.GroupId,
                        message = _mapper.Map<MessageDTO>(message)
                    };
                    await Clients.Group(inputRequest.GroupId).SendAsync("ReceiveChat", messageResponse);
                }
            }

            Console.WriteLine("SendItemChat 10");
            // Gửi thông báo (notification) nếu gửi thành công
            if (sendSuccessful && userInfo?.Token != null)
            {
                try
                {
                    Console.WriteLine("SendItemChat 11");
                    string body = string.IsNullOrEmpty(inputRequest.Content)
                        ? "You have unread message!"
                        : inputRequest.Content;

                    var receiverUsernames = chatGroupModel.Participant
                        .Select(p => p.UserName)
                        .Where(u => u != userInfo.Username)
                        .ToList();

                    Console.WriteLine("SendItemChat 12");
                    var notifyRequest = new SendNotationToGroupChatRequest
                    {
                        UserNames = receiverUsernames,
                        RequestId = inputRequest.RequestId,
                        Type = "chat",
                        ImageUrl = chatGroupModel.AvatarUrl,
                        Title = chatGroupModel.Name,
                        Body = body
                    };

                    await _notition.SendNotationToGroupChat(notifyRequest, userInfo.Token);
                }
                catch
                {
                    // Ignore exceptions from notification sending
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

