<<<<<<< Updated upstream
using HUBT_Social_Base.Service;
using HUBT_Social_Core.Decode;
using HUBT_Social_Core.Models.DTOs.NotationDTO;
using HUBT_Social_Core.Models.DTOs.NotationDTO;
using HUBT_Social_Core.Models.Requests.Firebase;
using HUBT_Social_Core.Settings;
using HUBT_Social_Firebase.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Notation_API.Src.Services;
=======
//using Hangfire;
//using HUBT_Social_Base.Service;
//using HUBT_Social_Core.Decode;
//using HUBT_Social_Core.Models.DTOs.NotationDTO;
//using HUBT_Social_Core.Models.Requests.Firebase;
//using HUBT_Social_Core.Settings;
//using HUBT_Social_Firebase.Services;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json.Linq;
//using Notation_API.Src.Services;
>>>>>>> Stashed changes

//namespace Notation_API.Src.Controllers
//{
//    [Route("api/notation")]
//    [ApiController]
//    [Authorize]
//    public class NotationController : ControllerBase
//    {
//        private readonly IFireBaseNotificationService _fireBaseNotificationService;
//        private readonly IUserService _userService;
//        private readonly IHttpCloudService _httpCloudService;
//        //private readonly INotificationRepository _notificationRepository;
//        private readonly ILogger<NotationController> _logger;

<<<<<<< Updated upstream
        [HttpPost("send-to-one")]
        public async Task<IActionResult> SendNotationToOne([FromBody] SendMessageRequest request)
        {
            try
            {
                //if (request.Token.StartsWith("userId_"))
                //{
                //    string userId = request.Token[7..]; // Cắt bỏ "userId_" để lấy ID thực
                //    string? userFcm = await _userService.GetUserFCMFromId(userId);

                //    if (string.IsNullOrEmpty(userFcm))
                //        return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));

                //    // Cập nhật Token bằng FCM Token thực tế
                //    request.Token = userFcm;
                //}
                await _fireBaseNotificationService.SendNotificationAsync(request);
                return Ok(LocalValue.Get(KeyStore.NotificationSend));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(LocalValue.Get(KeyStore.NotificationSendError));
            }
        }
        [HttpPost("send-to-many")]
        public async Task<IActionResult> SendNotationToMany([FromBody] SendGroupMessageRequest request)
        {
            try
            {
                await _fireBaseNotificationService.SendNotificationAsync(request);
                return Ok(LocalValue.Get(KeyStore.NotificationSend));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(LocalValue.Get(KeyStore.NotificationSendError));
            }
        }
        [HttpPost("send-to-group-chat")]
        public async Task<IActionResult> SendNotationToGroupChat([FromBody] SendNotationToGroupChatRequest request)
        {
            try
            {
                if(request.UserNames.Count == 0)
                {
                    return BadRequest(LocalValue.Get(KeyStore.NotificationSendError));
                }
                List<string>? FCMs = await _userService.GetListFMCFromListUserName(request.UserNames);
                if (FCMs == null)
                {
                    return BadRequest(LocalValue.Get(KeyStore.NotificationSendError));
                }
                SendMessageRequest sendRequest = new SendMessageRequest
                {
                    Body = request.Body,
                    ImageUrl = request.ImageUrl,
                    RequestId = request.RequestId,
                    Title = request.Title,
                    Type = request.Type
                };
                foreach (var fcm in FCMs)
                {
                    sendRequest.Token = fcm;
                    await _fireBaseNotificationService.SendNotificationAsync(sendRequest);
                }
                
                return Ok(LocalValue.Get(KeyStore.NotificationSend));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(LocalValue.Get(KeyStore.NotificationSendError));
            }
        }
=======
//        public NotationController(
//            IFireBaseNotificationService fireBaseNotificationService,
//            IUserService userService,
//            IHttpCloudService httpCloudService,
//            //INotificationRepository notificationRepository,
//            ILogger<NotationController> logger)
//        {
//            _fireBaseNotificationService = fireBaseNotificationService;
//            _userService = userService;
//            _httpCloudService = httpCloudService;
//           // _notificationRepository = notificationRepository;
//            _logger = logger;
//        }
>>>>>>> Stashed changes

