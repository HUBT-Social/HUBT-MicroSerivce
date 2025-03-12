
using Chat_Data_API.Hubs;
using HUBT_Social_Chat_Resources.Dtos.Collections.Enum;
using HUBT_Social_Chat_Resources.Dtos.Request.ChatRequest;
using HUBT_Social_Chat_Resources.Dtos.Response;
using HUBT_Social_Chat_Resources.Models;
using HUBT_Social_Chat_Service.Extention;
using HUBT_Social_Chat_Service.Helper;
using HUBT_Social_Chat_Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Text.RegularExpressions;
using System.Threading.Channels;

namespace Chat_Data_API.Src.Hubs
{
    [Authorize] // Yêu cầu token hợp lệ
    public class ChatHub : Hub
    {
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly IMongoCollection<ChatGroupModel> _chatRooms;
        private readonly IUploadService _uploadService;

        public ChatHub(
            IUserConnectionManager userConnectionManager,
            IMongoCollection<ChatGroupModel> chatRooms,
            IUploadService uploadService
        )
        {
            _userConnectionManager = userConnectionManager;
            _chatRooms = chatRooms;
            _uploadService = uploadService;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var connectionId = Context.ConnectionId;
                var userInfo = TokenHelper.GetUserInfoFromRequest(Context.GetHttpContext().Request);

                if (userInfo == null)
                {
                    throw new HubException("Token không hợp lệ.");
                }

                var groupIds = await _chatRooms.GetUserGroupConnectedAsync(userInfo.UserId);
                _userConnectionManager.AddConnection(userInfo.UserId, connectionId);

                await Task.WhenAll(groupIds.Select(groupId => Groups.AddToGroupAsync(connectionId, groupId)));
                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi kết nối: {ex.Message}");
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userInfo = TokenHelper.GetUserInfoFromRequest(Context.GetHttpContext().Request);
            if (userInfo != null)
            {
                await Clients.Others.SendAsync("UserDisconnected", userInfo.UserId);
                _userConnectionManager.RemoveConnection(userInfo.UserId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendItemChat(SendChatRequest inputRequest)
        {
            var userInfo = TokenHelper.GetUserInfoFromRequest(Context.GetHttpContext().Request);
            if (userInfo == null)
            {
                await Clients.Caller.SendAsync("SendErr", "Token không hợp lệ");
                return;
            }

            await Clients.Caller.SendAsync("ReceiveProcess", new
            {
                requestId = inputRequest.RequestId,
                status = MessageStatus.Pending
            });

            var chatRequest = new ChatRequest
            {
                UserId = userInfo.UserId,
                GroupId = inputRequest.GroupId,
                Content = inputRequest.Content,
                Medias = inputRequest.Medias,
                Files = inputRequest.Files
            };

            // Sử dụng Channel để xử lý từng phần
            var channel = Channel.CreateUnbounded<(bool, MessageModel?)>();
            _ = _uploadService.SendChatAsync(chatRequest, _chatRooms, channel);

            // Đọc kết quả từ channel và gửi về client ngay khi có
            await foreach (var (success, message) in channel.Reader.ReadAllAsync())
            {
                var status = success ? MessageStatus.Sent : MessageStatus.Failed;

                if (message is not null)
                {
                    // Gửi kết quả từng phần về client
                    await Clients.Caller.SendAsync("ReceiveProcess", new
                    {
                        requestId = inputRequest.RequestId,
                        status = status
                    });
                    MessageResponse messageResponse = new MessageResponse
                    {
                        groupId = inputRequest.GroupId,
                        message = message
                    };
                    await Clients.Group(chatRequest.GroupId).SendAsync("ReceiveChat", messageResponse);

                }
            }

            // Khi toàn bộ đã hoàn thành, gửi trạng thái cuối cùng
            await Clients.Caller.SendAsync("ReceiveProcess", new
            {
                requestId = inputRequest.RequestId,
                status = MessageStatus.Sent
            });
        }

        public async Task TypingText(string groupId)
        {
            var userInfo = TokenHelper.GetUserInfoFromRequest(Context.GetHttpContext().Request);
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


