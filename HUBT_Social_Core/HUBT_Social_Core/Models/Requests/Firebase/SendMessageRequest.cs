using HUBT_Social_Core.Models.Requests.Cloud;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HUBT_Social_Core.Models.Requests.Firebase;

public class SendNotificationToOneDeviceRequest : MessageRequest
{
    [Required]
    public string Token { get; set; } = string.Empty;
    
}

public class SendNotificationToMultiDevicesRequest : MessageRequest
{
    [Required]
    public List<string> Tokens { get; set; } = [];
}

public class SendNotificationToTopicRequest : MessageRequest
{
    [Required]
    public string Topic { get; set; } = string.Empty;
}

public class SendNotificationToMultiUserNamesRequest : MessageRequest
{
    [Required]
    public List<string> UserNames { get; set; } = [];

}

public class SendNotificationToOneUserNameRequest : MessageRequest
{
    [Required]
    public string UserName { get; set; } = string.Empty;
}

public class SendNotificationGeneralRequest : MessageRequest
{
    public string Token { get; set; } = string.Empty;
    public List<string> Tokens { get; set; } = [];
    public string Topic { get; set; } = string.Empty;

}

public class MessageRequest
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? Type { get; set; }
    public string? RequestId { get; set; }
    public Dictionary<string, string>? Data { get; set; }
}


public class ConditionRequest : RecipientFilterRequest
{
    public List<string>? UserNames { get; set; }
    public List<string>? ClassCodes { get; set; }
    public List<string>? FacultyCodes { get; set; }
    public List<string>? CourseCodes { get; set; }
    public bool SendAll { get; set; }

}

