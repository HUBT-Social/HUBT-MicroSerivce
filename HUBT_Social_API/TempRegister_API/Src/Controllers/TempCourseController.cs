using AutoMapper;
using HUBT_Social_Base;
using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Models.DTOs.UserDTO;
using HUBT_Social_Core.Models.OutSourceDataDTO;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Models.Requests.Firebase;
using HUBT_Social_Core.Models.Requests.Temp;
using HUBT_Social_Core.Settings;
using HUBT_Social_MongoDb_Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using SharpCompress;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml.Linq;
using TempRegister_API.Src.Models;
using TempRegister_API.Src.Service;

namespace TempRegister_API.Src.Controllers
{
    [Route("api/tempCourse")]
    [ApiController]
    public class TempCourseController(
        IMongoService<TempCourse> tempCourse,
        IMongoService<TempCourseDetail> tempCourseDetail,
        IOutService outService,
        INotifiService notifiService,
        IOptions<JwtSetting> option,
        IMapper mapper) : DataLayerController(mapper, option)
    {
        private readonly IMongoService<TempCourse> _tempCourse = tempCourse;
        private readonly IMongoService<TempCourseDetail> _tempCourseDetail = tempCourseDetail;
        private readonly IOutService _outService = outService;
        private readonly INotifiService _notifiService = notifiService;

        [HttpGet]
        public async Task<IActionResult> Get(int page)
        {
            if (page >= 0)
            {
                Console.WriteLine($"So ban gi: {_tempCourse.Count()}");
                var filter = Builders<TempCourse>.Filter.Eq(c => c.RoomCreated, false);
                var tempListCourse = await _tempCourse.GetSlide(page, 10, filter);
                if (!tempListCourse.Any()) { return BadRequest(); };

                List<CreateGroupByCourse> results = [];
                foreach (var course in tempListCourse)
                {
                    CreateGroupByCourse item = new()
                    {
                        Id = course.Id,
                        ClassName = course.TimeTableDTO.ClassName,
                        Subject = course.TimeTableDTO.Subject,
                        ListUserNames = [.. course.StudentIDs],
                    };
                    results.Add(item);
                }
                return Ok(results);

            }
            return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
        }

        [HttpGet("get-usernames-inclass")]
        public async Task<IActionResult> GetUsernamesInClass(string className)
        {
            if (!string.IsNullOrEmpty(className))
            {
                var tempCourse = await _tempCourse.Find((e) => e.TimeTableDTO.ClassName.Equals(className, StringComparison.CurrentCultureIgnoreCase));
                if (tempCourse == null) { return BadRequest(); };
                List<string> userName = [];

                userName.AddRange(tempCourse.First().StudentIDs);
                userName.AddRange(tempCourse.First().TeacherIDs);

                return userName.Count != 0 ? Ok(userName) : BadRequest();

            }
            return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
        }

        [HttpPost("create-course-detail")]
        public async Task<IActionResult> CreateCourseDetail(string coursesId)
        {
            if (string.IsNullOrEmpty(coursesId)) return BadRequest();

            bool isExits = await _tempCourseDetail.Exists(coursesId);
            if (isExits) { return BadRequest("Is already exits"); }

            var tempCourseDetail = await _CreateDetailCourse(coursesId);

            if (tempCourseDetail != null) return Ok(tempCourseDetail);
            return BadRequest();
        }
        
        private async Task<TempCourseDetail?> _CreateDetailCourse(string coursesId)
        {
                TempCourse? course = await _tempCourse.GetById(coursesId);

                if (course == null) return null;

                ResponseDTO responseDTO = await _outService.GetUserInClass(course.TimeTableDTO.ClassName);
                if (responseDTO.StatusCode != System.Net.HttpStatusCode.OK) return null;
                List<StudentDTO>? studentClassNames = responseDTO.ConvertTo<List<StudentDTO>>();
                if (studentClassNames == null || studentClassNames.Count == 0) return null;

                ResponseDTO responseSubjectDTO = await _outService.GetCoureInfo(course.TimeTableDTO.Subject);
                if (responseSubjectDTO.StatusCode != System.Net.HttpStatusCode.OK) return null;
                SubjectDTO? subjectDTO = responseSubjectDTO.ConvertTo<SubjectDTO>();
                if (subjectDTO == null) return null;

                Dictionary<string, string> usernameName = [];
                foreach (var student in studentClassNames)
                {
                    usernameName[student.MaSV] = student.Hoten;
                }

                TempCourseDetail tempCourseDetail = new()
                {
                    Id = coursesId,
                    Instructor = course.TeacherIDs,
                    CourseName = course.TimeTableDTO.Subject,
                    ClassName = course.TimeTableDTO.ClassName,
                    Students = [],
                    Credits = (int)subjectDTO.Sotin
                };

                // Thay đổi cách thêm sinh viên - sử dụng Dictionary
                foreach (var userName in course.StudentIDs)
                {
                    string name = usernameName[userName];
                    tempCourseDetail.Students[userName] = new Student { StudentId = userName, Name = name };
                }

                bool created = await _tempCourseDetail.Create(tempCourseDetail);
                return created ? tempCourseDetail : null;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest("ID is required.");
            var course = await _tempCourseDetail.GetById(id);
            if(course == null) 
            {
                var tempCourseDetail = await _CreateDetailCourse(id);

                if (tempCourseDetail != null) return Ok(tempCourseDetail);
                return BadRequest();
            }
            return Ok(course);
        }

        [HttpPut("{id}/module-score")]
        public async Task<IActionResult> UpdateModuleScore(string id, [FromBody] List<ScoreRequest> request)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Course ID is required.");

            if (request == null || !request.Any())
                return BadRequest("Score data is required.");

            // Lấy khóa học
            var course = await _tempCourseDetail.GetById(id);
            if (course == null)
                return NotFound("Course not found.");

            bool hasUpdates = false;

            foreach (var scoreRequest in request)
            {
                if (string.IsNullOrWhiteSpace(scoreRequest.StudentId) || 
                    string.IsNullOrWhiteSpace(scoreRequest.ScoreName))
                    continue;

                // Tìm sinh viên bằng Dictionary key
                if (!course.Students.TryGetValue(scoreRequest.StudentId, out var student))
                    continue;

                // Cập nhật hoặc thêm mới điểm bằng Dictionary
                student.ClassScores[scoreRequest.ScoreName] = scoreRequest.ScoreValue;

                // Tính lại ModuleScore
                student.ModuleScore = student.ClassScores.Any() ? student.ClassScores.Values.Average() : null;

                hasUpdates = true;
            }

            if (!hasUpdates)
                return BadRequest("No valid score updates applied.");

            // Cập nhật toàn bộ khóa học bằng Replace
            var filter = Builders<TempCourseDetail>.Filter.Eq(c => c.Id, id);
            var success = await _tempCourseDetail.Replace(filter, course);

            if (!success)
                return BadRequest("Failed to update module scores.");

            // Gọi hàm theo dõi tiến độ
            var violatingStudents = MonitorStudentProgress(course);
                Console.WriteLine($"Co canh bao {violatingStudents.ToJson().ToString()}");
            if (violatingStudents is not null && violatingStudents.Count != 0)
            {
                // Chạy gửi thông báo mà không chờ
                _ = Task.Run(() => _notifiService.SendRemindNotication(violatingStudents)
                    .ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            Console.WriteLine("Gửi thông báo nhắc nhở thất bại: ", task.Exception);
                        }
                    }));
            }

            return Ok(new
            {
                Message = "Module scores updated successfully."
            });
        }

        [HttpPut("{id}/attendance")]
        public async Task<IActionResult> MarkAttendance(string id, [FromBody] AttendanceRequest request)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Course ID is required.");

            if (request == null || request.Week <= 0 || request.Records == null || !request.Records.Any())
                return BadRequest("Invalid attendance data.");
            string weekStringType = request.Week.ToString();
            if (weekStringType == null) return BadRequest("Can not convert int week to string");

            // Lấy khóa học
            var course = await _tempCourseDetail.GetById(id);
            if (course == null)
                return NotFound("Course not found.");

            bool hasUpdates = false;

            foreach (var record in request.Records)
            {
                if (string.IsNullOrWhiteSpace(record.StudentId))
                    continue;

                // Tìm sinh viên bằng Dictionary key
                if (!course.Students.TryGetValue(record.StudentId, out var student))
                    continue;
                // Cập nhật điểm danh bằng Dictionary
                student.AttendanceRecords[weekStringType] = record.Status;

                hasUpdates = true;
            }

            if (!hasUpdates)
                return BadRequest("No valid attendance updates applied.");

            // Cập nhật CurrentWeek nếu cần
            if (request.Week > course.CurrentWeek)
                course.CurrentWeek = request.Week;

            // Gọi hàm theo dõi tiến độ
            var violatingStudents = MonitorStudentProgress(course);

            // Cập nhật toàn bộ khóa học bằng Replace
            var filter = Builders<TempCourseDetail>.Filter.Eq(c => c.Id, id);
            var success = await _tempCourseDetail.Replace(filter, course);

            if (!success)
                return BadRequest("Failed to mark attendance.");
            Console.WriteLine($"Co canh bao {violatingStudents.ToJson().ToString()}");
            if (violatingStudents is not null && violatingStudents.Count !=0)
            {
                // Chạy gửi thông báo mà không chờ
                _ = Task.Run(() => _notifiService.SendRemindNotication(violatingStudents)
                    .ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            Console.WriteLine("Gửi thông báo nhắc nhở thất bại: ", task.Exception);
                        }
                    }));
            }


            return Ok(new
            {
                Message = "Attendance marked successfully."
            });
        }

        private List<NotificatonRemindRequest> MonitorStudentProgress(TempCourseDetail course)
        {
            // Check if course has started (week > 0)
            if (course.CurrentWeek <= 0)
                return [];

            var violatingStudents = new List<NotificatonRemindRequest>();
            double maxAllowedAbsences = course.Credits * 4 * 0.3; // 30% of total sessions (Credits * 4)

            foreach (var studentEntry in course.Students)
            {
                if (studentEntry.Value.IsKDT) continue;
                var student = studentEntry.Value;
                var warnings = new List<string>();
                string notificationcode = "normal";
                // Calculate absences
                double absenceCount = CalculateAbsence(student.AttendanceRecords);
                warnings.Add($"Tại môn {course.CourseName}: ");
                // Check absence conditions
                if (absenceCount > 0)
                {
                    if (absenceCount > maxAllowedAbsences)
                    {
                        warnings.Add($"Bạn đã nghỉ {absenceCount} buổi, vượt quá 30% buổi cho phép nên không được thi trong kì thi sắp tới. Vui lòng chú ý học tập hơn trong lần sau.");
                        course.Students[studentEntry.Key].IsKDT = true;
                        notificationcode = "learning_alerts";
                    }
                    else
                    {
                        warnings.Add($"Bạn đã nghỉ {absenceCount} buổi. Vui lòng đi học đúng giờ và đầy đủ.");
                    }
                }

                // Check module score
                if (student.ModuleScore.HasValue && student.ModuleScore < 5)
                {
                    warnings.Add($"Điểm học phần ({student.ModuleScore}). Vui lòng cố gắng học tập.");
                }

                // Add to violating students list if there are warnings
                if (warnings.Count > 1)
                {
                    violatingStudents.Add(new NotificatonRemindRequest { UserName=studentEntry.Key, RemindCode=notificationcode, Content=string.Join(" ", warnings) });
                }
            }

            return violatingStudents;
        }

        private static double CalculateAbsence(Dictionary<string, AttendanceStatus> attendanceRecords)
        {
            if (attendanceRecords == null || attendanceRecords.Count == 0)
                return 0;

            double absentCount = 0;
            foreach (var attendance in attendanceRecords.Values)
            {
                if (attendance == AttendanceStatus.Absent)
                {
                    absentCount++;
                }
                else if (attendance == AttendanceStatus.Late)
                {
                    absentCount += 0.5;
                }
            }
            return absentCount;
        }



        [HttpPut("status")]
        public async Task<ActionResult> Put([FromQuery] string courseId)
        {
            try
            {
                Expression<Func<TempCourse, bool>> filter = c => c.Id == courseId;
                var update = Builders<TempCourse>.Update.Set(c => c.RoomCreated, true);
                bool updated = await _tempCourse.UpdateByFilter(filter, update);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(BaseOk(ex));
                return BadRequest();
            }
        }
    }
}                    