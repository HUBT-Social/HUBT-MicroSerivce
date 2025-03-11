namespace HUBT_Social_Core.Models.Requests.Firebase;

public class SendMessageRequest : MessageRequest
{
    public string Token { get; set; } = string.Empty;
    public string? RequestId { get; set; }
}
public class SendGroupMessageRequest : MessageRequest
{
    public string GroupId { get; set; } = string.Empty;
    public string? RequestId { get; set; }
}
public class MessageRequest
{
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = "default";
    public string Body { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}