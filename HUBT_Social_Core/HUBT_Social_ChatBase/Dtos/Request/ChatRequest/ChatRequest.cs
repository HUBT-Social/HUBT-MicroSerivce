namespace ChatBase.Dtos.Request.ChatRequest
{
    public class ChatRequest : SendChatRequest
    {
        public string UserId { get; set; } = string.Empty;

    }
}
