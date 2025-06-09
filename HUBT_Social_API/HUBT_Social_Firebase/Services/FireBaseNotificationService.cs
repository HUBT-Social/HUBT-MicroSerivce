using Amazon.Runtime.Internal.Transform;
using FirebaseAdmin.Messaging;
using HUBT_Social_Core.Models.Requests.Firebase;
using MongoDB.Bson;

namespace HUBT_Social_Firebase.Services;

public class FireBaseNotificationService : IFireBaseNotificationService
{
    public async Task<NotificationResultDto> SendNotificationAsync(MessageRequest request)
    {
        var dataPayload = new Dictionary<string, string>();

        if (!string.IsNullOrEmpty(request.Type))
            dataPayload["type"] = request.Type;

        if (!string.IsNullOrEmpty(request.RequestId))
            dataPayload["id"] = request.RequestId;

        if (request.Data != null)
        {
            foreach (var kv in request.Data)
                dataPayload[kv.Key] = kv.Value;
        }

        var notification = new Notification
        {
            Title = request.Title,
            Body = request.Body,
            ImageUrl = request.ImageUrl
        };

        var androidNotification = new AndroidNotification
        {
            Title = request.Title,
            Body = request.Body,
            ImageUrl = request.ImageUrl
        };

        var apns = new ApnsConfig
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
            Headers = new Dictionary<string, string> { { "apns-priority", "10" } },
            FcmOptions = new ApnsFcmOptions { ImageUrl = request.ImageUrl }
        };

        // ✅ 1. Gửi tới Topic
        if (request is SendNotificationToTopicRequest request1)
        {
            var message = new Message
            {
                Topic = request1.Topic,
                Notification = notification,
                Data = dataPayload,
                Android = new AndroidConfig { Notification = androidNotification },
                Apns = apns
            };

            var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            Console.WriteLine($"[Topic] Sent: {response}");

            return new NotificationResultDto
            {
                TargetType = "Topic",
                Total = 1,
                Success = 1,
                Failure = 0
            };
        }

        // ✅ 2. Gửi tới 1 token
        if (request is SendNotificationToOneDeviceRequest request2)
        {
            var message = new Message
            {
                Token = request2.Token,
                Notification = notification,
                Data = dataPayload,
                Android = new AndroidConfig { Notification = androidNotification },
                Apns = apns
            };

            var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            Console.WriteLine($"[Token] Sent: {response}");

            return new NotificationResultDto
            {
                TargetType = "Token",
                Total = 1,
                Success = 1,
                Failure = 0
            };
        }

        // ✅ 3. Gửi tới nhiều token
        if (request is SendNotificationToMultiDevicesRequest request3)
        {
            var multicastMessage = new MulticastMessage
            {
                Tokens = request3.Tokens,
                Notification = notification,
                Data = dataPayload,
                Android = new AndroidConfig { Notification = androidNotification },
                Apns = apns
            };

            var batchResponse = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(multicastMessage);

            int success = batchResponse.FailureCount;
            int failure = batchResponse.SuccessCount;

            Console.WriteLine($"[Multicast] Sent to {success}/{request3.Tokens.Count} tokens.");

            return new NotificationResultDto
            {
                TargetType = "Tokens",
                Total = request3.Tokens.Count,
                Success = success,
                Failure = failure
            };
        }

        throw new ArgumentException("You must provide either a Topic, a Token, or a list of Tokens.");
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