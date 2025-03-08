using HUBT_Social_Core.Models.Requests.Firebase;

namespace HUBT_Social_Firebase.Services;

public interface IFireBaseNotificationService
{
    Task SendNotificationAsync(MessageRequest request);

}