using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Core.Decode;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.ExamDTO;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Models.DTOs.UserDTO;
using HUBT_Social_Core.Models.OutSourceDataDTO;
using HUBT_Social_Core.Models.Requests.Temp;
using HUBT_Social_Core.Settings;
using HUBT_Social_Core.Settings.@enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using User_API.Src.Models;
using User_API.Src.Service;

namespace User_API.Src.Controllers
{
    [Route("api/teacher")]
    [ApiController]
    public class UserTeacher(
        ITempService tempService,
        IHelperService helperService,
        IOutSourceService outSourceService,
        IUserService userService) : ControllerBase
    {
        private readonly ITempService _tempService = tempService;
        private readonly IHelperService _helperService = helperService;
        private readonly IOutSourceService _outSourceService = outSourceService;
        private readonly IUserService _userService = userService;

        [HttpPost("timetable")]
        public async Task<IActionResult> CreateClassSchedule([FromBody] TimetableOutputDTO request)
        {
            string? accessToken = Request.Headers.ExtractBearerToken();
            if (accessToken == null)
            {
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));
            }

            ResponseDTO result = await _userService.GetUser(accessToken);
            AUserDTO? userDTO = result.ConvertTo<AUserDTO>();
            if (userDTO == null)
                return BadRequest(LocalValue.Get(KeyStore.UserNotFound));
            try
            {
                List<StudentDTO> studentDTOs = await _outSourceService.GetStudentByClassName(request.ClassName);
                if (studentDTOs.Count == 0)
                    return BadRequest($"No Student In {request.ClassName} class");

                foreach (StudentDTO student in studentDTOs)
                {
                    ClassScheduleVersionDTO classScheduleVersionDTO = await _tempService.GetClassScheduleVersion(student.MaSV);
                    if (classScheduleVersionDTO.ClassName == string.Empty)
                        classScheduleVersionDTO = new ClassScheduleVersionDTO
                        {
                            ClassName = student.MaSV,
                            ExpireTime = DateTime.Now.AddMonths(2)
                        };
                    await _tempService.StoreClassScheduleVersion(classScheduleVersionDTO);
                }
                if (request.Type == TimeTableType.Study)
                {
                    CreateTempCourseRequest createTempCourseRequest = new()
                    {
                        StudentIDs = studentDTOs.Select(s => s.MaSV).ToArray(),
                        TeacherIDs = [userDTO.UserName],
                        TimeTableDTO = new()
                        {
                            ClassName = request.ClassName.ToUpper(),
                            Room = request.Room,
                            ZoomID = request.ZoomID ?? "",
                            Subject = request.Subject,
                            Day = GetDayStringFromDate(request.StartTime),
                            Session = GetSessionFromTime(request.StartTime).ToUpper(),
                        },
                        CourseId = request.CourseId,
                    };
                    List<CouresDTO> couresDTOs = await _tempService.GetCourses("", request.ClassName);
                    if (!couresDTOs.Any(cs => cs.CourseID == request.CourseId))
                        await _tempService.StoreCourses(createTempCourseRequest);

                }
                else
                {
                    List<CouresDTO> couresDTOs = await _tempService.GetCourses("",request.ClassName);
                    if (!couresDTOs.Any(cs => cs.CourseID == request.CourseId))
                        return BadRequest("User did not enrol this subject");
                }
                TimetableOutputDTO response = await _tempService.StoreInTimeTable(request);


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
        private static string GetSessionFromTime(DateTime time)
        {
            var timeOfDay = time.TimeOfDay;

            if (timeOfDay >= TimeSpan.FromMinutes(30) && timeOfDay < TimeSpan.FromHours(6))
                return "Sáng";
            else if (timeOfDay >= TimeSpan.FromHours(6) && timeOfDay < TimeSpan.FromHours(11))
                return "Chiều";
            else if (timeOfDay >= TimeSpan.FromHours(11))
                return "Tối";
            else
                throw new ArgumentException("Time is outside of defined session ranges.");
        }

        private static string GetDayStringFromDate(DateTime date)
        {
            return date.DayOfWeek switch
            {
                DayOfWeek.Monday => "2",
                DayOfWeek.Tuesday => "3",
                DayOfWeek.Wednesday => "4",
                DayOfWeek.Thursday => "5",
                DayOfWeek.Friday => "6",
                DayOfWeek.Saturday => "7",
                DayOfWeek.Sunday => "cn",
                _ => throw new ArgumentException("Invalid DayOfWeek value.")
            };
        }
        [HttpPut("timetable")]
        public async Task<IActionResult> UpdateClassSchedule([FromBody] UpdateTimetableRequest request)
        {

            try
            {

                TimetableOutputDTO timetableOutputDTO = await _tempService.UpdateTimetable(request);
                List<CouresDTO> courses = await _tempService.GetCourses("", timetableOutputDTO.ClassName, timetableOutputDTO.CourseId);
                CouresDTO? course = courses.FirstOrDefault();
                if (course == null)
                    return BadRequest("Course not found in this class");
                foreach (string user in course.StudentIDs.Concat(course.TeacherIDs))
                {
                    ClassScheduleVersionDTO classScheduleVersionDTO = await _tempService.GetClassScheduleVersion(user);
                    if (classScheduleVersionDTO.ClassName == string.Empty)
                        classScheduleVersionDTO = new ClassScheduleVersionDTO
                        {
                            ClassName = user,
                            ExpireTime = DateTime.Now.AddMonths(2)
                        };
                    await _tempService.StoreClassScheduleVersion(classScheduleVersionDTO);
                }
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
