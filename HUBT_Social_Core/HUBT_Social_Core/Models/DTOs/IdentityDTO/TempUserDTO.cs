using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Core.Models.DTOs.IdentityDTO
{
    public class TempUserDTO
    {
        public string Email { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;


        public string Password { get; set; } = string.Empty;

        public DateTime ExpireTime { get; set; }
    }
}
