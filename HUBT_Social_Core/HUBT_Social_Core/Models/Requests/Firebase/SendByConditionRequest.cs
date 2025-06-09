using HUBT_Social_Core.Models.Requests.Cloud;
using HUBT_Social_Core.Models.Requests.Firebase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HUBT_Social_Core.Models.Requests.Firebase
{
    public class SendByConditionRequest
    {
        [Required]
        [JsonPropertyName("title")] public string Title { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("body")] public string Body { get; set; } = string.Empty;

        [JsonPropertyName("requestId")] public string? RequestId { get; set; }

        [Required]
        [JsonPropertyName("type")] public string Type { get; set; } = string.Empty;

        [JsonPropertyName("priority")] public string? Priority { get; set; }  

        [JsonPropertyName("deliveryChannels")] public List<string>? DeliveryChannels { get; set; }  

        [JsonPropertyName("scheduleEnabled")] public bool ScheduleEnabled { get; set; } = false; 

        [JsonPropertyName("scheduledTime")] public DateTime? ScheduledTime { get; set; }  

        [JsonPropertyName("imageFile")] public UploadBase64Request? ImageFile { get; set; } = null;  

        [JsonPropertyName("timestamp")] public DateTime? Timestamp { get; set; }  

        [JsonPropertyName("createdBy")] public string? CreatedBy { get; set; }  

        [JsonPropertyName("facultyCodes")] public List<string>? FacultyCodes { get; set; }

        [JsonPropertyName("courseCodes")] public List<string>? CourseCodes { get; set; }

        [JsonPropertyName("classCodes")] public List<string>? ClassCodes { get; set; }

        [JsonPropertyName("userNames")] public List<string>? UserNames { get; set; }

        [JsonPropertyName("sendAll")] public bool SendAll { get; set; } = false;
    }

    public class SendByAcademic
    {
        [JsonPropertyName("type")] public string Type { get; set; } = string.Empty;
        [JsonPropertyName("body")] public string Body { get; set; } = string.Empty;
        [JsonPropertyName("priority")] public string Priority { get; set; } = string.Empty;
        [JsonPropertyName("recipients")] public List<string> Recipients { get; set; } = [];
        [JsonPropertyName("createdBy")] public string? CreatedBy { get; set; }
        [JsonPropertyName("sendAll")] public bool SendAll { get; set; }
        [JsonPropertyName("timestamp")] public DateTime Timestamp { get; set; }
        [JsonPropertyName("channels")] public List<string> Channels { get; set; } = [];
    }
}
