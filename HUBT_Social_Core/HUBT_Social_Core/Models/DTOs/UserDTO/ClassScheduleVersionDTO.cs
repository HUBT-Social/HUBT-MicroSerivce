using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Core.Models.DTOs.UserDTO
{
    public class ClassScheduleVersionDTO
    {
        public string ClassName { get; set; } = string.Empty;

        public string VersionKey { get; set; } = string.Empty;

        public DateTime LastUpdate { get; set; }
        public DateTime ExpireTime { get; set; }
    }
}
