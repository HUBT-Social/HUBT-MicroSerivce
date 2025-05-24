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

namespace Notation_API.Src.Controllers
{
    [Route("api/notation")]
    [ApiController]
    [Authorize]
    public class NotationController(IFireBaseNotificationService fireBaseNotificationService, IUserService userService, IHttpCloudService httpCloudService) : ControllerBase
    {
        private readonly IFireBaseNotificationService _fireBaseNotificationService = fireBaseNotificationService;
        private readonly IUserService _userService = userService;
        private readonly IHttpCloudService _httpCloudService = httpCloudService;

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


        [HttpPost("send-by-condition")]
        public async Task<IActionResult> SendNotationByCondition(SendByConditionRequest request)
        {
            try
            {
                // Validate request
                if (request == null)
                {
                    return BadRequest("Request cannot be null.");
                }

                if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Body))
                {
                    return BadRequest("Title and Body are required.");
                }

                // Check conditions if SendAll is false
                if (!request.SendAll &&
                    (request.UserNames == null || !request.UserNames.Any()) &&
                    (request.ClassCodes == null || !request.ClassCodes.Any()) &&
                    (request.FacultyCodes == null || !request.FacultyCodes.Any()) &&
                    (request.CourseCodes == null || !request.CourseCodes.Any()))
                {
                    return BadRequest("At least one condition (UserNames, ClassCodes, FacultyCodes, or CourseCodes) is required when SendAll is false.");
                }

                // Map to ConditionRequest (fix swapped fields)
                var condition = new ConditionRequest
                {
                    ClassCodes = request.ClassCodes,
                    CourseCodes = request.CourseCodes, // Fixed: Correct mapping
                    FacultyCodes = request.FacultyCodes, // Fixed: Correct mapping
                    UserNames = request.UserNames,
                    SendAll = request.SendAll,
                };

                // Get FCM tokens
                List<string> fmcTokens = await _userService.GetListFMCFromCondition(condition);
      
                if (fmcTokens.Count == 0)
                {
                    return BadRequest("No valid FCM tokens found for the specified conditions.");
                }

                // Prepare notification request
                var sendRequest = new SendMessageRequest
                {
                    Body = request.Body,
                    RequestId = request.RequestId,
                    Title = request.Title,
                    Type = request.Type
                };

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

                // Report partial failures
                if (failedTokens.Any())
                {
                    return StatusCode(207, new
                    {
                        Message = "Notifications sent with partial failures.",
                        FailedTokens = failedTokens,
                        SuccessfulCount = fmcTokens.Count - failedTokens.Count,
                        TotalCount = fmcTokens.Count
                    });
                }

                return Ok("Đã gửi.");
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error processing notification request with RequestId: {RequestId}", request.RequestId);
                return StatusCode(500, LocalValue.Get(KeyStore.NotificationSendError));
            }
        }

        [HttpPost("topic-subscribe")]
        public async Task<IActionResult> SubscribeTopic([FromBody] SubScribeTopicDTO request)
        {

            try
            {
                bool result = await _fireBaseNotificationService.SubscribeTopicAsync(request.Topic, request.Tokens);
                if (result)
                    Console.WriteLine($"subscribe topic {request.Topic}");


                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(LocalValue.Get(KeyStore.NotificationSendError));
            }
        }
        [HttpPost("topic-unsubscribe")]
        public async Task<IActionResult> UnsubscribeTopic([FromBody] SubScribeTopicDTO request)
        {
            try
            {
                bool result = await _fireBaseNotificationService.UnsubscribeTopicAsync(request.Topic, request.Tokens);
                if (result)
                    Console.Write($"unsubcribe topic {request.Topic}");

                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(LocalValue.Get(KeyStore.NotificationSendError));
            }
        }
    }
}
