using HUBT_Social_Core.Models.Requests.Cloud;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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

public class ConditionRequest : RecipientFilterRequest
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

