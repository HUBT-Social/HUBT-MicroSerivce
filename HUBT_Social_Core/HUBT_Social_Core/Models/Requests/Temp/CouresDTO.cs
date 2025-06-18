using HUBT_Social_Core.Models.OutSourceDataDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Core.Models.Requests.Temp
{
    public class CouresDTO
    {
        public string Id { get; set; } = string.Empty;
        public string CourseID { get; set; } = string.Empty;

        public string[] StudentIDs { get; set; } = [];
        public string[] TeacherIDs { get; set; } = [];
        public string RoomId { get; set; } = string.Empty;
        public TimeTableDTO TimeTableDTO { get; set; } = new();
        public bool RoomCreated { set; get; } = false;
    }
}
