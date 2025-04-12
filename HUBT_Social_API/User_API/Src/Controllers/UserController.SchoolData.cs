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

namespace User_API.Src.Controllers
{
    [Route("api/user/schooldata")]
    [ApiController]
    public class UserShoolDataController(IUserService userService, IOutSourceService outSourceService, ITempService tempService) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly IOutSourceService _outSourceService = outSourceService;
        private readonly ITempService _tempService = tempService;

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

                if (classScheduleVersionDTO.ClassName == string.Empty && timetableOutputDTOs.Count == 0)
                {
                    List<TimeTableDTO>? timeTableDTOs = await _outSourceService.GetTimeTableByClassName(studentDTO.TenLop);
                    if (timeTableDTOs == null)
                        return BadRequest();
                    await userTimetableOutput.GenerateReformTimetables(timeTableDTOs);

                    classScheduleVersionDTO.ClassName = studentDTO.TenLop;
                    classScheduleVersionDTO.ExpireTime = userTimetableOutput.Endtime;
                    classScheduleVersionDTO = await _tempService.StoreClassScheduleVersion(classScheduleVersionDTO);
                    userTimetableOutput.VersionKey = classScheduleVersionDTO.VersionKey;
                    return Ok(userTimetableOutput);

                }
                else
                {
                    if (classScheduleVersionDTO.ClassName == string.Empty)
                    {
                        classScheduleVersionDTO.ClassName = studentDTO.TenLop;
                        classScheduleVersionDTO.ExpireTime = userTimetableOutput.Endtime;
                        classScheduleVersionDTO = await _tempService.StoreClassScheduleVersion(classScheduleVersionDTO);
                    }
                    if (timetableOutputDTOs.Count == 0)
                    {
                        List<TimeTableDTO>? timeTableDTOs = await _outSourceService.GetTimeTableByClassName(studentDTO.TenLop);
                        if (timeTableDTOs == null)
                            return BadRequest();
                        await userTimetableOutput.GenerateReformTimetables(timeTableDTOs);
                    }
                    else
                    {
                        userTimetableOutput.ReformTimetables = timetableOutputDTOs;
                    }
                    userTimetableOutput.VersionKey = classScheduleVersionDTO.VersionKey;
                    return Ok(userTimetableOutput);
                }

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

            if (timeTableDTO.Id == string.Empty)
                return BadRequest(LocalValue.Get(KeyStore.TimetableNotFound));

            List<StudentDTO> studentDTOs = await _outSourceService.GetStudentByClassName(studentDTO.TenLop);
            if (studentDTOs.Count != 0)
            {
                List<AUserDTO> aUserDTOs = [];
                foreach (var student in studentDTOs)
                {
                    ResponseDTO response = await _userService.FindUserByUserName(accessToken, student.MaSV);
                    AUserDTO? aUserDTO = response.ConvertTo<AUserDTO>();
                    if (aUserDTO != null)
                    {
                        aUserDTOs.Add(aUserDTO);
                    }
                }
                TimetableInfo timetableInfo = new(timeTableDTO, aUserDTOs);
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
    }
}
