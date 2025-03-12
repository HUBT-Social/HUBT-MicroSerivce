using ChatBase.Dtos.Collections;
using MongoDB.Driver.Core.WireProtocol.Messages;

namespace ChatBase.Dtos.Request.ChatRequest
{
    public class MessageInputRequest
    {
        public string GroupId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public ReplyMessage? ReplyToMessage { get; set; }
    }
}
