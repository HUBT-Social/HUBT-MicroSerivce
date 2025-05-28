
using HUBT_Social_Chat_Resources.Dtos.Collections;
using HUBT_Social_Chat_Resources.Dtos.Collections.Enum;
using MongoDB.Bson;
using Newtonsoft.Json;


namespace HUBT_Social_Chat_Resources.Models
{
    public class MessageModel
    {
        public string id { get; set; }
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
        private MessageModel(string sentBy, MessageType messageType, string itemId, string message = null, ReplyMessage? replyMessage = null)
        {
            this.sentBy = sentBy;
            this.message = message;
            this.messageType = messageType;
            this.replyMessage = replyMessage;
            this.id = itemId;
        }

        // Factory method cho tin nhắn văn bản
        public static MessageModel CreateTextMessage
        (
            string sentBy,
            string content,
            string itemId,
            ReplyMessage? replyMessage = null
        )
        {
            var message = new MessageModel(sentBy, MessageType.Text, itemId, content, replyMessage);
            return message;
        }

        // Factory method cho tin nhắn có file
        public static MessageModel CreateMediaMessage
        (
            string sentBy,
            string filePaths,
            string itemId,
            ReplyMessage? replyMessage = null
        )
        {
            var message = new MessageModel(sentBy, MessageType.Media, itemId, filePaths, replyMessage);
            return message;
        }

        public static MessageModel CreateMediaMessageAsync(string? userId, string fileResult, string id, ReplyMessage? replyToMessage)
        {
            throw new NotImplementedException();
        }
    }
}