//        [HttpPost("send-to-one")]
//        public async Task<IActionResult> SendNotationToOne([FromBody] SendMessageRequest request)
//        {
//            try
//            {
//                await _fireBaseNotificationService.SendNotificationAsync(request);
//                return Ok(LocalValue.Get(KeyStore.NotificationSend));
//            }
//            catch (Exception e)
//            {
//                _logger.LogError(e, "Error sending notification to one user");
//                return BadRequest(LocalValue.Get(KeyStore.NotificationSendError));
//            }
//        }

//        [HttpPost("send-to-many")]
//        public async Task<IActionResult> SendNotationToMany([FromBody] SendGroupMessageRequest request)
//        {
//            try
//            {
//                await _fireBaseNotificationService.SendNotificationAsync(request);
//                return Ok(LocalValue.Get(KeyStore.NotificationSend));
//            }
//            catch (Exception e)
//            {
//                _logger.LogError(e, "Error sending notification to many users");
//                return BadRequest(LocalValue.Get(KeyStore.NotificationSendError));
//            }
//        }

//        [HttpPost("send-to-group-chat")]
//        public async Task<IActionResult> SendNotationToGroupChat([FromBody] SendNotationToGroupChatRequest request)
//        {
//            try
//            {
//                if (request.UserNames?.Count == 0)
//                {
//                    return BadRequest(LocalValue.Get(KeyStore.NotificationSendError));
//                }

//                List<string>? FCMs = await _userService.GetListFMCFromListUserName(request.UserNames);
//                if (FCMs == null || !FCMs.Any())
//                {
//                    return BadRequest(LocalValue.Get(KeyStore.NotificationSendError));
//                }

//                SendMessageRequest sendRequest = new SendMessageRequest
//                {
//                    Body = request.Body,
//                    ImageUrl = request.ImageUrl,
//                    RequestId = request.RequestId,
//                    Title = request.Title,
//                    Type = request.Type
//                };

//                foreach (var fcm in FCMs)
//                {
//                    sendRequest.Token = fcm;
//                    await _fireBaseNotificationService.SendNotificationAsync(sendRequest);
//                }

//                return Ok(LocalValue.Get(KeyStore.NotificationSend));
//            }
//            catch (Exception e)
//            {
//                _logger.LogError(e, "Error sending notification to group chat");
//                return BadRequest(LocalValue.Get(KeyStore.NotificationSendError));
//            }
//        }

<<<<<<< Updated upstream
                // Handle image upload
                if (request.Image != null && !string.IsNullOrEmpty(request.Image.Base64String) && !string.IsNullOrEmpty(request.Image.FileName))
                {
                    try
                    {
                        var uploadResponse = await _httpCloudService.GetUrlFormFile(request.Image);
                        if (string.IsNullOrEmpty(uploadResponse))
                        {
                            //_logger.LogWarning("Image upload failed for notification with RequestId: {RequestId}", request.RequestId);
                            // Continue without image if upload fails
                        }
                        else
                        {
                            sendRequest.ImageUrl = uploadResponse;
                        }
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex, "Error uploading image for notification with RequestId: {RequestId}", request.RequestId);
                        // Continue without image
                    }
                }

                // Send notifications
                var failedTokens = new List<string>();
                foreach (var fmc in fmcTokens)
                {
                    try
                    {
                        sendRequest.Token = fmc;
                        await _fireBaseNotificationService.SendNotificationAsync(sendRequest);
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex, "Failed to send notification to FCM token: {Token}", fmc);
                        failedTokens.Add(fmc);
                    }
                }
=======
//        [HttpPost("topic-subscribe")]
//        public async Task<IActionResult> SubscribeTopic([FromBody] SubScribeTopicDTO request)
//        {
//            try
//            {
//                bool result = await _fireBaseNotificationService.SubscribeTopicAsync(request.Topic, request.Tokens);
//                if (result)
//                    _logger.LogInformation($"Successfully subscribed to topic {request.Topic}");

//                return Ok(result);
//            }
//            catch (Exception e)
//            {
//                _logger.LogError(e, $"Error subscribing to topic {request.Topic}");
//                return BadRequest(LocalValue.Get(KeyStore.NotificationSendError));
//            }
//        }

