using Amazon.Runtime.Internal.Transform;
using FirebaseAdmin.Messaging;
using HUBT_Social_Core.Models.Requests.Firebase;
using MongoDB.Bson;

namespace HUBT_Social_Firebase.Services;

public class FireBaseNotificationService : IFireBaseNotificationService
{
  
    
    public async Task SendNotificationAsync(MessageRequest request)
    {
        Message? message = new()
        {

            Android = new AndroidConfig
            {
                Notification = new AndroidNotification
                {
                    ImageUrl = request.ImageUrl,
                    Title = request.Title,
                    Body = request.Body
                }
            },
            Data = new Dictionary<string, string?>
            {
                { "type", request.Type },
                { "id", request.RequestId}
            }
        }; ;
        if (request is SendGroupMessageRequest)
        {
            message.Topic = "announcement";
        }
        else if (request is SendMessageRequest requestType)
        {
            message.Token = requestType.Token;
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