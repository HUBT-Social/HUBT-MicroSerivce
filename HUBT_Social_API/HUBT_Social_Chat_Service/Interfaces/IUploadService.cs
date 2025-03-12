
using HUBT_Social_Chat_Resources.Dtos.Request.ChatRequest;
using HUBT_Social_Chat_Resources.Models;
using MongoDB.Driver;
using System.Threading.Channels;

namespace HUBT_Social_Chat_Service.Interfaces
{
    public interface IUploadService
    {
        Task SendChatAsync(ChatRequest chatRequest, IMongoCollection<ChatGroupModel> collection , Channel<(bool, MessageModel?)> channel);
    }
}
