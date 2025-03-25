using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Models.OutSourceDataDTO;
using HUBT_Social_Core.Models.Requests.Firebase;
using HUBT_Social_Core.Settings;
using HUBT_Social_Core.Decode;
using HUBT_Social_Firebase.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Notation_API.Src.Services;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authorization;

namespace Notation_API.Src.Controllers
{
    [Route("api/notation")]
    [ApiController]
    [Authorize]
    public class WarningController(IOutSourceService notationService,
        IFireBaseNotificationService fireBaseNotificationService,
        IUserService userService
        ) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly IOutSourceService _notationService = notationService;
        private readonly IFireBaseNotificationService _fireBaseNotificationService = fireBaseNotificationService;
        [HttpGet("check-score")]
        public async Task<IActionResult> GetAVGScoreByMasv()
        {
            string? accessToken = Request.Headers.ExtractBearerToken();
            if (accessToken == null)
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));
                
            try
            {
                AUserDTO? userDTO = await _userService.GetUserFCM(accessToken);
                if (userDTO == null)
                    return BadRequest(LocalValue.Get(KeyStore.UserNotFound));

                AVGScoreDTO? result = await _notationService.GetAVGScoreByMasv(userDTO.UserName);
                if (result == null)
                    return NotFound();


                int course = int.Parse(result.MaSV[..2]);
                int year = DateTime.Now.Month < 8 ? DateTime.Now.Year - 1994 + course - 1 : DateTime.Now.Year - 1994 + course;

                if ((year > 3 && result.DiemTB4 < 2.2) || (year == 2 && result.DiemTB4 < 2) || (year == 1 && result.DiemTB4 < 1.8))
                {
                    SendMessageRequest message = new()
                    {
                        Token = userDTO.FCMToken,
                        Title = "Cảnh báo điểm số!!!",
                        Body = $"Sinh viên {result.MaSV} có điểm trung bình 10 là {result.DiemTB10}, điểm trung bình 4 là {result.DiemTB4}."
                    };
                    await _fireBaseNotificationService.SendNotificationAsync(message);
                }
                else
                {
                    SendMessageRequest message = new()
                    {
                        Token = userDTO.FCMToken,
                        Title = "điểm số của bạn",
                        Body = $"Sinh viên {result.MaSV} có điểm trung bình 10 là {result.DiemTB10}, điểm trung bình 4 là {result.DiemTB4}."
                    };
                    await _fireBaseNotificationService.SendNotificationAsync(message);
                }

                return  Ok(LocalValue.Get(KeyStore.MessageSentSuccessfully));

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(LocalValue.Get(KeyStore.InvalidFCMToken));
            }
        }

        
    }
}
