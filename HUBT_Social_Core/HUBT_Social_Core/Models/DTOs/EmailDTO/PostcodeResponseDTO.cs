using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Core.Models.DTOs.EmailDTO
{
    public class PostcodeRequest
    {
        public string UserAgent { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
    }
    public class CreatePostcodeRequest : PostcodeRequest
    {
        public string Receiver { get; set; } = string.Empty;
    }
}
