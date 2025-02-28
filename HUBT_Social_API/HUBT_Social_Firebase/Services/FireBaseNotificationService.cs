using FirebaseAdmin.Messaging;
using HUBT_Social_Core.Models.Requests.Firebase;
using MongoDB.Bson;

namespace HUBT_Social_Firebase.Services;

public class FireBaseNotificationService : IFireBaseNotificationService
{
    public async Task SendPushNotificationAsync(SendMessageRequest request)
    {
        await SendNotificationAsync(request, "default");
    }

    public async Task SendPushNotificationChatAsync(SendMessageRequest request)
    {
        await SendNotificationAsync(request, "chat");
    }

    public async Task SendPushNotificationInfromationAsync(SendMessageRequest request)
    {
        await SendNotificationAsync(request, "infromation");
    }

    public async Task SendPushNotificationTestAsync(SendMessageRequest request)
    {
        await SendNotificationAsync(request, "infromation");
    }
    public async Task SendPushNotificationWarrningAsync(SendMessageRequest request)
    {
        await SendNotificationAsync(request, "warring");
    }
    private static async Task SendNotificationAsync(SendMessageRequest request, string type)
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
                { "type", type }
            }
        };

        var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
        Console.WriteLine($"Successfully sent message: {message.ToJson()}");
    }
    //private static async Task SendAnnouncement(MessageRequest request, string type)
    //{

    //    var message = new Message
    //    {
    //        Topic = "announcement",
    //        Notification = new Notification
    //        {
    //            Title = request.Title,
    //            Body = request.Body
    //        },
    //        Data = new Dictionary<string, string>
    //        {
    //            { "type", type }
    //        }
    //    };

    //    var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
    //    Console.WriteLine($"Successfully sent message: {message.ToJson()}");
    //}
    //private static async Task SendGroupNotification(SendGroupMessageRequest request, string type)
    //{

    //    var message = new Message
    //    {
    //        Topic = request.GroupId,
    //        Notification = new Notification
    //        {
    //            Title = request.Title,
    //            Body = request.Body
    //        },
    //        Data = new Dictionary<string, string>
    //        {
    //            { "type", type }
    //        }
    //    };

    //    var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
    //    Console.WriteLine($"Successfully sent message: {message.ToJson()}");
    //}


}