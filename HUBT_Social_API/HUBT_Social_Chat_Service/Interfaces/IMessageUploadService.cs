
using HUBT_Social_Chat_Resources.Dtos.Request.ChatRequest;
using HUBT_Social_Chat_Resources.Models;
using HUBT_Social_MongoDb_Service.Services;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;

namespace HUBT_Social_Chat_Service.Interfaces
{
    public interface IMessageUploadService
    {
        Task<(bool Success, MessageModel? Message)> UploadMessageAsync(MessageRequest chatRequest, IMongoService<ChatGroupModel> _chatRooms);
    }
}
