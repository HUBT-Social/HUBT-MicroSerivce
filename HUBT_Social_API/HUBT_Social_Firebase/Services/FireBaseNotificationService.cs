using FirebaseAdmin.Messaging;
using HUBT_Social_Core.Models.Requests.Firebase;
using MongoDB.Bson;

namespace HUBT_Social_Firebase.Services;

public class FireBaseNotificationService : IFireBaseNotificationService
{
    public async Task SendPushNotificationAsync(SendMessageRequest request)
    {
        if (string.IsNullOrEmpty(request.Token))
        {
            throw new ArgumentException("Token is required.");
        }
        var message = new Message
        {
            Token = request.Token,
            Notification = new Notification
            {
                Title = request.Title,
                Body = request.Body
            },
            Data = new Dictionary<string, string>
            {
                { "type", "chat" }
            }
        };

        var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
        Console.WriteLine($"Successfully sent message: {message.ToJson()}");
    }
}