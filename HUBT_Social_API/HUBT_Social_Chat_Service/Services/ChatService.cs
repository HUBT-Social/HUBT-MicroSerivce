using HUBT_Social_Chat_Resources.Models;
using HUBT_Social_Chat_Service.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HUBT_Social_Chat_Service.ASP_Extensions;
using HUBT_Social_MongoDb_Service.Services;
using HUBT_Social_Chat_Resources.Dtos.Collections.Enum;
using HUBT_Social_Chat_Resources.Dtos.Response;
using HUBT_Social_MongoDb_Service.ASP_Extentions;
using HUBT_Social_Core.Settings;
using System.Linq.Expressions;
using HUBT_Social_Chat_Service.Extention;
using HUBT_Social_Chat_Service.Helper;

namespace HUBT_Social_Chat_Service.Services
{
    public class ChatService : IChatService
    {
        private readonly IMongoService<ChatGroupModel> _chatGroups;

        public ChatService(IMongoService<ChatGroupModel> chatRooms)
        {
            _chatGroups = chatRooms;
        }
        public async Task<(bool,string)> CreateGroupAsync(ChatGroupModel newGroupModel)
        {
            try
            {
                do
                {
                    newGroupModel.Id = newGroupModel.Name.ConvertToId();
                    var groupExists = await _chatGroups.Exists(newGroupModel.Id);
                    if (!groupExists)
                    {
                        break;
                    }
                } while (true);

                bool status = await _chatGroups.Create(newGroupModel);
                return status ? (status,newGroupModel.Id) : (status,LocalValue.Get(KeyStore.FailedToCreateGroup));
            }
            catch
            {
                return (false, LocalValue.Get(KeyStore.FailedToCreateGroup));
            }
        }
        public async Task<(bool, string)> DeleteGroupAsync(string id)
        {
            try
            {
                ChatGroupModel? chatGroupModel = await _chatGroups.GetById(id);
                if (chatGroupModel is null)
                {
                    return (false, LocalValue.Get(KeyStore.FailedToDeleteGroup));
                }
                bool status = await _chatGroups.Delete(chatGroupModel);
                return status ? (status, LocalValue.Get(KeyStore.GroupDeletedSuccessfully)) : (status, LocalValue.Get(KeyStore.FailedToDeleteGroup));
            }
            catch
            {
                return (false, LocalValue.Get(KeyStore.FailedToDeleteGroup));
            }
        }


        public async Task<List<GroupSearchResponse>> SearchGroupsAsync(string keyword, int page, int limit)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return new List<GroupSearchResponse>();

            // Predicate để lọc nhóm dựa trên keyword (tìm kiếm theo tên nhóm)
            Expression<Func<ChatGroupModel, bool>> predicate = cr => cr.Name.ToLower().Contains(keyword.ToLower());

            // Tìm các nhóm phù hợp
            var chatGroups = await _chatGroups.Find(predicate);

            // Chuyển đổi sang GroupSearchResponse
            var result = chatGroups
                .Select(cr => new GroupSearchResponse
                {
                    Id = cr.Id,
                    GroupName = cr.Name,
                    AvatarUrl = cr.AvatarUrl,
                    TotalNumber = cr.Participant.Count,
                })
                .Skip((page - 1) * limit) // Bỏ qua số bản ghi tương ứng
                .Take(limit) // Lấy số lượng bản ghi theo giới hạn
                .ToList();

            return result;
        }



        public async Task<List<GroupSearchResponse>> GetAllRoomsAsync(int page, int limit)
        {
            if (page < 1 || limit <= 0)
                return new List<GroupSearchResponse>();

            // Lấy danh sách phòng
            var rooms = await _chatGroups.Find(_ => true); // ✅ Gọi phương thức Find

            var results = rooms
                .OrderByDescending(cr => cr.CreatedAt) // ✅ Sắp xếp theo ngày tạo
                .Skip((page - 1) * limit) // ✅ Phân trang
                .Take(limit)
                .Select(cr => new GroupSearchResponse // ✅ Map sang DTO
                {
                    Id = cr.Id,
                    GroupName = cr.Name,
                    AvatarUrl = cr.AvatarUrl,
                    TotalNumber = cr.Participant.Count,
                })
                .ToList(); // ✅ Chuyển về danh sách

            return results;
        }

