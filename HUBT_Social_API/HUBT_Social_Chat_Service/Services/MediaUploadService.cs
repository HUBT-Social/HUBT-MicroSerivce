
using MongoDB.Driver;
using HUBT_Social_Base.Models;
using HUBT_Social_Base.Service;
using HUBT_Social_Chat_Resources.Dtos.Request.ChatRequest;
using HUBT_Social_Chat_Service.Interfaces;
using HUBT_Social_Chat_Resources.Models;
using HUBT_Social_Chat_Service.Extention;
using HUBT_Social_Chat_Resources.Dtos.Collections;
using HUBT_Social_MongoDb_Service.Services;
using HUBT_Social_MongoDb_Service.ASP_Extentions;
using Microsoft.AspNetCore.Http;
using System.Threading.Channels;

namespace HUBT_Social_Chat_Service.Services
{
    public class MediaUploadService : IMediaUploadService 
    {
        private readonly ICloudService _cloudService;
        public MediaUploadService(ICloudService cloudService) 
        {
            _cloudService = cloudService;
        }
        public async Task<(bool Success, MessageModel? Message)> UploadMediaAsync(MediaRequest mediaRequest, IMongoService<ChatGroupModel> _chatRooms)
        {
            var chatRoom = await _chatRooms.Find(cr => cr.Id == mediaRequest.GroupId).FirstOrDefaultAsync();
            if (chatRoom == null)
            {
                return (false, null);
            }

            var media = mediaRequest.Medias;
            if (media?.file == null)
            {
                return (false, null);
            }

            // Upload file lên cloud
            var fileResult = await _cloudService.UploadFileAsync(media.file);
            if (fileResult == null)
            {
                return (false, null);
            }

            var filePath = new FilePaths
            {
                Url = fileResult.Url,
                Type = fileResult.ResourceType
            };

            var message = await MessageModel.CreateMediaMessageAsync(mediaRequest.UserId, filePath , mediaRequest.Medias.Id, mediaRequest.ReplyToMessage);
            var updateResult = await _chatRooms.SaveChatItemAsync(chatRoom.Id, message);

            return (updateResult, message);
        }

    }
}
