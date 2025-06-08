using AutoMapper;
using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs.UserDTO;
using HUBT_Social_Core.Models.OutSourceDataDTO;
using HUBT_Social_Core.Models.Requests.Temp;
using HUBT_Social_Core.Settings;
using HUBT_Social_MongoDb_Service.ASP_Extentions;
using HUBT_Social_MongoDb_Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver.Core.Operations;
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
        public async Task<IActionResult> GetTimetable([FromQuery] string? id, [FromQuery] string? className)
        {
            if (!string.IsNullOrEmpty(id))
            {
                TempTimetable? timetable = await _tempTimeTable.GetById(id);
                if (timetable != null)
                {
                    TimetableOutputDTO timetableOutputDTO = _mapper.Map<TimetableOutputDTO>(timetable);
                    return Ok(timetableOutputDTO);
                }
                return NotFound(new { message = "Timetable not found" });
            }
            else if (!string.IsNullOrEmpty(className))
            {
                List<TempTimetable> timetables = await _tempTimeTable.Find(
                    t => t.ClassName.Equals(className, StringComparison.CurrentCultureIgnoreCase)
                    ).ToListAsync();
                if (timetables.Count != 0)
                {
                    List<TimetableOutputDTO> timetableOutputDTOs = _mapper.Map<List<TimetableOutputDTO>>(timetables);
                    return Ok(timetableOutputDTOs);
                }
                return NotFound("No timetables found for the specified class name");
            }
            return BadRequest("Either id or className must be provided");
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
                return await _tempTimeTable.Update(tempTimetable) ? 
                    Ok(tempTimetable) : BadRequest("Update Fail");
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
        [HttpGet("courses")]
        public async Task<IActionResult> GetCourse([FromQuery] string className, [FromQuery] string? coursesId)
        {
            if (!string.IsNullOrEmpty(className))
            {
                List<TempCourse> courses = await _tempCourse.Find(cs =>
                            cs.TimeTableDTO.ClassName.Equals(className, StringComparison.CurrentCultureIgnoreCase)
                            ).ToListAsync();

                if (courses.Count > 0)
                {
                    if (!coursesId.IsNullOrEmpty())
                    {
                        courses = courses.Where(courses => courses.Id.Equals(coursesId, StringComparison.CurrentCultureIgnoreCase)).ToList();

                    }
                    List<CouresDTO> courseDTOs = _mapper.Map<List<CouresDTO>>(courses);
                
                    return Ok(courseDTOs);
                }
            }

            return BadRequest("Either id or className must be provided");
                
        }
    }
}
