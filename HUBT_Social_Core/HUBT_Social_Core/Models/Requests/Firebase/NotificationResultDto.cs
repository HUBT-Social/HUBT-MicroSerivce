using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Core.Models.Requests.Firebase
{
    public class NotificationResultDto
    {
        public string TargetType { get; set; } = string.Empty; // Topic / Token / Tokens
        public int Total { get; set; }
        public int Success { get; set; }
        public int Failure { get; set; }
    }

}
