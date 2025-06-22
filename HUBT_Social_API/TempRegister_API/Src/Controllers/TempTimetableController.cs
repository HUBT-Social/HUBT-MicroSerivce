using AutoMapper;
using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs.UserDTO;
using HUBT_Social_Core.Models.OutSourceDataDTO;
using HUBT_Social_Core.Models.Requests.Temp;
using HUBT_Social_Core.Settings;
using HUBT_Social_MongoDb_Service.ASP_Extentions;
using HUBT_Social_MongoDb_Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver.Core.Operations;
using System.Linq.Expressions;
using System.Xml;
using TempRegister_API.Src.Models;

namespace TempRegister_API.Src.Controllers
{
    [Route("api/temptimetable")]
    [ApiController]
    public class TempTimetableController(
        IMongoService<TempTimetable> tempTimeTable,
        IMongoService<TempClassScheduleVersion> tempClassScheduleVersion,
        IMongoService<TempCourse> tempCourse,
        IOptions<JwtSetting> option,
        IMapper mapper) : DataLayerController(mapper, option)
    {
        private readonly IMongoService<TempTimetable> _tempTimeTable = tempTimeTable;
        private readonly IMongoService<TempCourse> _tempCourse = tempCourse;

        private readonly IMongoService<TempClassScheduleVersion> _tempClassScheduleVersion = tempClassScheduleVersion;
        [HttpGet]
        public async Task<IActionResult> GetTimetable([FromQuery] string? id, [FromQuery] string? className ,[FromQuery] string? coursesId)
        {
            if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(className) && string.IsNullOrEmpty(coursesId))
            {
                return BadRequest("At least one query parameter must be provided.");
            }
            Expression<Func<TempTimetable, bool>> predicate = cs => true;

            if (!string.IsNullOrEmpty(coursesId))
            {
                predicate = predicate.And(cs =>
                    cs.CourseId.Equals(coursesId));
            }

            if (!string.IsNullOrEmpty(className))
            {
                predicate = predicate.And(cs =>
                    cs.ClassName.Equals(className, StringComparison.CurrentCultureIgnoreCase));
            }

            if (!string.IsNullOrEmpty(id))
            {
                predicate = predicate.And(cs =>
                    cs.Id.Equals(id));
            }

            List<TempTimetable> timeTables = await _tempTimeTable.Find(predicate).ToListAsync();

            if (timeTables.Count != 0)
            {
                List<TimetableOutputDTO> timeTableDTO = _mapper.Map<List<TimetableOutputDTO>>(timeTables);
                return Ok(timeTableDTO);
            }

            return NotFound("No courses found with the given filters.");
            
        }

