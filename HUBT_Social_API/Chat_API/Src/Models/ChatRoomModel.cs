using HUBTSOCIAL.Src.Features.Chat.Collections;
using MongoDB.Bson.Serialization.Attributes;


namespace HUBTSOCIAL.Src.Features.Chat.Models;

public class ChatRoomModel
{
    [BsonId]
    public string Id { get; set; } = string.Empty; // Id của phòng chat
    public string Name { get; set; } = string.Empty; // Tên của phòng chat
    public string AvatarUrl { get; set; } = string.Empty; // URL ảnh đại diện của phòng chat
    public string BackGroundUrl { get; set; } = string.Empty;
    public DateTime LastInteractionTime { get; set; }
    public List<Participant> Participant { get; set; } = new(); // Danh sách ID người dùng tham gia phòng chat
    public List<MessageModel> Content { get; set; } = new(); // Danh sách các tin nhắn, file, media trong phòng chat
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời gian tạo phòng chat
    public TypeChatRoom TypeChatRoom { get; set; } = TypeChatRoom.None;

}

