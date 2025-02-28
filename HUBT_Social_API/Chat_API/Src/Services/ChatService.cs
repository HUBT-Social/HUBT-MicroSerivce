using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Chat.DTOs;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBTSOCIAL.Src.Features.Chat.Collections;
using HUBTSOCIAL.Src.Features.Chat.DTOs;
using HUBTSOCIAL.Src.Features.Chat.Helpers;
using HUBTSOCIAL.Src.Features.Chat.Models;
using MongoDB.Driver;

namespace HUBT_Social_API.Features.Chat.Services;

public class ChatService : IChatService
{
    private readonly IMongoCollection<ChatRoomModel> _chatRooms;
    private readonly IRoomService _roomService;

    public ChatService(IMongoCollection<ChatRoomModel> chatRooms,IRoomService roomService)
    {
        _chatRooms = chatRooms;
        _roomService = roomService;
    }

    /// <summary>
    /// Thêm một nhóm chat mới vào cơ sở dữ liệu.
    /// </summary>
    /// <param name="newRoomModel">
    /// Đối tượng chứa thông tin của nhóm chat mới, bao gồm tên nhóm, danh sách thành viên, avatar, v.v.
    /// </param>
    /// <returns>
    /// ID của nhóm chat vừa được thêm nếu thành công. 
    /// Nếu không thành công, trả về null.
    /// </returns>
    /// <example>
    /// Ví dụ:
    /// var groupId = await CreateGroupAsync(new ChatRoomModel { Name = "Group A" });
    /// </example>
    public async Task<string?> CreateGroupAsync(ChatRoomModel newRoomModel)
    {
        try
        {
            do
            {
                // Sinh ID mới dựa trên tên phòng
                newRoomModel.Id = await GenerateGroupId(newRoomModel.Name);

                // Kiểm tra xem ID đã tồn tại trong cơ sở dữ liệu hay không
                var roomExists = await _chatRooms.Find(r => r.Id == newRoomModel.Id).AnyAsync();

                // Nếu ID chưa tồn tại thì thoát vòng lặp
                if (!roomExists)
                {
                    break;
                }
            } while (true);
                        
            await _chatRooms.InsertOneAsync(newRoomModel); 
            return newRoomModel.Id;
        }
        catch
        {
            return null;
        }
    }
        private async Task<string> GenerateGroupId(string groupName)
        {
            // Loại bỏ dấu và ký tự biểu tượng (icon, emoji)
            string normalizedGroupName = new string(groupName
                .Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark && c <= 127) // Loại bỏ emoji và chỉ giữ ký tự ASCII
                .ToArray())
                .Normalize(NormalizationForm.FormC)
                .ToLowerInvariant();

            // Thay thế khoảng trắng bằng dấu '.' và loại bỏ ký tự không hợp lệ
            normalizedGroupName = Regex.Replace(normalizedGroupName, @"\s+", "."); // Thay thế khoảng trắng bằng '.'
            normalizedGroupName = Regex.Replace(normalizedGroupName, @"[^a-z0-9\.]", string.Empty); // Loại bỏ ký tự không hợp lệ

            // Tạo chuỗi ngẫu nhiên gồm 5 chữ số
            Random random = new Random();
            string randomPart = random.Next(10000, 99999).ToString();