//        [HttpPost("topic-unsubscribe")]
//        public async Task<IActionResult> UnsubscribeTopic([FromBody] SubScribeTopicDTO request)
//        {
//            try
//            {
//                bool result = await _fireBaseNotificationService.UnsubscribeTopicAsync(request.Topic, request.Tokens);
//                if (result)
//                    _logger.LogInformation($"Successfully unsubscribed from topic {request.Topic}");

//                return Ok(result);
//            }
//            catch (Exception e)
//            {
//                _logger.LogError(e, $"Error unsubscribing from topic {request.Topic}");
//                return BadRequest(LocalValue.Get(KeyStore.NotificationSendError));
//            }
//        }
>>>>>>> Stashed changes

//        [HttpPost("send-by-condition")]
//        public async Task<IActionResult> SendNotationByCondition([FromBody] SendByConditionRequest request)
//        {
//            try
//            {
//                // Basic validation
//                if (!ModelState.IsValid)
//                {
//                    return BadRequest(ModelState);
//                }

<<<<<<< Updated upstream
                return Ok("Đã gửi.");
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error processing notification request with RequestId: {RequestId}", request.RequestId);
                return StatusCode(500, LocalValue.Get(KeyStore.NotificationSendError));
            }
        }
=======
//                if (request == null)
//                {
//                    return BadRequest("Request cannot be null.");
//                }
>>>>>>> Stashed changes

//                // Set timestamp if not provided
//                if (!request.Timestamp.HasValue)
//                {
//                    request.Timestamp = DateTime.UtcNow;
//                }

//                // Set createdBy if not provided
//                if (string.IsNullOrEmpty(request.CreatedBy))
//                {
//                    request.CreatedBy = GetCurrentUserId();
//                }

//                // Validate scheduling
//                if (request.ScheduleEnabled && !request.ScheduledTime.HasValue)
//                {
//                    return BadRequest("ScheduledTime is required when ScheduleEnabled is true.");
//                }

//                if (request.ScheduleEnabled && request.ScheduledTime.HasValue && request.ScheduledTime.Value <= DateTime.UtcNow)
//                {
//                    return BadRequest("ScheduledTime must be in the future.");
//                }

//                // Check conditions if SendAll is false
//                if (!request.SendAll &&
//                    (request.UserNames == null || !request.UserNames.Any()) &&
//                    (request.ClassCodes == null || !request.ClassCodes.Any()) &&
//                    (request.FacultyCodes == null || !request.FacultyCodes.Any()) &&
//                    (request.CourseCodes == null || !request.CourseCodes.Any()))
//                {
//                    return BadRequest("At least one condition is required when SendAll is false.");
//                }

//                // Validate delivery channels
//                if (request.DeliveryChannels != null && request.DeliveryChannels.Any())
//                {
//                    var validChannels = new[] { "push", "sms", "email" };
//                    var invalidChannels = request.DeliveryChannels.Where(c => !validChannels.Contains(c.ToLower())).ToList();

//                    if (invalidChannels.Any())
//                    {
//                        return BadRequest($"Invalid delivery channels: {string.Join(", ", invalidChannels)}. Valid channels are: {string.Join(", ", validChannels)}");
//                    }
//                }
//                else
//                {
//                    request.DeliveryChannels = new List<string> { "push" };
//                }

//                // Handle scheduling
//                if (request.ScheduleEnabled && request.ScheduledTime.HasValue)
//                {
//                    return await HandleScheduledNotification(request);
//                }

//                // Handle immediate sending
//                return await SendImmediateNotification(request);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error in SendNotationByCondition");
//                return StatusCode(500, LocalValue.Get(KeyStore.NotificationSendError));
//            }
//        }

//        private async Task<IActionResult> HandleScheduledNotification(SendByConditionRequest request)
//        {
//            try
//            {
//                var jobId = $"notification_{Guid.NewGuid()}";

//                var scheduledNotification = new ScheduledNotification
//                {
//                    Id = Guid.NewGuid().ToString(),
//                    JobId = jobId,
//                    Title = request.Title,
//                    Body = request.Body,
//                    Type = request.Type,
//                    Priority = request.Priority ?? "medium",
//                    DeliveryChannels = request.DeliveryChannels,
//                    ScheduledTime = request.ScheduledTime.Value,
//                    CreatedBy = request.CreatedBy,
//                    CreatedAt = request.Timestamp ?? DateTime.UtcNow,
//                    Status = "Scheduled",
//                    FacultyCodes = request.FacultyCodes,
//                    CourseCodes = request.CourseCodes,
//                    ClassCodes = request.ClassCodes,
//                    UserNames = request.UserNames,
//                    SendAll = request.SendAll
//                };

