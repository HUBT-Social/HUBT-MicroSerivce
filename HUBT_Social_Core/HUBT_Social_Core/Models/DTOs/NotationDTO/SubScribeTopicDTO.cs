using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Core.Models.DTOs.NotationDTO
{
    public class SubScribeTopicDTO
    {
        public string Topic { get; set; } = string.Empty;
        public List<string> Tokens { get; set; } = [];
    }
}
