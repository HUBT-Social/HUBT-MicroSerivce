using System.ComponentModel.DataAnnotations;

namespace HUBTSOCIAL.Src.Features.Chat.DTOs
{
    public class CreateGroupRequest
    {
        [Required]
        public string GroupName { get; set; }
        [Required]
        public List<string> UserIds { get; set; } = [];
    }

}