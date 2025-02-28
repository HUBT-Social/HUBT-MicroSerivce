using System.Security.Claims;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using HUBT_Social_API.Features.Chat.DTOs;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBTSOCIAL.Src.Features.Chat.Collections;
using HUBTSOCIAL.Src.Features.Chat.Helpers;
using Microsoft.AspNetCore.SignalR;

namespace HUBT_Social_API.Features.Chat.ChatHubs;

public class ChatHub : Hub
{
    private readonly IUploadChatServices _uploadChatServices;
    private readonly IUserConnectionManager _userConnectionManager;


    public ChatHub
    (
        IUploadChatServices uploadChatServices,
        IUserConnectionManager userConnectionManager
    )
    {
        _uploadChatServices = uploadChatServices;
        _userConnectionManager = userConnectionManager;
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            var connectionId = Context.ConnectionId;

            // Lấy UserName từ Claims
            var userId = Context.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId != null)
            {
                var groupIds =
                    await RoomChatHelper.GetUserGroupConnected(userId);
                _userConnectionManager.AddConnection(userId, connectionId);
                foreach (var groupId in groupIds)
                {
                    await Groups.AddToGroupAsync(connectionId, groupId);
                }
            }

            await base.OnConnectedAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reconnecting user: {ex.Message}");
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userName = Context.User.Identity?.Name;
        if (userName != null)
        {
            await Clients.Others.SendAsync("UserDisconnected", userName);
            _userConnectionManager.RemoveConnection(userName);
        }

        await base.OnDisconnectedAsync(exception);
    }


    public async Task SendItemChat(SendChatRequest inputRequest)
    {
        var userId = Context.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            await Clients.Caller.SendAsync("SendErr", "Token không hợp lệ");
            return;
        }

        await Clients.Caller
        .SendAsync("ReceiveProcess", 
            new
            {
                requestId = inputRequest.RequestId, 
                status = MessageStatus.Pending
            });

        var chatRequest = new ChatRequest
        {
            UserId = userId,
            GroupId = inputRequest.GroupId,
            Content = inputRequest.Content,
            Medias = inputRequest.Medias,
            Files = inputRequest.Files
        };

        bool isSuccess = await _uploadChatServices.SendChatAsync(chatRequest);
        var status = isSuccess ? MessageStatus.Sent : MessageStatus.Failed;

        await Clients.Caller
            .SendAsync("ReceiveProcess", 
                new
                {
                    requestId= inputRequest.RequestId, 
                    status = status
                });
    }



    // Thông báo người dùng đang gõ
    public async Task TypingText(string groupId)
    {
        var userId = Context.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        Console.WriteLine("Id user: ");

        if(userId == null)
        {
            await Clients.Caller.SendAsync("TypingErr","Token no vali");
            return; 
        }
        try
        {
            
            await Clients.Group(groupId).SendAsync("ReceiveTyping", groupId, userId);
        }
        catch (Exception ex)
        {
            // Log lỗi và xử lý tùy theo nhu cầu
            Console.WriteLine($"Error notifying typing: {ex.Message}");
        }
    }
}