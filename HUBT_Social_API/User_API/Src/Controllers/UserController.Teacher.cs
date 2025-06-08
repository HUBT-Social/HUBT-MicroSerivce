using HUBT_Social_Core.Models.DTOs.ExamDTO;
using HUBT_Social_Core.Models.DTOs.UserDTO;
using HUBT_Social_Core.Models.Requests.Temp;
using HUBT_Social_Core.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using User_API.Src.Models;
using User_API.Src.Service;
using static User_API.Src.Controllers.UserShoolDataController;

namespace User_API.Src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTeacher(IUserService userService,
        IOutSourceService outSourceService,
        ITempService tempService,
        IHelperService helperService,
        IChatService chatService) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly IOutSourceService _outSourceService = outSourceService;
        private readonly ITempService _tempService = tempService;
        private readonly IHelperService _helperService = helperService;
        private readonly IChatService _chatService = chatService;


        [HttpPost("timetable")]
        public async Task<IActionResult> CreateClassSchedule([FromBody] TimetableOutputDTO request)
        {

            try
            {
                ClassScheduleVersionDTO classScheduleVersionDTO = await _tempService.GetClassScheduleVersion(request.ClassName);
                if (classScheduleVersionDTO.ClassName == string.Empty)
                    return BadRequest(LocalValue.Get(KeyStore.TimetableNotSetYet));
                TimetableOutputDTO response = await _tempService.StoreIn(request);

                await _tempService.StoreClassScheduleVersion(classScheduleVersionDTO);

                if (response.Id != string.Empty)
                    return Ok(response);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(LocalValue.Get(KeyStore.TimetableNotFound));
            }


            return BadRequest(LocalValue.Get(KeyStore.UnableToStoreInDatabase));
        }

        [HttpPut("timetable")]
        public async Task<IActionResult> UpdateClassSchedule([FromBody] TimetableOutputDTO request)
        {

            try
            {
                ClassScheduleVersionDTO classScheduleVersionDTO = await _tempService.GetClassScheduleVersion(request.ClassName);
                if (classScheduleVersionDTO.ClassName == string.Empty)
                    return BadRequest(LocalValue.Get(KeyStore.TimetableNotSetYet));
                TimetableOutputDTO response = await _tempService.StoreIn(request);

                await _tempService.StoreClassScheduleVersion(classScheduleVersionDTO);

                if (response.Id != string.Empty)
                    return Ok(response);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(LocalValue.Get(KeyStore.TimetableNotFound));
            }


            return BadRequest(LocalValue.Get(KeyStore.UnableToStoreInDatabase));
        }
        [HttpPost("extract-questions")]
        public async Task<IActionResult> ExtractQuestions([FromForm] FileUploadModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Đầu vào không Hợp lệ");

            if (request == null || request.File.Length == 0)
                return BadRequest("File không hợp lệ.");
            Question[] questions = await _helperService.ExtractQuestions(request.File);
            if (questions.Length > 0)
            {
                QuizDetail examDTO = new()
                {
                    Title = request.Title,
                    Description = request.Description,
                    Image = request.ImageUrl,
                    Major = request.Major,
                    Credits = request.Credits,
                    Questions = questions
                };
                ExamDTO result = await _tempService.StoreExam(examDTO);
                Console.Write(result);
                return Ok(examDTO);
            }
            return BadRequest("Khong tim thay cau hoi.");
        }

    }
}
