using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Core.Models.Requests.Chat
{
    public class CreateGroupRequest
    {
        [Required]
        public string GroupName { get; set; } = string.Empty;
        [Required]
        public string[] UserNames { get; set; } = [];
    }
}
