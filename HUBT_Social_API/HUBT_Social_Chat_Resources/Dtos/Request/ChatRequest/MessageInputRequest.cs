using HUBT_Social_Chat_Resources.Dtos.Collections;


namespace HUBT_Social_Chat_Resources.Dtos.Request.ChatRequest
{
    public class MessageInputRequest
    {
        public string GroupId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public ReplyMessage? ReplyToMessage { get; set; }
    }
}
