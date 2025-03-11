using ChatBase.Dtos.Collections;
using ChatBase.Dtos.Collections.Enum;
using MongoDB.Bson;
using MongoDB.Driver.Core.WireProtocol.Messages;

namespace ChatBase.Models
{
    public class MessageModel
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

        // Constructor private để ép buộc dùng factory method
        private MessageModel(string sentBy, MessageType messageType, string message = null, ReplyMessage? replyMessage = null)
        {
            this.sentBy = sentBy;
            this.message = message;
            this.messageType = messageType;
            this.replyMessage = replyMessage;
        }

        // Factory method cho tin nhắn văn bản
        public static async Task<MessageModel> CreateTextMessageAsync
        (
            string sentBy,
            string content,
            ReplyMessage? replyMessage = null
        )
        {
            var message = new MessageModel(sentBy, MessageType.Text, content, replyMessage);
            return message;
        }

        // Factory method cho tin nhắn có file
        public static async Task<MessageModel> CreateMediaMessageAsync
        (
            string sentBy,
            List<FilePaths> filePaths,
            ReplyMessage? replyMessage = null
        )
        {
            string mess = filePaths.ToJson();
            var message = new MessageModel(sentBy, MessageType.Media, mess, replyMessage);
            return message;
        }


    }
}