//                await _notificationRepository.SaveScheduledNotificationAsync(scheduledNotification);

//                // Schedule job với DateTimeOffset để tránh timezone issues
//                var scheduledTimeOffset = new DateTimeOffset(request.ScheduledTime.Value);
//                var hangfireJobId = BackgroundJob.Schedule(
//                    () => ProcessScheduledNotification(scheduledNotification.Id),
//                    scheduledTimeOffset
//                );

//                // Cập nhật Hangfire job ID
//                scheduledNotification.HangfireJobId = hangfireJobId;
//                await _notificationRepository.UpdateScheduledNotificationAsync(scheduledNotification);

//                return Ok(new
//                {
//                    Message = "Notification scheduled successfully",
//                    ScheduledId = scheduledNotification.Id,
//                    JobId = jobId,
//                    HangfireJobId = hangfireJobId,
//                    ScheduledTime = request.ScheduledTime
//                });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error scheduling notification");
//                return StatusCode(500, "Error scheduling notification");
//            }
//        }

//        private async Task<IActionResult> SendImmediateNotification(SendByConditionRequest request)
//        {
//            try
//            {
//                var condition = new ConditionRequest
//                {
//                    ClassCodes = request.ClassCodes,
//                    CourseCodes = request.CourseCodes,
//                    FacultyCodes = request.FacultyCodes,
//                    UserNames = request.UserNames,
//                    SendAll = request.SendAll,
//                };

//                var recipients = await GetRecipientsByChannels(condition, request.DeliveryChannels);

//                if (!recipients.Any())
//                {
//                    return BadRequest("No recipients found for the specified conditions and delivery channels.");
//                }

//                var sendRequest = new SendMessageRequest
//                {
//                    Body = request.Body,
//                    RequestId = request.RequestId,
//                    Title = request.Title,
//                    Type = request.Type,
//                };

//                // Handle image upload
//                if (request.ImageFile != null && !string.IsNullOrEmpty(request.ImageFile.FileData))
//                {
//                    try
//                    {
//                        var uploadResponse = await _httpCloudService.GetUrlFormBase6(request.ImageFile);
//                        if (!string.IsNullOrEmpty(uploadResponse))
//                        {
//                            sendRequest.ImageUrl = uploadResponse;
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        _logger.LogWarning(ex, "Failed to upload image for notification");
//                    }
//                }

//                var results = new Dictionary<string, object>();

//                foreach (var channel in request.DeliveryChannels)
//                {
//                    switch (channel.ToLower())
//                    {
//                        case "push":
//                            results["push"] = await SendPushNotifications(sendRequest, recipients.FcmTokens);
//                            break;
//                        case "sms":
//                            results["sms"] = await SendSmsNotifications(sendRequest, recipients.PhoneNumbers);
//                            break;
//                        case "email":
//                            results["email"] = await SendEmailNotifications(sendRequest, recipients.Emails);
//                            break;
//                    }
//                }

//                await LogNotificationHistory(request, recipients, results);

//                return Ok(new
//                {
//                    Message = "Notifications sent successfully",
//                    Results = results,
//                    Timestamp = DateTime.UtcNow
//                });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error sending immediate notification");
//                return StatusCode(500, "Error sending notification");
//            }
//        }

//        private async Task<NotificationRecipients> GetRecipientsByChannels(ConditionRequest condition, List<string> channels)
//        {
//            var recipients = new NotificationRecipients();

//            if (channels.Contains("push", StringComparer.OrdinalIgnoreCase))
//            {
//                recipients.FcmTokens = await _userService.GetListFMCFromCondition(condition) ?? new List<string>();
//            }

//            //if (channels.Contains("sms", StringComparer.OrdinalIgnoreCase))
//            //{
//            //    recipients.PhoneNumbers = await _userService.GetPhoneNumbersFromCondition(condition) ?? new List<string>();
//            //}

