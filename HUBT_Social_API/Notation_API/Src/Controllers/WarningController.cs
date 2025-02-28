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

namespace Notation_API.Src.Controllers
{
    [Route("api/notation")]
    [ApiController]
    public class WarningController(INotationService notationService,
        IFireBaseNotificationService fireBaseNotificationService,
        IUserService userService
        ) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly INotationService _notationService = notationService;
        private readonly IFireBaseNotificationService _fireBaseNotificationService = fireBaseNotificationService;
        [HttpGet("check-score")]
        public async Task<IActionResult> GetAVGScoreByMasv(string masv)
        {
            string? accessToken = Request.Headers.ExtractBearerToken();
            if (accessToken == null)
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));
            AVGScoreDTO? result = await _notationService.GetAVGScoreByMasv(masv);
            if (result == null)
                return NotFound();   
                
            try
            {
                string? userFcm = await _userService.GetUserFCM(accessToken);
                if (userFcm == null)
                    return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));
                int course = int.Parse(result.MaSV[..2]);
                int year = DateTime.Now.Month < 8 ? DateTime.Now.Year - 1994 + course - 1 : DateTime.Now.Year - 1994 + course;

                if ((year > 3 && result.DiemTB4 < 2.2) || (year == 2 && result.DiemTB4 < 2) || (year == 1 && result.DiemTB4 < 1.8))
                {
                    SendMessageRequest message = new()
                    {
                        Token = userFcm,
                        Title = "Cảnh báo điểm số!!!",
                        Body = $"Sinh viên {result.MaSV} có điểm trung bình 10 là {result.DiemTB10}, điểm trung bình 4 là {result.DiemTB4}."
                    };
                    await _fireBaseNotificationService.SendPushNotificationWarrningAsync(message);
                }
                else
                {
                    SendMessageRequest message = new()
                    {
                        Token = userFcm,
                        Title = "điểm số của bạn",
                        Body = $"Sinh viên {result.MaSV} có điểm trung bình 10 là {result.DiemTB10}, điểm trung bình 4 là {result.DiemTB4}."
                    };
                    await _fireBaseNotificationService.SendPushNotificationInfromationAsync(message);
                }

                return  Ok(LocalValue.Get(KeyStore.MessageSentSuccessfully));

            }
            catch (Exception)
            {
                return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
            }
        }

        
        //[HttpGet("get-student")]
        //public async Task<IActionResult> GetStudentByMasv(string masv)
        //{
        //    StudentDTO? result = await _notationService.GetStudentByMasv(masv);
        //    return result != null ? Ok(result) : NotFound();
        //}
        //[HttpGet("get-student-score")]
        //public async Task<IActionResult> GetStudentScoreByMasv(string masv)
        //{
        //    List<ScoreDTO>? result = await _notationService.GetStudentScoreByMasv(masv);
        //    return result != null ? Ok(result) : NotFound();
        //}
        //[HttpGet("get-time-table")]
        //public async Task<IActionResult> GetTimeTableByClassName(string className)
        //{
        //    List<TimeTableDTO>? result = await _notationService.GetTimeTableByClassName(className);
        //    return result != null ? Ok(result) : NotFound();
        //}
    }
}
