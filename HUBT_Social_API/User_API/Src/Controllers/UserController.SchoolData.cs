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
using System.Runtime.InteropServices;

namespace User_API.Src.Controllers
{
    [Route("api/user/schooldata")]
    [ApiController]
    public class UserShoolDataController(IUserService userService,
        IOutSourceService outSourceService,
        ITempService tempService,
        IChatService chatService) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly IOutSourceService _outSourceService = outSourceService;
        private readonly ITempService _tempService = tempService;
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

            try
            {
                ClassScheduleVersionDTO? classScheduleVersionDTO = await _tempService.GetClassScheduleVersion(userDTO.UserName);

                UserTimetableOutput userTimetableOutput = new()
                {
                    Starttime = DateTime.UtcNow.Date,
                    Endtime = DateTime.UtcNow.Date.AddMonths(2),
                };
                List<CouresDTO> couresDTOs = await _tempService.GetCourses(userDTO.UserName);
                List<TimetableOutputDTO> timetableOutputDTOs = [];
                foreach (CouresDTO couresDTO in couresDTOs)
                {
                    List<TimetableOutputDTO> newTimeTableDTOs = await _tempService.GetTimetable("","",couresDTO.Id);
                    timetableOutputDTOs.AddRange(newTimeTableDTOs);
                }

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
                    classScheduleVersionDTO.ClassName = userDTO.UserName;
                    classScheduleVersionDTO.ExpireTime = userTimetableOutput.Endtime;
                    classScheduleVersionDTO = await _tempService.StoreClassScheduleVersion(classScheduleVersionDTO);
                }
                StudentDTO? studentDTO = await _outSourceService.GetStudentByMasv(userDTO.UserName);

                if (timetableOutputDTOs.Count == 0 && studentDTO != null)
                {
                    
                    List<TimeTableDTO>? timeTableDTOs = await _outSourceService.GetTimeTableByClassName(studentDTO.TenLop);
                    List<SubjectDTO>? subjectDTOs = await _outSourceService.GetCouresAsync(studentDTO.TenLop);
                    if (timeTableDTOs == null || subjectDTOs == null)
                        return BadRequest();

                    List<CouresDTO> newCouresDTOs = [];
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
                            newCouresDTOs.Add(couresDTO);

                        }
                    }
                    userTimetableOutput.GenerateReformTimetables(newCouresDTOs);
                    userTimetableOutput.ReformTimetables = await _tempService.StoreInTimeTable(userTimetableOutput.ReformTimetables);
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

            List<TimetableOutputDTO> timeTableDTOs = await _tempService.GetTimetable(timetableId,"");
            TimetableOutputDTO? timeTableDTO = timeTableDTOs.FirstOrDefault();
            if (timeTableDTO == null)
                return BadRequest(LocalValue.Get(KeyStore.TimetableNotFound));
            List<CouresDTO> couresDTOs = await _tempService.GetCourses(studentDTO.MaSV, timeTableDTO.ClassName, timeTableDTO.CourseId);
            CouresDTO? couresDTO = couresDTOs.FirstOrDefault();
            if (timeTableDTO.Id == string.Empty || couresDTO == null)
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
        [HttpGet("questions")]
        public async Task<IActionResult> GetQuestions([FromQuery] string? major, [FromQuery] int page = 0, [FromQuery] int limit = 0)
        {
            string? accessToken = Request.Headers.ExtractBearerToken();
            if (accessToken != null)
            {
                List<ExamDTO> questions = await _tempService.GetExams(major,page,limit);
                return Ok(questions);
            }

            return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));
        }
        [HttpGet("questions-detail")]
        public async Task<IActionResult> GetQuestionsDetail([FromQuery] string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Yêu cầu không hợp lệ.");

            ExamDTO? ExamDTO = await _tempService.GetExam(id);

            if (ExamDTO != null)
            {
                Question[] questions = await _tempService.GetExamQuestions(id);
                QuizDetail createQuizRequest = new(ExamDTO)
                {
                    Questions = questions
                };
                return Ok(createQuizRequest);
            }
            return BadRequest("Cây hỏi không đổi được.");
        }
        
    }
}
