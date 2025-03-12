
using HUBT_Social_Chat_Resources.Models;

namespace HUBT_Social_Chat_Resources.Dtos.Response
{
    public class MessageResponse
    {
        public string groupId { get; set; }
        public MessageModel message { get; set; }
    }
}
