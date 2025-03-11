using System.ComponentModel.DataAnnotations;

namespace ChatBase.Dtos.Request.InitRequest
{
    public class CreateGroupRequest
    {
        [Required]
        public string GroupName { get; set; }
        [Required]
        public List<string> UserIds { get; set; } = [];
    }
}
