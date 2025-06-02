using HUBT_Social_Chat_Resources.Models;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Settings.@enum;
using System.ComponentModel.DataAnnotations;

namespace HUBT_Social_Chat_Resources.Dtos.Request.InitRequest
{
    public class CreateGroupRequest
    {
        [Required]
        public string GroupName { get; set; } = string.Empty;
        [Required]
        public List<string> UserNames { get; set; } = [];
        [Required]
        public TypeChatRoom GroupType { get; set; } = TypeChatRoom.SingleChat;
    }
    public class CreateGroupRequestData
    {
        [Required]
        public string GroupName { get; set; } = string.Empty;
        [Required]
        public List<Participant> Participants { get; set; } = [];
        [Required]
        public TypeChatRoom GroupType { get; set; } = TypeChatRoom.SingleChat;
    }
}
