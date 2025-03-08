using HUBT_Social_Core.Models.Requests.Firebase;
using HUBT_Social_Core.Settings;
using HUBT_Social_Firebase.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Notation_API.Src.Services;

namespace Notation_API.Src.Controllers
{
    [Route("api/notation")]
    [ApiController]
    [Authorize]
    public class NotationController(IFireBaseNotificationService fireBaseNotificationService) : ControllerBase
    {
        private readonly IFireBaseNotificationService _fireBaseNotificationService = fireBaseNotificationService;
        [HttpPost("send-to-one")]
        public async Task<IActionResult> SendNotationToOne([FromBody] SendMessageRequest request)
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
    }
}
