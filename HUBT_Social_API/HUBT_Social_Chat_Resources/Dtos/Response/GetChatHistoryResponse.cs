using HUBT_Social_Chat_Resources.Models;

namespace HUBT_Social_Chat_Resources.Dtos.Response
{
    public class GetChatHistoryResponse
    {
        public string groupId { get; set; } = string.Empty;
        public List<MessageModel>? messages { get; set; }
    }
}