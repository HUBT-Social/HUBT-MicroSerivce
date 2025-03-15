using HUBT_Social_Chat_Resources.Models;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using System.ComponentModel.DataAnnotations;

namespace HUBT_Social_Chat_Resources.Dtos.Request.InitRequest
{
    public class CreateGroupRequest
    {
        [Required]
        public string GroupName { get; set; }
        [Required]
        public List<string> UserIds { get; set; } = [];
    }
    public class CreateGroupRequestData
    {
        [Required]
        public string GroupName { get; set; }
        [Required]
        public List<Participant> Participants { get; set; } = [];
    }
}