        public async Task<List<GroupLoadingResponse>> GetRoomsOfUserIdAsync(string userId, int page, int limit)
        {
            if (page <= 0 || limit <= 0 || string.IsNullOrEmpty(userId))
                return new List<GroupLoadingResponse>();

            Expression<Func<ChatGroupModel, bool>> predicate = cr => cr.Participant.Any(p => p.UserId == userId);

            // Truy vấn danh sách phòng, áp dụng phân trang
            var chatRooms = (await _chatGroups.Find(predicate)) // ✅ Gọi phương thức Find đã sửa
                .OrderByDescending(cr => cr.LastInteractionTime)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToList();

            if (!chatRooms.Any()) // ✅ Tránh gọi Task.WhenAll nếu không có phòng
                return new List<GroupLoadingResponse>();

            // Gọi GetGroupByIdAsync song song
            var tasks = chatRooms.Select(cr => GetGroupByIdAsync(cr));
            var responses = await Task.WhenAll(tasks);

            // Lọc kết quả không null
            return responses.Where(r => r != null).ToList();
        }


            private async Task<GroupLoadingResponse?> GetGroupByIdAsync(ChatGroupModel chatRoom)
            {
                var (LastInteraction, LastTime) = GetRecentChatItemAsync(chatRoom);


                // Trả về đối tượng RoomLoadingRespone với thông tin cần thiết
                return new GroupLoadingResponse
                {
                    Id = chatRoom.Id,
                    GroupName = chatRoom.Name,
                    AvatarUrl = chatRoom.AvatarUrl,
                    LastMessage = LastInteraction,
                    LastInteractionTime = LastTime
                };
            }

            private (string LastInteraction, string LastTime) GetRecentChatItemAsync(ChatGroupModel chatRoom)
            {
                // Nếu không có danh sách ChatItems hoặc rỗng, trả về chuỗi rỗng
                if (chatRoom.Content == null || !chatRoom.Content.Any())
                    return (string.Empty, string.Empty);

            // Lấy tin nhắn mới nhất dựa vào Timestamp
               MessageModel? recentMessage = chatRoom.Content.LastOrDefault();
                if(recentMessage == null && recentMessage?.createdAt == null)
                {
                     return (string.Empty, string.Empty);
                }
                string LastTime = FormatLastInteractionTime(recentMessage.createdAt);

                // Lấy nickname bất đồng bộ
                string? nickName = chatRoom.Content.LastOrDefault()?.sentBy.UserIdToName(chatRoom);

                // Kiểm tra nếu tin nhắn là loại "Message"
                if (recentMessage.messageType == MessageType.Text)
                {
                    string? recent = recentMessage.message ?? "";
                    // Trả về chuỗi hiển thị
                    return (GetMessagePreview(nickName, recent), LastTime);
                }
                if (recentMessage.messageType == MessageType.Media)
                {
                    return ($"{nickName}: [Photo/Media]", LastTime);
                }
                if (recentMessage.messageType == MessageType.File)
                {
                    return ($"{nickName}: [File]", LastTime);
                }
                if (recentMessage.messageType == MessageType.Voice)
                {
                    return ($"{nickName}: [Voice]", LastTime);
                }

                // Nếu không phải loại "Message", trả về chuỗi rỗng hoặc thông báo khác
                return (string.Empty, string.Empty);
            }

            private string FormatLastInteractionTime(DateTime timestamp)
            {
                var now = DateTime.Now;

                // Nếu trong cùng một ngày
                if (timestamp.Date == now.Date)
                {
                    return timestamp.ToString("HH:mm"); // {giờ:phút}
                }

                // Nếu thuộc ngày trước (trong cùng năm và tháng)
                if (timestamp.Year == now.Year && timestamp.Month == now.Month && timestamp.Day == now.Day - 1)
                {
                    return "Hôm qua";
                }

                // Kiểm tra nếu cùng tuần (trước ngày hôm qua)
                if (timestamp.Year == now.Year && timestamp.DayOfYear >= now.DayOfYear - 7 && timestamp.DayOfYear < now.DayOfYear - 1)
                {
                    return timestamp.ToString("dddd"); // {thứ}
                }

                // Nếu cùng năm nhưng khác tháng
                if (timestamp.Year == now.Year)
                {
                    return timestamp.ToString("dd/MM"); // {ngày+tháng}
                }

                // Nếu khác năm
                return timestamp.ToString("MM/yyyy"); // {tháng+năm}
            }

            private string GetMessagePreview(string? nickName, string? content)
        {
            // Kết hợp tên người gửi và nội dung tin nhắn
            string fullMessage = $"{nickName}: {content}";

            // Nếu chuỗi dài hơn 30 ký tự, cắt và thêm dấu "..."
            return fullMessage.Length > 30
                ? fullMessage.Substring(0, 30) + "..."
                : fullMessage;
        }

        public async Task<ChatGroupModel?> GetGroupById(string groupId)
        {
            var group = await _chatGroups.GetById(groupId);
            if (group == null) 
            {
                return null;
            }
            return group;
        }
    }
}
