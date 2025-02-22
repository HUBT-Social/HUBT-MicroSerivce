namespace HUBT_Social_Core.Models.Requests.Firebase;

public class SendMessageRequest
{
    public string Token { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}