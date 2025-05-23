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
                if (request == null) { return BadRequest(); }
                if ((request.UserNames == null || !request.UserNames.Any()) &&
                    (request.ClassCodes == null || !request.ClassCodes.Any()) &&
                    (request.FacultyCodes == null || !request.FacultyCodes.Any()) &&
                    (request.CourseCodes == null || !request.CourseCodes.Any()))
                {
                    return BadRequest("Cần ít nhất một điều kiện để gửi thông báo.");
                }
                ConditionRequest condition = new ConditionRequest
                {
                    ClassCodes = request.ClassCodes,
                    CourseCodes = request.FacultyCodes,
                    FacultyCodes = request.CourseCodes,
                    UserNames = request.UserNames
                };
                List<string>? FMCs = await _userService.GetListFMCFromCondition(condition);
                if (FMCs == null) { return BadRequest(); }
                if (FMCs.Count == 0) { return BadRequest(); }

                SendMessageRequest sendRequest = new SendMessageRequest
                {
                    Body = request.Body,
                    RequestId = request.RequestId,
                    Title = request.Title,
                    Type = request.Type
                };

                if (request.Image != null && !string.IsNullOrEmpty(request.Image.Base64String) && !string.IsNullOrEmpty(request.Image.FileName))
                {
                    var uploadResponse = await _httpCloudService.GetUrlFormFile(request.Image);
                    if (uploadResponse != null)
                    {
                        sendRequest.ImageUrl = uploadResponse;
                    }
                }

                foreach (var fmc in FMCs)
                {
                    sendRequest.Token = fmc;
                    await _fireBaseNotificationService.SendNotificationAsync(sendRequest);
                }
                return Ok("Da gui.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(LocalValue.Get(KeyStore.NotificationSendError));
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
