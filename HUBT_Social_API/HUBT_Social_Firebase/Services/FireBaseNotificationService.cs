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
            Notification = new Notification
            {
                Title = request.Title,
                Body = request.Body,
                ImageUrl = request.ImageUrl 
            },
            Android = new AndroidConfig
            {
                Notification = new AndroidNotification
                {
                    ImageUrl = request.ImageUrl,
                    Title = request.Title,
                    Body = request.Body
                }
            },
            Apns = new ApnsConfig
            {
                Aps = new Aps
                {
                    Alert = new ApsAlert
                    {
                        Title = request.Title,
                        Body = request.Body
                    },
                    Sound = "default",
                    Badge = 1,
                    MutableContent = true
                },
                Headers = new Dictionary<string, string>
                    {
                        { "apns-priority", "10" }
                    },
                FcmOptions = new ApnsFcmOptions
                {
                    ImageUrl = request.ImageUrl
                }
            },
            Data = new Dictionary<string, string?>
                {
                    { "type", request.Type },
                    { "id", request.RequestId }
                }
        };

        if (request is SendGroupMessageRequest request1)
        {
            message.Topic = request1.GroupId;
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

    public async Task<bool> SubscribeTopicAsync(string topic, string token)
    {
        TopicManagementResponse topicManagementResponse = await FirebaseMessaging.DefaultInstance.SubscribeToTopicAsync(
                [token],
                topic
            );

        Console.WriteLine(topicManagementResponse.SuccessCount.ToString());
        
        return topicManagementResponse.SuccessCount > 0;
    }
    public async Task<bool> SubscribeTopicAsync(string topic, List<string> tokens)
    {
        TopicManagementResponse topicManagementResponse = await FirebaseMessaging.DefaultInstance.SubscribeToTopicAsync(
                tokens,
                topic
            );

        Console.WriteLine(topicManagementResponse.SuccessCount.ToString());

        return topicManagementResponse.SuccessCount > 0;
    }
    public async Task<bool> UnsubscribeTopicAsync(string topic, string token)
    {
        TopicManagementResponse topicManagementResponse = await FirebaseMessaging.DefaultInstance.UnsubscribeFromTopicAsync(
                [token],
                topic
            );
        Console.WriteLine(topicManagementResponse.SuccessCount.ToString());
        return topicManagementResponse.SuccessCount > 0;
    }
    public async Task<bool> UnsubscribeTopicAsync(string topic, List<string> tokens)
    {
        TopicManagementResponse topicManagementResponse = await FirebaseMessaging.DefaultInstance.UnsubscribeFromTopicAsync(
                tokens,
                topic
            );
        Console.WriteLine(topicManagementResponse.SuccessCount.ToString());
        return topicManagementResponse.SuccessCount > 0;
    }
}