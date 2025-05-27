using HUBT_Social_Core.Decode;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using User_API.Src.Service;
using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Core.Models.OutSourceDataDTO;
using User_API.Src.Models;
using HUBT_Social_Core.Models.DTOs.UserDTO;
using HUBT_Social_Core.Models.Requests.Temp;
using HUBT_Social_Core.Models.Requests.Chat;
using HUBT_Social_Core.Models.DTOs.ExamDTO;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using HUBT_Social_Core.Settings.@enum;

namespace User_API.Src.Controllers
{
    [Route("api/user/schooldata")]
    [ApiController]
    public class UserShoolDataController(IUserService userService,
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
        [HttpGet("timetable")]
        public async Task<IActionResult> GetUserTimeTable()
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

            StudentDTO? studentDTO = await _outSourceService.GetStudentByMasv(userDTO.UserName);
            if (studentDTO == null)
                return NotFound();

            try
            {
                ClassScheduleVersionDTO? classScheduleVersionDTO = await _tempService.GetClassScheduleVersion(studentDTO.TenLop);

                UserTimetableOutput userTimetableOutput = new(_tempService)
                {
                    Starttime = DateTime.UtcNow.Date,
                    Endtime = DateTime.UtcNow.Date.AddMonths(2),
                };

                List<TimetableOutputDTO> timetableOutputDTOs;
                timetableOutputDTOs = await _tempService.GetList(studentDTO.TenLop);

                //if (classScheduleVersionDTO.ClassName == string.Empty && timetableOutputDTOs.Count == 0)
                //{
                //    List<TimeTableDTO>? timeTableDTOs = await _outSourceService.GetTimeTableByClassName(studentDTO.TenLop);
                //    if (timeTableDTOs == null)
                //        return BadRequest();
                //    await userTimetableOutput.GenerateReformTimetables(timeTableDTOs);

                //    classScheduleVersionDTO.ClassName = studentDTO.TenLop;
                //    classScheduleVersionDTO.ExpireTime = userTimetableOutput.Endtime;
                //    classScheduleVersionDTO = await _tempService.StoreClassScheduleVersion(classScheduleVersionDTO);
                //    userTimetableOutput.VersionKey = classScheduleVersionDTO.VersionKey;
                //    return Ok(userTimetableOutput);

                //}
                
                if (classScheduleVersionDTO.ClassName == string.Empty)
                {
                    classScheduleVersionDTO.ClassName = studentDTO.TenLop;
                    classScheduleVersionDTO.ExpireTime = userTimetableOutput.Endtime;
                    classScheduleVersionDTO = await _tempService.StoreClassScheduleVersion(classScheduleVersionDTO);
                }
                if (timetableOutputDTOs.Count == 0)
                {
                    List<TimeTableDTO>? timeTableDTOs = await _outSourceService.GetTimeTableByClassName(studentDTO.TenLop);
                    List<SubjectDTO>? subjectDTOs = await _outSourceService.GetCouresAsync(studentDTO.TenLop);
                    if (timeTableDTOs == null || subjectDTOs == null)
                        return BadRequest();

                    List<CouresDTO> couresDTOs = [];
                    Random random = new();
                    Queue<int> lastPickedIndices = new(); // Track the last few picked indices
                   
                    foreach (var timetable in timeTableDTOs)
                    {
                        int randomIndex;
                        do
                        {
                            randomIndex = random.Next(0, subjectDTOs.Count);
                        } while (lastPickedIndices.Contains(randomIndex)); // Ensure the new index is not in the history

                        lastPickedIndices.Enqueue(randomIndex);


                        SubjectDTO subjectDTO = subjectDTOs[randomIndex];
                        CreateTempCourseRequest createTempCourseRequest = new()
                        {
                            CourseId = subjectDTO.Id,
                            TimeTableDTO = timetable,
                            StudentIDs = (await _outSourceService.GetStudentByClassName(timetable.ClassName))
                                                           .Select(student => student.MaSV)
                                                           .ToArray()
                        };
                        if (createTempCourseRequest.TimeTableDTO.Room != "baitap")
                            createTempCourseRequest.TimeTableDTO.Subject = subjectDTO.TenMon;
                        if (createTempCourseRequest.CourseId != string.Empty)
                        {
                            ResponseDTO response = await _userService.GetUserByRole("TEACHER",0);
                            GetUserByRoleResponses? getUserByRoles = response.ConvertTo<GetUserByRoleResponses>();
                            if (getUserByRoles != null)
                            {
                                List<AUserDTO> teacherDTOs = getUserByRoles.AUserDTOs;
                                int index = random.Next(0, teacherDTOs.Count);
                                AUserDTO SelectTeacher = teacherDTOs[index];
                                createTempCourseRequest.TeacherIDs = [SelectTeacher.UserName];
                            }


                            CreateGroupRequest createGroupRequest = new()
                            {
                                GroupName = $"{createTempCourseRequest.TimeTableDTO.Session} Thứ {createTempCourseRequest.TimeTableDTO.Day} - {createTempCourseRequest.TimeTableDTO.Subject} - {createTempCourseRequest.TimeTableDTO.ClassName}",
                                UserNames = [.. createTempCourseRequest.TeacherIDs, .. createTempCourseRequest.StudentIDs],
                                GroupType = TypeChatRoom.GroupChat
                            };
                            
                            CreateChatResponse chat = await _chatService.CreateChatRoom(createGroupRequest, accessToken);
                            if (!string.IsNullOrEmpty(chat.Id))
                            {
                                createTempCourseRequest.RoomId = chat.Id;    
                                Console.WriteLine("Them nhom chat thanh cong");
                            }
                            else 
                                Console.WriteLine("Khong them nhom chat duoc ");
                            
                            CouresDTO couresDTO = await _tempService.StoreCourses(createTempCourseRequest);
                            couresDTOs.Add(couresDTO);

                        }
                    }
                    await userTimetableOutput.GenerateReformTimetables(couresDTOs);
                }
                else
                {
                    userTimetableOutput.ReformTimetables = timetableOutputDTOs;
                }
                userTimetableOutput.Starttime = DateTime.UtcNow.AddMonths(-2);
                userTimetableOutput.VersionKey = classScheduleVersionDTO.VersionKey;
                return Ok(userTimetableOutput);
                

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(LocalValue.Get(KeyStore.TimetableNotFound));

            }
        }
        [HttpGet("timetable-info")]
        public async Task<IActionResult> GetTimeTableInfo(string timetableId)
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