//            //if (channels.Contains("email", StringComparer.OrdinalIgnoreCase))
//            //{
//            //    recipients.Emails = await _userService.GetEmailsFromCondition(condition) ?? new List<string>();
//            //}

//            return recipients;
//        }

//        private async Task<object> SendPushNotifications(SendMessageRequest request, List<string> fcmTokens)
//        {
//            var failedTokens = new List<string>();
//            var successCount = 0;

//            foreach (var token in fcmTokens)
//            {
//                try
//                {
//                    request.Token = token;
//                    await _fireBaseNotificationService.SendNotificationAsync(request);
//                    successCount++;
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogWarning(ex, $"Failed to send push notification to token: {token}");
//                    failedTokens.Add(token);
//                }
//            }

//            return new
//            {
//                TotalCount = fcmTokens.Count,
//                SuccessfulCount = successCount,
//                FailedCount = failedTokens.Count,
//                FailedTokens = failedTokens
//            };
//        }

//        private async Task<object> SendSmsNotifications(SendMessageRequest request, List<string> phoneNumbers)
//        {
//            // TODO: Implement SMS service
//            _logger.LogInformation($"SMS notification would be sent to {phoneNumbers.Count} numbers");

//            return new
//            {
//                TotalCount = phoneNumbers.Count,
//                SuccessfulCount = 0,
//                Message = "SMS sending not implemented yet"
//            };
//        }

//        private async Task<object> SendEmailNotifications(SendMessageRequest request, List<string> emails)
//        {
//            // TODO: Implement Email service
//            _logger.LogInformation($"Email notification would be sent to {emails.Count} addresses");

//            return new
//            {
//                TotalCount = emails.Count,
//                SuccessfulCount = 0,
//                Message = "Email sending not implemented yet"
//            };
//        }

//        private string GetCurrentUserId()
//        {
//            return User?.Identity?.Name ?? "system";
//        }

//        private async Task LogNotificationHistory(SendByConditionRequest request, NotificationRecipients recipients, Dictionary<string, object> results)
//        {
//            try
//            {
//                var history = new NotificationHistory
//                {
//                    Id = Guid.NewGuid().ToString(),
//                    Title = request.Title,
//                    Body = request.Body,
//                    Type = request.Type,
//                    Priority = request.Priority,
//                    DeliveryChannels = request.DeliveryChannels,
//                    CreatedBy = request.CreatedBy,
//                    CreatedAt = request.Timestamp ?? DateTime.UtcNow,
//                    Recipients = recipients,
//                    Results = results
//                };

//                await _notificationRepository.SaveHistoryAsync(history);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error logging notification history");
//            }
//        }

//        [Queue("notifications")]
//        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 60, 300, 900 })]
//        public async Task ProcessScheduledNotification(string scheduledNotificationId)
//        {
//            try
//            {
//                var scheduledNotification = await _notificationRepository.GetScheduledNotificationAsync(scheduledNotificationId);

//                if (scheduledNotification == null)
//                {
//                    throw new ArgumentException($"Scheduled notification with ID {scheduledNotificationId} not found");
//                }

//                if (scheduledNotification.Status != "Scheduled")
//                {
//                    _logger.LogInformation($"Scheduled notification {scheduledNotificationId} already processed with status: {scheduledNotification.Status}");
//                    return;
//                }

//                await _notificationRepository.UpdateScheduledNotificationStatusAsync(scheduledNotificationId, "Processing");

//                var sendRequest = MapToSendByConditionRequest(scheduledNotification);
//                var result = await SendImmediateNotification(sendRequest);

//                await _notificationRepository.UpdateScheduledNotificationStatusAsync(scheduledNotificationId, "Completed");

//                _logger.LogInformation($"Scheduled notification {scheduledNotificationId} processed successfully");
//            }
//            catch (Exception ex)
//            {
//                await _notificationRepository.UpdateScheduledNotificationStatusAsync(scheduledNotificationId, "Failed");
//                _logger.LogError(ex, $"Error processing scheduled notification {scheduledNotificationId}");
//                throw;
//            }
//        }

