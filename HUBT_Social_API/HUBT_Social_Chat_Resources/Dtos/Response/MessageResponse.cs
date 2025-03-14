
using HUBT_Social_Chat_Resources.Models;

namespace HUBT_Social_Chat_Resources.Dtos.Response
{
    public class MessageResponse<T>
    {
        public string groupId { get; set; }
        public T message { get; set; }
    }
}
