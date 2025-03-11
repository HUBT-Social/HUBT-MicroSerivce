using Chat_API.Src.Models;
using ChatBase.Models;

namespace ChatBase.Dtos.Response
{
    public class MessageResponse
    {
        public string groupId { get; set; }
        public MessageModel message { get; set; }
    }
}
