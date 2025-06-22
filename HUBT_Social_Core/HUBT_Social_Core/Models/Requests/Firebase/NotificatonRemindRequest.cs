using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Core.Models.Requests.Firebase
{
    public class NotificatonRemindRequest
    {
        public string UserName {  get; set; } = string.Empty;
        public string RemindCode { get; set; } = string.Empty;
        public string Content {  get; set; } = string.Empty;
    }
}