            // Kết hợp thành ID nhóm
            return $"@{normalizedGroupName}.{randomPart}";
        }

    /// <summary>
    /// Xóa một nhóm chat khỏi cơ sở dữ liệu dựa trên ID.
    /// </summary>
    /// <param name="id">
    /// ID của nhóm chat cần xóa.
    /// </param>
    /// <returns>
    /// Trả về true nếu xóa thành công, ngược lại false.
    /// </returns>
    /// <example>
    /// Ví dụ:
    /// var isDeleted = await DeleteGroupAsync("group123");
    /// </example>
    public async Task<bool> DeleteGroupAsync(string id)
    {
        try
        {
            DeleteResult result = await _chatRooms.DeleteOneAsync(c => c.Id == id); 
            return result.DeletedCount > 0;
        }
        catch
        {
            return false;
        }
    }

    
    /// <summary>
    /// Tìm kiếm các nhóm chat theo từ khóa.
    /// </summary>
    /// <param name="keyword">
    /// Từ khóa cần tìm kiếm, áp dụng cho tên nhóm chat.
    /// </param>
    /// <returns>
    /// Danh sách các nhóm chat khớp với từ khóa tìm kiếm, chứa các trường cần thiết.
    /// Nếu từ khóa trống, trả về danh sách rỗng.
    /// </returns>
    /// <example>
    /// Ví dụ:
    /// var groups = await SearchGroupsAsync("team");
    /// </example>
    public async Task<List<RoomSearchReponse>> SearchGroupsAsync(string keyword, int page, int limit)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return new List<RoomSearchReponse>();

        var filter = Builders<ChatRoomModel>.Filter.Regex(
            cr => cr.Name, 
            new MongoDB.Bson.BsonRegularExpression(keyword, "i")
        );

        var projection = Builders<ChatRoomModel>.Projection.Expression(cr => new RoomSearchReponse
        {
            Id = cr.Id,
            GroupName = cr.Name,
            AvatarUrl = cr.AvatarUrl,
            TotalNumber = cr.Participant.Count,
        });

        return await _chatRooms
            .Find(filter)
            .Project(projection)
            .Skip((page - 1) * limit) // Bỏ qua số bản ghi tương ứng
            .Limit(limit)
            .ToListAsync();
    }

    /// <summary>
    /// Lấy danh sách tất cả các nhóm chat từ cơ sở dữ liệu.
    /// </summary>
    /// <returns>
    /// Danh sách các đối tượng `ChatRoomModel` chứa thông tin của tất cả nhóm chat.
    /// </returns>
    /// <example>
    /// Ví dụ:
    /// var allRooms = await GetAllRoomsAsync();
    /// </example>
    public async Task<List<RoomSearchReponse>> GetAllRoomsAsync( int page, int limit)
    {
        var projection = Builders<ChatRoomModel>.Projection.Expression(cr => new RoomSearchReponse
        {
            Id = cr.Id,
            GroupName = cr.Name,
            AvatarUrl = cr.AvatarUrl,
            TotalNumber = cr.Participant.Count,
        });

        // Truy vấn tất cả các nhóm
        var results = await _chatRooms
            .Find(Builders<ChatRoomModel>.Filter.Empty) // Lấy tất cả tài liệu
            .Project(projection)
            .Skip((page - 1) * limit) // Bỏ qua số bản ghi tương ứng
            .Limit(limit)
            .ToListAsync();

        return results;
    }
    /// <summary>
    /// Lấy tất cả phòng chat có chứa ID người dùng.
    /// </summary>
    /// <param name="userId">
    /// ID của người dùng, được truyền vào sau khi thực hiện decode từ Access Token.
    /// </param>
    /// <returns>
    /// Danh sách các phòng chat mà người dùng tham gia. 
    /// Nếu không có phòng nào, trả về danh sách rỗng.
    /// </returns>
    /// 
    /// <example>
    /// Ví dụ: 
    /// var rooms = await GetRoomsByIdUserAsync("user123");
    /// </example>
    public async Task<List<RoomLoadingRespone>> GetRoomsOfUserIdAsync(string userId, int page, int limit)
    {
        if (page <= 0 || limit <= 0)
            return new List<RoomLoadingRespone>();

        // Tạo bộ lọc để tìm các phòng chat có chứa userName trong danh sách Participant
        var filter = Builders<ChatRoomModel>.Filter.ElemMatch(
            cr => cr.Participant,
            p => p.UserId == userId
        );

        // Lấy các phòng chat phù hợp với bộ lọc, áp dụng phân trang
        var chatRooms = await _chatRooms
            .Find(filter)
            .SortByDescending(cr => cr.LastInteractionTime)
            .Skip((page - 1) * limit) // Bỏ qua số lượng bản ghi tương ứng với trang
            .Limit(limit)             // Giới hạn số lượng bản ghi trên mỗi trang
            .ToListAsync();

        // Duyệt qua các phòng chat và gọi GetGroupByIdAsync cho từng phòng song song
        var tasks = chatRooms.Select(cr => GetGroupByIdAsync(cr.Id)).ToList();

        // Chờ tất cả các tác vụ hoàn thành
        var listRespone = await Task.WhenAll(tasks);

        // Lọc các kết quả không phải là null
        var filteredList = listRespone.Where(r => r != null).ToList();

        // Nếu danh sách rỗng, trả về danh sách mới
        return filteredList.Any() ? filteredList : new List<RoomLoadingRespone>();
    }

    private async Task<RoomLoadingRespone?> GetGroupByIdAsync(string id)
    {
        // Tìm phòng chat theo ID
        ChatRoomModel chatRoom = await _chatRooms.Find(c => c.Id == id).FirstOrDefaultAsync();

        // Nếu không tìm thấy phòng, trả về null
        if (chatRoom == null)
            return null;

        // Lấy tin nhắn gần đây và thời gian tương tác
        var (LastInteraction, LastTime) = await GetRecentChatItemAsync(chatRoom);

        // Trả về đối tượng RoomLoadingRespone với thông tin cần thiết
        return new RoomLoadingRespone
        {
            Id = chatRoom.Id,
            GroupName = chatRoom.Name,
            AvatarUrl = chatRoom.AvatarUrl,
            LastMessage = LastInteraction,
            LastInteractionTime = LastTime
        };
    }

    private async Task<(string LastInteraction, string LastTime)> GetRecentChatItemAsync(ChatRoomModel chatRoom)
    {
        // Nếu không có danh sách ChatItems hoặc rỗng, trả về chuỗi rỗng
        if (chatRoom.Content == null || !chatRoom.Content.Any())
            return (string.Empty, string.Empty);

        // Lấy tin nhắn mới nhất dựa vào Timestamp
        var recentMessage = chatRoom.Content
            .OrderByDescending(m => m.createdAt)
            .FirstOrDefault();
        
        string LastTime = FormatLastInteractionTime(recentMessage.createdAt);

        // Lấy nickname bất đồng bộ
        string? nickName = await RoomChatHelper.GetNickNameAsync(recentMessage.id, recentMessage.sentBy);

        // Kiểm tra nếu tin nhắn là loại "Message"
        if (recentMessage.messageType == MessageType.Text)
        {
            string? recent = recentMessage.message??"";
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


}