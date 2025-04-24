using HUBT_Social_Core.Models.OutSourceDataDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Core.Models.Requests.Temp
{
    public class CreateTempCourseRequest
    {
        public string CourseId { get; set; } = string.Empty;
        public string[] StudentIDs { get; set; } = [];
        public TimeTableDTO TimeTableDTO { get; set; } = new();
    }
}
