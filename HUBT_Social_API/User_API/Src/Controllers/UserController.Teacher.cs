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
    public class UserTeacher(
        ITempService tempService,
        IHelperService helperService) : ControllerBase
    {
        private readonly ITempService _tempService = tempService;
        private readonly IHelperService _helperService = helperService;


        [HttpPost("timetable")]
        public async Task<IActionResult> CreateClassSchedule([FromBody] TimetableOutputDTO request)
        {

            try
            {
                ClassScheduleVersionDTO classScheduleVersionDTO = await _tempService.GetClassScheduleVersion(request.ClassName);
                if (classScheduleVersionDTO.ClassName == string.Empty)
                    return BadRequest(LocalValue.Get(KeyStore.TimetableNotSetYet));
                TimetableOutputDTO response = await _tempService.StoreInTimeTable(request);

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
        public async Task<IActionResult> UpdateClassSchedule([FromBody] UpdateTimetableRequest request)
        {

            try
            {
                TimetableOutputDTO timetableOutputDTO = await _tempService.UpdateTimetable(request);
                return Ok(timetableOutputDTO);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(LocalValue.Get(KeyStore.TimetableNotFound));
            }
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
