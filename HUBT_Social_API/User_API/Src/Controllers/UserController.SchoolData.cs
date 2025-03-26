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

namespace User_API.Src.Controllers
{
    [Route("api/user/schooldata")]
    [ApiController]
    public class UserShoolDataController(IUserService userService,IOutSourceService outSourceService) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly IOutSourceService _outSourceService = outSourceService;


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

            List<TimeTableDTO>? timeTableDTOs = await _outSourceService.GetTimeTableByClassName(studentDTO.TenLop);
            if (timeTableDTOs != null)
            {
                UserTimetableOutput userTimetableOutput = new()
                {
                    Starttime = DateTime.UtcNow,
                    Endtime = DateTime.UtcNow.AddMonths(2),
                };
                userTimetableOutput.GenerateReformTimetables(timeTableDTOs);
                return Ok(userTimetableOutput);
            }



            return BadRequest();
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

            TimeTableDTO? timeTableDTO = await _outSourceService.GetTimeTableById(timetableId);
            
            if (timeTableDTO == null)
                return BadRequest(LocalValue.Get(KeyStore.TimetableNotFound));
            
            List<StudentDTO> studentDTOs = await _outSourceService.GetStudentByClassName(studentDTO.TenLop);
            if (studentDTOs.Count != 0)
            {
                List<AUserDTO> aUserDTOs = [];
                foreach (var student in studentDTOs)
                {
                    ResponseDTO response = await _userService.FindUserByUserName(accessToken,student.MaSV);
                    AUserDTO? aUserDTO = response.ConvertTo<AUserDTO>();
                    if (aUserDTO != null)
                    {
                        aUserDTOs.Add(aUserDTO);
                    }
                }
                TimetableInfo timetableInfo = new(timeTableDTO, aUserDTOs, DateTime.UtcNow);
                return Ok(timetableInfo);
            }


                
            return BadRequest(LocalValue.Get(KeyStore.TimetableMemberNotfound));
        }
    }
}
