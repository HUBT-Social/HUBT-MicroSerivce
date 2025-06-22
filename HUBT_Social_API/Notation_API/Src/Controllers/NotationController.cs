using FirebaseAdmin.Messaging;
using Hangfire;
using HUBT_Social_Base.Service;
using HUBT_Social_Core.Decode;
using HUBT_Social_Core.Models.DTOs.EmailDTO;
using HUBT_Social_Core.Models.DTOs.NotationDTO;
using HUBT_Social_Core.Models.Requests.Firebase;
using HUBT_Social_Core.Settings;
using HUBT_Social_Email_Service.Services;
using HUBT_Social_Firebase.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Notation_API.Src.Repository;
using Notation_API.Src.Services;
using Org.BouncyCastle.Asn1.Ocsp;
using System.ComponentModel.DataAnnotations;

namespace Notation_API.Src.Controllers
{
    [Route("api/notification")]
    [ApiController]
    [Authorize]
    public class NotationController : ControllerBase
    {
        private readonly IFireBaseNotificationService _fireBaseNotificationService;
        private readonly IUserService _userService;
        private readonly IHttpCloudService _httpCloudService;
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<NotationController> _logger;
        private readonly JwtSetting _jwtSettings;
        private readonly IEmailNotification _emailNotification;

        // Constants for validation and configuration
        private static readonly string[] ValidChannels = { "push", "sms", "email", "inapp" };
        private static readonly string[] ValidPriorities = { "low", "medium", "high", "urgent" };
        private const int MaxBatchSize = 1000;
        private const int DefaultPageSize = 10;
        private const int MaxPageSize = 100;

        public NotationController(
            IFireBaseNotificationService fireBaseNotificationService,
            IUserService userService,
            IHttpCloudService httpCloudService,
            INotificationRepository notificationRepository,
            ILogger<NotationController> logger,
            IOptions<JwtSetting> jwtSettings,
            IEmailNotification emailNotification)

        {
            _fireBaseNotificationService = fireBaseNotificationService;
            _userService = userService;
            _httpCloudService = httpCloudService;
            _notificationRepository = notificationRepository;
            _logger = logger;
            _jwtSettings = jwtSettings.Value;
            _emailNotification = emailNotification;
        }

