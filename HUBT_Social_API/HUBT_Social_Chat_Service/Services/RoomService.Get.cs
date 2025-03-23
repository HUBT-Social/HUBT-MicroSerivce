using HUBT_Social_Chat_Resources.Dtos.Request.GetRequest;
using HUBT_Social_Chat_Resources.Dtos.Response;
using HUBT_Social_Chat_Resources.Models;
using HUBT_Social_Chat_Service.Interfaces;
using HUBT_Social_MongoDb_Service.ASP_Extentions;
using HUBT_Social_MongoDb_Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Chat_Service.Services
{
    public class RoomGetService(IMongoService<ChatGroupModel> chatGroups) : IRoomGetService
    {
        private readonly IMongoService<ChatGroupModel> _chatGroups = chatGroups;
    
        public async Task<List<MessageModel>> GetMessageHistoryAsync(GetHistoryRequest request)
        {
            List<MessageModel> messages = new List<MessageModel>();

            // Tìm phòng chat dựa trên ID phòng
            var chatRoom = await _chatGroups.Find(c => c.Id == request.ChatRoomId).FirstOrDefaultAsync();

            // Nếu không tìm thấy phòng chat, trả về danh sách rỗng
            if (chatRoom == null)
                return messages;
            int totalMessages = chatRoom.Content.Count;
            int count = request.Limit ?? 20;
            int startIndex = chatRoom.Content.Count - count - request.CurrentQuantity;
            if (startIndex < 0)
            {
                startIndex = 0;
                count = totalMessages - request.CurrentQuantity;
            }


            // Tìm ChatHistory dựa trên RoomId
            var chatHistory = await _chatGroups
                .GetSlide<MessageModel>
                    (
                        request.ChatRoomId,
                        cr => cr.Content,
                        startIndex,
                        count
                    );



            // Lọc theo MessageType và sắp xếp theo thời gian tăng dần
            //var filteredItems = chatHistory
            //    //.Where(item => request.Type == MessageType.All ||
            //    //            (item.messageType & request.Type) != 0)
            //    //.OrderBy(item => item.createdAt) // Sắp xếp tăng dần thay vì giảm dần
            //    .ToList();

            return chatHistory;
        }

        public async Task<(List<ChatUserResponse>,ChatGroupModel)> GetRoomUserAsync(string groupId)
        {
            var chatRoom = await _chatGroups.Find(c => c.Id == groupId).FirstOrDefaultAsync();


            if (chatRoom == null || chatRoom.Participant == null)
            {
                return (new List<ChatUserResponse>(),new ChatGroupModel());
            }

            // Lấy thêm thông tin user nếu cần
            var participants = chatRoom.Participant;
            List<ChatUserResponse> res = new List<ChatUserResponse>();
            foreach (var participant in participants)
            {
                ChatUserResponse chatUserResponse = new ChatUserResponse();
                chatUserResponse.id = participant.UserId;
                chatUserResponse.name = participant.NickName;
                chatUserResponse.profilePhoto = participant.ProfilePhoto ?? participant.DefaultAvatarImage;
                res.Add(chatUserResponse);
            }

            return (res,chatRoom);
        }
    }
}
