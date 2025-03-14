using HUBT_Social_Chat_Resources.Dtos.Collections.Enum;
using HUBT_Social_Chat_Resources.Dtos.Collections;
using HUBT_Social_Chat_Resources.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Chat_Resources.Dtos.Response
{
    //chỉ dùng cho việc test
    public class TetsResponse
    {
        public string groupId { get; set; } = string.Empty;
        //public List<MessageModel> messages { get; set; }
        public Test messageModel { get; set; }

    }
    public class Test
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string key { get; set; } = Guid.NewGuid().ToString();
        public string message { get; set; }
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
        public string sentBy { get; set; }
        public ReplyMessage? replyMessage { get; set; }
        public Reaction? reactions { get; set; } = new();
        public MessageType messageType { get; set; }
        public MessageStatus status { get; set; } = MessageStatus.Pending;
        //public MessageActionStatus actionStatus { get; set; } = MessageActionStatus.Normal;
        public TimeSpan? voiceMessageDuration { get; set; }
    }
}    
