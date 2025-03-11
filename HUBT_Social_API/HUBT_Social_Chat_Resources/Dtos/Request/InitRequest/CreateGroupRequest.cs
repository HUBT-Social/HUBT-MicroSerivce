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
}
