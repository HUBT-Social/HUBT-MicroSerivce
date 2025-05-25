using HUBT_Social_Core.Models.Requests.Cloud;
using System.ComponentModel.DataAnnotations;

namespace HUBT_Social_Core.Models.Requests.Firebase;

public class SendMessageRequest : MessageRequest
{
    public string Token { get; set; } = string.Empty;
    
}
public class SendGroupMessageRequest : MessageRequest
{
    public string GroupId { get; set; } = string.Empty;
    
}
public class SendNotationToGroupChatRequest : MessageRequest
{
    public List<string> UserNames { get; set; } = [];

}
public class SendByConditionRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;

    public UploadBase64Request? Image { get; set; } = null;

    public string? RequestId { get; set; }

    [Required]
    public string Type { get; set; } = string.Empty;

    public List<string>? FacultyCodes { get; set; }

    public List<string>? CourseCodes { get; set; }

    public List<string>? ClassCodes { get; set; }

    public List<string>? UserNames { get; set; }

    public bool SendAll { get; set; }
}
public class ConditionRequest
{
    public List<string>? UserNames { get; set; }          
    public List<string>? ClassCodes { get; set; }        
    public List<string>? FacultyCodes { get; set; }         
    public List<string>? CourseCodes { get; set; }
    public bool SendAll { get; set; }

}

public class MessageRequest
{
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = "default";
    public string Body { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? RequestId { get; set; }

}