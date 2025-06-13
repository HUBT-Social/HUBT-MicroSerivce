using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Core.Models.DTOs.UserDTO
{
    public class GetUserByRoleResponses
    {
        public List<AUserDTO> AUserDTOs { get; set; } = [];
        public bool HasMore { get; set; } = false;
        public string? Message { get; set; } = string.Empty;
    }
}
