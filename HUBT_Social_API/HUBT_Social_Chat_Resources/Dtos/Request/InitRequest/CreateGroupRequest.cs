using HUBT_Social_Chat_Resources.Dtos.Collections.Enum;
using HUBT_Social_Chat_Resources.Models;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
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
        public int GroupType = 0;
    }
    public class CreateGroupRequestData
    {
        [Required]
        public string GroupName { get; set; } = string.Empty;
        [Required]
        public List<Participant> Participants { get; set; } = [];
        [Required]
        public TypeChatRoom GroupType = TypeChatRoom.SingleChat;
    }
}
