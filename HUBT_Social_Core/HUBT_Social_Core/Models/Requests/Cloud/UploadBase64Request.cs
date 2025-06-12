using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HUBT_Social_Core.Models.Requests.Cloud
{
    public class UploadBase64Request
    {
        [JsonPropertyName("fileName")]  public string FileName { get; set; } = null!;
        [JsonPropertyName("fileData")]  public string FileData { get; set; } = null!;
    }
}