            StudentDTO? studentDTO = await _outSourceService.GetStudentByMasv(userDTO.UserName);
            if (studentDTO == null)
                return NotFound();

            TimetableOutputDTO timeTableDTO = await _tempService.Get(timetableId);
            CouresDTO couresDTO = await _tempService.GetCourses(timeTableDTO.ClassName,timeTableDTO.CourseId);

            if (timeTableDTO.Id == string.Empty || couresDTO.Id == string.Empty)
                return BadRequest(LocalValue.Get(KeyStore.TimetableNotFound));

            
            List<AUserDTO> aUserDTOs = [];
            foreach (string studentId in couresDTO.StudentIDs)
            {
                ResponseDTO response = await _userService.FindUserByUserName(accessToken, studentId);
                AUserDTO? aUserDTO = response.ConvertTo<AUserDTO>();
                if (aUserDTO != null)
                {
                    aUserDTOs.Add(aUserDTO);
                }
            }
            List<AUserDTO> teacherDTOs = [];
            foreach (string teacherId in couresDTO.TeacherIDs)
            {
                ResponseDTO response = await _userService.FindUserByUserName(accessToken, teacherId);
                AUserDTO? teacherDTO = response.ConvertTo<AUserDTO>();
                if (teacherDTO != null)
                {
                    teacherDTOs.Add(teacherDTO);
                }
            }

            if (aUserDTOs.Count != 0 && teacherDTOs.Count != 0)
            {
                TimetableInfo timetableInfo = new(timeTableDTO, aUserDTOs,teacherDTOs, couresDTO.RoomId);
                return Ok(timetableInfo);
            }
            



            return BadRequest(LocalValue.Get(KeyStore.TimetableMemberNotfound));
        }
        [HttpGet("check-version")]
        public async Task<IActionResult> CheckTimetableVersion(string Key)
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

            StudentDTO? studentDTO = await _outSourceService.GetStudentByMasv(userDTO.UserName);
            if (studentDTO == null)
                return NotFound();
            ClassScheduleVersionDTO classScheduleVersionDTO = await _tempService.GetClassScheduleVersion(studentDTO.TenLop);

            if (classScheduleVersionDTO.ClassName != string.Empty)
                return Ok(classScheduleVersionDTO.VersionKey == Key);


            return BadRequest(LocalValue.Get(KeyStore.TimetableMemberNotfound));
        }
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
                ExamDTO examDTO = new()
                    {
                        Title = request.Title,
                        Description = request.Description,
                        Image = request.ImageUrl,
                        Major = request.Major,
                        Credits = request.Credits,
                        Questions = questions
                    };
                examDTO = await _tempService.StoreExam(examDTO);
                return Ok(examDTO);
            }
        return BadRequest("Cây hỏi không đổi được.");
        }
        public class FileUploadModel
        {
            [Required]
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = "Môn học giúp bạn có thể cải thiện kỹ năng";
            public string ImageUrl { get; set; } = "https://cdn.pixabay.com/photo/2016/10/25/12/28/chemistry-1762804_1280.png";
            [Required]
            public string Major { get; set; } = string.Empty;
            public int Credits { get; set; } = 2;
            [Required]
            public IFormFile File { get; set; } = null!;
        }
    }
}