//        private SendByConditionRequest MapToSendByConditionRequest(ScheduledNotification scheduledNotification)
//        {
//            return new SendByConditionRequest
//            {
//                Title = scheduledNotification.Title,
//                Body = scheduledNotification.Body,
//                Type = scheduledNotification.Type,
//                Priority = scheduledNotification.Priority,
//                DeliveryChannels = scheduledNotification.DeliveryChannels,
//                CreatedBy = scheduledNotification.CreatedBy,
//                Timestamp = scheduledNotification.CreatedAt,
//                FacultyCodes = scheduledNotification.FacultyCodes,
//                CourseCodes = scheduledNotification.CourseCodes,
//                ClassCodes = scheduledNotification.ClassCodes,
//                UserNames = scheduledNotification.UserNames,
//                SendAll = scheduledNotification.SendAll
//            };
//        }

//        [HttpDelete("scheduled/{scheduledId}")]
//        public async Task<IActionResult> CancelScheduledNotification(string scheduledId)
//        {
//            try
//            {
//                var scheduledNotification = await _notificationRepository.GetScheduledNotificationAsync(scheduledId);

//                if (scheduledNotification == null)
//                {
//                    return NotFound("Scheduled notification not found");
//                }

//                if (scheduledNotification.Status != "Scheduled")
//                {
//                    return BadRequest($"Cannot cancel notification with status: {scheduledNotification.Status}");
//                }

//                if (!string.IsNullOrEmpty(scheduledNotification.HangfireJobId))
//                {
//                    BackgroundJob.Delete(scheduledNotification.HangfireJobId);
//                }

//                await _notificationRepository.UpdateScheduledNotificationStatusAsync(scheduledId, "Cancelled");

//                return Ok(new { Message = "Scheduled notification cancelled successfully" });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, $"Error cancelling scheduled notification {scheduledId}");
//                return StatusCode(500, "Error cancelling scheduled notification");
//            }
//        }

//        [HttpGet("scheduled")]
//        public async Task<IActionResult> GetScheduledNotifications(
//            [FromQuery] int page = 1,
//            [FromQuery] int pageSize = 10,
//            [FromQuery] string? status = null)
//        {
//            try
//            {
//                var result = await _notificationRepository.GetScheduledNotificationsAsync(page, pageSize, status);
//                return Ok(result);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting scheduled notifications");
//                return StatusCode(500, "Error getting scheduled notifications");
//            }
//        }

//        // Supporting classes remain the same
//        public class NotificationRecipients
//        {
//            public List<string> FcmTokens { get; set; } = new();
//            public List<string> PhoneNumbers { get; set; } = new();
//            public List<string> Emails { get; set; } = new();
//            public bool Any() => FcmTokens.Any() || PhoneNumbers.Any() || Emails.Any();
//        }

//        public class ScheduledNotification
//        {
//            public string Id { get; set; } = string.Empty;
//            public string JobId { get; set; } = string.Empty;
//            public string? HangfireJobId { get; set; }
//            public string Title { get; set; } = string.Empty;
//            public string Body { get; set; } = string.Empty;
//            public string Type { get; set; } = string.Empty;
//            public string Priority { get; set; } = string.Empty;
//            public List<string> DeliveryChannels { get; set; } = new();
//            public DateTime ScheduledTime { get; set; }
//            public string CreatedBy { get; set; } = string.Empty;
//            public DateTime CreatedAt { get; set; }
//            public string Status { get; set; } = string.Empty;
//            public List<string>? FacultyCodes { get; set; }
//            public List<string>? CourseCodes { get; set; }
//            public List<string>? ClassCodes { get; set; }
//            public List<string>? UserNames { get; set; }
//            public bool SendAll { get; set; }
//            public DateTime? ProcessedAt { get; set; }
//            public string? ErrorMessage { get; set; }
//            public int RetryCount { get; set; } = 0;
//        }

//        public class NotificationHistory
//        {
//            public string Id { get; set; } = string.Empty;
//            public string Title { get; set; } = string.Empty;
//            public string Body { get; set; } = string.Empty;
//            public string Type { get; set; } = string.Empty;
//            public string Priority { get; set; } = string.Empty;
//            public List<string> DeliveryChannels { get; set; } = new();
//            public string CreatedBy { get; set; } = string.Empty;
//            public DateTime CreatedAt { get; set; }
//            public NotificationRecipients Recipients { get; set; } = new();
//            public Dictionary<string, object> Results { get; set; } = new();
//        }
//    }
//}