using HUBT_Social_Core.Models.Requests.Cloud;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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

        [JsonPropertyName("priority")] public string? Priority { get; set; }  /* Not used yet  */

        [JsonPropertyName("deliveryChannels")] public List<string>? DeliveryChannels { get; set; }  /* Not used yet */

        [JsonPropertyName("scheduleEnabled")] public bool ScheduleEnabled { get; set; } = false;  /*Not used yet  */

        [JsonPropertyName("scheduledTime")] public DateTime? ScheduledTime { get; set; }  /*Not used yet  */

        [JsonPropertyName("imageFile")] public UploadBase64Request? ImageFile { get; set; } = null;  /*Not used yet  */

        [JsonPropertyName("timestamp")] public DateTime? Timestamp { get; set; }  /*Not used yet  */

        [JsonPropertyName("createdBy")] public string? CreatedBy { get; set; }  /*Not used yet  */

        [JsonPropertyName("facultyCodes")] public List<string>? FacultyCodes { get; set; }

        [JsonPropertyName("courseCodes")] public List<string>? CourseCodes { get; set; }

        [JsonPropertyName("classCodes")] public List<string>? ClassCodes { get; set; }

        [JsonPropertyName("userNames")] public List<string>? UserNames { get; set; }

        [JsonPropertyName("sendAll")] public bool SendAll { get; set; } = false;
    }
}
