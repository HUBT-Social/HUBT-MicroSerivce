using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Core.Models.DTOs.IdentityDTO
{
    public class AcademicStaffDTO
    {
        public string userId {  get; set; } = string.Empty;
        public List<string> roles { get; set; } 
        public List<string> subjects { get; set; } 
    }
}
