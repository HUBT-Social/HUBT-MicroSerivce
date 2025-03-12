using ChatBase.Dtos.Collections.Enum;
using System.ComponentModel.DataAnnotations;

namespace ChatBase.Dtos.Request.GetRequest
{
    public class GetHistoryRequest
    {
        [Required]
        public string ChatRoomId { get; set; } = string.Empty;
        public int Page { get; set; }
        public int Limit { get; set; }
        public MessageType Type { get; set; }
        public DateTime? Time { get; set; } = DateTime.Now;
    }
}
