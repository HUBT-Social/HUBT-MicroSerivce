using HUBT_Social_Chat_Resources.Dtos.Collections.Enum;
using HUBT_Social_Chat_Resources.Dtos.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Chat_Resources.Models
{
    public class MessageDTO
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string key { get; set; } = Guid.NewGuid().ToString();
        public string message { get; set; } = string.Empty;
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
        public string sentBy { get; set; } = string.Empty;
        public ReplyMessage? replyMessage { get; set; }
        public Reaction? reactions { get; set; } = new();
        public MessageType messageType { get; set; }
        public MessageStatus status { get; set; } = MessageStatus.Pending;
        //public MessageActionStatus actionStatus { get; set; } = MessageActionStatus.Normal;
        public TimeSpan? voiceMessageDuration { get; set; }
    }
}
