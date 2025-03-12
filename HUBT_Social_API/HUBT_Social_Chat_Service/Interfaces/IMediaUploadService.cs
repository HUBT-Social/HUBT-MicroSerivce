
using HUBT_Social_Chat_Resources.Dtos.Request.ChatRequest;
using HUBT_Social_Chat_Resources.Models;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;

namespace HUBT_Social_Chat_Service.Interfaces
{
    public interface IMediaUploadService
    {
        Task<(bool Success, MessageModel? Message)> UploadMediaAsync(MediaRequest chatRequest, IMongoCollection<ChatGroupModel> _chatRooms);
    }
}
