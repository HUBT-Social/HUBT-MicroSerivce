using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Core.Models.Requests
{
    public class CreateGroupByCourse
    {
        public string ClassName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string? Teacher {  get; set; } = string.Empty;
        public ListUserNameDTO listUserNames { get; set; } = new ListUserNameDTO();
    }
}
