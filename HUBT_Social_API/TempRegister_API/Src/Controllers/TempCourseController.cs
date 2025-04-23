using AutoMapper;
using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Models.OutSourceDataDTO;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Settings;
using HUBT_Social_MongoDb_Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TempRegister_API.Src.Models;

namespace TempRegister_API.Src.Controllers
{
    [Route("api/tempCourse")]
    [ApiController]
    public class TempCourseController(
        IMongoService<TempCourse> tempCourse,
        IOptions<JwtSetting> option,
        IMapper mapper) : DataLayerController(mapper, option)
    {
        private readonly IMongoService<TempCourse> _tempCourse = tempCourse;

        [HttpGet]
        public async Task<IActionResult> Get(int page)
        {
            if (page>=0)
            {
                Console.WriteLine("So ban gi: ", _tempCourse.Count());
                var tempListCourse = await _tempCourse.GetSlide(page, 10);
                if (!tempListCourse.Any()) { return BadRequest(); };
               
                List<CreateGroupByCourse> results = new List<CreateGroupByCourse>();
                foreach (var course in tempListCourse)
                {
                    ListUserNameDTO member = new ListUserNameDTO();
                    member.userNames = course.StudentIDs;
                    CreateGroupByCourse item = new CreateGroupByCourse
                    {
                        ClassName = course.TimeTableDTO.ClassName, 
                        Subject = course.TimeTableDTO.Subject,
                        listUserNames = member,
                    };
                    results.Add(item);
                }
                return Ok(results);

            }
            return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
        }
    }
}
