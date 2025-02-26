using System.Security.Claims;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using HUBT_Social_API.Features.Chat.DTOs;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBTSOCIAL.Src.Features.Chat.Collections;
using HUBTSOCIAL.Src.Features.Chat.Helpers;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace HUBT_Social_API.Features.Chat.ChatHubs;

public interface IUserConnectionManager
{
    void AddConnection(string userId, string connectionId);
    void RemoveConnection(string userId);
    string GetConnectionId(string userId);
    IEnumerable<string> GetUserConnections(string userId);
}

public class UserConnectionManager : IUserConnectionManager
{
    private readonly ConcurrentDictionary<string, HashSet<string>> _userConnections 
        = new ConcurrentDictionary<string, HashSet<string>>();

    public void AddConnection(string userId, string connectionId)
    {
        var connections = _userConnections.GetOrAdd(userId, _ => new HashSet<string>());
        lock (connections)
        {
            connections.Add(connectionId);
        }
    }

    public void RemoveConnection(string userId)
    {
        if (_userConnections.TryGetValue(userId, out var connections))
        {
            lock (connections)
            {
                connections.Clear();
                _userConnections.TryRemove(userId, out _);
            }
        }
    }

    public string GetConnectionId(string userId)
    {
        return _userConnections.TryGetValue(userId, out var connections) 
            ? connections.FirstOrDefault() 
            : null;
    }

    public IEnumerable<string> GetUserConnections(string userId)
    {
        return _userConnections.TryGetValue(userId, out var connections) 
            ? connections.ToList() 
            : Enumerable.Empty<string>();
    }
}

public class ChatHub : Hub
{
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly ITokenService _tokenService;
    private readonly IUploadChatServices _uploadChatServices;
    private readonly IUserConnectionManager _userConnectionManager;

    public ChatHub(
        IHubContext<ChatHub> hubContext,
        IUploadChatServices uploadChatServices,
        ITokenService tokenService,
        IUserConnectionManager userConnectionManager)
    {
        _hubContext = hubContext;
        _uploadChatServices = uploadChatServices;
        _tokenService = tokenService;
        _userConnectionManager = userConnectionManager;
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            var connectionId = Context.ConnectionId;
            var userId = Context.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                var groupIds = await RoomChatHelper.GetUserGroupConnected(userId);
                _userConnectionManager.AddConnection(userId, connectionId);

                foreach (var groupId in groupIds)
                {
                    await Groups.AddToGroupAsync(connectionId, groupId);
                    await Clients.Group(groupId).SendAsync("UserRejoined", new { userId, groupId });
                }
            }

            await base.OnConnectedAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi kết nối lại người dùng: {ex.Message}");
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            await Clients.Others.SendAsync("UserDisconnected", userId);
            _userConnectionManager.RemoveConnection(userId);
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

        await Clients.Caller.SendAsync("ReceiveProcess", inputRequest.RequestId, MessageStatus.Pending);

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

        await Clients.Caller.SendAsync("ReceiveProcess", inputRequest.RequestId, status);
    }

    public async Task TypingText(string groupId)
    {
        var userId = Context.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            await Clients.Caller.SendAsync("TypingErr", "Token không hợp lệ");
            return;
        }

        try
        {
            await _hubContext.Clients.Group(groupId).SendAsync("ReceiveTyping", groupId, userId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi thông báo đang gõ: {ex.Message}");
        }
    }
}