        [HttpGet("classes/available")]
        public async Task<IActionResult> GetAvailableClasses()
        {
            var timetable = await _tempClassScheduleVersion.GetAll();
            if (timetable == null)
            {
                return BadRequest("Khong tim duoc du lieu hop le!");
            }

            var classScheduleVersionDTO = _mapper.Map<List<ClassScheduleVersionDTO>>(timetable);
            DateTime currentTime = DateTime.Now;
            var classAvailable = classScheduleVersionDTO
                .Where(schedule => schedule.ExpireTime > currentTime)
                .Select(schedule => schedule.ClassName) // Adjust this based on the actual property you want to return
                .ToList();

            return Ok(classAvailable);
        }
        [HttpPut("change-schedule-timetable")]
        public async Task<IActionResult> ChangeScheduleTimeTable(string id, DateTime? newStartTime, DateTime? newEndTime, string? zoomID, string? room)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new { message = "ID không được để trống." });
            }

            // Check if at least one parameter is provided
            if (!newStartTime.HasValue && !newEndTime.HasValue && string.IsNullOrEmpty(zoomID) && string.IsNullOrEmpty(room))
            {
                return BadRequest(new { message = "Phải cung cấp ít nhất một trường để cập nhật (thời gian, ZoomID hoặc phòng)." });
            }

            // Validate time constraints if provided
            DateTime now = DateTime.UtcNow; // Use UTC for consistency
            if (newStartTime.HasValue && newStartTime < now)
            {
                return BadRequest(new { message = "Thời gian bắt đầu không thể là quá khứ." });
            }

            if (newEndTime.HasValue && newEndTime < now)
            {
                return BadRequest(new { message = "Thời gian kết thúc không thể là quá khứ." });
            }

            // Retrieve timetable
            TempTimetable? timetable = await _tempTimeTable.GetById(id);
            if (timetable == null)
            {
                return NotFound(new { message = "Không tìm thấy buổi học hợp lệ." });
            }

            // Validate time constraints with existing times if only one is provided
            DateTime startTime = newStartTime ?? timetable.StartTime;
            DateTime endTime = newEndTime ?? timetable.EndTime;

            if (endTime <= startTime)
            {
                return BadRequest(new { message = "Thời gian kết thúc phải sau thời gian bắt đầu." });
            }

            // Check for changes
            bool hasChanges = false;
            if (newStartTime.HasValue && newStartTime.Value != timetable.StartTime)
            {
                timetable.StartTime = newStartTime.Value;
                hasChanges = true;
            }
            if (newEndTime.HasValue && newEndTime.Value != timetable.EndTime)
            {
                timetable.EndTime = newEndTime.Value;
                hasChanges = true;
            }
            if (!string.IsNullOrEmpty(zoomID) && zoomID != timetable.ZoomID)
            {
                timetable.ZoomID = zoomID;
                hasChanges = true;
            }
            if (!string.IsNullOrEmpty(room) && room != timetable.Room)
            {
                timetable.Room = room;
                hasChanges = true;
            }

            // If no changes, return success without updating
            if (!hasChanges)
            {
                return Ok(timetable); // No changes needed, return current timetable
            }

            try
            {
                bool updateSuccessful = await _tempTimeTable.Update(timetable);
                if (updateSuccessful)
                {
                    return Ok(timetable);
                }
                return StatusCode(500, new { message = "Lỗi cập nhật thời khóa biểu." });
            }
            catch (Exception ex)
            {
                // Log the exception (use your logging framework, e.g., ILogger)
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật thời khóa biểu.", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TimetableOutputDTO timetableOutDTO)
        {
            timetableOutDTO.Id = string.Empty;
            TempTimetable timetable = _mapper.Map<TempTimetable>(timetableOutDTO);

            if (await _tempTimeTable.Create(timetable))
            {
                timetableOutDTO.Id = timetable.Id;
                return Ok(timetableOutDTO);
            }
            return BadRequest(LocalValue.Get(KeyStore.UnableToStoreInDatabase));
        }
        [HttpPut]
        public async Task<IActionResult> UpdateTimetbale([FromBody] UpdateTimetableRequest request)
        {
            if (!request.Id.IsNullOrEmpty())
            {
                TempTimetable? tempTimetable = await _tempTimeTable.GetById(request.Id);
                if (tempTimetable == null)  
                {
                    return NotFound("Timetable not found");
                }
                tempTimetable.StartTime = request.NewStartTime;
                tempTimetable.EndTime = request.NewEndTime;
                tempTimetable.Subject = request.Subject;
                tempTimetable.Room = request.Room;
                tempTimetable.ZoomID = request.ZoomID;
                TimetableOutputDTO timetableOutputDTO = _mapper.Map<TimetableOutputDTO>(tempTimetable);
                return await _tempTimeTable.Update(tempTimetable) ? 
                    Ok(timetableOutputDTO) : BadRequest("Update Fail");
            }
            return BadRequest("Id is null");
        }
        [HttpPost("create-many")]
        public async Task<IActionResult> CreateMany([FromBody] List<TimetableOutputDTO> timetableOutDTO)
        {
            List<TempTimetable> timetable = _mapper.Map<List<TempTimetable>>(timetableOutDTO);

            if (await _tempTimeTable.CreateMany(timetable))
            {
                return Ok(timetable);
            }
            return BadRequest(LocalValue.Get(KeyStore.UnableToStoreInDatabase));
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteTimeTable(string id)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new { message = "ID không được để trống." });
            }
            // Retrieve timetable
            TempTimetable? timetable = await _tempTimeTable.GetById(id);
            if (timetable == null)
            {
                return NotFound(new { message = "Không tìm thấy buổi học hợp lệ." });
            }

            try
            {
                bool deleteSuccessful = await _tempTimeTable.Delete(timetable);
                if (deleteSuccessful)
                {
                    return Ok(new { message = "Xóa thời khóa biểu thành công." });
                }
                return StatusCode(500, new { message = "Lỗi xoa thời khóa biểu." });
            }
            catch (Exception ex)
            {
                // Log the exception (use your logging framework, e.g., ILogger)
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xoa thời khóa biểu.", error = ex.Message });
            }
        }
        [HttpGet("classscheduleversion")]
        public async Task<IActionResult> GetClassScheduleVersion([FromQuery] string className)
        {
            TempClassScheduleVersion? timetable = await _tempClassScheduleVersion.GetById(className.ToLower());
            if (timetable != null)
            {
                ClassScheduleVersionDTO ClassScheduleVersionDTO = _mapper.Map<ClassScheduleVersionDTO>(timetable);
                return Ok(ClassScheduleVersionDTO);
            }
            return BadRequest("Timetable not found");
        }
        [HttpPost("classscheduleversion")]
        public async Task<IActionResult> CreateClassScheduleVersion([FromBody] ClassScheduleVersionDTO request)
        {
            TempClassScheduleVersion ClassScheduleVesion = new()
            {
                ClassName = request.ClassName.ToLower(),
                VersionKey = Guid.NewGuid().ToString(),
                LastUpdate = DateTime.UtcNow,
                ExpireTime = request.ExpireTime
            };
            if (await _tempClassScheduleVersion.Create(ClassScheduleVesion))
            {
                return Ok(ClassScheduleVesion);
            }
            else if (await _tempClassScheduleVersion.Update(ClassScheduleVesion))
            {
                return Ok(ClassScheduleVesion);
            }
            return BadRequest("Request fail");
        }
        [HttpPost("courses")]
        public async Task<IActionResult> CreateCourse([FromBody] CreateTempCourseRequest request)
        {
            List<TempCourse> courses = await _tempCourse.Find(cs =>
                            cs.TimeTableDTO.ClassName == request.TimeTableDTO.ClassName &&
                            cs.CourseID == request.CourseId
                            ).ToListAsync();
            if (courses.Count <= 0)
            {
                TempCourse course = new()
                {
                    CourseID = request.CourseId,
                    StudentIDs = request.StudentIDs,
                    TeacherIDs = request.TeacherIDs,
                    TimeTableDTO = request.TimeTableDTO,
                    RoomId = request.RoomId
                };
                if (await _tempCourse.Create(course))
                {
                    return Ok(course);
                }
            }
            //else
            //{
            //    var updatedClasses = existingCourse.StudentIDs?.ToList() ?? []; 
            //    if (!updatedClasses.Contains(request.TimeTableDTO.ClassName)) 
            //    {
            //        updatedClasses.Add(request.TimeTableDTO.ClassName);
            //    }
            //    existingCourse.StudentIDs = [.. updatedClasses]; 

            //    if (await _tempCourse.Update(existingCourse))
            //    {
            //        return Ok(existingCourse);
            //    }
            //}
            return BadRequest("Database exit");

        }
        [HttpPost("courses/add")]
        public async Task<IActionResult> CreateCourse([FromBody] AddStudentToCouresRequest request)
        {
            TempCourse? course = await _tempCourse.Find(cs =>
                            cs.TimeTableDTO.ClassName == request.ClassName &&
                            cs.CourseID == request.CourseId
                            ).FirstOrDefaultAsync();  
            if (course != null)
            {
                var updatedStudentIDs = course.StudentIDs?.ToList() ?? [];
                if (!updatedStudentIDs.Contains(request.StudentId)) 
                {
                    updatedStudentIDs.Add(request.StudentId);
                }
                course.StudentIDs = [.. updatedStudentIDs];

                if (await _tempCourse.Update(course))
                {
                    return Ok(course);
                }
            }
            return BadRequest("Database exit");

        }
        [HttpGet("courseId")]
        public async Task<IActionResult> GetCourse(
            [FromQuery] string? className)
        {
            if (string.IsNullOrEmpty(className))
            {
                return BadRequest("At least one query parameter must be provided.");
            }

            Expression<Func<TempCourse, bool>> predicate = cs => true;

            if (!string.IsNullOrEmpty(className))
            {
                predicate = predicate.And(cs =>
                    cs.TimeTableDTO.ClassName.Equals(className, StringComparison.CurrentCultureIgnoreCase));
            }
            List<TempCourse> courses = await _tempCourse.Find(predicate).ToListAsync();

            if (courses.Count != 0)
            {
                var courseDTOs = _mapper.Map<List<CouresDTO>>(courses);
                List<CourseNameIdResponse> result = [];
                foreach (var courseDTO in courseDTOs)
                {
                    result.Add(new CourseNameIdResponse
                    {
                        id = courseDTO.Id,
                        name = courseDTO.TimeTableDTO.Subject,
                        code = courseDTO.Id

                    });
                }
                return Ok(result);
            }

            return NotFound("No courses found with the given filters.");
        }
        public class CourseNameIdResponse
        {
            public string id { get; set; } = string.Empty;
            public string name { get; set; } = string.Empty;
            public string code { get; set; } = string.Empty;
        }


            [HttpGet("courses")]
        public async Task<IActionResult> GetCourse(
            [FromQuery] string? userName,
            [FromQuery] string? className,
            [FromQuery] string? coursesId)
        {
            if (string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(className) && string.IsNullOrEmpty(coursesId))
            {
                return BadRequest("At least one query parameter must be provided.");
            }

            Expression<Func<TempCourse, bool>> predicate = cs => true;

            if (!string.IsNullOrEmpty(userName))
            {
                predicate = predicate.And(cs =>
                    cs.StudentIDs.Contains(userName) ||
                    cs.TeacherIDs.Contains(userName));
            }

            if (!string.IsNullOrEmpty(className))
            {
                predicate = predicate.And(cs =>
                    cs.TimeTableDTO.ClassName.Equals(className, StringComparison.CurrentCultureIgnoreCase));
            }

            if (!string.IsNullOrEmpty(coursesId))
            {
                predicate = predicate.And(cs =>
                    cs.CourseID.Equals(coursesId));
            }

            List<TempCourse> courses = await _tempCourse.Find(predicate).ToListAsync();

            if (courses.Count != 0)
            {
                var courseDTOs = _mapper.Map<List<CouresDTO>>(courses);
                return Ok(courseDTOs);
            }

            return NotFound("No courses found with the given filters.");
        }
    }
}
