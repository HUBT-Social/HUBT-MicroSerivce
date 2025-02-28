
using FirebaseAdmin.Messaging;
using HUBTSOCIAL.Src.Features.Chat.Models;

namespace HUBT_Social_API.Features.Chat.DTOs
{
    public class MessageResponse
    {
        public string groupId { get; set; }
        public MessageModel message {get; set;} 
    }
}