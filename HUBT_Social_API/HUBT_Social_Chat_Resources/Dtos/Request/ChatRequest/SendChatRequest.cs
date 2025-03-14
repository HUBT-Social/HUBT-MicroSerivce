using Microsoft.AspNetCore.Http;

namespace HUBT_Social_Chat_Resources.Dtos.Request.ChatRequest
{
    public class SendChatRequest
    {
        public string RequestId { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        public string? Content { get; set; } = null;
        public List<FileRequest>? Medias { get; set; } = null;
        public List<FileRequest>? Files { get; set; } = null;
        public string? ReplyToMessageId { get; set; }
    }
    public class FileRequest
    {
        public string Id { get; set; } = string.Empty;
        public IFormFile? file { get; set; }
    }

}
