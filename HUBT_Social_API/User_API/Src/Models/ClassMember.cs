using HUBT_Social_Core.Models.DTOs.IdentityDTO;

namespace User_API.Src.Models
{
    public class ClassMember
    {
        public string className { get; set; } = string.Empty;
        public List<AUserDTO> member { get; set; } = [];
    }
}
