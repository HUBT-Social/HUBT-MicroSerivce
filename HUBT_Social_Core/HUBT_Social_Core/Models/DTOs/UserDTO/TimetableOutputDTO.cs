using HUBT_Social_Core.Settings.@enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Core.Models.DTOs.UserDTO
{
    public class TimetableOutputDTO
    {
        public string Id { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public string? ZoomID { get; set; } = string.Empty;
        public TimeTableType Type { get; set; } = TimeTableType.Study;
    }
}
