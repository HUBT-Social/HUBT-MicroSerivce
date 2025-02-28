
using HUBT_Social_API.Features.Chat.ChatHubs;
using HUBT_Social_API.Features.Chat.DTOs;
using Microsoft.AspNetCore.SignalR;
namespace HUBT_Social_API.Features.Chat.Services.Interfaces;
public interface IMessageUploadService
{
    Task<bool> UploadMessageAsync(MessageRequest chatRequest,IHubContext<ChatHub> hubContext);
}