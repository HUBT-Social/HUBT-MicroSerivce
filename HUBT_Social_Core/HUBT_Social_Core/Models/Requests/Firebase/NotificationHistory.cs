using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Core.Models.Requests.Firebase
{
    public class NotificationHistory
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public List<string> DeliveryChannels { get; set; } = [];
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int Recipients { get; set; }
        public string Status { get; set; } = "Pending";
        public Dictionary<string, NotificationResultDto> Results { get; set; } = [];
    }

    public class NotificationHistoryResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public int Recipients { get; set; }
        public DateTime Time { get; set; }
        public string Status { get; set; } = "Pending";

    }
}
