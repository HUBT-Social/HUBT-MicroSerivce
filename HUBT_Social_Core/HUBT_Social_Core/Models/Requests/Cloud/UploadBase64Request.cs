using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Core.Models.Requests.Cloud
{
    public class UploadBase64Request
    {
        public string Base64String { get; set; } = null!;
        public string FileName { get; set; } = null!;
    }
}
