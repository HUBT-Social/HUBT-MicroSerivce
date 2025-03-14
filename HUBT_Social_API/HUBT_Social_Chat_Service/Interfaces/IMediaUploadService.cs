
using HUBT_Social_Chat_Resources.Dtos.Request.ChatRequest;
using HUBT_Social_Chat_Resources.Models;
using HUBT_Social_MongoDb_Service.Services;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using System.Threading.Channels;

namespace HUBT_Social_Chat_Service.Interfaces
{
    public interface IMediaUploadService
    {
        Task<(bool Success, MessageModel? Message)> UploadMediaAsync(MediaRequest mediaRequest, IMongoService<ChatGroupModel> _chatRooms);
    }
}
