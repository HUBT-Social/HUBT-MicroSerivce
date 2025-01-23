using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Core.Models.DTOs.IdentityDTO
{
    public class TokenDTO
    {
        public string UserId { get; set; } = string.Empty;

        public string RefreshTo { get; set; } = string.Empty;

        public string AccessToken { get; set; } = string.Empty;

        public DateTime ExpireTime { get; set; }
    }
}
