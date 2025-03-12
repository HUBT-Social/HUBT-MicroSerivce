using ChatBase.Dtos.Collections.Enum;

namespace ChatBase.Dtos.Collections
{
    public class ReplyMessage
    {
        public string message { get; set; } = string.Empty;
        //public string? FirstMediaUrl { get; set;} = string.Empty;

        public string replyBy { get; set; } = string.Empty;
        public string replyTo { get; set; } = string.Empty;
        public MessageType messageType { get; set; } = MessageType.None;
        public TimeSpan? voiceMessageDuration { get; set; } = TimeSpan.Zero;
        public string messageId { get; set; } = string.Empty;
    }
}
