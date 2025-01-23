using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Core.Models.DTOs.EmailDTO
{
    public class ValidatePostcodeRequest
    {
        public string Postcode { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string UserAgent { get; set; } = string.Empty;
        public string IPAddress { get; set; } = string.Empty;

    }
}
