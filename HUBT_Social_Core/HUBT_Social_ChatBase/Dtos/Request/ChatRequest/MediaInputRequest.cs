using ChatBase.Dtos.Collections;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver.Core.WireProtocol.Messages;

namespace ChatBase.Dtos.Request.ChatRequest
{
    public class MediaInputRequest
    {
        public string GroupId { get; set; } = string.Empty;
        public List<IFormFile> Medias { get; set; }
        public ReplyMessage? ReplyToMessage { get; set; }
    }
}
