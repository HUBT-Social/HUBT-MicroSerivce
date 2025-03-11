namespace HUBT_Social_Chat_Resources.Dtos.Collections.Enum
{
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
}
