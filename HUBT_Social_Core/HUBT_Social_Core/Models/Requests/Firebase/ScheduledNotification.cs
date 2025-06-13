using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Core.Models.Requests.Firebase
{
    public class ScheduledNotification
    {
        public string Id { get; set; } = string.Empty;
        public string JobId { get; set; } = string.Empty;
        public string? HangfireJobId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public List<string> DeliveryChannels { get; set; } = new();
        public DateTime ScheduledTime { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<string>? FacultyCodes { get; set; }
        public List<string>? CourseCodes { get; set; }
        public List<string>? ClassCodes { get; set; }
        public List<string>? UserNames { get; set; }
        public bool SendAll { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? ErrorMessage { get; set; }
        public int RetryCount { get; set; } = 0;
    }
}
