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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Notation_API.Src.Repository;
using Notation_API.Src.Services;
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
        private readonly JwtSetting  _jwtSettings;
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
            IOptions<JwtSetting>  jwtSettings,
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

        [HttpPost("send-to-one")]
        public async Task<IActionResult> SendNotificationToOne([FromBody] SendMessageRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _fireBaseNotificationService.SendNotificationAsync(request);
                _logger.LogInformation("Successfully sent notification to single user");
                return Ok(new { message = LocalValue.Get(KeyStore.NotificationSend), timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification to single user with token: {Token}", request.Token);
                return BadRequest(new { error = LocalValue.Get(KeyStore.NotificationSendError) });
            }
        }

        [HttpPost("send-to-many")]
        public async Task<IActionResult> SendNotificationToMany([FromBody] SendGroupMessageRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _fireBaseNotificationService.SendNotificationAsync(request);
                _logger.LogInformation($"Successfully sent notification to {request.GroupId}");
                return Ok(new { message = LocalValue.Get(KeyStore.NotificationSend), timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification to multiple users");
                return BadRequest(new { error = LocalValue.Get(KeyStore.NotificationSendError) });
            }
        }

        [HttpPost("send-to-group-chat")]
        public async Task<IActionResult> SendNotificationToGroupChat([FromBody] SendNotationToGroupChatRequest request)
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

                var sendRequest = new SendMessageRequest
                {
                    Body = request.Body,
                    ImageUrl = request.ImageUrl,
                    RequestId = request.RequestId,
                    Title = request.Title,
                    Type = request.Type
                };

                // Use parallel processing for better performance
                var tasks = fcmTokens.Select(async fcm =>
                {
                    var individualRequest = new SendMessageRequest
                    {
                        Body = sendRequest.Body,
                        ImageUrl = sendRequest.ImageUrl,
                        RequestId = sendRequest.RequestId,
                        Title = sendRequest.Title,
                        Type = sendRequest.Type,
                        Token = fcm
                    };

                    try
                    {
                        await _fireBaseNotificationService.SendNotificationAsync(individualRequest);
                        return new { Token = fcm, Success = true, Error = (string)null };
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to send notification to FCM token: {Token}", fcm);
                        return new { Token = fcm, Success = false, Error = ex.Message };
                    }
                });

                var results = await Task.WhenAll(tasks);
                var successCount = results.Count(r => r.Success);
                var failedCount = results.Count(r => !r.Success);

                _logger.LogInformation("Group chat notification sent: {Success} successful, {Failed} failed",
                    successCount, failedCount);

                return Ok(new
                {
                    message = LocalValue.Get(KeyStore.NotificationSend),
                    successCount,
                    failedCount,
                    totalCount = fcmTokens.Count,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification to group chat");
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

                return Ok(new
                {
                    message = "Notification scheduled successfully",
                    scheduledId = scheduledNotification.Id,
                    hangfireJobId,
                    scheduledTime = request.ScheduledTime,
                    timestamp = DateTime.UtcNow
                });
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
                    IncludeEmails   = request.DeliveryChannels.Contains("email")
                };

                var recipients = await GetRecipientsByChannels(condition);

                if (!recipients.HasAnyRecipients())
                {
                    return BadRequest(new { error = "No recipients found for the specified conditions and delivery channels" });
                }

                var sendRequest = await CreateSendMessageRequest(request);
                var results = await SendToMultipleChannels(sendRequest, recipients, request.DeliveryChannels);

                await LogNotificationHistory(request, recipients, results);

                _logger.LogInformation("Immediate notification sent successfully to {Count} recipients",
                    recipients.GetTotalRecipientCount());

                return Ok(new
                {
                    message = "Notifications sent successfully",
                    results,
                    recipientsSummary = recipients.GetSummary(),
                    timestamp = DateTime.UtcNow
                });
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

        private async Task<Dictionary<string, object>> SendToMultipleChannels(
            MessageRequest request,
            NotificationRecipients recipients,
            List<string> channels)
        {
            var results = new Dictionary<string, object>();
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

        private async Task<object> SendPushNotifications(MessageRequest request, List<string> fcmTokens)
        {
            if (!fcmTokens?.Any() == true)
            {
                return new { TotalCount = 0, SuccessfulCount = 0, FailedCount = 0, Message = "No FCM tokens provided" };
            }

            var semaphore = new SemaphoreSlim(10, 10); // Limit concurrent requests
            var successCount = 0;
            var failedTokens = new List<string>();

            var tasks = fcmTokens.Select(async token =>
            {
                await semaphore.WaitAsync();
                try
                {
                    var tokenRequest = new SendMessageRequest
                    {
                        Body = request.Body,
                        ImageUrl = request.ImageUrl,
                        RequestId = request.RequestId,
                        Title = request.Title,
                        Type = request.Type,
                        Token = token
                    };

                    await _fireBaseNotificationService.SendNotificationAsync(tokenRequest);
                    Interlocked.Increment(ref successCount);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send push notification to token: {Token}", token);
                    failedTokens.Add(token);
                }
                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);

            return new
            {
                TotalCount = fcmTokens.Count,
                SuccessfulCount = successCount,
                FailedCount = failedTokens.Count,
                FailedTokens = failedTokens.Take(10).ToList() // Limit logged failed tokens
            };
        }

        private async Task<object> SendSmsNotifications(MessageRequest request, List<string> phoneNumbers)
        {
            // TODO: Implement SMS service with proper provider integration
            _logger.LogInformation("SMS notification would be sent to {Count} numbers", phoneNumbers?.Count ?? 0);

            return new
            {
                TotalCount = phoneNumbers?.Count ?? 0,
                SuccessfulCount = 0,
                FailedCount = 0,
                Message = "SMS sending not implemented yet"
            };
        }

        private async Task<object> SendEmailNotifications(MessageRequest request, List<string> emails)
        {
            if (emails == null || emails.Count == 0)
            {
                return new
                {
                    Message = "Không có email nào được tìm thấy."
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

                return new
                {
                    TotalCount = emails.Count,
                    SuccessfulCount = success ? emails.Count : 0,
                    FailedCount = success ? 0 : emails.Count,
                    Message = success ? "Gửi email thành công." : "Tất cả email gửi thất bại."
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    Message = $"Lỗi khi gửi email: {ex.Message}"
                };
            }
        }

        private async Task LogNotificationHistory(SendByConditionRequest request, NotificationRecipients recipients, Dictionary<string, object> results)
        {
            try
            {
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
                    Recipients = recipients,
                    Results = results
                };

                await _notificationRepository.SaveHistoryAsync(history);
                _logger.LogDebug("Notification history logged successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging notification history");
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
        public async Task<IActionResult> GetNotificationHistory(
            [FromQuery, Range(1, int.MaxValue)] int page = 1,
            [FromQuery, Range(1, MaxPageSize)] int pageSize = DefaultPageSize,
            [FromQuery] string? type = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                pageSize = Math.Min(pageSize, MaxPageSize);

                var result = await _notificationRepository.GetNotificationHistoryAsync(page, pageSize, type, fromDate, toDate);

                return Ok(new
                {
                    data = result,
                    pagination = new
                    {
                        page,
                        pageSize,
                        hasNext = result?.Count == pageSize,
                        filters = new { type, fromDate, toDate }
                    },
                    timestamp = DateTime.UtcNow
                });
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

        public static object GetSummary(this NotificationRecipients recipients)
        {
            return new
            {
                fcmTokens = recipients.FcmTokens?.Count ?? 0,
                phoneNumbers = recipients.PhoneNumbers?.Count ?? 0,
                emails = recipients.Emails?.Count ?? 0,
                total = recipients.GetTotalRecipientCount()
            };
        }
    }
}