using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Core.Models.DTOs
{
    public class ResponseUserRoleDTO
    {
        public List<AUserDTO> users = new List<AUserDTO>();
        public bool hasMore = true;
        public string? message = string.Empty;
    }
}
