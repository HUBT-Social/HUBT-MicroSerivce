namespace HUBTSOCIAL.Src.Features.Chat.Collections;

public enum TypeChatRoom
{
    /// <summary>
    /// 
    /// </summary>
    SingleChat, 
    GroupChat,
    None
}
public enum ParticipantRole
{
    Member,
    Admin,
    Owner
    
}
[Flags]
public enum MessageType
{
    None = 0,
    Picture = 1 << 0,   // Ảnh
    Video = 1 << 1,     // Video
    Media = Picture | Video, // Gộp cả Picture và Video
    Text = 1 << 2,      // Tin nhắn văn bản
    Voice = 1 << 3,     // Tin nhắn âm thanh
    Custom = 1 << 4,    // Tin nhắn tùy chỉnh
    File = 1 << 5,      // Tập tin
    All = ~0            // Tất cả các loại tin nhắn
}
public enum ReactionDetail
{
    Love,
    HaHa,
    Wow,
    Sad,
    Angry,
    Like
}
public enum MessageStatus
{

    Sent,


    Delivered,


    Read,


    Failed,


    Pending
}
public enum MessageActionStatus
{
    Normal,
    Pinned,
    Unsent
}
public enum MediaTyle
{
    Picture,
    Video
}
public enum ImageType
{
    asset,
    network,
    base64
}