        [HttpPost("send-notification")]
        public async Task<IActionResult> SendNotification([FromBody] SendNotificationGeneralRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            List<MessageRequest> messages = new List<MessageRequest>();
            if (request.Topic is not null)
            {
                SendNotificationToTopicRequest sendNotificationToTopicRequest = new()
                {
                    Topic = request.Topic,
                    Body = request.Body,
                    Data = request.Data,
                    ImageUrl = request.ImageUrl,
                    RequestId = request.RequestId,
                    Title = request.Title
                };
                sendNotificationToTopicRequest.Topic = request.Topic;
                messages.Add(sendNotificationToTopicRequest);
            }

            if (request.Token is not null)
            {
                SendNotificationToOneDeviceRequest sendNotificationToTopicRequest = new()
                {
                    Token = request.Token,
                    Body = request.Body,
                    Data = request.Data,
                    ImageUrl = request.ImageUrl,
                    RequestId = request.RequestId,
                    Title = request.Title
                };
                sendNotificationToTopicRequest.Token = request.Token;
                messages.Add(sendNotificationToTopicRequest);
            }

            if (request.Tokens is not null)
            {
                SendNotificationToMultiDevicesRequest sendNotificationToTopicRequest = new()
                {
                    Tokens = request.Tokens,
                    Body = request.Body,
                    Data = request.Data,
                    ImageUrl = request.ImageUrl,
                    RequestId = request.RequestId,
                    Title = request.Title
                };
                sendNotificationToTopicRequest.Tokens = request.Tokens;
                messages.Add(sendNotificationToTopicRequest);
            }

            try
            {
                if (messages.Count > 0)
                {
                    foreach (var notification in messages)
                    {
                        await _fireBaseNotificationService.SendNotificationAsync(request);
                    }
                }
                _logger.LogInformation("Successfully sent notification");
                return Ok(new { message = LocalValue.Get(KeyStore.NotificationSend), timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification");
                return BadRequest(new { error = LocalValue.Get(KeyStore.NotificationSendError) });
            }
        }

        //[HttpPost("send-to-one")]
        //public async Task<IActionResult> SendNotificationToOne([FromBody] SendMessageRequest request)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    try
        //    {
        //        await _fireBaseNotificationService.SendNotificationAsync(request);
        //        _logger.LogInformation("Successfully sent notification to single user");
        //        return Ok(new { message = LocalValue.Get(KeyStore.NotificationSend), timestamp = DateTime.UtcNow });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Failed to send notification to single user with token: {Token}", request.Token);
        //        return BadRequest(new { error = LocalValue.Get(KeyStore.NotificationSendError) });
        //    }
        //}

        //[HttpPost("send-to-many")]
        //public async Task<IActionResult> SendNotificationToMany([FromBody] SendGroupMessageRequest request)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    try
        //    {
        //        await _fireBaseNotificationService.SendNotificationAsync(request);
        //        _logger.LogInformation($"Successfully sent notification to {request.GroupId}");
        //        return Ok(new { message = LocalValue.Get(KeyStore.NotificationSend), timestamp = DateTime.UtcNow });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Failed to send notification to multiple users");
        //        return BadRequest(new { error = LocalValue.Get(KeyStore.NotificationSendError) });
        //    }
        //}
        [HttpPost("send-to-one-username")]
        public async Task<IActionResult> SendNotificationToGroupChat([FromBody] SendNotificationToOneUserNameRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request.UserName == null)
            {
                return BadRequest(new { error = "UserName cannot be empty" });
            }

            try
            {
                var Token = await _userService.GetFCMFromUserName(request.UserName);
                if (Token == null)
                {
                    return BadRequest(new { error = "No valid FCM tokens found for provided username" });
                }

                var sendRequest = new SendNotificationToOneDeviceRequest
                {
                    Body = request.Body,
                    ImageUrl = request.ImageUrl,
                    RequestId = request.RequestId,
                    Title = request.Title,
                    Type = request.Type,
                    Data = request.Data,
                    Token = Token
                };

                try
                {
                    var response = await _fireBaseNotificationService.SendNotificationAsync(sendRequest);
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send notification to FCM token:");
                    return StatusCode(500, "Failed to send notification");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification to usernam");
                return BadRequest(new { error = LocalValue.Get(KeyStore.NotificationSendError) });
            }
        }
        [HttpPost("send-remind-to-multi-username")]
        [AllowAnonymous]
        public async Task<IActionResult> SendRemindNotification([FromBody] List<NotificatonRemindRequest> request)
        {

            if(request == null || request.Count == 0)
            {
                return BadRequest();
            }
            foreach(var item in request)
            {
                Console.WriteLine(item.UserName,": ", item.RemindCode," ",item.Content);
            }
            try
            {
                foreach (var item in request) 
                {
                    var Token = await _userService.GetFCMFromUserName(item.UserName);
                    if (Token == null)
                    {
                        continue;
                    }
                    Console.WriteLine($"Token:{Token}");

                    var sendRequest = new SendNotificationToOneDeviceRequest
                    {
                        Body = item.Content,
                        Title = "Thông báo nhắc nhở học tập.",
                        Type = item.RemindCode,
                        Token = Token
                    };

                    try
                    {
                        var response = await _fireBaseNotificationService.SendNotificationAsync(sendRequest);
                        return Ok(response);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to send notification to FCM token:");
                        continue;
                    }
                
                }
                return Ok("Sent");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification to usernam");
                return BadRequest(new { error = LocalValue.Get(KeyStore.NotificationSendError) });
            }
        }

        [HttpPost("send-to-multi-username")]
        public async Task<IActionResult> SendNotificationToGroupChat([FromBody] SendNotificationToMultiUserNamesRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request.UserNames?.Count == 0)
            {
                return BadRequest(new { error = "UserNames list cannot be empty" });
            }

            try
            {
                var fcmTokens = await _userService.GetListFMCFromListUserName(request.UserNames);
                if (fcmTokens == null || !fcmTokens.Any())
                {
                    return BadRequest(new { error = "No valid FCM tokens found for provided usernames" });
                }

                var sendRequest = new SendNotificationToMultiDevicesRequest
                {
                    Body = request.Body,
                    ImageUrl = request.ImageUrl,
                    RequestId = request.RequestId,
                    Title = request.Title,
                    Type = request.Type,
                    Data = request.Data,
                    Tokens = fcmTokens
                };

                try
                {
                    var response = await _fireBaseNotificationService.SendNotificationAsync(sendRequest);
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send notification to FCM token:");
                    return StatusCode(500, "Failed to send notification");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification to multi username");
                return BadRequest(new { error = LocalValue.Get(KeyStore.NotificationSendError) });
            }
        }

        [HttpPost("topic-subscribe")]
        public async Task<IActionResult> SubscribeTopic([FromBody] SubScribeTopicDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(request.Topic))
                return BadRequest(new { error = "Topic cannot be empty" });

            if (request.Tokens?.Count == 0)
                return BadRequest(new { error = "Tokens list cannot be empty" });

            try
            {
                var result = await _fireBaseNotificationService.SubscribeTopicAsync(request.Topic, request.Tokens);

                if (result)
                {
                    _logger.LogInformation("Successfully subscribed {Count} tokens to topic: {Topic}",
                        request.Tokens.Count, request.Topic);
                }

                return Ok(new { success = result, topic = request.Topic, tokenCount = request.Tokens.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to subscribe to topic: {Topic}", request.Topic);
                return BadRequest(new { error = LocalValue.Get(KeyStore.NotificationSendError) });
            }
        }

        [HttpPost("topic-unsubscribe")]
        public async Task<IActionResult> UnsubscribeTopic([FromBody] SubScribeTopicDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(request.Topic))
                return BadRequest(new { error = "Topic cannot be empty" });

            if (request.Tokens?.Count == 0)
                return BadRequest(new { error = "Tokens list cannot be empty" });

            try
            {
                var result = await _fireBaseNotificationService.UnsubscribeTopicAsync(request.Topic, request.Tokens);

                if (result)
                {
                    _logger.LogInformation("Successfully unsubscribed {Count} tokens from topic: {Topic}",
                        request.Tokens.Count, request.Topic);
                }

                return Ok(new { success = result, topic = request.Topic, tokenCount = request.Tokens.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to unsubscribe from topic: {Topic}", request.Topic);
                return BadRequest(new { error = LocalValue.Get(KeyStore.NotificationSendError) });
            }
        }

        [HttpPost("send-by-condition")]
        public async Task<IActionResult> SendNotificationByCondition([FromBody] SendByConditionRequest request)
        {
            // Comprehensive validation
            var validationResult = ValidateSendByConditionRequest(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { errors = validationResult.Errors });
            }

            try
            {
                // Normalize and set defaults
                NormalizeRequest(request);

                // Handle scheduling vs immediate sending
                if (request.ScheduleEnabled && request.ScheduledTime.HasValue)
                {
                    return await HandleScheduledNotification(request);
                }

                return await SendImmediateNotification(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendNotificationByCondition");
                return StatusCode(500, new { error = "Internal server error occurred" });
            }
        }
        [HttpPost("send-by-academic")]
        public async Task<IActionResult> SendNotificationByAcademic([FromBody] SendByAcademic request)
        {
            // Validate the request
            var validationResult = ValidateSendByAcademicRequest(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { errors = validationResult.Errors });
            }

            try
            {
                // Normalize request
                NormalizeAcademicRequest(request);

                // Get recipients based on user IDs
                var condition = new ConditionRequest
                {
                    UserNames = request.SendAll ? [] : request.Recipients,
                    SendAll = request.SendAll,
                    IncludeFcmTokens = request.Channels.Contains("push"),
                    IncludePhoneNumbers = request.Channels.Contains("sms"),
                    IncludeEmails = request.Channels.Contains("email")

                };

                var recipients = await GetRecipientsByChannels(condition);

                if (request.SendAll && !recipients.HasAnyRecipients())
                {
                    return BadRequest(new { error = "No recipients found for the specified users and delivery channels" });
                }

                // Create message request
                var sendRequest = new MessageRequest
                {
                    Body = request.Body,
                    Title = request.Type, // Using Type as Title for consistency
                    Type = request.Type,
                };

                // Send notifications through multiple channels
                var results = await SendToMultipleChannels(sendRequest, recipients, request.Channels);

                // Log notification history
                string? notificationId = await LogAcademicNotificationHistory(request, recipients, results);

                _logger.LogInformation("Academic notification sent successfully to {Count} recipients",
                 recipients.GetTotalRecipientCount());

                var notificationResponse = new NotificationHistoryResponse
                {
                    Id = notificationId,
                    Title = request.Type,
                    Body = request.Body,
                    Type = request.Type,
                    CreatedBy = request.CreatedBy,
                    Priority = request.Priority,
                    Recipients = results["push"].Success,
                    Time = DateTime.UtcNow,
                    Status = "sent"
                };

                return Ok(notificationResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending academic notification");
                return StatusCode(500, new { error = "Error sending academic notification" });
            }
        }

        private static ValidationResult ValidateSendByAcademicRequest(SendByAcademic request)
        {
            var errors = new List<string>();

            if (request == null)
            {
                errors.Add("Request cannot be null");
                return new ValidationResult { IsValid = false, Errors = errors };
            }

            // Basic validation
            if (string.IsNullOrWhiteSpace(request.Body))
                errors.Add("Body is required");

            if (string.IsNullOrWhiteSpace(request.Type))
                errors.Add("Type is required");

            if (request.Recipients?.Any() != true && request.SendAll == false)
                errors.Add("At least one recipient is required");

            // Channels validation
            if (request.Channels?.Any() == true)
            {
                var invalidChannels = request.Channels
                    .Where(c => !ValidChannels.Contains(c, StringComparer.OrdinalIgnoreCase))
                    .ToList();

                if (invalidChannels.Any())
                {
                    errors.Add($"Invalid delivery channels: {string.Join(", ", invalidChannels)}. Valid channels: {string.Join(", ", ValidChannels)}");
                }
            }

            // Priority validation
            if (!string.IsNullOrEmpty(request.Priority) &&
                !ValidPriorities.Contains(request.Priority, StringComparer.OrdinalIgnoreCase))
            {
                errors.Add($"Invalid priority. Valid priorities: {string.Join(", ", ValidPriorities)}");
            }

            return new ValidationResult { IsValid = errors.Count == 0, Errors = errors };
        }

        private void NormalizeAcademicRequest(SendByAcademic request)
        {
            // Set timestamp if not provided
            if (request.Timestamp == default)
            {
                request.Timestamp = DateTime.UtcNow;
            }

            // Default delivery channels
            if (request.Channels?.Any() != true)
            {
                request.Channels = ["push"];
            }
            if (string.IsNullOrEmpty(request.CreatedBy))
            {
                request.CreatedBy = Request.ExtractTokenInfo(_jwtSettings)?.Username ?? "system";
            }

            // Normalize channel names to lowercase
            request.Channels = request.Channels
                .Select(c => c.ToLower())
                .Distinct()
                .ToList();

            // Default priority
            if (string.IsNullOrEmpty(request.Priority))
            {
                request.Priority = "medium";
            }
        }

        private async Task<string?> LogAcademicNotificationHistory(SendByAcademic request, NotificationRecipients recipients, Dictionary<string, NotificationResultDto> results)
        {
            try
            {
                var history = new NotificationHistory
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = request.Type,
                    Body = request.Body,
                    Type = request.Type,
                    Priority = request.Priority,
                    DeliveryChannels = request.Channels,
                    CreatedBy = request.CreatedBy,
                    CreatedAt = request.Timestamp,
                    Recipients = recipients.Count,
                    Results = results
                };

                await _notificationRepository.SaveHistoryAsync(history);
                _logger.LogDebug("Academic notification history logged successfully");

                return history.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging academic notification history");
                return null;
            }
        }
        private ValidationResult ValidateSendByConditionRequest(SendByConditionRequest request)
        {
            var errors = new List<string>();

            if (request == null)
            {
                errors.Add("Request cannot be null");
                return new ValidationResult { IsValid = false, Errors = errors };
            }

            // Basic validation
            if (string.IsNullOrWhiteSpace(request.Title))
                errors.Add("Title is required");

            if (string.IsNullOrWhiteSpace(request.Body))
                errors.Add("Body is required");

            // Scheduling validation
            if (request.ScheduleEnabled)
            {
                if (!request.ScheduledTime.HasValue)
                    errors.Add("ScheduledTime is required when ScheduleEnabled is true");
                else if (request.ScheduledTime.Value <= DateTime.UtcNow)
                    errors.Add("ScheduledTime must be in the future");
            }

            // Conditions validation
            if (!request.SendAll &&
                !HasValidConditions(request.UserNames, request.ClassCodes, request.FacultyCodes, request.CourseCodes))
            {
                errors.Add("At least one condition is required when SendAll is false");
            }

            // Delivery channels validation
            if (request.DeliveryChannels?.Any() == true)
            {
                var invalidChannels = request.DeliveryChannels
                    .Where(c => !ValidChannels.Contains(c, StringComparer.OrdinalIgnoreCase))
                    .ToList();

                if (invalidChannels.Count != 0)
                {
                    errors.Add($"Invalid delivery channels: {string.Join(", ", invalidChannels)}. Valid channels: {string.Join(", ", ValidChannels)}");
                }
            }

            // Priority validation
            if (!string.IsNullOrEmpty(request.Priority) &&
                !ValidPriorities.Contains(request.Priority, StringComparer.OrdinalIgnoreCase))
            {
                errors.Add($"Invalid priority. Valid priorities: {string.Join(", ", ValidPriorities)}");
            }

            return new ValidationResult { IsValid = errors.Count == 0, Errors = errors };
        }

        private static bool HasValidConditions(params IEnumerable<string>?[] conditions)
        {
            return conditions.Any(condition => condition?.Any() == true);
        }

        private void NormalizeRequest(SendByConditionRequest request)
        {
            // Set timestamp if not provided
            request.Timestamp ??= DateTime.UtcNow;

            // Set createdBy if not provided
            if (string.IsNullOrEmpty(request.CreatedBy))
            {
                request.CreatedBy = Request.ExtractTokenInfo(_jwtSettings)?.Username ?? "system";
            }

            // Default delivery channels
            if (request.DeliveryChannels?.Any() != true)
            {
                request.DeliveryChannels = ["push"];
            }

            // Normalize channel names to lowercase
            request.DeliveryChannels = request.DeliveryChannels
                .Select(c => c.ToLower())
                .Distinct()
                .ToList();

            // Default priority
            if (string.IsNullOrEmpty(request.Priority))
            {
                request.Priority = "medium";
            }
        }

        private async Task<IActionResult> HandleScheduledNotification(SendByConditionRequest request)
        {
            try
            {
                var scheduledNotification = CreateScheduledNotification(request);
                await _notificationRepository.SaveScheduledNotificationAsync(scheduledNotification);

                // Schedule job with better timezone handling
                var scheduledTimeOffset = new DateTimeOffset(request.ScheduledTime.Value, TimeSpan.Zero);
                var hangfireJobId = BackgroundJob.Schedule(
                    () => ProcessScheduledNotification(scheduledNotification.Id),
                    scheduledTimeOffset
                );

                scheduledNotification.HangfireJobId = hangfireJobId;
                await _notificationRepository.UpdateScheduledNotificationAsync(scheduledNotification);

                _logger.LogInformation("Notification scheduled successfully with ID: {Id}", scheduledNotification.Id);

                var history = new NotificationHistory
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = request.Title,
                    Body = request.Body,
                    Type = request.Type,
                    Priority = request.Priority,
                    DeliveryChannels = request.DeliveryChannels,
                    CreatedBy = request.CreatedBy,
                    CreatedAt = request.Timestamp.Value,
                    Status = "Scheduled"
                };

                await _notificationRepository.SaveHistoryAsync(history);
                _logger.LogDebug("Notification history logged successfully");

                var notificationResponse = new NotificationHistoryResponse
                {
                    Id = history.Id,
                    Title = request.Title,
                    Body = request.Body,
                    Type = request.Type,
                    Priority = request.Priority,
                    CreatedBy = request.CreatedBy,
                    Recipients = 0,
                    Time = DateTime.UtcNow,
                    Status = "Scheduled"
                };

                return Ok(notificationResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scheduling notification");
                return StatusCode(500, new { error = "Error scheduling notification" });
            }
        }

        private static ScheduledNotification CreateScheduledNotification(SendByConditionRequest request)
        {
            return new ScheduledNotification
            {
                Id = Guid.NewGuid().ToString(),
                JobId = $"notification_{Guid.NewGuid()}",
                Title = request.Title,
                Body = request.Body,
                Type = request.Type,
                Priority = request.Priority,
                DeliveryChannels = request.DeliveryChannels,
                ScheduledTime = request.ScheduledTime.Value,
                CreatedBy = request.CreatedBy,
                CreatedAt = request.Timestamp.Value,
                Status = "Scheduled",
                FacultyCodes = request.FacultyCodes,
                CourseCodes = request.CourseCodes,
                ClassCodes = request.ClassCodes,
                UserNames = request.UserNames,
                SendAll = request.SendAll
            };
        }

        private async Task<IActionResult> SendImmediateNotification(SendByConditionRequest request)
        {
            try
            {
                var condition = new ConditionRequest
                {
                    ClassCodes = request.ClassCodes,
                    CourseCodes = request.CourseCodes,
                    FacultyCodes = request.FacultyCodes,
                    UserNames = request.UserNames,
                    SendAll = request.SendAll,
                    IncludeFcmTokens = request.DeliveryChannels.Contains("push"),
                    IncludePhoneNumbers = request.DeliveryChannels.Contains("sms"),
                    IncludeEmails = request.DeliveryChannels.Contains("email")
                };

                var recipients = await GetRecipientsByChannels(condition);

                if (!recipients.HasAnyRecipients())
                {
                    return BadRequest(new { error = "No recipients found for the specified conditions and delivery channels" });
                }

                var sendRequest = await CreateSendMessageRequest(request);
                var results = await SendToMultipleChannels(sendRequest, recipients, request.DeliveryChannels);

                string? notificationId = await LogNotificationHistory(request, recipients, results);

                _logger.LogInformation("Immediate notification sent successfully to {Count} recipients",
                    recipients.GetTotalRecipientCount());

                var notificationResponse = new NotificationHistoryResponse
                {
                    Id = notificationId,
                    Title = request.Title,
                    Body = request.Body,
                    Type = request.Type,
                    Priority = request.Priority,
                    CreatedBy = request.CreatedBy,
                    Recipients = results["push"].Success,
                    Time = DateTime.UtcNow,
                    Status = "sent"
                };

                return Ok(notificationResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending immediate notification");
                return StatusCode(500, new { error = "Error sending notification" });
            }
        }

        private async Task<MessageRequest> CreateSendMessageRequest(SendByConditionRequest request)
        {
            var sendRequest = new MessageRequest
            {
                Body = request.Body,
                RequestId = request.RequestId,
                Title = request.Title,
                Type = request.Type,
            };

            // Handle image upload with better error handling
            if (request.ImageFile != null && !string.IsNullOrEmpty(request.ImageFile.FileData))
            {
                try
                {
                    var uploadResponse = await _httpCloudService.GetUrlFormBase6(request.ImageFile);
                    if (!string.IsNullOrEmpty(uploadResponse))
                    {
                        sendRequest.ImageUrl = uploadResponse;
                        _logger.LogDebug("Image uploaded successfully for notification");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to upload image for notification, continuing without image");
                }
            }

            return sendRequest;
        }

        private async Task<Dictionary<string, NotificationResultDto>> SendToMultipleChannels(
            MessageRequest request,
            NotificationRecipients recipients,
            List<string> channels)
        {
            var results = new Dictionary<string, NotificationResultDto>();
            var tasks = new List<Task>();

            foreach (var channel in channels)
            {
                var task = channel.ToLower() switch
                {
                    "push" => SendPushNotifications(request, recipients.FcmTokens)
                        .ContinueWith(t => results["push"] = t.Result),
                    "sms" => SendSmsNotifications(request, recipients.PhoneNumbers)
                        .ContinueWith(t => results["sms"] = t.Result),
                    "email" => SendEmailNotifications(request, recipients.Emails)
                        .ContinueWith(t => results["email"] = t.Result),
                    _ => Task.CompletedTask
                };
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
            return results;
        }

        private async Task<NotificationRecipients> GetRecipientsByChannels(ConditionRequest condition)
        {
            var recipients = await _userService.GetNotificationRecipientsFromCondition(condition);
            return recipients;
        }

        private async Task<NotificationResultDto> SendPushNotifications(MessageRequest request, List<string> fcmTokens)
        {
            if (!fcmTokens?.Any() == true)
            {
                return new NotificationResultDto();
            }

            try
            {
                SendNotificationToMultiDevicesRequest sendNotificationToMultiDevicesRequest = new()
                {
                    Body = request.Body,
                    Data = request.Data,
                    ImageUrl = request.ImageUrl,
                    RequestId = request.RequestId,
                    Title = request.Title,
                    Tokens = fcmTokens,
                    Type = request.Type,
                };

                var response = await _fireBaseNotificationService.SendNotificationAsync(sendNotificationToMultiDevicesRequest);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send push notification to token");
            }
            return new NotificationResultDto();
        }

        private async Task<NotificationResultDto> SendSmsNotifications(MessageRequest request, List<string> phoneNumbers)
        {
            // TODO: Implement SMS service with proper provider integration
            _logger.LogInformation("SMS notification would be sent to {Count} numbers", phoneNumbers?.Count ?? 0);

            return new NotificationResultDto()
            {
                Success = 0,
                Failure = phoneNumbers.Count,
                TargetType = "sms",
                Total = phoneNumbers.Count
            };
        }

        private async Task<NotificationResultDto> SendEmailNotifications(MessageRequest request, List<string> emails)
        {
            if (emails == null || emails.Count == 0)
            {
                return new NotificationResultDto()
                {
                    Success = 0,
                    Failure = emails.Count,
                    TargetType = "mail",
                    Total = emails.Count
                };
            }

            try
            {
                var emailRequest = new SendNotificationMailRequest
                {
                    ToEmails = emails,
                    Title = request.Title,
                    Body = request.Body,
                    ImageUrl = request.ImageUrl
                };

                bool success = await _emailNotification.SendNotificationAsync(emailRequest);

                return new NotificationResultDto()
                {
                    Success = 0,
                    Failure = emails.Count,
                    TargetType = "mail",
                    Total = emails.Count
                };
            }
            catch (Exception)
            {
                return new NotificationResultDto()
                {
                    Success = 0,
                    Failure = emails.Count,
                    TargetType = "mail",
                    Total = emails.Count
                };
            }
        }

        private async Task<string?> LogNotificationHistory(SendByConditionRequest request, NotificationRecipients recipients, Dictionary<string, NotificationResultDto> results)
        {
            try
            {
                string notificationId = Guid.NewGuid().ToString();
                var history = new NotificationHistory
                {
                    Id = notificationId,
                    Title = request.Title,
                    Body = request.Body,
                    Type = request.Type,
                    Priority = request.Priority,
                    DeliveryChannels = request.DeliveryChannels,
                    CreatedBy = request.CreatedBy,
                    CreatedAt = request.Timestamp.Value,
                    Recipients = recipients.Count,
                    Status = results.Count != 0 ? "sent" : "faild",
                    Results = results
                };

                await _notificationRepository.SaveHistoryAsync(history);
                _logger.LogDebug("Notification history logged successfully");
                return notificationId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging notification history");
                return null;
            }
        }


        [NonAction]
        [AutomaticRetry(Attempts = 3, DelaysInSeconds = [60, 300, 900])]
        public async Task ProcessScheduledNotification(string scheduledNotificationId)
        {
            try
            {
                var scheduledNotification = await _notificationRepository.GetScheduledNotificationAsync(scheduledNotificationId);

                if (scheduledNotification == null)
                {
                    throw new ArgumentException($"Scheduled notification with ID {scheduledNotificationId} not found");
                }

                if (scheduledNotification.Status != "Scheduled")
                {
                    _logger.LogInformation("Scheduled notification {Id} already processed with status: {Status}",
                        scheduledNotificationId, scheduledNotification.Status);
                    return;
                }

                await _notificationRepository.UpdateScheduledNotificationStatusAsync(scheduledNotificationId, "Processing");

                var sendRequest = MapToSendByConditionRequest(scheduledNotification);
                await SendImmediateNotification(sendRequest);

                await _notificationRepository.UpdateScheduledNotificationStatusAsync(scheduledNotificationId, "Completed");

                _logger.LogInformation("Scheduled notification {Id} processed successfully", scheduledNotificationId);
            }
            catch (Exception ex)
            {
                await _notificationRepository.UpdateScheduledNotificationStatusAsync(scheduledNotificationId, "Failed");
                _logger.LogError(ex, "Error processing scheduled notification {Id}", scheduledNotificationId);
                throw;
            }
        }

        private SendByConditionRequest MapToSendByConditionRequest(ScheduledNotification scheduledNotification)
        {
            return new SendByConditionRequest
            {
                Title = scheduledNotification.Title,
                Body = scheduledNotification.Body,
                Type = scheduledNotification.Type,
                Priority = scheduledNotification.Priority,
                DeliveryChannels = scheduledNotification.DeliveryChannels,
                CreatedBy = scheduledNotification.CreatedBy,
                Timestamp = scheduledNotification.CreatedAt,
                FacultyCodes = scheduledNotification.FacultyCodes,
                CourseCodes = scheduledNotification.CourseCodes,
                ClassCodes = scheduledNotification.ClassCodes,
                UserNames = scheduledNotification.UserNames,
                SendAll = scheduledNotification.SendAll
            };
        }

        [HttpDelete("scheduled/{scheduledId}")]
        public async Task<IActionResult> CancelScheduledNotification(string scheduledId)
        {
            if (string.IsNullOrWhiteSpace(scheduledId))
                return BadRequest(new { error = "Scheduled ID is required" });

            try
            {
                var scheduledNotification = await _notificationRepository.GetScheduledNotificationAsync(scheduledId);

                if (scheduledNotification == null)
                {
                    return NotFound(new { error = "Scheduled notification not found" });
                }

                if (scheduledNotification.Status != "Scheduled")
                {
                    return BadRequest(new { error = $"Cannot cancel notification with status: {scheduledNotification.Status}" });
                }

                // Cancel Hangfire job
                if (!string.IsNullOrEmpty(scheduledNotification.HangfireJobId))
                {
                    var deleted = BackgroundJob.Delete(scheduledNotification.HangfireJobId);
                    _logger.LogInformation("Hangfire job {JobId} deletion result: {Result}",
                        scheduledNotification.HangfireJobId, deleted);
                }

                await _notificationRepository.UpdateScheduledNotificationStatusAsync(scheduledId, "Cancelled");

                return Ok(new { message = "Scheduled notification cancelled successfully", timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling scheduled notification {Id}", scheduledId);
                return StatusCode(500, new { error = "Error cancelling scheduled notification" });
            }
        }
        [HttpDelete("delete/id={id}")]
        public async Task<IActionResult> DeleteNotificationById(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return BadRequest("Id must be not null");
            }
            bool isDeletedSuccess = await _notificationRepository.DeleteNotificationByIdAsync(id);
            if(isDeletedSuccess)
            {
                return Ok(id);
            }
            return BadRequest("Error to delete, please check current id and try later");
        }
        
        [HttpGet("scheduled")]
        public async Task<IActionResult> GetScheduledNotifications(
            [FromQuery, Range(1, int.MaxValue)] int page = 1,
            [FromQuery, Range(1, MaxPageSize)] int pageSize = DefaultPageSize,
            [FromQuery] string? status = null,
            [FromQuery] string? createdBy = null)
        {
            try
            {
                // Ensure page size doesn't exceed maximum
                pageSize = Math.Min(pageSize, MaxPageSize);

                var result = await _notificationRepository.GetScheduledNotificationsAsync(page, pageSize, status, createdBy);

                return Ok(new
                {
                    data = result,
                    pagination = new
                    {
                        page,
                        pageSize,
                        hasNext = result?.HasNextPage,
                        filters = new { status, createdBy }
                    },
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting scheduled notifications");
                return StatusCode(500, new { error = "Error retrieving scheduled notifications" });
            }
        }

        [HttpGet("history")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNotificationHistory(
            [FromQuery, Range(0, int.MaxValue)] int startAt = 1,
            [FromQuery, Range(1, MaxPageSize)] int pageSize = DefaultPageSize)
        {
            try
            {
                var result = await _notificationRepository.GetNotificationHistoryAsync(startAt, pageSize);
                List<NotificationHistoryResponse> notificationHistory = result.Select(notification =>
                    new NotificationHistoryResponse
                    {
                        Id = notification.Id,
                        Body = notification.Body,
                        Recipients = notification.Recipients,
                        Status = notification.Status,
                        Priority = notification.Priority,
                        CreatedBy = notification.CreatedBy,
                        Time = notification.CreatedAt,
                        Title = notification.Title,
                        Type = notification.Type
                    }
                ).ToList();
                return Ok(notificationHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notification history");
                return StatusCode(500, new { error = "Error retrieving notification history" });
            }
        }
    }

    // Helper classes for better structure
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
    }

    public static class NotificationRecipientsExtensions
    {
        public static bool HasAnyRecipients(this NotificationRecipients recipients)
        {
            return (recipients.FcmTokens?.Any() == true) ||
                   (recipients.PhoneNumbers?.Any() == true) ||
                   (recipients.Emails?.Any() == true);
        }

        public static int GetTotalRecipientCount(this NotificationRecipients recipients)
        {
            return (recipients.FcmTokens?.Count ?? 0) +
                   (recipients.PhoneNumbers?.Count ?? 0) +
                   (recipients.Emails?.Count ?? 0);
        }

    }
}