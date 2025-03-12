using HUBT_Social_Chat_Resources.Dtos.Collections.Enum;
using System.ComponentModel.DataAnnotations;

namespace HUBT_Social_Chat_Resources.Dtos.Request.GetRequest
{
    public class GetHistoryRequest
    {
        [Required]
        public string ChatRoomId { get; set; } = string.Empty;
        public int CurrentQuantity { get; set; } = 0;
        public int? Limit { get; set; } = 20;
        public MessageType? Type { get; set; } = MessageType.All;
    }
}
