
using MongoDB.Driver;
using HUBT_Social_Base.Models;
using HUBT_Social_Base.Service;
using HUBT_Social_Chat_Resources.Dtos.Request.ChatRequest;
using HUBT_Social_Chat_Service.Interfaces;
using HUBT_Social_Chat_Resources.Models;
using HUBT_Social_Chat_Service.Extention;
using HUBT_Social_Chat_Resources.Dtos.Collections;

namespace HUBT_Social_Chat_Service.Services
{
    public class MediaUploadService : IMediaUploadService 
    {
        private readonly ICloudService _cloudService;
        public MediaUploadService(ICloudService cloudService) 
        {
            _cloudService = cloudService;
        }
        public async Task<(bool Success, MessageModel? Message)> UploadMediaAsync(MediaRequest mediaRequest, IMongoCollection<ChatGroupModel> _chatRooms)
        {

            // Lấy ChatRoom từ MongoDB
            var filterGetChatRoom = Builders<ChatGroupModel>.Filter.Eq(cr => cr.Id, mediaRequest.GroupId);
            ChatGroupModel chatRoom = await _chatRooms.Find(filterGetChatRoom).FirstOrDefaultAsync();

            if (chatRoom == null)
            {
                return (false, null); // Không có chatroom, trả về false và null
            }


            List<FilePaths> FilePaths = new List<FilePaths>();


            // Xử lý danh sách file tải lên
            if (mediaRequest.Medias != null)
            {

                List<FileUploadResult> fileUrls = await _cloudService.UploadFilesAsync(mediaRequest.Medias);
                
                if (fileUrls != null)
                {
                    foreach (var item in fileUrls)
                    {
                        FilePaths newFilePath = new FilePaths
                        {
                            Url = item.Url,
                            Type = item.ResourceType
                        };
                        FilePaths.Add(newFilePath);
                    }


                }
            }

            MessageModel message = await MessageModel.CreateMediaMessageAsync(mediaRequest.UserId, FilePaths, mediaRequest.ReplyToMessage);

        
            UpdateResult updateResult = await _chatRooms.SaveChatItemAsync(chatRoom.Id, message);

            return (updateResult.ModifiedCount > 0, message);
        }
    }
}
