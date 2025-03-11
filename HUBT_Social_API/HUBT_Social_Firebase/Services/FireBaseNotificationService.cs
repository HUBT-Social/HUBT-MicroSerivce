using Amazon.Runtime.Internal.Transform;
using FirebaseAdmin.Messaging;
using HUBT_Social_Core.Models.Requests.Firebase;
using MongoDB.Bson;

namespace HUBT_Social_Firebase.Services;

public class FireBaseNotificationService : IFireBaseNotificationService
{
  
    
    public async Task SendNotificationAsync(MessageRequest request)
    {
        Message? message = null;
        if (request is SendGroupMessageRequest requestType)
        {
            message = new Message
            {
                Topic = "announcement",
                Notification = new Notification
                {
                    Title = requestType.Title,
                    Body = requestType.Body,
                    ImageUrl = requestType.ImageUrl
                },
                Data = new Dictionary<string, string?>
            {
                { "type", requestType.Type },
                { "id", requestType.RequestId}
            }
            };
        }
        else if (request is SendMessageRequest requestType1)
        {
            message = new Message
            {
                Token = requestType1.Token,
                Notification = new Notification
                {
                    Title = requestType1.Title,
                    Body = requestType1.Body,
                    ImageUrl = requestType1.ImageUrl
                },
                Data = new Dictionary<string, string?>
            {
                { "type", requestType1.Type },
                { "id", requestType1.RequestId }
            }
            };
        }


        if (message != null)
        {
            var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            Console.WriteLine($"Successfully sent message: {message.ToJson()}");
        }
        else
        {
            Console.WriteLine("Failed to send message: Invalid request type.");
        }
    }

}