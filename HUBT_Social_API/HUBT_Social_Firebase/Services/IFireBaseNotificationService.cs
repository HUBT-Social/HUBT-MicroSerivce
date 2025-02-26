using HUBT_Social_Core.Models.Requests.Firebase;

namespace HUBT_Social_Firebase.Services;

public interface IFireBaseNotificationService
{
    Task SendPushNotificationAsync(SendMessageRequest request);
    Task SendPushNotificationWarrningAsync(SendMessageRequest request);
    Task SendPushNotificationInfromationAsync(SendMessageRequest request);
    Task SendPushNotificationChatAsync(SendMessageRequest request);
    Task SendPushNotificationTestAsync(SendMessageRequest request);
}