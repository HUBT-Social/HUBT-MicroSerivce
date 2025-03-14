using HUBT_Social_Chat_Resources.Dtos.Collections;
using Microsoft.AspNetCore.Http;

namespace HUBT_Social_Chat_Resources.Dtos.Request.ChatRequest
{
    public class MediaInputRequest
    {
        public string GroupId { get; set; } = string.Empty;
        public List<FileRequest> Medias { get; set; }
        public ReplyMessage? ReplyToMessage { get; set; }
    }
}
