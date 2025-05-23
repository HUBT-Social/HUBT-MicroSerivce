using HUBT_Social_Core.Models.Requests.Firebase;

namespace HUBT_Social_Firebase.Services;

public interface IFireBaseNotificationService
{
    Task SendNotificationAsync(MessageRequest request);

    Task<bool> SubscribeTopicAsync(string topic, string token);
    Task<bool> SubscribeTopicAsync(string topic, List<string> tokens);
    Task<bool> UnsubscribeTopicAsync(string topic, string token);
    Task<bool> UnsubscribeTopicAsync(string topic, List<string> tokens);
}