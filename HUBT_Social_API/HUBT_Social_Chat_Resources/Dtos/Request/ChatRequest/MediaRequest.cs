using HUBT_Social_Chat_Resources.Dtos.Collections;

namespace HUBT_Social_Chat_Resources.Dtos.Request.ChatRequest
{
    public class MediaRequest
    {
        public string? UserId { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        public FileRequest? Medias { get; set; }
        public ReplyMessage? ReplyToMessage { get; set; }
    }
}
